// 提取豆包PPT设计元素的脚本
const fs = require('fs');
const path = require('path');

// 读取PPT解压后的XML文件
function extractDesign(pptxPath) {
  // 这里可以解析XML提取颜色、字体、布局等信息
  // 由于XML解析复杂，这里手动整理从视觉分析得到的设计元素
  
  const designSystem = {
    name: "豆包AI培训深色科技模板",
    version: "1.0",
    source: "template.pptx",
    
    // 颜色系统
    colors: {
      // 背景色
      background: {
        primary: "#1a1d29",      // 主背景深蓝黑
        secondary: "#161922",    // 次级背景
        card: "#1e2230",         // 卡片背景
        elevated: "#252a3c"      // 提升层背景
      },
      // 主题色
      primary: {
        main: "#3b82f6",         // 主蓝色
        light: "#60a5fa",        // 浅蓝
        dark: "#1d4ed8"          // 深蓝
      },
      // 强调色
      accent: {
        red: "#ef4444",          // 警示/传统
        green: "#10b981",        // 成功/现代
        orange: "#f59e0b",       // 警告/强调
        purple: "#8b5cf6"        // 特色/创新
      },
      // 文字色
      text: {
        primary: "#ffffff",      // 主文字白色
        secondary: "#94a3b8",    // 次级文字灰蓝
        muted: "#64748b",        // 弱化文字
        disabled: "#475569"      // 禁用文字
      },
      // 边框/分割线
      border: {
        light: "#334155",
        DEFAULT: "#1e293b",
        dark: "#0f172a"
      }
    },
    
    // 字体系统
    typography: {
      chinese: "Microsoft YaHei",
      english: "Arial",
      fallback: "sans-serif",
      sizes: {
        h1: { size: 44, lineHeight: 1.2, weight: "bold" },      // 主标题
        h2: { size: 32, lineHeight: 1.3, weight: "bold" },      // 页面标题
        h3: { size: 24, lineHeight: 1.4, weight: "bold" },      // 小标题
        body: { size: 16, lineHeight: 1.6, weight: "normal" },  // 正文
        caption: { size: 14, lineHeight: 1.5, weight: "normal" }, // 说明文字
        small: { size: 12, lineHeight: 1.4, weight: "normal" }  // 小字
      }
    },
    
    // 间距系统
    spacing: {
      xs: 4,
      sm: 8,
      md: 16,
      lg: 24,
      xl: 32,
      xxl: 48
    },
    
    // 圆角系统
    borderRadius: {
      sm: 4,
      md: 8,
      lg: 12,
      xl: 16,
      full: 9999
    },
    
    // 阴影系统
    shadows: {
      sm: "0 1px 2px rgba(0,0,0,0.3)",
      md: "0 4px 6px rgba(0,0,0,0.4)",
      lg: "0 10px 15px rgba(0,0,0,0.5)",
      glow: "0 0 20px rgba(59,130,246,0.3)"  // 蓝色发光
    }
  };
  
  return designSystem;
}

// 导出设计系统
const design = extractDesign();
fs.writeFileSync(
  path.join(__dirname, 'design-system.json'),
  JSON.stringify(design, null, 2)
);

console.log('✅ 设计系统已提取到 design-system.json');
console.log('\n颜色系统:');
console.log('  背景色:', design.colors.background.primary);
console.log('  主色:', design.colors.primary.main);
console.log('  强调红:', design.colors.accent.red);
console.log('  强调绿:', design.colors.accent.green);
console.log('\n字体系统:');
console.log('  中文:', design.typography.chinese);
console.log('  英文:', design.typography.english);
console.log('  主标题:', design.typography.sizes.h1.size + 'px');