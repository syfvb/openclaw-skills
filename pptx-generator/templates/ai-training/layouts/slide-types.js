// AI Training Template - Slide Layout Definitions
// Based on template.pptx design system

const pptxgen = require("pptxgenjs");

// Theme configuration
const theme = {
  bg: "1a1a2e",
  bgDark: "161b33",
  primary: "0f3460",
  accent: "e94560",
  accentGreen: "00d9ff",
  textPrimary: "ffffff",
  textSecondary: "b8c5d6",
  textMuted: "6b7280",
  cardBg: "16213e",
  border: "2d3748"
};

// ==========================================
// Slide Type 1: Cover Page (封面页)
// ==========================================
function createCoverSlide(pres, data) {
  const slide = pres.addSlide();
  slide.background = { color: theme.bg };
  
  // 装饰线条
  slide.addShape(pres.shapes.LINE, {
    x: 0, y: 1.2, w: 10, h: 0,
    line: { color: theme.accent, width: 3 }
  });
  
  slide.addShape(pres.shapes.LINE, {
    x: 0, y: 4.8, w: 10, h: 0,
    line: { color: theme.border, width: 1 }
  });

  // 课程编号标签
  slide.addShape(pres.shapes.ROUNDED_RECTANGLE, {
    x: 0.5, y: 0.8, w: 2, h: 0.5,
    fill: { color: theme.accent },
    rectRadius: 0.1
  });
  slide.addText(data.module || "模块1", {
    x: 0.5, y: 0.8, w: 2, h: 0.5,
    fontSize: 14, fontFace: "Microsoft YaHei",
    color: "FFFFFF", bold: true,
    align: "center", valign: "middle"
  });

  // 主标题
  slide.addText(data.title || "主标题", {
    x: 0.5, y: 1.8, w: 9, h: 1.2,
    fontSize: 44, fontFace: "Microsoft YaHei",
    color: theme.textPrimary, bold: true, align: "left"
  });

  // 副标题
  if (data.subtitle) {
    slide.addText(data.subtitle, {
      x: 0.5, y: 3.2, w: 9, h: 0.6,
      fontSize: 20, fontFace: "Microsoft YaHei",
      color: theme.accent, align: "left"
    });
  }

  // 底部信息
  slide.addText(data.footer || "新一代AI实战培训班", {
    x: 0.5, y: 5.0, w: 9, h: 0.4,
    fontSize: 14, fontFace: "Microsoft YaHei",
    color: theme.textMuted, align: "left"
  });

  return slide;
}

// ==========================================
// Slide Type 2: Content - Two Column (双栏对比)
// ==========================================
function createTwoColumnSlide(pres, data) {
  const slide = pres.addSlide();
  slide.background = { color: theme.bg };

  // 页码
  addPageNumber(slide, pres, data.pageNum);

  // 标题
  slide.addText(data.title || "标题", {
    x: 0.5, y: 0.4, w: 9, h: 0.8,
    fontSize: 32, fontFace: "Microsoft YaHei",
    color: theme.textPrimary, bold: true, align: "left"
  });

  // 左栏
  slide.addShape(pres.shapes.RECTANGLE, {
    x: 0.5, y: 1.4, w: 4.3, h: 3.8,
    fill: { color: theme.cardBg },
    line: { color: data.leftAccent || theme.border, width: 2 }
  });

  slide.addText(data.leftTitle || "左侧标题", {
    x: 0.7, y: 1.55, w: 3.9, h: 0.5,
    fontSize: 18, fontFace: "Microsoft YaHei",
    color: data.leftAccent || theme.textSecondary, bold: true, align: "left"
  });

  slide.addText(data.leftContent || "左侧内容", {
    x: 0.7, y: 2.1, w: 3.9, h: 3.0,
    fontSize: 14, fontFace: "Microsoft YaHei",
    color: theme.textSecondary, align: "left"
  });

  // 右栏
  slide.addShape(pres.shapes.RECTANGLE, {
    x: 5.2, y: 1.4, w: 4.3, h: 3.8,
    fill: { color: theme.cardBg },
    line: { color: data.rightAccent || theme.accentGreen, width: 2 }
  });

  slide.addText(data.rightTitle || "右侧标题", {
    x: 5.4, y: 1.55, w: 3.9, h: 0.5,
    fontSize: 18, fontFace: "Microsoft YaHei",
    color: data.rightAccent || theme.accentGreen, bold: true, align: "left"
  });

  slide.addText(data.rightContent || "右侧内容", {
    x: 5.4, y: 2.1, w: 3.9, h: 3.0,
    fontSize: 14, fontFace: "Microsoft YaHei",
    color: theme.textSecondary, align: "left"
  });

  return slide;
}

// ==========================================
// Slide Type 3: Content - Three Points (三点总结)
// ==========================================
function createThreePointsSlide(pres, data) {
  const slide = pres.addSlide();
  slide.background = { color: theme.bg };

  // 页码
  addPageNumber(slide, pres, data.pageNum);

  // 标题
  slide.addText(data.title || "总结", {
    x: 0.5, y: 0.4, w: 9, h: 0.8,
    fontSize: 36, fontFace: "Microsoft YaHei",
    color: theme.textPrimary, bold: true, align: "left"
  });

  // 分隔线
  slide.addShape(pres.shapes.LINE, {
    x: 0.5, y: 1.3, w: 9, h: 0,
    line: { color: theme.accent, width: 2 }
  });

  // 三点内容
  const points = data.points || [
    { num: "1", title: "要点一", desc: "描述内容" },
    { num: "2", title: "要点二", desc: "描述内容" },
    { num: "3", title: "要点三", desc: "描述内容" }
  ];

  points.forEach((point, idx) => {
    const yPos = 1.6 + idx * 1.2;
    
    // 编号圆圈
    slide.addShape(pres.shapes.OVAL, {
      x: 0.8, y: yPos, w: 0.5, h: 0.5,
      fill: { color: theme.accent }
    });
    slide.addText(point.num, {
      x: 0.8, y: yPos, w: 0.5, h: 0.5,
      fontSize: 18, fontFace: "Arial",
      color: "FFFFFF", bold: true,
      align: "center", valign: "middle"
    });

    // 标题
    slide.addText(point.title, {
      x: 1.5, y: yPos + 0.05, w: 2, h: 0.4,
      fontSize: 20, fontFace: "Microsoft YaHei",
      color: theme.accent, bold: true, align: "left", valign: "middle"
    });

    // 描述
    slide.addText(point.desc, {
      x: 3.5, y: yPos + 0.05, w: 6, h: 0.4,
      fontSize: 16, fontFace: "Microsoft YaHei",
      color: theme.textSecondary, align: "left", valign: "middle"
    });
  });

  // 底部标语
  if (data.footer) {
    slide.addShape(pres.shapes.RECTANGLE, {
      x: 0.5, y: 5.0, w: 9, h: 0.5,
      fill: { color: theme.cardBg }
    });
    slide.addText(data.footer, {
      x: 0.5, y: 5.0, w: 9, h: 0.5,
      fontSize: 16, fontFace: "Microsoft YaHei",
      color: theme.accentGreen, bold: true,
      align: "center", valign: "middle"
    });
  }

  return slide;
}

// ==========================================
// Helper: Add Page Number (页码)
// ==========================================
function addPageNumber(slide, pres, num) {
  slide.addShape(pres.shapes.OVAL, {
    x: 9.3, y: 5.1, w: 0.4, h: 0.4,
    fill: { color: theme.primary }
  });
  slide.addText(String(num), {
    x: 9.3, y: 5.1, w: 0.4, h: 0.4,
    fontSize: 12, fontFace: "Arial",
    color: "FFFFFF", bold: true,
    align: "center", valign: "middle"
  });
}

// Export functions
module.exports = {
  theme,
  createCoverSlide,
  createTwoColumnSlide,
  createThreePointsSlide
};