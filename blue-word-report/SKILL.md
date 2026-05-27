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

1. Run `bash ~/.openclaw/skills/minimax-docx/scripts/env_check.sh` — must say READY.
2. Content source: a Markdown file path, or content from conversation history.

## Workflow

### 1. Prepare content

* Read the source Markdown file (or extract from conversation).
* **If content is from a URL:** `web_fetch` 有 20000 字符硬限制，长文章会被截断。必须用 `curl` 下载完整 HTML，再用 Python 脚本提取正文：
  ```bash
  curl -s <URL> > /tmp/page.html && python3 -c "
  import re, sys
  html = open('/tmp/page.html').read()
  text = re.sub(r'<[^>]+>', '\n', html)
  text = re.sub(r'\n\s*\n', '\n\n', text).strip()
  print(text[:100000])
  " > /tmp/content.txt
  ```
* Identify all headings (H1/H2/H3), tables, and body text.
* Extract the document title (first H1 or file name).

### 2. Write C# script (gen.csx)

Write a single `gen.csx` file in the workspace. If the script exceeds ~10KB, use `write` for the framework then `edit` to append content sections.

**Script template** (copy from `references/script-template.csx`):

```bash
cp ~/.openclaw/skills/blue-word-report/references/script-template.csx /root/.openclaw/workspace/gen.csx
```

Then edit the template:

* Replace the `// === CONTENT ===` section with actual document content
* Map Markdown H1 → `P(body, "Heading 1", "...")`, H2 → `Heading 2`, H3 → `Heading 3`
* Convert Markdown tables → `Table(body, headers, rows)`
* Convert body text → `P(body, "Normal", "...")`

### 3. Execute

```bash
cd /root/.openclaw/workspace && dotnet-script gen.csx
```

### 4. Verify

```bash
ls -lh gaokao_volunteer_report.docx  # or the output filename
file *.docx  # should say "Microsoft Word 2007+"
```

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
9. **`web_fetch` 有 20000 字符硬限制** — 长文章必须用 `curl` + Python 提取完整内容，否则内容被截断导致遗漏关键信息

## Model Selection

* For documents under 30K content: any model works
* For documents over 30K content: **use qwen3.5-plus** (1M context window)
* GLM-5 (200K context) tends to simplify or skip formatting on long documents

## Output

Save to `/root/.openclaw/workspace/<report-name>.docx`