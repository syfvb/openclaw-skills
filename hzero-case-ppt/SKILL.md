---
name: hzero-case-ppt
description: "从素材来源提取案例内容，按模板格式生成HZERO案例单页PPT。使用python-pptx库保留母版样式，按段落索引精确替换文本。触发词：案例PPT、标杆案例、生成案例、PPT模板填充。"
license: MIT
---

# HZERO案例PPT生成技能

## 核心技术要点

### ✅ 推荐方案：python-pptx

|方案|母版保留|代码复杂度|可靠性|推荐|
|-|-|-|-|-|
|**python-pptx**|✅ 完整保留|⭐⭐⭐⭐ 简洁|高|✅ **推荐**|
|PPTxGenJS|❌ 丢失|⭐⭐⭐|低|❌|
|XML手动编辑|✅ 保留|⭐⭐⭐⭐⭐ 极复杂|低|❌|

### 关键原理

**PPTX文本结构**：模板中每个段落（paragraph）包含多个 run 元素，文本被拆分存储。

**正确替换方法**：

```python
def set_paragraph_text(paragraph, new_text):
    """清空其他run，在第一个run设置完整文本（保留样式）"""
    runs = paragraph.runs
    if runs:
        runs[0].text = new_text  # 保留第一个run的样式
        for run in runs[1:]:
            run.text = ''  # 清空其他run
```

**错误方法**：

* ❌ 文本匹配替换（模板有重复文本如"痛点一"）
* ❌ 直接替换 run.text（文本被拆分导致不完整）

---

## 模板文件

**路径**：`template.pptx`

**模板结构**：

* 页面尺寸：13.333" × 7.5"（非标准，从 presentation.xml 提取）
* 段落总数：54个
* 形状总数：15个
* 母版：2个
* 主题：3个

---

## 段落索引映射（固定不变）

|段落索引|区域|模板文本|替换内容|
|-|-|-|-|
|0|标题|标杆案例-XXX客户XXX智能体|标杆案例-{客户}{场景}智能体|
|1|客户信息|一句话客户行业和地位：XXX客户是XXX领域的领军企业。|一句话客户行业和地位：{行业地位}|
|2|客户信息|客户应用后收益亮点（高亮标红）...|客户应用后收益亮点：{收益亮点}|
|4|痛点区域|客户在XXX业务域有以下痛点：|客户在{业务域}业务域有以下痛点：|
|7|痛点一|痛点一：XXXX|痛点一：{痛点描述}|
|9|痛点二|痛点二：XXXX|痛点二：{痛点描述}|
|11|痕点三|痛点一：XXXX（模板写错）|痛点三：{痛点描述}|
|13|痛点四|痛点一：XXXX（模板写错）|痛点四：{痛点描述}|
|16|收益区域|应用后有如下收益：|应用后有如下收益：|
|19|收益一|收益一：XXXX|收益一：{收益描述}|
|21|收益二|收益二：XXXX|收益二：{收益描述}|
|23|收益三|收益三：XXXX|收益三：{收益描述}|
|25|收益四|收益四：XXXX|收益四：{收益描述}|
|37|LOGO区域|客户LOGO|{客户名称}|
|38|LOGO说明|需要客户授权|清空|
|31|多余文本|痛点一|清空|

**解决方案区域**：

* 找到包含"示意图"的段落 → 替换为方案标题
* 找到包含"核心步骤"的段落 → 替换为方案描述
* 按出现顺序：方案1标题、方案1描述、方案2标题、方案2描述...

---

## 文本长度限制（模板硬性要求）

|区域|长度限制|特殊要求|
|-|-|-|
|行业地位|≤40字符|一句话描述客户行业和地位|
|收益亮点|≤40字符|用↑↓替代"提升/下降"|
|痛点描述|≤25字符|每条痛点独立描述|
|收益描述|≤25字符|**必须包含量化数字**|
|方案描述|≤30字符|方案核心特点描述|

---

## 完整代码模板

```python
from pptx import Presentation
import os

# 案例素材
case_data = {
    '客户名称': '{客户名称}',
    '标题': '标杆案例-{客户名称}{场景名称}智能体',
    '客户行业地位': '{行业地位，≤40字符}',
    '收益亮点': '{收益亮点，≤40字符，用↑↓}',
    '业务域': '{业务域}',
    '痛点': [
        '{痛点一，≤25字符}',
        '{痛点二，≤25字符}',
        '{痛点三，≤25字符}',
        '{痛点四，≤25字符}'
    ],
    '收益': [
        '{收益一，≤25字符，含数字}',
        '{收益二，≤25字符}',
        '{收益三，≤25字符}',
        '{收益四，≤25字符}'
    ],
    '解决方案': [
        ('{方案1标题}', '{方案1描述，≤30字符}'),
        ('{方案2标题}', '{方案2描述，≤30字符}'),
        ('{方案3标题}', '{方案3描述，≤30字符}'),
        ('{方案4标题}', '{方案4描述，≤30字符}'),
        ('{方案5标题}', '{方案5描述，≤30字符}')
    ]
}

# 加载模板
template_path = os.path.expanduser('~/.openclaw/skills/hzero-case-ppt/template.pptx')
output_path = 'output/{客户名称}-{场景名称}案例.pptx'

prs = Presentation(template_path)
slide = prs.slides[0]

# 替换段落文本函数
def set_paragraph_text(paragraph, new_text):
    runs = paragraph.runs
    if runs:
        runs[0].text = new_text
        for run in runs[1:]:
            run.text = ''

# 收集所有段落
all_paragraphs = []
for shape in slide.shapes:
    if shape.has_text_frame:
        for para in shape.text_frame.paragraphs:
            all_paragraphs.append(para)

# 段落索引替换映射
para_replacements = {
    0: case_data['标题'],
    1: f"一句话客户行业和地位：{case_data['客户行业地位']}",
    2: f"客户应用后收益亮点：{case_data['收益亮点']}",
    4: f"客户在{case_data['业务域']}业务域有以下痛点：",
    7: f"痛点一：{case_data['痛点'][0]}",
    9: f"痛点二：{case_data['痛点'][1]}",
    11: f"痛点三：{case_data['痛点'][2]}",
    13: f"痛点四：{case_data['痛点'][3]}",
    16: "应用后有如下收益：",
    19: f"收益一：{case_data['收益'][0]}",
    21: f"收益二：{case_data['收益'][1]}",
    23: f"收益三：{case_data['收益'][2]}",
    25: f"收益四：{case_data['收益'][3]}",
    37: case_data['客户名称'],
    38: "",
    31: "",
}

# 执行替换
for para_idx, new_text in para_replacements.items():
    if para_idx < len(all_paragraphs):
        set_paragraph_text(all_paragraphs[para_idx], new_text)

# 替换解决方案区域
solution_idx = 0
for para in all_paragraphs:
    full_text = ''.join([run.text for run in para.runs])
    if '示意图' in full_text and solution_idx < 5:
        set_paragraph_text(para, case_data['解决方案'][solution_idx][0])
        solution_idx += 1
    elif '核心步骤' in full_text and solution_idx <= 5:
        idx = solution_idx - 1
        if idx < 5:
            set_paragraph_text(para, case_data['解决方案'][idx][1])

# 保存
prs.save(output_path)
print(f"✅ PPT生成成功: {output_path}")
```

---

## 验证检查清单

生成后必须验证：

```python
import zipfile

with zipfile.ZipFile(output_path, 'r') as z:
    files = z.namelist()

    # 1. 母版检查
    assert any('slideMaster' in f for f in files), "母版丢失"

    # 2. 主题检查
    assert any('theme' in f for f in files), "主题丢失"

    # 3. 文件大小检查
    file_size = os.path.getsize(output_path)
    assert file_size > 100000, f"文件过小: {file_size}"

print("✅ 所有验证通过")
```

---

## 素材来源处理

### 1. PPT文件提取

```bash
python -m markitdown {PPT文件路径}
```

### 2. Word文件提取

```bash
python -m markitdown {Word文件路径}
```

### 3. 推文链接提取

检查远程浏览器是否已经启动，如果没启动提示用户启动，然后用 browser 工具访问链接并提取内容。

### 4. PPT内嵌视频提取

参考 `video-summarizer` 技能：

1. ffmpeg 提取关键帧
2. image 模型分析帧内容
3. 提取语音转文字（audio-transcription 或 funasr）

---

## 常见问题排查

|问题|原因|解决方案|
|-|-|-|
|母版丢失|使用PPTxGenJS|改用python-pptx|
|文本未替换|直接替换run.text|清空其他run，只设置第一个|
|痛点三四错误|模板写的是"痛点一"|按段落索引11、13替换|
|文本长度超限|未检查长度|严格控制在限制内|
|验证失败|markitdown提取不完整|用python-pptx直接读取|

---

## 提示词模板

### 完整版（首次使用或复杂案例）

```markdown
## 任务
从素材来源提取案例内容，按模板格式生成单页PPT。

## 素材来源
- 文件：{PPT/Word/推文链接}

## 模板文件
`template.pptx`

## 技术要求
使用 python-pptx 库，按段落索引替换，清空其他run保留样式。

## 案例素材（严格控制长度）
**客户名称**：{客户名称}
**标题**：标杆案例-{客户}{场景}智能体
**客户行业地位**：{≤40字符}
**收益亮点**：{≤40字符，用↑↓}
**业务域**：{业务域}
**痛点**：{4条，每条≤25字符}
**收益**：{4条，每条≤25字符，含数字}
**解决方案**：{5个，描述≤30字符}

## 输出文件
`output/{客户名称}-{场景名称}案例.pptx`

## 验证要求
检查母版、主题、文件大小≈112KB
```

### 简化版（日常快速使用）

```markdown
从{素材来源}提取案例，按模板生成PPT。

模板：`template.pptx`

要求：python-pptx + 段落索引替换 + 长度限制（行业≤40，痛点/收益≤25含数字，方案≤30）

输出：`output/{客户}-{场景}案例.pptx`
```

---

## 版本记录

* **2026-05-19**：首次创建，验证python-pptx方案可靠性
* 模板段落索引映射固定（54段落，15形状）
* 文本长度限制基于模板硬性要求

---

## 相关技能

* `pptx-generator`：PPTX生成基础技能
* `video-summarizer`：视频内容提取
* `audio-transcription`：语音转文字
* `browser-automation`：推文内容提取