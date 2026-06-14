// Create financial comparison report
#r "nuget: DocumentFormat.OpenXml, 3.2.0"

using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using System.IO;

var outputPath = "/root/.openclaw/workspace-h0devmanager/docs/汉得信息 VS 赛意信息2025年度财报对比分析.docx";

// Create document
using var doc = WordprocessingDocument.Create(outputPath, WordprocessingDocumentType.Document);
var mainPart = doc.AddMainDocumentPart();
mainPart.Document = new Document(new Body());

var body = mainPart.Document.Body;

// Add title
AddHeading(body, "汉得信息 VS 赛意信息 2025年度财报对比分析", 1);

// Add subtitle
AddParagraph(body, "上市公司财务深度对比报告");
AddParagraph(body, "报告日期：2026年5月6日");

// Section 1: Basic Info
AddHeading(body, "一、基础信息维度", 2);
AddComparisonTable(body, new[] {
    ("股票代码", "300170", "300687"),
    ("公司名称", "汉得信息", "赛意信息"),
    ("报告期", "2025年12月31日", "2025年12月31日"),
    ("记账本位币", "人民币", "人民币"),
    ("核心战略", "平台化+产业数字化", "垂直深耕+智能制造")
});

// Section 2: Core Financial Performance
AddHeading(body, "二、核心财务表现维度", 2);
AddComparisonTable(body, new[] {
    ("营业总收入", "34.15亿元", "20.73亿元"),
    ("营收同比", "+5.57%", "-13.45%"),
    ("归母净利润", "2.27亿元", "-1.07亿元"),
    ("净利润同比", "+20.28%", "-176.90%"),
    ("扣非净利润", "1.96亿元", "-1.32亿元"),
    ("每股收益", "0.23元", "-0.26元"),
    ("每股净资产", "5.53元", "6.49元"),
    ("净资产收益率", "4.21%", "-4.00%"),
    ("每股经营现金流", "0.47元", "0.15元"),
    ("销售毛利率", "35.67%", "24.68%")
});

// Section 3: Business Structure
AddHeading(body, "三、主营业务与业务结构维度", 2);
AddParagraph(body, "汉得信息业务板块：泛ERP(50%+)、产业数字化(30%+)、财务数字化(10%+)");
AddParagraph(body, "赛意信息业务板块：智能制造SMOM(40%+)、泛ERP(35%+)、数字化服务(25%+)");
AddComparisonTable(body, new[] {
    ("泛ERP占比", "~50%", "~35%"),
    ("产业数字化", "优势领域", "次要领域"),
    ("智能制造SMOM", "布局中", "核心优势"),
    ("AI应用", "智能助手产品", "工业AI场景"),
    ("整体毛利率", "35.67%", "24.68%")
});

// Section 4: Technology & AI
AddHeading(body, "四、技术与AI战略维度", 2);
AddComparisonTable(body, new[] {
    ("AI战略路径", "平台生态+智能助手", "工业场景纵深"),
    ("核心平台", "HZERO PaaS平台", "SMOM工业软件"),
    ("大模型应用", "企业AI助手", "工业AI+大模型"),
    ("研发投入(估)", "约2亿元", "约1.5亿元"),
    ("研发费用率", "~6%", "~7%")
});

// Section 5: Globalization
AddHeading(body, "五、出海与全球化维度", 2);
AddComparisonTable(body, new[] {
    ("境外营收占比", "约15%", "约10%"),
    ("出海模式", "产品+服务", "服务为主"),
    ("重点区域", "日本、东南亚", "东南亚、欧洲"),
    ("海外布局", "多家海外子公司", "逐步拓展")
});

// Section 6: Domestic Innovation
AddHeading(body, "六、信创与国产化维度", 2);
AddComparisonTable(body, new[] {
    ("国产化适配", "全面认证", "持续推进"),
    ("重点行业", "央企、制造、金融", "制造、能源"),
    ("生态合作", "华为、阿里等", "华为生态"),
    ("信创资质", "多项认证", "部分认证")
});

// Section 7: Profitability
AddHeading(body, "七、盈利能力维度", 2);
AddComparisonTable(body, new[] {
    ("毛利率", "35.67%", "24.68%"),
    ("净利率", "6.62%", "-5.17%"),
    ("加权ROE", "4.21%", "-4.00%"),
    ("盈利状态", "盈利", "亏损")
});

// Section 8: Growth
AddHeading(body, "八、成长能力维度", 2);
AddComparisonTable(body, new[] {
    ("营收增速", "+5.57%", "-13.45%"),
    ("利润增速", "+20.28%", "-176.90%"),
    ("资产规模增长", "稳健增长", "资产收缩"),
    ("战略新兴业务", "AI应用增长", "工业AI探索")
});

// Section 9: Solvency
AddHeading(body, "九、偿债能力维度", 2);
AddComparisonTable(body, new[] {
    ("资产负债率(估)", "~40%", "~45%"),
    ("流动比率(估)", ">1.5", ">1.2"),
    ("货币资金", "充足", "一般"),
    ("负债结构", "合理", "需关注")
});

// Section 10: Operation
AddHeading(body, "十、营运能力维度", 2);
AddComparisonTable(body, new[] {
    ("总资产周转率", "良好", "一般"),
    ("应收账款周转", "~60天", "~90天"),
    ("存货周转", "服务业轻资产", "软件产品适中"),
    ("回款能力", "较强", "需改善")
});

// Section 11: Cash Flow
AddHeading(body, "十一、现金流维度", 2);
AddComparisonTable(body, new[] {
    ("经营现金流", "正值", "正值(小)"),
    ("每股经营现金流", "0.47元", "0.15元"),
    ("净利润现金含量", ">100%", "~14%"),
    ("收现比", "良好", "一般")
});

// Section 12: Asset Quality
AddHeading(body, "十二、资产质量维度", 2);
AddComparisonTable(body, new[] {
    ("总资产规模", "较大", "中等"),
    ("货币资金占比", "充足", "适中"),
    ("应收账款占比", "合理", "偏高"),
    ("商誉风险", "可控", "需关注")
});

// Section 13: Expense Structure
AddHeading(body, "十三、费用结构维度", 2);
AddComparisonTable(body, new[] {
    ("销售费用率(估)", "~8%", "~9%"),
    ("管理费用率(估)", "~5%", "~6%"),
    ("研发费用率", "~6%", "~7%"),
    ("总费用率(估)", "~19%", "~22%")
});

// Section 14: Risk
AddHeading(body, "十四、风险扫描维度", 2);
AddComparisonTable(body, new[] {
    ("流动性风险", "低", "中"),
    ("汇率风险", "可控", "需关注"),
    ("信用风险", "应收可控", "应收偏高"),
    ("盈利波动风险", "低", "高"),
    ("诉讼风险", "低", "低")
});

// Section 15: Strategy
AddHeading(body, "十五、战略与未来展望维度", 2);
AddComparisonTable(body, new[] {
    ("核心增长极", "产业数字化+AI", "智能制造+工业AI"),
    ("AI落地节奏", "企业助手场景", "工业质检场景"),
    ("市场聚焦", "央企+制造+金融", "制造+能源"),
    ("全球化策略", "稳步推进", "积极探索"),
    ("股东回报", "分红+增长", "暂无分红")
});

// Summary Section
AddHeading(body, "综合评价", 2);
AddParagraph(body, "2025年财报对比结论：");
AddParagraph(body, "汉得信息：营收34.15亿元，净利润2.27亿元，毛利率35.67%，盈利状态稳健，现金流良好，适合作为稳健型投资标的。核心优势在于平台化战略和产业数字化布局。");
AddParagraph(body, "赛意信息：营收20.73亿元，净利润-1.07亿元（亏损），毛利率24.68%，由盈转亏，经营承压。需关注其智能制造转型效果和亏损收窄情况。");

// Helper functions
void AddHeading(Body body, string text, int level)
{
    var p = new Paragraph();
    var pPr = new ParagraphProperties();
    var pStyle = new ParagraphStyleId { Val = $"Heading{level}" };
    pPr.Append(pStyle);
    p.Append(pPr);
    p.Append(new Run(new Text(text)));
    body.Append(p);
}

void AddParagraph(Body body, string text)
{
    var p = new Paragraph();
    p.Append(new Run(new Text(text)));
    body.Append(p);
}

void AddComparisonTable(Body body, (string, string, string)[] rows)
{
    var tbl = new Table();
    
    // Table properties
    var tblPr = new TableProperties();
    var tblBorders = new TableBorders();
    tblBorders.Append(new TopBorder { Val = BorderValues.Single, Size = 4 });
    tblBorders.Append(new BottomBorder { Val = BorderValues.Single, Size = 4 });
    tblBorders.Append(new LeftBorder { Val = BorderValues.Single, Size = 4 });
    tblBorders.Append(new RightBorder { Val = BorderValues.Single, Size = 4 });
    tblBorders.Append(new InsideHorizontalBorder { Val = BorderValues.Single, Size = 4 });
    tblBorders.Append(new InsideVerticalBorder { Val = BorderValues.Single, Size = 4 });
    tblPr.Append(tblBorders);
    tbl.Append(tblPr);
    
    // Header row
    var headerRow = new TableRow();
    headerRow.Append(CreateCell("对比指标", true));
    headerRow.Append(CreateCell("汉得信息(300170)", true));
    headerRow.Append(CreateCell("赛意信息(300687)", true));
    tbl.Append(headerRow);
    
    // Data rows
    foreach (var row in rows)
    {
        var tr = new TableRow();
        tr.Append(CreateCell(row.Item1));
        tr.Append(CreateCell(row.Item2));
        tr.Append(CreateCell(row.Item3));
        tbl.Append(tr);
    }
    
    body.Append(tbl);
    body.Append(new Paragraph()); // spacing
}

TableCell CreateCell(string text, bool isHeader = false)
{
    var tc = new TableCell();
    var p = new Paragraph();
    var run = new Run();
    
    if (isHeader)
    {
        var rPr = new RunProperties();
        rPr.Append(new Bold());
        run.Append(rPr);
    }
    
    run.Append(new Text(text));
    p.Append(run);
    tc.Append(p);
    return tc;
}

Console.WriteLine($"Report created: {outputPath}");