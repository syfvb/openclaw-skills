// 使用豆包模板生成完整的第1课PPT
const pptxgen = require("pptxgenjs");
const layouts = require("./layouts/slide-layouts");

const pres = new pptxgen();
pres.layout = 'LAYOUT_16x9';

// Slide 1: 封面
layouts.createCover(pres, {
  module: "模块1：AI新时代认知",
  title: "第1课 OpenClaw出现后，\nAI学习路线彻底变了",
  subtitle: '从"学习原理"到"应用实战"的范式转移',
  footer: "新一代AI实战培训班 · 只讲能用的实战技巧"
});

// Slide 2: 范式转移对比
layouts.createComparison(pres, {
  pageNum: 2,
  title: "OpenClaw出现后，AI学习路线彻底变了",
  subtitle: '从"学习原理"到"应用实战"的范式转移',
  leftTitle: "传统模式：重原理，轻实践",
  leftContent: "❌ 需要掌握复杂的数学公式与底层算法\n❌ 学习周期长，门槛高\n❌ 学完原理还是不会用",
  rightTitle: "OpenClaw 新模式：即学即用",
  rightContent: "✅ 利用工具直接解决实际问题\n✅ 聚焦应用场景，快速产出价值\n✅ 零基础也能快速上手"
});

// Slide 3: 核心要点
layouts.createThreePoints(pres, {
  pageNum: 3,
  title: "本节课核心要点",
  points: [
    { 
      title: "过去学AI", 
      desc: "原理驱动（机器学习、大模型架构）" 
    },
    { 
      title: "现在学AI", 
      desc: "应用驱动（Agent、工具、指令）" 
    },
    { 
      title: "未来竞争力", 
      desc: "解决问题的能力（AI自动化工程师）" 
    }
  ]
});

// Slide 4: 过去学AI的详细列表
layouts.createListDetail(pres, {
  pageNum: 4,
  title: "过去学AI：漫长的原理之路",
  subtitle: "就像在复杂的迷宫中寻找出口",
  items: [
    "机器学习算法（SVM, Random Forest, K-means）",
    "深度学习框架（TensorFlow, PyTorch）",
    "神经网络结构（CNN, RNN, Transformer）",
    "大模型架构与训练（预训练, 千亿参数）",
    "模型微调与部署（Fine-tuning, TensorRT）",
    "数据处理与特征工程（ETL, 归一化, 清洗）"
  ]
});

// Slide 5: 现在学AI的流程
layouts.createProcessSteps(pres, {
  pageNum: 5,
  title: "现在学AI：直接上手解决问题",
  steps: [
    { title: "任务场景", desc: "明确要解决的\n实际问题与目标" },
    { title: "选Agent", desc: "选择合适的\nAI智能体工具" },
    { title: "配工具", desc: "配置所需的\n技能插件与API" },
    { title: "写指令", desc: "设计清晰的\nPrompt指令集" },
    { title: "跑闭环", desc: "自动执行、\n验证结果" }
  ]
});

// Slide 6: 未来核心竞争力
layouts.createComparison(pres, {
  pageNum: 6,
  title: "未来核心竞争力：AI自动化工程师",
  subtitle: "传统能力 VS 未来能力",
  leftTitle: "淘汰型能力（不用再花大量时间学）",
  leftContent: '❌ "懂AI原理"：机器会自动优化\n❌ "会写基础代码"：AI能自动生成代码',
  leftAccent: "ef4444",
  rightTitle: "核心竞争力（必须掌握）",
  rightContent: '✅ 不是"懂AI"，而是"会用AI解决问题"\n✅ 不是"会写代码"，而是"会让AI自动写代码"',
  rightAccent: "10b981"
});

// Slide 7: 课程总结
layouts.createSummary(pres, {
  pageNum: 7,
  title: "课程总结",
  mainPoint: "核心转变：AI 自动化工程师\n跳过原理，直接实战，从工具使用者转变为驾驭 AI 的自动化工程师。",
  subPoints: [
    "任务场景 → 选Agent → 配工具 → 写指令 → 跑闭环",
    "学会用AI提效，成为能落地的AI实战者",
    '未来拼的不是"造AI"，而是"用AI"的能力'
  ]
});

// Slide 8: 下节课预告
layouts.createPreview(pres, {
  pageNum: 8,
  title: "下节课预告",
  nextLesson: "第2课：什么是Agent？为什么它比普通AI强10倍？",
  description: "我们将深入了解 Agent 的定义、核心能力，并对比它与普通 AI 的巨大差异。",
  points: [
    "核心定义解析：Agent 的本质与起源",
    "关键能力模型：感知、决策与行动闭环",
    "性能对比分析：Agent vs 传统 AI 差异"
  ]
});

// 输出
pres.writeFile({ fileName: './output/lesson1-complete.pptx' })
  .then(() => console.log('✅ 第1课PPT生成完成！'))
  .catch(err => console.error('❌ 错误:', err));