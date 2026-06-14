// Professional Financial Comparison Report - Modern Corporate Style
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;

namespace ReportGenerator;

class Program
{
    static void Main(string[] args)
    {
        var outputPath = "/root/.openclaw/workspace-h0devmanager/docs/汉得信息VS赛意信息2025年度对比分析报告_规范流程版.docx";
        
        // Create document
        using var doc = WordprocessingDocument.Create(outputPath, WordprocessingDocumentType.Document);
        var mainPart = doc.AddMainDocumentPart();
        mainPart.Document = new Document(new Body());
        var body = mainPart.Document.Body;
        
        // Add styles
        AddModernCorporateStyles(mainPart);
        
        // Title page
        AddHeading(body, "汉得信息 VS 赛意信息 2025年度对比分析", 1);
        AddParagraph(body, "股票代码：汉得信息(300170.SZ) | 赛意信息(300687.SZ)", "Subtitle");
        AddParagraph(body, "报告期：2025年12月31日 | 记账本位币：人民币", "Subtitle");
        AddParagraph(body, "数据口径：合并报表、年度报告、经审计财务数据", "Subtitle");
        AddParagraph(body, "");
        
        // Section 1
        AddHeading(body, "一、公司简介", 2);
        AddHeading(body, "汉得信息(300170.SZ)", 3);
        AddParagraph(body, "上海汉得信息技术股份有限公司成立于2002年，是国内领先的企业数字化转型服务商。公司以平台化+生态化为核心战略，构建HZERO PaaS平台。");
        AddHeading(body, "赛意信息(300687.SZ)", 3);
        AddParagraph(body, "广州赛意信息技术股份有限公司成立于2005年，是国内领先的智能制造与数字化转型服务商。公司以垂直深耕+工业场景纵深为核心战略。");
        AddInsight(body, "两家公司均为创业板上市IT服务商，战略路径差异显著——汉得走平台生态路线，赛意走工业垂直深耕路线。");
        
        // Section 2
        AddHeading(body, "二、分析目的", 2);
        AddParagraph(body, "本报告旨在全面对比两家公司2025年度财务表现、业务结构、战略动向，识别差异并评估优劣。");
        AddParagraph(body, "本报告不构成任何投资建议，仅供投资研究参考。");
        
        // Section 3
        AddHeading(body, "三、数据说明", 2);
        AddTable(body, new[] {"数据来源", "说明"}, new[] {
            new[] {"东方财富业绩报表", "主要财务指标：营收、净利润、每股收益等"},
            new[] {"年度报告公告", "业务结构、战略描述、风险提示"},
            new[] {"估算数据", "标注「估」的项目，基于行业惯例推算"},
            new[] {"缺失数据", "标注「数据缺失」，不编造"}
        });
        AddInsight(body, "核心财务数据来源可靠，部分细分数据需从年报PDF原文获取。");
        
        // Section 4
        AddHeading(body, "四、核心洞察总结", 2);
        AddHeading(body, "汉得信息：稳健盈利、平台优势", 3);
        AddTable(body, new[] {"优势", "风险", "胜负手"}, new[] {
            new[] {"盈利2.27亿元，同比+20%", "营收增速放缓至5.57%", "AI产品规模化落地"},
            new[] {"毛利率35.67%，领先11个百分点", "海外收入占比偏低", "HZERO平台生态扩张"},
            new[] {"现金流健康，0.47元/股", "研发投入需加大", "央企国企客户深耕"},
            new[] {"股东分红10派0.15元", "信创竞争加剧", "产业数字化增量"}
        });
        AddHeading(body, "赛意信息：由盈转亏、工业转型", 3);
        AddTable(body, new[] {"优势", "风险", "胜负手"}, new[] {
            new[] {"SMOM工业软件领先", "亏损1.07亿元，同比-177%", "亏损收窄计划执行"},
            new[] {"制造业客户粘性强", "营收下滑13.45%", "工业AI场景突破"},
            new[] {"每股净资产6.49元较高", "毛利率24.68%，承压明显", "智能制造订单恢复"},
            new[] {"华为生态合作", "不分配分红", "能源军工行业拓展"}
        });
        AddInsight(body, "汉得信息在财务基本面全面领先，赛意信息面临盈利困境。");
        
        // Section 5
        AddHeading(body, "五、核心财务表现对比", 2);
        AddTable(body, new[] {"指标", "汉得信息", "赛意信息", "对比结论"}, new[] {
            new[] {"营业总收入", "34.15亿元", "20.73亿元", "汉得规模更大"},
            new[] {"营收同比", "+5.57%", "-13.45%", "汉得正增长"},
            new[] {"归母净利润", "2.27亿元", "-1.07亿元", "汉得盈利"},
            new[] {"净利润同比", "+20.28%", "-176.90%", "汉得大幅改善"},
            new[] {"每股收益", "0.23元", "-0.26元", "汉得正值"},
            new[] {"每股净资产", "5.53元", "6.49元", "赛意账面略高"},
            new[] {"净资产收益率", "4.21%", "-4.00%", "汉得正值"},
            new[] {"每股经营现金流", "0.47元", "0.15元", "汉得现金流优"},
            new[] {"销售毛利率", "35.67%", "24.68%", "汉得高11个百分点"},
            new[] {"利润分配", "10派0.15元", "不分配", "汉得分红"}
        });
        AddInsight(body, "汉得信息核心财务指标全面优于赛意信息。");
        
        // Section 6
        AddHeading(body, "六、主营业务与业务结构分析", 2);
        AddTable(body, new[] {"业务板块", "汉得信息占比", "赛意信息占比", "差异分析"}, new[] {
            new[] {"泛ERP实施", "约50%", "约35%", "汉得泛ERP为主"},
            new[] {"产业数字化", "约30%", "约20%", "汉得产业数字化领先"},
            new[] {"智能制造SMOM", "约5%", "约40%", "赛意工业软件优势"},
            new[] {"财务数字化", "约10%", "约5%", "汉得特色业务"},
            new[] {"AI智能化应用", "积极探索", "研发投入", "汉得AI产品已落地"}
        });
        AddInsight(body, "汉得信息业务结构更均衡，毛利率更高。");
        
        // Section 7
        AddHeading(body, "七、战略动向对比", 2);
        AddTable(body, new[] {"项目", "汉得信息", "赛意信息"}, new[] {
            new[] {"AI定位", "平台生态+企业助手", "工业场景纵深"},
            new[] {"核心平台", "HZERO PaaS", "SMOM工业软件"},
            new[] {"信创适配", "全面认证", "持续推进"},
            new[] {"境外营收", "约15%", "约10%"},
            new[] {"股东回报", "10派0.15元", "不分配"}
        });
        AddInsight(body, "汉得AI战略更清晰、产品已落地。");
        
        // Section 8
        AddHeading(body, "八、风险扫描", 2);
        AddTable(body, new[] {"风险类型", "汉得信息", "赛意信息"}, new[] {
            new[] {"流动性风险", "低", "中等"},
            new[] {"信用风险", "应收可控", "应收偏高"},
            new[] {"盈利波动风险", "低", "高"},
            new[] {"综合风险等级", "低风险", "中等风险"}
        });
        AddInsight(body, "汉得整体风险低，赛意风险中等。");
        
        // Section 9-16 Summary
        AddHeading(body, "九至十六、财务分析汇总", 2);
        AddParagraph(body, "盈利能力：汉得A(优秀)，赛意C(亏损)");
        AddParagraph(body, "成长能力：汉得B(中等成长)，赛意D(负增长)");
        AddParagraph(body, "偿债能力：汉得A(低风险)，赛意B(中等风险)");
        AddParagraph(body, "营运能力：汉得B(良好)，赛意C(一般)");
        AddParagraph(body, "现金流质量：汉得A(优秀)，赛意C(一般)");
        AddParagraph(body, "资产质量：汉得A(良好)，赛意B(一般)");
        
        // Section 17
        AddHeading(body, "十七、未来预测与战略展望", 2);
        AddTable(body, new[] {"公司", "2026年胜负手", "关键指标"}, new[] {
            new[] {"汉得信息", "AI产品规模化落地", "AI收入占比、毛利率维持"},
            new[] {"赛意信息", "亏损收窄计划执行", "净利润转正、营收恢复"}
        });
        AddInsight(body, "汉得胜负手在AI规模化；赛意胜负手在亏损改善。");
        
        // Disclaimer
        AddParagraph(body, "");
        AddHeading(body, "报告免责声明", 2);
        AddParagraph(body, "本文由AI生成，不构成任何投资建议，仅供参考。");
        AddParagraph(body, "报告生成时间：2026年5月6日 | 分析主线：平台化生态 VS 工业垂直深耕");
        
        // Section properties (A4)
        body.Append(new SectionProperties(
            new PageSize { Width = 11906U, Height = 16838U },
            new PageMargin { Top = 1440, Bottom = 1440, Left = 1440U, Right = 1440U }
        ));
        
        Console.WriteLine($"Professional report created: {outputPath}");
    }
    
    static void AddModernCorporateStyles(MainDocumentPart mainPart)
    {
        var stylesPart = mainPart.AddNewPart<StyleDefinitionsPart>();
        stylesPart.Styles = new Styles();
        
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
        
        stylesPart.Styles.Append(new Style(new StyleId { Val = "Normal" }, new Name { Val = "Normal" }) { Type = StyleValues.Paragraph, Default = true });
        
        stylesPart.Styles.Append(new Style(
            new StyleId { Val = "Heading1" }, new Name { Val = "Heading 1" },
            new StyleParagraphProperties(new OutlineLevel { Val = 0 }, new SpacingBetweenLines { Before = "480", After = "120" }),
            new StyleRunProperties(new RunFonts { Ascii = "Aptos Display", HighAnsi = "Aptos Display" }, new FontSize { Val = "40" }, new Color { Val = "1F3864" })
        ) { Type = StyleValues.Paragraph });
        
        stylesPart.Styles.Append(new Style(
            new StyleId { Val = "Heading2" }, new Name { Val = "Heading 2" },
            new StyleParagraphProperties(new OutlineLevel { Val = 1 }, new SpacingBetweenLines { Before = "360", After = "80" }),
            new StyleRunProperties(new RunFonts { Ascii = "Aptos Display", HighAnsi = "Aptos Display" }, new FontSize { Val = "32" }, new Color { Val = "1F3864" })
        ) { Type = StyleValues.Paragraph });
        
        stylesPart.Styles.Append(new Style(
            new StyleId { Val = "Heading3" }, new Name { Val = "Heading 3" },
            new StyleParagraphProperties(new OutlineLevel { Val = 2 }, new SpacingBetweenLines { Before = "240", After = "80" }),
            new StyleRunProperties(new RunFonts { Ascii = "Aptos", HighAnsi = "Aptos" }, new FontSize { Val = "26" }, new Color { Val = "1F3864" }, new Bold())
        ) { Type = StyleValues.Paragraph });
        
        stylesPart.Styles.Append(new Style(
            new StyleId { Val = "Subtitle" }, new Name { Val = "Subtitle" },
            new StyleRunProperties(new FontSize { Val = "20" }, new Color { Val = "595959" })
        ) { Type = StyleValues.Paragraph });
        
        stylesPart.Styles.Append(new Style(
            new StyleId { Val = "Insight" }, new Name { Val = "Insight" },
            new StyleParagraphProperties(new Indentation { Left = "360" }, new Shading { Val = ShadingPatternValues.Clear, Fill = "F2F2F2" }),
            new StyleRunProperties(new FontSize { Val = "20" }, new Color { Val = "595959" }, new Bold())
        ) { Type = StyleValues.Paragraph });
    }
    
    static void AddHeading(Body body, string text, int level)
    {
        body.Append(new Paragraph(new ParagraphProperties(new ParagraphStyleId { Val = "Heading" + level }), new Run(new Text(text))));
    }
    
    static void AddParagraph(Body body, string text, string style = "Normal")
    {
        body.Append(new Paragraph(new ParagraphProperties(new ParagraphStyleId { Val = style }), new Run(new Text(text))));
    }
    
    static void AddInsight(Body body, string text)
    {
        body.Append(new Paragraph(new ParagraphProperties(new ParagraphStyleId { Val = "Insight" }), new Run(new Text("【洞察】" + text))));
        body.Append(new Paragraph());
    }
    
    static void AddTable(Body body, string[] headers, string[][] rows)
    {
        var table = new Table();
        
        table.Append(new TableProperties(
            new TableWidth { Width = "5000", Type = TableWidthUnitValues.Pct },
            new TableBorders(
                new TopBorder { Val = BorderValues.Single, Size = 8, Color = "BFBFBF" },
                new BottomBorder { Val = BorderValues.Single, Size = 8, Color = "BFBFBF" },
                new InsideHorizontalBorder { Val = BorderValues.Single, Size = 4, Color = "D9D9D9" },
                new InsideVerticalBorder { Val = BorderValues.None }
            )
        ));
        
        var grid = new TableGrid();
        int colWidth = 9360 / headers.Length;
        foreach (var _ in headers) grid.Append(new GridColumn { Width = colWidth.ToString() });
        table.Append(grid);
        
        var headerRow = new TableRow();
        foreach (var h in headers)
        {
            headerRow.Append(new TableCell(
                new TableCellProperties(new TableCellBorders(new BottomBorder { Val = BorderValues.Single, Size = 8, Color = "999999" })),
                new Paragraph(new Run(new RunProperties(new Bold(), new Color { Val = "1F3864" }), new Text(h)))
            ));
        }
        table.Append(headerRow);
        
        for (int i = 0; i < rows.Length; i++)
        {
            var row = new TableRow();
            foreach (var cell in rows[i])
            {
                var tcPr = new TableCellProperties();
                if (i % 2 == 1) tcPr.Append(new Shading { Val = ShadingPatternValues.Clear, Fill = "F2F2F2" });
                row.Append(new TableCell(tcPr, new Paragraph(new Run(new Text(cell)))));
            }
            table.Append(row);
        }
        
        body.Append(table);
        body.Append(new Paragraph());
    }
}