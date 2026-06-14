---
name: llm-wiki
description: "Wiki库增量摄入流程。触发句式：{XXX}-wiki库新增了内容，处理一下。读取schema规则，用obsidian CLI写入词条。"
---

# Wiki库增量摄入

处理 Obsidian wiki 库的增量更新，将原始资料编译到知识层。

## 触发句式

`{可选：openclaw}, {XXX}-wiki库/XXX知识库 新增了内容，你处理一下`

## 前置依赖

- `obsidian` CLI 已安装（见 obsidian skill）
- 目标 vault 在 Obsidian 中已打开

## 执行步骤（严格按序，禁止跳步）

### 1. 锁定目标库

提取库名 XXX，目标库路径：`XXX-wiki`

### 2. 读取 schema 规则（强制）

- 路径：`XXX-wiki/schema/CLAUDE.md`
- **全程遵守其规则**：文件命名、YAML格式、双向链接、词条结构、术语归一等
- 术语映射：`XXX-wiki/schema/TERM_MAPPING.md`（AI不可修改，但可追加新变体别名）
- **找不到 CLAUDE.md 立即提醒用户**，不可自行处理

### 3. 增量检测

- 读取 `XXX-wiki/raw/.processed_files.json`，获取已处理文件列表及其 hash/mtime
- 扫描 `raw/` 一级子目录，对比 hash：
  - **新增文件**：必须处理
  - **修改文件**：重新处理，更新对应词条
  - **未变更文件**：跳过，禁止重复处理

### 4. 文件处理

对每个新增/修改文件：

1. **提取文本**：python-pptx（PPTX）、python-docx（DOCX）、Python 内置（TXT/MD）、openpyxl（XLSX）
2. **OCR 图片**：提取文件内嵌图片，执行 OCR，原位嵌入上下文（不单独成独立章节）
3. **术语归一**：按 TERM_MAPPING.md 将所有产品名/平台名/专有名词替换为标准术语；未收录变体自动追加
4. **生成词条**：按 CLAUDE.md 定义的词条固定结构生成 Markdown 内容

### 5. 写入 Wiki（obsidian CLI）

```bash
# 新词条
obsidian vault=XXX-wiki create name="词条名" content="# 标题\n\n..."

# 更新已有词条（追加内容）
obsidian vault=XXX-wiki append file="词条名" content="..."

# 设置 frontmatter 属性
obsidian vault=XXX-wiki property:set file="词条名" name="updated" value="2026-06-13"
obsidian vault=XXX-wiki property:set file="词条名" name="source" value="raw/xxx.pptx"
```

### 6. 验证

```bash
# 检查未解析双向链接
obsidian vault=XXX-wiki unresolved

# 有未解析链接时，尝试修复（补建缺失词条或修正引用写法）
# 无法修复的标注 [UNCERTAIN]
```

### 7. 更新处理记录

将本次处理的文件信息写入 `XXX-wiki/raw/.processed_files.json`（hash、mtime、size、processed 日期）。

### 8. 反馈结果

报告：
- 处理文件数量（新增 / 修改）
- 新增词条列表
- 更新词条列表
- 待确认项（[UNCERTAIN]、[OCR-UNCERTAIN]）
- 未解析链接情况

## 异常处理

| 异常 | 处理方式 |
|------|----------|
| 找不到 CLAUDE.md | 立即提醒用户，停止执行 |
| raw/ 目录为空 | 提示用户放入原始文件 |
| obsidian CLI 不可用 | 提示用户检查 obsidian skill 配置 |
| unresolved 链接 | 优先修复；无法修复的标注 [UNCERTAIN] |
| TERM_MAPPING 未收录变体 | 语义判断后自动追加别名（同一概念才追加） |
| 大文件（>100MB PPTX） | 分批处理，注意内存 |

## 典型调用示例

**用户：** HZERO-wiki库新增了内容，你处理一下

**执行：**
1. 目标库：HZERO-wiki
2. 读取 HZERO-wiki/schema/CLAUDE.md + TERM_MAPPING.md
3. 扫描 raw/，发现 3 个新文件
4. 逐个提取文本 → 术语归一 → 生成词条
5. `obsidian vault=HZERO-wiki create` 写入 3 个词条
6. `obsidian vault=HZERO-wiki unresolved` 检查链接
7. 反馈：处理 3 个 PPTX，生成 3 个词条，0 个未解析链接
