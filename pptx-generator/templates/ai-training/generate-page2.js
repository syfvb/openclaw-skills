// 使用豆包模板生成第2页：过去的AI学习
const pptxgen = require("pptxgenjs");
const layouts = require("./layouts/slide-layouts");

const pres = new pptxgen();
pres.layout = 'LAYOUT_16x9';

// 第2页：过去的AI学习 - 使用双栏对比布局（左侧内容，右侧痛点）
layouts.createComparison(pres, {
  pageNum: 2,
  title: "过去学AI，我们在学什么？",
  subtitle: "（低效且难落地）",
  leftTitle: "1. 核心学习内容（全是底层原理）",
  leftContent: "• 机器学习：算法、模型、特征工程\n• 深度学习：神经网络、卷积、循环结构\n• 大模型相关：架构、训练、微调、部署",
  leftAccent: "3b82f6",
  rightTitle: "2. 学习痛点",
  rightContent: "❌ 门槛高：需要数学、编程基础，普通人难入门\n❌ 落地难：学完原理，还是不会用AI解决实际问题\n❌ 周期长：从入门到上手，需要几个月甚至半年",
  rightAccent: "ef4444"
});

// 输出
pres.writeFile({ fileName: './output/page2-past-ai-learning.pptx' })
  .then(() => {
    console.log('✅ 第2页PPT生成完成！');
    console.log('📁 文件: ./output/page2-past-ai-learning.pptx');
  })
  .catch(err => console.error('❌ 错误:', err));