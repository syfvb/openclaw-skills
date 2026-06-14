// 完整的PPT布局组件库 - 基于豆包AI培训模板
const pptxgen = require("pptxgenjs");
const design = require("../design-system.json");

// 颜色快捷访问
const C = design.colors;
const T = design.typography;

// ==========================================
// 工具函数
// ==========================================

// 添加页码
function addPageNumber(slide, pres, num, theme = {}) {
  const accent = theme.accent || C.primary.main.replace('#', '');
  slide.addShape(pres.shapes.OVAL, {
    x: 9.3, y: 5.15, w: 0.35, h: 0.35,
    fill: { color: accent }
  });
  slide.addText(String(num), {
    x: 9.3, y: 5.15, w: 0.35, h: 0.35,
    fontSize: 10, fontFace: T.english,
    color: "FFFFFF", bold: true,
    align: "center", valign: "middle"
  });
}

// 添加装饰线条
function addDecorativeLine(slide, pres, x, y, width, color, thickness = 2) {
  slide.addShape(pres.shapes.LINE, {
    x, y, w: width, h: 0,
    line: { color, width: thickness }
  });
}

// 添加渐变背景效果（模拟）
function addGradientOverlay(slide, theme) {
  // 使用半透明形状模拟渐变
  slide.addShape(pres.shapes.RECTANGLE, {
    x: 0, y: 0, w: 10, h: 5.625,
    fill: { color: theme.bg || C.background.primary.replace('#', '') }
  });
}

// ==========================================
// Layout 1: 封面页 (Cover)
// ==========================================
function createCover(pres, data) {
  const slide = pres.addSlide();
  const bg = C.background.primary.replace('#', '');
  const accent = C.primary.main.replace('#', '');
  
  // 背景
  slide.background = { color: bg };
  
  // 顶部装饰线
  addDecorativeLine(slide, pres, 0, 1.0, 10, accent, 3);
  
  // 模块标签
  slide.addShape(pres.shapes.ROUNDED_RECTANGLE, {
    x: 0.5, y: 0.6, w: 2.2, h: 0.45,
    fill: { color: accent },
    rectRadius: 0.08
  });
  slide.addText(data.module || "模块1", {
    x: 0.5, y: 0.6, w: 2.2, h: 0.45,
    fontSize: 13, fontFace: T.chinese,
    color: "FFFFFF", bold: true,
    align: "center", valign: "middle"
  });
  
  // 主标题
  slide.addText(data.title || "主标题", {
    x: 0.5, y: 1.5, w: 9, h: 1.5,
    fontSize: T.sizes.h1.size, fontFace: T.chinese,
    color: C.text.primary.replace('#', ''), bold: true,
    align: "left"
  });
  
  // 副标题
  if (data.subtitle) {
    slide.addText(data.subtitle, {
      x: 0.5, y: 3.1, w: 9, h: 0.8,
      fontSize: T.sizes.h3.size, fontFace: T.chinese,
      color: C.text.secondary.replace('#', ''),
      align: "left"
    });
  }
  
  // 底部信息
  slide.addText(data.footer || "AI实战培训班", {
    x: 0.5, y: 5.0, w: 9, h: 0.4,
    fontSize: T.sizes.caption.size, fontFace: T.chinese,
    color: C.text.muted.replace('#', ''),
    align: "left"
  });
  
  // 底部装饰线
  addDecorativeLine(slide, pres, 0.5, 4.9, 9, C.border.light.replace('#', ''), 1);
  
  return slide;
}

// ==========================================
// Layout 2: 双栏对比页 (Comparison)
// ==========================================
function createComparison(pres, data) {
  const slide = pres.addSlide();
  const bg = C.background.primary.replace('#', '');
  const accentRed = C.accent.red.replace('#', '');
  const accentGreen = C.accent.green.replace('#', '');
  
  slide.background = { color: bg };
  addPageNumber(slide, pres, data.pageNum || 2);
  
  // 标题
  slide.addText(data.title || "标题", {
    x: 0.5, y: 0.3, w: 9, h: 0.8,
    fontSize: T.sizes.h2.size, fontFace: T.chinese,
    color: C.text.primary.replace('#', ''), bold: true,
    align: "left"
  });
  
  // 副标题
  if (data.subtitle) {
    slide.addText(data.subtitle, {
      x: 0.5, y: 1.0, w: 9, h: 0.5,
      fontSize: T.sizes.body.size, fontFace: T.chinese,
      color: C.text.secondary.replace('#', ''),
      align: "left"
    });
  }
  
  // 左栏（传统/过去）- 红色系
  slide.addShape(pres.shapes.RECTANGLE, {
    x: 0.4, y: 1.6, w: 4.4, h: 3.6,
    fill: { color: C.background.card.replace('#', '') },
    line: { color: accentRed, width: 2 }
  });
  
  slide.addText(data.leftTitle || "左侧标题", {
    x: 0.6, y: 1.75, w: 4.0, h: 0.5,
    fontSize: T.sizes.h3.size, fontFace: T.chinese,
    color: accentRed, bold: true, align: "left"
  });
  
  slide.addText(data.leftContent || "左侧内容", {
    x: 0.6, y: 2.3, w: 4.0, h: 2.8,
    fontSize: T.sizes.body.size, fontFace: T.chinese,
    color: C.text.secondary.replace('#', ''), align: "left"
  });
  
  // 右栏（现代/现在）- 绿色系
  slide.addShape(pres.shapes.RECTANGLE, {
    x: 5.2, y: 1.6, w: 4.4, h: 3.6,
    fill: { color: C.background.card.replace('#', '') },
    line: { color: accentGreen, width: 2 }
  });
  
  slide.addText(data.rightTitle || "右侧标题", {
    x: 5.4, y: 1.75, w: 4.0, h: 0.5,
    fontSize: T.sizes.h3.size, fontFace: T.chinese,
    color: accentGreen, bold: true, align: "left"
  });
  
  slide.addText(data.rightContent || "右侧内容", {
    x: 5.4, y: 2.3, w: 4.0, h: 2.8,
    fontSize: T.sizes.body.size, fontFace: T.chinese,
    color: C.text.secondary.replace('#', ''), align: "left"
  });
  
  return slide;
}

// ==========================================
// Layout 3: 三栏要点页 (Three Points)
// ==========================================
function createThreePoints(pres, data) {
  const slide = pres.addSlide();
  const bg = C.background.primary.replace('#', '');
  const accent = C.primary.main.replace('#', '');
  
  slide.background = { color: bg };
  addPageNumber(slide, pres, data.pageNum || 3);
  
  // 标题
  slide.addText(data.title || "核心要点", {
    x: 0.5, y: 0.3, w: 9, h: 0.8,
    fontSize: T.sizes.h2.size, fontFace: T.chinese,
    color: C.text.primary.replace('#', ''), bold: true,
    align: "left"
  });
  
  // 分隔线
  addDecorativeLine(slide, pres, 0.5, 1.1, 9, accent, 2);
  
  const points = data.points || [];
  const colors = [
    C.accent.red.replace('#', ''),
    C.accent.green.replace('#', ''),
    C.accent.orange.replace('#', '')
  ];
  
  points.forEach((point, idx) => {
    const yPos = 1.4 + idx * 1.3;
    const color = colors[idx % colors.length];
    
    // 编号圆圈
    slide.addShape(pres.shapes.OVAL, {
      x: 0.6, y: yPos, w: 0.5, h: 0.5,
      fill: { color }
    });
    slide.addText(String(idx + 1), {
      x: 0.6, y: yPos, w: 0.5, h: 0.5,
      fontSize: 18, fontFace: T.english,
      color: "FFFFFF", bold: true,
      align: "center", valign: "middle"
    });
    
    // 标题
    slide.addText(point.title, {
      x: 1.3, y: yPos, w: 2.5, h: 0.5,
      fontSize: T.sizes.h3.size, fontFace: T.chinese,
      color, bold: true, align: "left", valign: "middle"
    });
    
    // 描述
    slide.addText(point.desc, {
      x: 4.0, y: yPos, w: 5.5, h: 0.5,
      fontSize: T.sizes.body.size, fontFace: T.chinese,
      color: C.text.secondary.replace('#', ''),
      align: "left", valign: "middle"
    });
  });
  
  return slide;
}

// ==========================================
// Layout 4: 列表详情页 (List Detail)
// ==========================================
function createListDetail(pres, data) {
  const slide = pres.addSlide();
  const bg = C.background.primary.replace('#', '');
  const accent = C.primary.main.replace('#', '');
  
  slide.background = { color: bg };
  addPageNumber(slide, pres, data.pageNum || 4);
  
  // 标题
  slide.addText(data.title || "标题", {
    x: 0.5, y: 0.3, w: 9, h: 0.8,
    fontSize: T.sizes.h2.size, fontFace: T.chinese,
    color: C.text.primary.replace('#', ''), bold: true,
    align: "left"
  });
  
  // 副标题
  if (data.subtitle) {
    slide.addText(data.subtitle, {
      x: 0.5, y: 1.0, w: 9, h: 0.5,
      fontSize: T.sizes.body.size, fontFace: T.chinese,
      color: C.text.secondary.replace('#', ''),
      align: "left"
    });
  }
  
  // 列表项
  const items = data.items || [];
  items.forEach((item, idx) => {
    const yPos = 1.6 + idx * 0.6;
    
    // 项目符号
    slide.addShape(pres.shapes.OVAL, {
      x: 0.6, y: yPos + 0.15, w: 0.12, h: 0.12,
      fill: { color: accent }
    });
    
    // 内容
    slide.addText(item, {
      x: 0.9, y: yPos, w: 8.6, h: 0.5,
      fontSize: T.sizes.body.size, fontFace: T.chinese,
      color: C.text.secondary.replace('#', ''),
      align: "left", valign: "middle"
    });
  });
  
  return slide;
}

// ==========================================
// Layout 5: 流程步骤页 (Process Steps)
// ==========================================
function createProcessSteps(pres, data) {
  const slide = pres.addSlide();
  const bg = C.background.primary.replace('#', '');
  const accent = C.primary.main.replace('#', '');
  
  slide.background = { color: bg };
  addPageNumber(slide, pres, data.pageNum || 5);
  
  // 标题
  slide.addText(data.title || "流程", {
    x: 0.5, y: 0.3, w: 9, h: 0.8,
    fontSize: T.sizes.h2.size, fontFace: T.chinese,
    color: C.text.primary.replace('#', ''), bold: true,
    align: "left"
  });
  
  // 步骤
  const steps = data.steps || [];
  const stepWidth = 9 / steps.length;
  
  steps.forEach((step, idx) => {
    const xPos = 0.5 + idx * stepWidth;
    const colors = [C.accent.red, C.accent.orange, C.accent.green, C.primary.main];
    const color = colors[idx % colors.length].replace('#', '');
    
    // 步骤框
    slide.addShape(pres.shapes.ROUNDED_RECTANGLE, {
      x: xPos + 0.1, y: 1.8, w: stepWidth - 0.2, h: 2.5,
      fill: { color: C.background.card.replace('#', '') },
      line: { color, width: 2 },
      rectRadius: 0.1
    });
    
    // 步骤编号
    slide.addShape(pres.shapes.OVAL, {
      x: xPos + stepWidth/2 - 0.25, y: 1.4, w: 0.5, h: 0.5,
      fill: { color }
    });
    slide.addText(String(idx + 1), {
      x: xPos + stepWidth/2 - 0.25, y: 1.4, w: 0.5, h: 0.5,
      fontSize: 18, fontFace: T.english,
      color: "FFFFFF", bold: true,
      align: "center", valign: "middle"
    });
    
    // 步骤标题
    slide.addText(step.title, {
      x: xPos + 0.2, y: 2.0, w: stepWidth - 0.4, h: 0.6,
      fontSize: T.sizes.h3.size, fontFace: T.chinese,
      color, bold: true, align: "center"
    });
    
    // 步骤描述
    slide.addText(step.desc, {
      x: xPos + 0.2, y: 2.7, w: stepWidth - 0.4, h: 1.5,
      fontSize: T.sizes.body.size - 2, fontFace: T.chinese,
      color: C.text.secondary.replace('#', ''),
      align: "center"
    });
    
    // 箭头（除了最后一个）
    if (idx < steps.length - 1) {
      slide.addText("→", {
        x: xPos + stepWidth - 0.15, y: 2.8, w: 0.3, h: 0.5,
        fontSize: 24, fontFace: T.english,
        color: C.text.muted.replace('#', ''),
        align: "center", valign: "middle"
      });
    }
  });
  
  return slide;
}

// ==========================================
// Layout 6: 总结页 (Summary)
// ==========================================
function createSummary(pres, data) {
  const slide = pres.addSlide();
  const bg = C.background.primary.replace('#', '');
  const accent = C.primary.main.replace('#', '');
  
  slide.background = { color: bg };
  addPageNumber(slide, pres, data.pageNum || 7);
  
  // 标题
  slide.addText(data.title || "总结", {
    x: 0.5, y: 0.3, w: 9, h: 0.8,
    fontSize: T.sizes.h2.size, fontFace: T.chinese,
    color: C.text.primary.replace('#', ''), bold: true,
    align: "left"
  });
  
  // 分隔线
  addDecorativeLine(slide, pres, 0.5, 1.1, 9, accent, 2);
  
  // 总结卡片
  slide.addShape(pres.shapes.ROUNDED_RECTANGLE, {
    x: 0.5, y: 1.4, w: 9, h: 1.2,
    fill: { color: C.background.card.replace('#', '') },
    line: { color: accent, width: 2 },
    rectRadius: 0.1
  });
  
  slide.addText(data.mainPoint || "核心转变", {
    x: 0.7, y: 1.55, w: 8.6, h: 0.9,
    fontSize: T.sizes.h3.size, fontFace: T.chinese,
    color: C.text.primary.replace('#', ''), bold: true,
    align: "left"
  });
  
  // 子要点
  const subPoints = data.subPoints || [];
  subPoints.forEach((point, idx) => {
    const yPos = 2.8 + idx * 0.8;
    
    slide.addText("✓", {
      x: 0.7, y: yPos, w: 0.4, h: 0.5,
      fontSize: 20, fontFace: T.english,
      color: C.accent.green.replace('#', ''),
      align: "left", valign: "middle"
    });
    
    slide.addText(point, {
      x: 1.2, y: yPos, w: 8.3, h: 0.5,
      fontSize: T.sizes.body.size, fontFace: T.chinese,
      color: C.text.secondary.replace('#', ''),
      align: "left", valign: "middle"
    });
  });
  
  return slide;
}

// ==========================================
// Layout 7: 预告页 (Preview)
// ==========================================
function createPreview(pres, data) {
  const slide = pres.addSlide();
  const bg = C.background.primary.replace('#', '');
  const accent = C.accent.orange.replace('#', '');
  
  slide.background = { color: bg };
  addPageNumber(slide, pres, data.pageNum || 8, { accent });
  
  // 标题
  slide.addText(data.title || "下节课预告", {
    x: 0.5, y: 0.3, w: 9, h: 0.8,
    fontSize: T.sizes.h2.size, fontFace: T.chinese,
    color: accent, bold: true,
    align: "left"
  });
  
  // 预告卡片
  slide.addShape(pres.shapes.ROUNDED_RECTANGLE, {
    x: 0.5, y: 1.2, w: 9, h: 3.5,
    fill: { color: C.background.card.replace('#', '') },
    line: { color: accent, width: 2 },
    rectRadius: 0.1
  });
  
  // 课程标题
  slide.addText(data.nextLesson || "第2课：标题", {
    x: 0.7, y: 1.4, w: 8.6, h: 0.7,
    fontSize: T.sizes.h3.size, fontFace: T.chinese,
    color: C.text.primary.replace('#', ''), bold: true,
    align: "left"
  });
  
  // 课程描述
  slide.addText(data.description || "课程描述", {
    x: 0.7, y: 2.1, w: 8.6, h: 0.8,
    fontSize: T.sizes.body.size, fontFace: T.chinese,
    color: C.text.secondary.replace('#', ''),
    align: "left"
  });
  
  // 学习要点
  const points = data.points || [];
  points.forEach((point, idx) => {
    const yPos = 3.0 + idx * 0.5;
    
    slide.addText("▸", {
      x: 0.9, y: yPos, w: 0.3, h: 0.4,
      fontSize: 16, fontFace: T.english,
      color: accent,
      align: "left", valign: "middle"
    });
    
    slide.addText(point, {
      x: 1.3, y: yPos, w: 8.2, h: 0.4,
      fontSize: T.sizes.body.size - 2, fontFace: T.chinese,
      color: C.text.secondary.replace('#', ''),
      align: "left", valign: "middle"
    });
  });
  
  return slide;
}

// 导出所有布局函数
module.exports = {
  // 工具函数
  addPageNumber,
  addDecorativeLine,
  
  // 布局函数
  createCover,
  createComparison,
  createThreePoints,
  createListDetail,
  createProcessSteps,
  createSummary,
  createPreview
};