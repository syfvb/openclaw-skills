// 汉得飞搭低代码平台特点报告
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;

namespace FeidaReport;

class Program
{
    static void Main(string[] args)
    {
        var outputPath = "/root/.openclaw/workspace-h0assistant/汉得飞搭低代码平台特点.docx";
        
        using var doc = WordprocessingDocument.Create(outputPath, WordprocessingDocumentType.Document);
        var mainPart = doc.AddMainDocumentPart();
        mainPart.Document = new Document(new Body());
        var body = mainPart.Document.Body;
        
        AddStyles(mainPart);
        
        // Title
        AddHeading(body, "汉得飞搭低代码平台特点分析", 1);
        AddParagraph(body, "基于 HZERO 底座的企业级低代码开发平台", "Subtitle");
        AddParagraph(body, "报告日期：2026年5月7日", "Subtitle");
        AddParagraph(body, "");
        
        // Section 1
        AddHeading(body, "一、飞搭平台概述", 2);
        AddParagraph(body, "飞搭（FeiDa）是汉得信息（Hand Enterprise Solutions）推出的企业级低代码/无代码应用开发平台，基于 HZERO 微服务架构底座构建，面向中大型企业复杂业务场景，提供可视化、模型驱动的应用开发能力。");
        
        // Section 2
        AddHeading(body, "二、核心特点对比", 2);
        
        AddHeading(body, "1. 企业级技术底座", 3);
        AddParagraph(body, "与市场上多数低代码平台不同，飞搭并非基于公有云或第三方框架构建，而是建立在汉得自研的 HZERO 微服务架构之上：");
        AddBulletPoint(body, "HZERO 经过多年企业级项目验证，支撑复杂业务场景");
        AddBulletPoint(body, "内置组织、权限、流程、报表等企业级基础能力");
        AddBulletPoint(body, "不是\"从零开始\"的低代码，而是\"站在巨人肩膀上\"的开发模式");
        
        AddHeading(body, "2. 低代码 + 高代码融合", 3);
        AddParagraph(body, "飞搭解决\"低代码天花板\"问题的独特方式：");
        AddBulletPoint(body, "低代码部分：飞搭提供可视化设计，产出 JSON 格式元数据");
        AddBulletPoint(body, "高代码部分：基于 HZERO 底座进行专业开发，拥有完整源码控制权");
        AddBulletPoint(body, "两者在同一体系内无缝集成，非割裂方案");
        
        AddHeading(body, "3. 元数据驱动架构", 3);
        AddParagraph(body, "飞搭的设计产物与传统低代码平台有本质区别：");
        AddBulletPoint(body, "设计产物为 JSON 格式元数据，非传统源码");
        AddBulletPoint(body, "依赖飞搭低代码引擎解析渲染");
        AddBulletPoint(body, "优势：版本管理、迁移部署更灵活");
        AddBulletPoint(body, "注意：不支持直接修改导出的\"源码\"进行二次开发");
        
        AddHeading(body, "4. 私有化部署与信创支持", 3);
        AddParagraph(body, "飞搭在部署模式上的差异化定位：");
        AddBulletPoint(body, "以私有化部署为主，区别于公有云 SaaS 模式");
        AddBulletPoint(body, "支持国产数据库、中间件、操作系统");
        AddBulletPoint(body, "符合信创要求，满足大型企业合规需求");
        
        AddHeading(body, "5. HZERO 生态一体化", 3);
        AddParagraph(body, "飞搭与 HZERO 生态的深度整合：");
        AddBulletPoint(body, "低代码页面与 HZERO 标准功能无缝衔接");
        AddBulletPoint(body, "统一的用户体系、权限体系、数据模型");
        AddBulletPoint(body, "可复用 HZERO 丰富的企业级组件和业务能力");
        
        // Section 3 - Table
        AddHeading(body, "三、与主流低代码平台对比", 2);
        AddTable(body, new[] {"对比维度", "飞搭（HZERO）", "典型低代码平台"}, new[] {
            new[] {"技术底座", "自研 HZERO 微服务架构", "基于公有云或第三方框架"},
            new[] {"部署模式", "私有化部署为主", "公有云 SaaS 为主"},
            new[] {"\"源码\"本质", "JSON 格式元数据", "通常封闭存储"},
            new[] {"扩展方式", "HZERO 高代码无缝集成", "依赖平台开放接口"},
            new[] {"目标客群", "中大型企业、复杂业务", "中小企业、轻量级应用"},
            new[] {"国产化", "支持信创环境", "有限或需额外配置"}
        });
        AddInsight(body, "飞搭在私有化部署、国产化适配、企业级底座方面具有明显差异化优势。");
        
        // Section 4
        AddHeading(body, "四、适用场景", 2);
        
        AddHeading(body, "飞搭优势场景", 3);
        AddBulletPoint(body, "复杂 ERP 类应用开发");
        AddBulletPoint(body, "大规模私有化部署需求");
        AddBulletPoint(body, "需要与现有 HZERO 系统深度集成");
        AddBulletPoint(body, "有信创/国产化合规要求");
        
        AddHeading(body, "其他平台优势场景", 3);
        AddBulletPoint(body, "简单表单/审批快速搭建");
        AddBulletPoint(body, "快速上线 MVP 验证");
        AddBulletPoint(body, "轻量级应用，无复杂集成需求");
        
        // Section 5
        AddHeading(body, "五、总结", 2);
        AddParagraph(body, "飞搭的定位是\"面向企业级复杂场景的低代码平台\"，而非\"面向快速搭建简单应用的轻量级工具\"。");
        AddParagraph(body, "");
        AddParagraph(body, "其核心竞争优势在于：");
        AddBulletPoint(body, "HZERO 企业级底座的支撑能力");
        AddBulletPoint(body, "低代码与高代码的无缝融合方案");
        AddBulletPoint(body, "私有化部署和信创适配能力");
        AddBulletPoint(body, "与 HZERO 生态的深度整合");
        AddParagraph(body, "");
        AddParagraph(body, "飞搭的竞争对手更像是 OutSystems、Mendix 等国际企业级低代码平台，而非钉钉宜搭、腾讯云微搭等轻量工具。");
        
        // Page setup
        body.Append(new SectionProperties(
            new PageSize { Width = 11906U, Height = 16838U },
            new PageMargin { Top = 1440, Bottom = 1440, Left = 1440U, Right = 1440U }
        ));
        
        doc.Save();
        Console.WriteLine($"Document created: {outputPath}");
    }
    
    static void AddStyles(MainDocumentPart mainPart)
    {
        var stylesPart = mainPart.AddNewPart<StyleDefinitionsPart>();
        stylesPart.Styles = new Styles();
        
        stylesPart.Styles.Append(new DocDefaults(
            new RunPropertiesDefault(new RunPropertiesBaseStyle(
                new RunFonts { Ascii = "Microsoft YaHei", HighAnsi = "Microsoft YaHei", EastAsia = "Microsoft YaHei" },
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
            new StyleRunProperties(new RunFonts { Ascii = "Microsoft YaHei", HighAnsi = "Microsoft YaHei" }, new FontSize { Val = "40" }, new Color { Val = "1F3864" })
        ) { Type = StyleValues.Paragraph });
        
        stylesPart.Styles.Append(new Style(
            new StyleId { Val = "Heading2" }, new Name { Val = "Heading 2" },
            new StyleParagraphProperties(new OutlineLevel { Val = 1 }, new SpacingBetweenLines { Before = "360", After = "80" }),
            new StyleRunProperties(new RunFonts { Ascii = "Microsoft YaHei", HighAnsi = "Microsoft YaHei" }, new FontSize { Val = "32" }, new Color { Val = "1F3864" })
        ) { Type = StyleValues.Paragraph });
        
        stylesPart.Styles.Append(new Style(
            new StyleId { Val = "Heading3" }, new Name { Val = "Heading 3" },
            new StyleParagraphProperties(new OutlineLevel { Val = 2 }, new SpacingBetweenLines { Before = "240", After = "80" }),
            new StyleRunProperties(new RunFonts { Ascii = "Microsoft YaHei", HighAnsi = "Microsoft YaHei" }, new FontSize { Val = "26" }, new Color { Val = "1F3864" }, new Bold())
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
        var styleId = level switch { 1 => "Heading1", 2 => "Heading2", _ => "Heading3" };
        body.Append(new Paragraph(
            new ParagraphProperties(new ParagraphStyleId { Val = styleId }),
            new Run(new Text(text))
        ));
    }
    
    static void AddParagraph(Body body, string text, string styleId = "Normal")
    {
        if (string.IsNullOrEmpty(text))
        {
            body.Append(new Paragraph());
            return;
        }
        body.Append(new Paragraph(
            new ParagraphProperties(new ParagraphStyleId { Val = styleId }),
            new Run(new Text(text))
        ));
    }
    
    static void AddBulletPoint(Body body, string text)
    {
        body.Append(new Paragraph(
            new ParagraphProperties(new Indentation { Left = "720", Hanging = "360" }),
            new Run(new Text("• " + text))
        ));
    }
    
    static void AddInsight(Body body, string text)
    {
        body.Append(new Paragraph(
            new ParagraphProperties(
                new Indentation { Left = "360" },
                new Shading { Val = ShadingPatternValues.Clear, Fill = "E7F3FF" },
                new ParagraphBorders(new LeftBorder { Val = BorderValues.Single, Size = 24, Color = "1F4E79", Space = 8 }),
                new SpacingBetweenLines { Before = "200", After = "200" }
            ),
            new Run(
                new RunProperties(new Italic(), new Color { Val = "1F4E79" }),
                new Text(text)
            )
        ));
    }
    
    static void AddTable(Body body, string[] headers, string[][] rows)
    {
        var table = new Table();
        
        table.Append(new TableProperties(
            new TableWidth { Width = "5000", Type = TableWidthUnitValues.Pct },
            new TableBorders(
                new TopBorder { Val = BorderValues.Single, Size = 8, Color = "000000" },
                new BottomBorder { Val = BorderValues.Single, Size = 8, Color = "000000" },
                new LeftBorder { Val = BorderValues.Single, Size = 8, Color = "000000" },
                new RightBorder { Val = BorderValues.Single, Size = 8, Color = "000000" },
                new InsideHorizontalBorder { Val = BorderValues.Single, Size = 4, Color = "000000" },
                new InsideVerticalBorder { Val = BorderValues.Single, Size = 4, Color = "000000" }
            )
        ));
        
        var headerRow = new TableRow();
        foreach (var header in headers)
        {
            headerRow.Append(CreateTableCell(header, true));
        }
        table.Append(headerRow);
        
        foreach (var row in rows)
        {
            var tableRow = new TableRow();
            foreach (var cell in row)
            {
                tableRow.Append(CreateTableCell(cell, false));
            }
            table.Append(tableRow);
        }
        
        body.Append(table);
        body.Append(new Paragraph());
    }
    
    static TableCell CreateTableCell(string text, bool isHeader)
    {
        var cell = new TableCell();
        
        var cellProps = new TableCellProperties(new TableCellWidth { Width = "0", Type = TableWidthUnitValues.Auto });
        if (isHeader)
        {
            cellProps.Append(new Shading { Val = ShadingPatternValues.Clear, Color = "auto", Fill = "1F4E79" });
        }
        cell.Append(cellProps);
        
        var para = new Paragraph(
            new ParagraphProperties(
                new Justification { Val = JustificationValues.Center },
                new SpacingBetweenLines { Before = "60", After = "60" }
            ),
            new Run(
                new RunProperties(
                    isHeader ? new Bold() : null,
                    isHeader ? new Color { Val = "FFFFFF" } : null
                ),
                new Text(text)
            )
        );
        cell.Append(para);
        
        return cell;
    }
}
