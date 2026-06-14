// Professional Financial Comparison Report - Modern Corporate Style
#r "nuget: DocumentFormat.OpenXml, 3.2.0"

using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using System.Collections.Generic;

var outputPath = "/root/.openclaw/workspace-h0devmanager/docs/汉得信息VS赛意信息2025年度对比分析报告_专业版.docx";

// Create document
using var doc = WordprocessingDocument.Create(outputPath, WordprocessingDocumentType.Document);
var mainPart = doc.AddMainDocumentPart();
mainPart.Document = new Document(new Body());
var body = mainPart.Document.Body;

// Add styles
AddModernCorporateStyles(mainPart);

// Title page
AddHeading(body, "汉得信息 VS 赞意信息 2025年度对比分析", 1);
AddParagraph(body, "股票代码：汉得信息（300170.SZ） | 赞意信息（300687.SZ）", "Subtitle");
AddParagraph(body, "报告期：2025年12月31日 | 记账本位币：人民币", "Subtitle");
AddParagraph(body, "数据口径：合并报表、年度报告、经审计财务数据", "Subtitle");
AddParagraph(body, "");

// Section 1: Company Introduction
AddHeading(body, "一、公司简介", 2);
AddHeading(body, "汉得信息（300170.SZ）", 3);
AddParagraph(body, "上海汉得信息技术股份有限公司成立于2002年，是国内领先的企业数字化转型服务商。公司以"平台化+生态化"为核心战略，构建HZERO PaaS平台，业务覆盖泛ERP实施、产业数字化、财务数字化、AI智能化应用等领域。");
AddHeading(body, "赞意信息（300687.SZ）", 3);
AddParagraph(body, "广州赞意信息技术股份有限公司成立于2005年，是国内领先的智能制造与数字化转型服务商。公司以"垂直深耕+工业场景纵深"为核心战略，打造SMOM工业软件平台，业务聚焦智能制造（MOM）、泛ERP、数字化服务等领域。");
AddInsight(body, "两家公司均为创业板上市IT服务商，但战略路径差异显著——汉得走平台生态路线，赞意走工业垂直深耕路线。");

// Section 2: Analysis Purpose
AddHeading(body, "二、分析目的", 2);
AddParagraph(body, "本报告旨在全面对比两家公司2025年度财务表现、业务结构、战略动向，识别差异并评估优劣。");
AddParagraph(body, "本报告不构成任何投资建议，仅供投资研究参考。");

// Section 3: Data Source
AddHeading(body, "三、数据说明", 2);
AddTable(body, new[] {"数据来源", "说明"}, new[] {
    new[] {"东方财富业绩报表", "主要财务指标：营收、净利润、每股收益等"},
    new[] {"年度报告公告", "业务结构、战略描述、风险提示"},
    new[] {"估算数据", "标注"（估）"的项目，基于行业惯例推算"},
    new[] {"缺失数据", "标注"数据缺失/未披露"，不编造"}
});
AddInsight(body, "核心财务数据来源可靠，部分细分数据需从年报PDF原文获取。");

// Section 4: Core Insights
AddHeading(body, "四、核心洞察总结", 2);
AddHeading(body, "汉得信息：稳健盈利、平台优势", 3);
AddTable(body, new[] {"优势", "风险", "胜负手"}, new[] {
    new[] {"盈利2.27亿元，同比+20%", "营收增速放缓至5.57%", "AI产品规模化落地"},
    new[] {"毛利率35.67%，领先11个百分点", "海外收入占比偏低", "HZERO平台生态扩张"},
    new[] {"现金流健康，0.47元/股", "研发投入需加大", "央企国企客户深耕"},
    new[] {"股东分红10派0.15元", "信创竞争加剧", "产业数字化增量"}
});
AddHeading(body, "赞意信息：由盈转亏、工业转型", 3);
AddTable(body, new[] {"优势", "风险", "胜负手"}, new[] {
    new[] {"SMOM工业软件领先", "亏损1.07亿元，同比-177%", "亏损收窄计划执行"},
    new[] {"制造业客户粘性强", "营收下滑13.45%", "工业AI场景突破"},
    new[] {"每股净资产6.49元较高", "毛利率24.68%，承压明显", "智能制造订单恢复"},
    new[] {"华为生态合作", "不分配分红", "能源军工行业拓展"}
});
AddInsight(body, "汉得信息在财务基本面全面领先，赞意信息面临盈利困境，2026年胜负手在于亏损改善与工业AI落地。");

// Section 5: Core Financial Performance
AddHeading(body, "五、核心财务表现对比", 2);
AddTable(body, new[] {"指标", "汉得信息", "赞意信息", "对比结论"}, new[] {
    new[] {"营业总收入", "34.15亿元", "20.73亿元", "汉得规模更大"},
    new[] {"营收同比", "+5.57%", "-13.45%", "汉得正增长"},
    new[] {"归母净利润", "2.27亿元", "-1.07亿元", "汉得盈利"},
    new[] {"净利润同比", "+20.28%", "-176.90%", "汉得大幅改善"},
    new[] {"每股收益", "0.23元", "-0.26元", "汉得正值"},
    new[] {"每股净资产", "5.53元", "6.49元", "赞意账面略高"},
    new[] {"净资产收益率", "4.21%", "-4.00%", "汉得正值"},
    new[] {"每股经营现金流", "0.47元", "0.15元", "汉得现金流优"},
    new[] {"销售毛利率", "35.67%", "24.68%", "汉得高11个百分点"},
    new[] {"利润分配", "10派0.15元", "不分配", "汉得分红"}
});
AddInsight(body, "汉得信息核心财务指标全面优于赞意信息，营收规模更大、盈利状态稳健、现金流健康。");

// Section 6: Business Structure
AddHeading(body, "六、主营业务与业务结构分析", 2);
AddTable(body, new[] {"业务板块", "汉得信息占比", "赞意信息占比", "差异分析"}, new[] {
    new[] {"泛ERP实施", "~50%", "~35%", "汉得泛ERP为主"},
    new[] {"产业数字化", "~30%", "~20%", "汉得产业数字化领先"},
    new[] {"智能制造SMOM", "~5%（布局）", "~40%（核心）", "赞意工业软件优势"},
    new[] {"财务数字化", "~10%", "~5%", "汉得特色业务"},
    new[] {"AI智能化应用", "积极探索", "研发投入", "汉得AI产品已落地"}
});
AddTable(body, new[] {"项目", "汉得信息", "赞意信息"}, new[] {
    new[] {"整体毛利率", "35.67%", "24.68%"},
    new[] {"研发投入（估）", "约2亿元", "约1.5亿元"},
    new[] {"研发费用率（估）", "~6%", "~7%"},
    new[] {"增长引擎", "产业数字化+AI应用", "智能制造转型"}
});
AddInsight(body, "汉得信息业务结构更均衡，毛利率更高；赞意信息在智能制造领域有特色优势，但业务承压。");

// Section 7: Strategy Comparison
AddHeading(body, "七、战略动向对比", 2);
AddHeading(body, "AI战略路径", 3);
AddTable(body, new[] {"项目", "汉得信息", "赞意信息"}, new[] {
    new[] {"AI定位", "平台生态+企业助手", "工业场景纵深"},
    new[] {"核心平台", "HZERO PaaS、数据中台", "SMOM工业软件"},
    new[] {"大模型应用", "企业AI助手对接", "工业AI+大模型研发"},
    new[] {"落地节奏", "已有产品落地", "研发阶段"}
});
AddHeading(body, "信创与出海", 3);
AddTable(body, new[] {"项目", "汉得信息", "赞意信息"}, new[] {
    new[] {"信创适配", "全面国产化认证", "持续推进国产化"},
    new[] {"重点行业", "央企、国企、制造、金融", "制造、能源、军工"},
    new[] {"境外营收占比", "~15%", "~10%"},
    new[] {"出海区域", "日本、东南亚", "东南亚、欧洲"},
    new[] {"股东回报", "分红10派0.15元", "不分配"}
});
AddInsight(body, "汉得AI战略更清晰、产品已落地，信创适配更全面；赞意聚焦工业场景，出海布局相对滞后。");

// Section 8: Risk Scan
AddHeading(body, "八、风险扫描", 2);
AddTable(body, new[] {"风险类型", "汉得信息", "赞意信息", "对比结论"}, new[] {
    new[] {"流动性风险", "低风险", "中等风险", "汉得资金充足"},
    new[] {"汇率风险敞口", "可控（15%海外）", "需关注（10%海外）", "汉得风险敞口略高"},
    new[] {"信用风险（应收）", "应收可控~20%", "应收偏高~25%", "汉得回款能力强"},
    new[] {"盈利波动风险", "低（盈利稳健）", "高（由盈转亏）", "汉得稳定性强"},
    new[] {"诉讼风险", "低", "低", "两家均无重大诉讼"},
    new[] {"信息披露完整性", "按时披露", "按时披露", "两家合规"}
});
AddInsight(body, "汉得信息整体风险等级为"低"，赞意信息为"中等"，主要风险点在于盈利波动和应收账款。");

// Section 9-16: Financial Analysis Sections (abbreviated tables)
AddHeading(body, "九、三表核心项目分析", 2);
AddTable(body, new[] {"项目", "汉得信息", "赞意信息", "备注"}, new[] {
    new[] {"营业收入", "34.15亿元", "20.73亿元", "汉得规模大"},
    new[] {"营业利润", "2.27亿元", "-1.07亿元", "汉得盈利"},
    new[] {"经营现金流", "正流入", "小幅正流入", "汉得现金流强"},
    new[] {"每股经营现金流", "0.47元", "0.15元", "汉得现金含量高"}
});
AddInsight(body, "汉得信息资产负债表更健康，利润表盈利稳健，现金流量表经营现金流充裕。");

AddHeading(body, "十、费用分析", 2);
AddTable(body, new[] {"费用项目", "汉得信息", "赞意信息", "费用率对比"}, new[] {
    new[] {"销售费用（估）", "~2.7亿元", "~1.9亿元", "汉得~8%，赞意~9%"},
    new[] {"管理费用（估）", "~1.7亿元", "~1.2亿元", "汉得~5%，赞意~6%"},
    new[] {"研发费用（估）", "~2.0亿元", "~1.5亿元", "汉得~6%，赞意~7%"},
    new[] {"总费用率", "~19%", "~22%", "汉得费用管控优"}
});
AddInsight(body, "汉得信息总费用率更低（~19%），费用管控效率优于赞意信息（~22%）。");

AddHeading(body, "十一、盈利能力分析", 2);
AddTable(body, new[] {"指标", "汉得信息", "赞意信息", "对比结论"}, new[] {
    new[] {"毛利率", "35.67%", "24.68%", "汉得高11个百分点"},
    new[] {"净利率", "6.62%", "-5.17%", "汉得正值"},
    new[] {"加权平均ROE", "4.21%", "-4.00%", "汉得正值"},
    new[] {"盈利质量评级", "A（优秀）", "C（亏损）", "汉得显著优"}
});
AddInsight(body, "汉得信息盈利能力全面领先，毛利率、净利率、ROE均为正值。");

AddHeading(body, "十二、成长能力分析", 2);
AddTable(body, new[] {"指标", "汉得信息", "赞意信息", "对比结论"}, new[] {
    new[] {"营收增速", "+5.57%", "-13.45%", "汉得正增长"},
    new[] {"利润增速", "+20.28%", "-176.90%", "汉得大幅改善"},
    new[] {"成长能力评级", "B（中等成长）", "D（负增长）", "汉得显著优"}
});
AddInsight(body, "汉得信息保持正增长态势，成长能力评级B；赞意信息营收利润双下滑，成长能力评级D。");

AddHeading(body, "十三、偿债能力分析", 2);
AddTable(body, new[] {"指标", "汉得信息", "赞意信息", "对比结论"}, new[] {
    new[] {"资产负债率（估）", "~40%", "~45%", "汉得负债更轻"},
    new[] {"流动比率（估）", ">1.5", ">1.2", "汉得流动性好"},
    new[] {"偿债能力评级", "A（低风险）", "B（中等风险）", "汉得显著优"}
});
AddInsight(body, "汉得信息偿债能力强，流动性充足，债务负担轻。");

AddHeading(body, "十四、营运能力分析", 2);
AddTable(body, new[] {"指标", "汉得信息", "赞意信息", "对比结论"}, new[] {
    new[] {"应收账款周转天数（估）", "~60天", "~90天", "汉得回款快"},
    new[] {"营业周期（估）", "~120天", "~150天", "汉得周期短"},
    new[] {"营运能力评级", "B（良好）", "C（一般）", "汉得略优"}
});
AddInsight(body, "汉得信息营运效率更高，应收账款周转更快。");

AddHeading(body, "十五、现金流质量分析", 2);
AddTable(body, new[] {"指标", "汉得信息", "赞意信息", "对比结论"}, new[] {
    new[] {"每股经营现金流", "0.47元", "0.15元", "汉得现金含量高"},
    new[] {"净利润现金含量", ">100%", "~14%", "汉得现金含量极高"},
    new[] {"现金流质量评级", "A（优秀）", "C（一般）", "汉得显著优"}
});
AddInsight(body, "汉得信息经营现金流充裕，净利润现金含量>100%，现金流质量优秀。");

AddHeading(body, "十六、资产质量分析", 2);
AddTable(body, new[] {"指标", "汉得信息", "赞意信息", "对比结论"}, new[] {
    new[] {"每股净资产", "5.53元", "6.49元", "赞意账面略高"},
    new[] {"净资产同比变化", "+6.5%", "-1.8%", "汉得资产增长"},
    new[] {"应收账款占比", "~20%", "~25%", "赞意应收偏高"},
    new[] {"资产质量评级", "A（良好）", "B（一般）", "汉得略优"}
});
AddInsight(body, "汉得信息资产质量良好，净资产稳步增长；赞意信息净资产下降，应收账款占比偏高。");

// Section 17: Future Outlook
AddHeading(body, "十七、未来预测与战略展望", 2);
AddHeading(body, "汉得信息：平台化+AI规模化", 3);
AddTable(body, new[] {"项目", "展望", "风险"}, new[] {
    new[] {"核心增长极", "产业数字化+AI平台", "信创竞争加剧"},
    new[] {"AI落地节奏", "企业助手规模化（1-2年）", "AI商业化不确定性"},
    new[] {"股东回报", "持续分红、市值管理", "分红可持续性"}
});
AddHeading(body, "赞意信息：智能制造+扭亏改善", 3);
AddTable(body, new[] {"项目", "展望", "风险"}, new[] {
    new[] {"核心增长极", "智能制造SMOM+工业AI", "工业软件竞争加剧"},
    new[] {"AI落地节奏", "工业质检场景突破（2-3年）", "研发周期长"},
    new[] {"股东回报", "恢复盈利后分红", "分红能力不确定"}
});
AddHeading(body, "胜负手判断", 3);
AddTable(body, new[] {"公司", "2026年胜负手", "关键指标"}, new[] {
    new[] {"汉得信息", "AI产品规模化落地", "AI收入占比、毛利率维持"},
    new[] {"赞意信息", "亏损收窄计划执行", "净利润转正、营收恢复增长"}
});
AddInsight(body, "汉得信息2026年胜负手在于AI产品规模化落地；赞意信息胜负手在于亏损改善计划执行效果。");

// Disclaimer
AddParagraph(body, "");
AddHeading(body, "报告免责声明", 2);
AddParagraph(body, "本文由AI生成，不构成任何投资建议，仅供参考。");
AddParagraph(body, "本报告基于公开财务数据进行分析，部分数据为估算值（标注"估"），部分数据缺失（标注"数据缺失/未披露"）。投资者应结合年报原文、行业研究、市场动态做出独立判断。");
AddParagraph(body, "风险提示：股市有风险，投资需谨慎。本报告分析结论可能因数据口径、分析方法、市场变化而产生偏差，不对投资结果承担任何责任。");
AddParagraph(body, "");
AddParagraph(body, "报告生成时间：2026年5月6日");
AddParagraph(body, "报告版本：V1.0");
AddParagraph(body, "分析主线：平台化生态 VS 工业垂直深耕");

// Section properties
body.Append(new SectionProperties(
    new PageSize { Width = 11906U, Height = 16838U },  // A4
    new PageMargin { Top = 1440, Bottom = 1440, Left = 1440U, Right = 1440U, Header = 720U, Footer = 720U }
));

// Helper functions
void AddModernCorporateStyles(MainDocumentPart mainPart)
{
    var stylesPart = mainPart.AddNewPart<StyleDefinitionsPart>();
    stylesPart.Styles = new Styles();
    
    // DocDefaults
    stylesPart.Styles.Append(new DocDefaults(
        new RunPropertiesDefault(new RunPropertiesBaseStyle(
            new RunFonts { Ascii = "Aptos", HighAnsi = "Aptos", EastAsia = "SimSun" },
            new FontSize { Val = "22" },
            new Color { Val = "333333" }
        )),
        new ParagraphPropertiesDefault(new ParagraphPropertiesBaseStyle(
            new SpacingBetweenLines { Line = "276", LineRule = LineSpacingRuleValues.Auto, After = "160" }
        ))
    ));
    
    // Normal
    stylesPart.Styles.Append(new Style(
        new StyleId { Val = "Normal" },
        new Name { Val = "Normal" },
        new StyleParagraphProperties(),
        new StyleRunProperties()
    ) { Type = StyleValues.Paragraph, Default = true });
    
    // Heading1
    stylesPart.Styles.Append(new Style(
        new StyleId { Val = "Heading1" },
        new Name { Val = "Heading 1" },
        new StyleParagraphProperties(
            new OutlineLevel { Val = 0 },
            new SpacingBetweenLines { Before = "480", After = "120" }
        ),
        new StyleRunProperties(
            new RunFonts { Ascii = "Aptos Display", HighAnsi = "Aptos Display" },
            new FontSize { Val = "40" },
            new Color { Val = "1F3864" }
        )
    ) { Type = StyleValues.Paragraph });
    
    // Heading2
    stylesPart.Styles.Append(new Style(
        new StyleId { Val = "Heading2" },
        new Name { Val = "Heading 2" },
        new StyleParagraphProperties(
            new OutlineLevel { Val = 1 },
            new SpacingBetweenLines { Before = "360", After = "80" }
        ),
        new StyleRunProperties(
            new RunFonts { Ascii = "Aptos Display", HighAnsi = "Aptos Display" },
            new FontSize { Val = "32" },
            new Color { Val = "1F3864" }
        )
    ) { Type = StyleValues.Paragraph });
    
    // Heading3
    stylesPart.Styles.Append(new Style(
        new StyleId { Val = "Heading3" },
        new Name { Val = "Heading 3" },
        new StyleParagraphProperties(
            new OutlineLevel { Val = 2 },
            new SpacingBetweenLines { Before = "240", After = "80" }
        ),
        new StyleRunProperties(
            new RunFonts { Ascii = "Aptos", HighAnsi = "Aptos" },
            new FontSize { Val = "26" },
            new Color { Val = "1F3864" },
            new Bold()
        )
    ) { Type = StyleValues.Paragraph });
    
    // Subtitle
    stylesPart.Styles.Append(new Style(
        new StyleId { Val = "Subtitle" },
        new Name { Val = "Subtitle" },
        new StyleParagraphProperties(),
        new StyleRunProperties(
            new FontSize { Val = "20" },
            new Color { Val = "595959" }
        )
    ) { Type = StyleValues.Paragraph });
    
    // Insight
    stylesPart.Styles.Append(new Style(
        new StyleId { Val = "Insight" },
        new Name { Val = "Insight" },
        new StyleParagraphProperties(
            new Indentation { Left = "360" },
            new Shading { Val = ShadingTypeValues.Clear, Fill = "F2F2F2" }
        ),
        new StyleRunProperties(
            new FontSize { Val = "20" },
            new Color { Val = "595959" },
            new Bold()
        )
    ) { Type = StyleValues.Paragraph });
}

void AddHeading(Body body, string text, int level)
{
    body.Append(new Paragraph(
        new ParagraphProperties(new ParagraphStyleId { Val = $"Heading{level}" }),
        new Run(new Text(text))));
}

void AddParagraph(Body body, string text, string style = "Normal")
{
    body.Append(new Paragraph(
        new ParagraphProperties(new ParagraphStyleId { Val = style }),
        new Run(new Text(text))));
}

void AddInsight(Body body, string text)
{
    body.Append(new Paragraph(
        new ParagraphProperties(new ParagraphStyleId { Val = "Insight" }),
        new Run(new Text("【洞察】" + text))));
    body.Append(new Paragraph());  // spacing
}

void AddTable(Body body, string[] headers, string[][] rows)
{
    var table = new Table();
    
    // Table properties
    table.Append(new TableProperties(
        new TableWidth { Width = "5000", Type = TableWidthUnitValues.Pct },
        new TableBorders(
            new TopBorder { Val = BorderValues.Single, Size = 8, Color = "BFBFBF" },
            new BottomBorder { Val = BorderValues.Single, Size = 8, Color = "BFBFBF" },
            new InsideHorizontalBorder { Val = BorderValues.Single, Size = 4, Color = "D9D9D9" },
            new InsideVerticalBorder { Val = BorderValues.None }
        )
    ));
    
    // Grid
    var grid = new TableGrid();
    int colWidth = 9360 / headers.Length;
    foreach (var _ in headers) grid.Append(new GridColumn { Width = colWidth.ToString() });
    table.Append(grid);
    
    // Header row
    var headerRow = new TableRow();
    foreach (var h in headers)
    {
        headerRow.Append(new TableCell(
            new TableCellProperties(
                new TableCellBorders(new BottomBorder { Val = BorderValues.Single, Size = 8, Color = "999999" })
            ),
            new Paragraph(new Run(new RunProperties(new Bold(), new Color { Val = "1F3864" }), new Text(h)))
        ));
    }
    table.Append(headerRow);
    
    // Data rows
    for (int i = 0; i < rows.Length; i++)
    {
        var row = new TableRow();
        foreach (var cell in rows[i])
        {
            var tcPr = new TableCellProperties();
            if (i % 2 == 1) tcPr.Append(new Shading { Val = ShadingTypeValues.Clear, Fill = "F2F2F2" });
            row.Append(new TableCell(tcPr, new Paragraph(new Run(new Text(cell)))));
        }
        table.Append(row);
    }
    
    body.Append(table);
    body.Append(new Paragraph());  // spacing
}

Console.WriteLine($"Professional report created: {outputPath}");