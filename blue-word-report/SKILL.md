---

name: blue-word-report
description: "Generate professional Word (.docx) reports in blue tech style using minimax-docx (C# OpenXML). Trigger: 生成蓝色科技风格Word报告 / 生成蓝色风格Word文档 / blue word report."
---

# Blue Word Report

Generate professional Word documents in a consistent blue tech style via minimax-docx (C# OpenXML).

## Triggers

* "生成蓝色科技风格Word报告"
* "生成蓝色风格Word文档"
* "blue word report"

## Prerequisites

1. Run `bash \~/.openclaw/skills/minimax-docx/scripts/env\_check.sh` — must say READY.
2. Content source: a Markdown file path, or content from conversation history.

## Workflow

### Decision: Normal vs Large Document

**Normal (< 30K content):** go straight to Step 1 → 2 → 3 → 4.

**Large (≥ 30K content OR user asks for 100+ pages):** go to Large Document Workflow below first.

---

### Normal Workflow (< 30K content)

#### 1\. Prepare content

* Read the source Markdown file (or extract from conversation).
* **If content is from a URL:** `web\_fetch` 有 20000 字符硬限制，长文章会被截断。必须用 `curl` 下载完整 HTML，再用 Python 脚本提取正文：

```bash
  curl -s <URL> > /tmp/page.html \&\& python3 -c "
  import re, sys
  html = open('/tmp/page.html').read()
  text = re.sub(r'<\[^>]+>', '\\n', html)
  text = re.sub(r'\\n\\s\*\\n', '\\n\\n', text).strip()
  print(text\[:100000])
  " > /tmp/content.txt
  ```

* Identify all headings (H1/H2/H3), tables, and body text.
* Extract the document title (first H1 or file name).

#### 2\. Write C# script (gen.csx)

Write a single `gen.csx` file in the workspace. If the script exceeds \~10KB, use `write` for the framework then `edit` to append content sections.

**Script template** (copy from `references/script-template.csx`):

```bash
cp \~/.openclaw/skills/blue-word-report/references/script-template.csx /root/.openclaw/workspace/gen.csx
```

Then edit the template:

* Replace the `// === CONTENT ===` section with actual document content
* Map Markdown H1 → `P(body, "Heading 1", "...")`, H2 → `Heading 2`, H3 → `Heading 3`
* Convert Markdown tables → `Table(body, headers, rows)`
* Convert body text → `P(body, "Normal", "...")`

#### 3\. Execute

```bash
cd /root/.openclaw/workspace \&\& dotnet-script gen.csx
```

#### 4\. Verify

```bash
ls -lh gaokao\_volunteer\_report.docx  # or the output filename
file \*.docx  # should say "Microsoft Word 2007+"
```

---

### Large Document Workflow (≥ 30K content)

适用于 100+ 页的超大文档。核心思路：**用 markdown 做中间层，用状态文件做章节间桥梁，最后一次性转换为 docx。**

#### Phase 1: 规划（写任何内容之前）

**1.1 生成完整大纲** → `doc-outline.md`

```markdown
# 报告标题

> 目标读者：XXX
> 文档类型：XXX方案文档
> 预估页数：XXX页

## 第一章 XXX（预估 X 页）
- 核心论点：...
- 关键数据/表格：...
- 与其他章节的关联：...

## 第二章 XXX（预估 X 页）
- ...
```

**1.2 定义风格指南** → `doc-style.md`

```markdown
# 术语表（全文统一，不可混用）
- 系统 → 平台
- 用户 → 客户
- 功能 → 能力

# 语气
- 正式书面，不用"我们""你""我"
- 不用感叹号
- 不用口语化表达

# 数据格式
- 统一用"XX%"，不写"百分之XX"
- 金额统一"XX万元"
- 日期统一"YYYY年MM月DD日"

# 章节结构
- 每章开头：概述段（2-3 句话说清本章要讲什么）
- 每章结尾：小结段（总结本章结论）
```

**1.3 定义章节依赖** → `doc-deps.md`

```markdown
# 章节依赖关系

## 第三章
- 依赖：第一章结论 B（"采用方案A"）
- 引用：第一章数据表 1-1（"市场规模120亿"）

## 第五章
- 依赖：第三章结论 A
- 引用：第三章表格 3-2
```

#### Phase 2: 逐章写作

**2.1 启动：创建状态文件** → `doc-state.md`

```markdown
# 文档写作状态

## 元信息
- 文档标题：XXX
- 总章节：X 章
- 当前进度：第 0 章（未开始）
- 创建时间：YYYY-MM-DD HH:MM
- 最后更新：YYYY-MM-DD HH:MM

## 已完成章节
（每写完一章追加以下格式）

### 第 X 章 XXX
- 页数：约 X 页
- 核心结论：...
- 关键数据：...
- 术语使用：用了"平台"没用"系统"
- 伏笔/引用：提到了"详见第X章"
- 跨章关联：承接了第X章的结论Y
```

**2.2 每章写入流程**

对每一章，执行以下步骤：

```
Step 1: 读取上下文
  read → doc-state.md（已写章节摘要）
  read → doc-outline.md（本章要求）
  read → doc-style.md（术语/格式约束）
  read → doc-deps.md（本章依赖的前序内容）

Step 2: 写作
  将本章内容写入 → doc-content/chapter-XX.md
  （每章一个文件，避免单文件过大）

Step 3: 更新状态
  edit → doc-state.md，追加本章摘要
```

**2.3 目录结构**

```
doc-build/
├── doc-outline.md        # 大纲
├── doc-style.md          # 风格指南
├── doc-deps.md           # 章节依赖
├── doc-state.md          # 写作状态（核心！）
└── content/
    ├── chapter-01.md     # 第一章内容
    ├── chapter-02.md     # 第二章内容
    ├── ...
    └── chapter-XX.md     # 最后一章
```

#### Phase 3: 合并与转换

**3.1 合并所有章节为一个 markdown**

```bash
cd /root/.openclaw/workspace/doc-build
# 按顺序合并
for f in content/chapter-\*.md; do cat "$f"; echo -e "\\n\\n"; done > full-report.md
```

**3.2 用蓝风格脚本转换为 docx**

将 `full-report.md` 传入 normal workflow 的 Step 2-4，生成最终 docx。

如果 markdown 内容超过 gen.csx 单次处理能力，用以下方式分段写入：

```bash
cp \~/.openclaw/skills/blue-word-report/references/script-template.csx /root/.openclaw/workspace/gen.csx
# 先写框架（write），再逐章追加内容（edit）
```

#### Phase 4: 一致性校验

全部生成后，执行最终校验：

```
Step 1: 读取 doc-state.md + doc-outline.md
Step 2: 检查：
  - 术语是否统一（对照 doc-style.md 术语表）
  - 章间引用是否对得上（"详见第X章" 是否存在）
  - 结论是否矛盾（doc-state.md 中各章核心结论是否冲突）
  - 数据是否一致（同一数据在不同章节是否相同）
Step 3: 如有问题，edit 修复后重新转换
```

#### 大文档关键规则

1. **每章写完必须更新 doc-state.md** — 这是章节间的唯一桥梁，漏更新 = 后面章节可能逻辑断裂
2. **每章写入前必须读 doc-state.md** — 否则会重复或矛盾
3. **术语表不可违反** — 发现不一致必须修复，不能"灵活处理"
4. **单章内容控制在 3000-5000 字** — 超过则拆分为子章节
5. **不要跳章** — 严格按大纲顺序写，跳章容易遗漏依赖
6. **状态文件用追加模式** — 每章结束后 edit 追加，不要覆盖已有内容

## Style Specification

### Colors

|Element|Color|Hex|
|-|-|-|
|Title / H1|深蓝|#1F3864|
|H2 / H3 / Table header|中蓝|#2F5496|
|Body text|深灰|#333333|
|Table inner border|浅灰|#B0B0B0|
|Even row (zebra)|浅蓝灰|#EDF2F9|
|Header text|白色|#FFFFFF|

### Fonts

* English: **Calibri**
* Chinese: **SimSun** (body), **SimHei** (headings)

### Paragraph Styles

|Style|Font|Size|Color|Spacing|
|-|-|-|-|-|
|Title|SimSun, Bold|18pt|#1F3864|Center, After=200, Line=240|
|Heading 1|SimHei, Bold|16pt|#1F3864|Before=360, After=120, Line=240, OutlineLevel=0|
|Heading 2|SimHei, Bold|14pt|#2F5496|Before=360, After=120, Line=240, OutlineLevel=1|
|Heading 3|SimHei, Bold|12pt|#2F5496|Before=240, After=120, Line=240, OutlineLevel=2|
|Normal|SimSun|11pt|#333333|After=120, Line=276|

### Table Style

* **Header:** Medium blue background (#2F5496), white bold centered text
* **Borders:** Top/bottom medium blue (#2F5496), no left/right borders, inner light gray (#B0B0B0)
* **Zebra striping:** Even rows light blue-gray (#EDF2F9), odd rows white
* **First column:** Bold content
* **Cell spacing:** After=0, Line=276

### Page Setup

* Margins: Top/Bottom 2.54cm (1440 DXA), Left/Right 2.48cm (1406 DXA)

## Critical Rules

1. **Use English double quotes** `"` in C# code — never Chinese quotes `"` or `"`
2. **PageMargin** properties need `int` type (e.g., `567`), not `UInt32Value`
3. **Use `AddNewPart<StyleDefinitionsPart>()`** — do not access `mp.StyleDefinitionsPart` directly (it's null)
4. **Use `var doc =`** not `using var doc =` in csx (scope issue)
5. **Title must use `ParagraphStyleId { Val = "Title" }`** — the `P()` helper auto-applies bold/color for Title style
6. **All strings in C# must use ASCII double quotes** — the script fails to compile with Chinese quotes
7. **For long documents (30K+ content), use qwen3.5-plus model** — GLM-5 tends to simplify content or skip formatting
8. **Emoji characters** (⭐✅⚠️) can cause C# compilation issues — replace with text alternatives (5星/4星/可冲刺/有机会)
9. **`web\_fetch` 有 20000 字符硬限制** — 长文章必须用 `curl` + Python 提取完整内容，否则内容被截断导致遗漏关键信息
10. **Large documents (30K+/100+ pages): 必须走 Large Document Workflow** — 先建 doc-build/ 目录，用 doc-state.md 做章节间桥梁，逐章写入，最后合并转 docx
11. **doc-state.md 是大文档的命脉** — 每章写完必须更新，每章写入前必须读取，漏更新 = 逻辑断裂
12. **单章内容控制在 3000-5000 字** — 超过则拆分为子章节（chapter-XX-1.md, chapter-XX-2.md）

## Model Selection

* For documents under 30K content: any model works
* For documents over 30K content: **use mimo-v2.5** (1M context window)
* GLM-5 (200K context) tends to simplify or skip formatting on long documents

## Output

Save to `/root/.openclaw/workspace/<report-name>.docx`

