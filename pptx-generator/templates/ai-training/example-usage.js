// 使用AI Training模板生成PPT的示例
const pptxgen = require("pptxgenjs");
const layouts = require("./layouts/slide-types");

const pres = new pptxgen();
pres.layout = 'LAYOUT_16x9';

// Slide 1: 封面
layouts.createCoverSlide(pres, {
  module: "模块1：AI新时代认知",
  title: "第1课 OpenClaw出现后，\nAI学习路线彻底变了",
  subtitle: "跳过原理，直接进入AI实战新时代",
  footer: "新一代AI实战培训班 · 只讲能用的实战技巧"
});

// Slide 2: 双栏对比（过去 vs 现在）
layouts.createTwoColumnSlide(pres, {
  pageNum: 2,
  title: "过去学AI vs 现在学AI",
  leftTitle: "过去：原理驱动",
  leftContent: "❌ 机器学习、深度学习算法\n❌ 大模型架构与训练\n❌ 需要数学和编程基础\n❌ 学习周期长，落地难",
  leftAccent: "e94560",
  rightTitle: "现在：应用驱动",
  rightContent: "✅ 直接上手用Agent\n✅ 任务场景→选Agent→配工具\n✅ 零基础可学，门槛低\n✅ 学完就能用，落地快",
  rightAccent: "00d9ff"
});

// Slide 3: 三点总结
layouts.createThreePointsSlide(pres, {
  pageNum: 3,
  title: "第1课总结",
  points: [
    { num: "1", title: "时代变化", desc: "OpenClaw出现，AI学习从原理层直接跳到应用层" },
    { num: "2", title: "学习重点", desc: "放弃啃原理，专注用Agent解决问题" },
    { num: "3", title: "核心目标", desc: "学会用AI提效，成为能落地的AI实战者" }
  ],
  footer: "🚀 开启AI实战之旅，让AI成为你的超级助手！"
});

// 输出
pres.writeFile({ fileName: './output/demo-using-template.pptx' })
  .then(() => console.log('✅ PPT generated: demo-using-template.pptx'))
  .catch(err => console.error('❌ Error:', err));