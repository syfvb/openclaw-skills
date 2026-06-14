# AI Training PPT 模板

基于豆包生成的精美PPT设计提取的代码模板。

## 目录结构

```
~/.openclaw/skills/pptx-generator/templates/ai-training/
├── README.md                    # 本文件
├── template.pptx               # 原始模板文件（参考用）
├── colors/
│   └── theme.json              # 配色方案
├── layouts/
│   └── slide-types.js          # 布局组件
├── assets/                     # 图片资源（预留）
└── example-usage.js            # 使用示例
```

## 设计特点

- **风格**：深色科技风
- **配色**：深蓝黑背景 + 科技红强调色
- **字体**：Microsoft YaHei（中文）+ Arial（英文）
- **适用场景**：AI培训、技术分享、产品发布

## 可用布局组件

### 1. createCoverSlide(pres, data)
封面页布局

**参数：**
- `module`: 模块名称（如"模块1：AI新时代认知"）
- `title`: 主标题
- `subtitle`: 副标题
- `footer`: 底部信息

### 2. createTwoColumnSlide(pres, data)
双栏对比布局（适合过去vs现在、传统vs创新等对比场景）

**参数：**
- `pageNum`: 页码
- `title`: 页面标题
- `leftTitle`: 左侧标题
- `leftContent`: 左侧内容（支持换行）
- `leftAccent`: 左侧强调色（可选）
- `rightTitle`: 右侧标题
- `rightContent`: 右侧内容
- `rightAccent`: 右侧强调色（可选）

### 3. createThreePointsSlide(pres, data)
三点总结布局

**参数：**
- `pageNum`: 页码
- `title`: 页面标题
- `points`: 要点数组 [{num, title, desc}]
- `footer`: 底部标语（可选）

## 使用示例

```javascript
const pptxgen = require("pptxgenjs");
const layouts = require("./layouts/slide-types");

const pres = new pptxgen();
pres.layout = 'LAYOUT_16x9';

// 创建封面
layouts.createCoverSlide(pres, {
  module: "模块1：AI新时代认知",
  title: "第1课 OpenClaw出现后，AI学习路线彻底变了",
  subtitle: "跳过原理，直接进入AI实战新时代",
  footer: "新一代AI实战培训班"
});

// 输出
pres.writeFile({ fileName: 'output.pptx' });
```

## 完整示例

见 `example-usage.js`

## 配色参考

```json
{
  "background": "#1a1a2e",      // 主背景色
  "backgroundDark": "#161b33",   // 深色背景
  "primary": "#0f3460",          // 主色调
  "accent": "#e94560",           // 强调色（科技红）
  "accentGreen": "#00d9ff",      // 成功/正面色（科技青）
  "textPrimary": "#ffffff",      // 主文字
  "textSecondary": "#b8c5d6",    // 次级文字
  "cardBg": "#16213e"            // 卡片背景
}
```

## 扩展建议

如需更多布局，可参考 `template.pptx` 中的设计，提取新的布局组件添加到 `slide-types.js`。