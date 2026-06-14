// 汉得飞搭低代码平台特点报告
#r "nuget: DocumentFormat.OpenXml, 3.2.0"

using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;

var outputPath = "/root/.openclaw/workspace-h0assistant/汉得飞搭低代码平台特点.docx";

// Create document
using var doc = WordprocessingDocument.Create(outputPath, WordprocessingDocumentType.Document);
var mainPart = doc.AddMainDocumentPart();
mainPart.Document = new Document(new Body());
var body = mainPart.Document.Body;

// Add styles
AddCorporateStyles(mainPart);

// Title
AddTitle(body, "汉得飞搭低代码平台特点分析");
AddParagraph(body, "");
AddParagraph(body, "基于 HZERO 底座的企业级低代码开发平台");
AddParagraph(body, "");
AddParagraph(body, "报告日期：2026年5月7日");
AddParagraph(body, "");

// Section 1: Overview
AddHeading(body, "一、飞搭平台概述", 1);
AddParagraph(body, "飞搭（FeiDa）是汉得信息（Hand Enterprise Solutions）推出的企业级低代码/无代码应用开发平台，基于 HZERO 微服务架构底座构建，面向中大型企业复杂业务场景，提供可视化、模型驱动的应用开发能力。");

// Section 2: Core Features
AddHeading(body, "二、核心特点对比", 1);

AddHeading(body, "1. 企业级技术底座", 2);
AddParagraph(body, "与市场上多数低代码平台不同，飞搭并非基于公有云或第三方框架构建，而是建立在汉得自研的 HZERO 微服务架构之上：");
AddBulletPoint(body, "HZERO 经过多年企业级项目验证，支撑复杂业务场景");
AddBulletPoint(body, "内置组织、权限、流程、报表等企业级基础能力");
AddBulletPoint(body, "不是\"从零开始\"的低代码，而是\"站在巨人肩膀上\"的开发模式");

AddHeading(body, "2. 低代码 + 高代码融合", 2);
AddParagraph(body, "飞搭解决\"低代码天花板\"问题的独特方式：");
AddBulletPoint(body, "低代码部分：飞搭提供可视化设计，产出 JSON 格式元数据");
AddBulletPoint(body, "高代码部分：基于 HZERO 底座进行专业开发，拥有完整源码控制权");
AddBulletPoint(body, "两者在同一体系内无缝集成，非割裂方案");

AddHeading(body, "3. 元数据驱动架构", 2);
AddParagraph(body, "飞搭的设计产物与传统低代码平台有本质区别：");
AddBulletPoint(body, "设计产物为 JSON 格式元数据，非传统源码");
AddBulletPoint(body, "依赖飞搭低代码引擎解析渲染");
AddBulletPoint(body, "优势：版本管理、迁移部署更灵活");
AddBulletPoint(body, "注意：不支持直接修改导出的\"源码\"进行二次开发");

AddHeading(body, "4. 私有化部署与信创支持", 2);
AddParagraph(body, "飞搭在部署模式上的差异化定位：");
AddBulletPoint(body, "以私有化部署为主，区别于公有云 SaaS 模式");
AddBulletPoint(body, "支持国产数据库、中间件、操作系统");
AddBulletPoint(body, "符合信创要求，满足大型企业合规需求");

AddHeading(body, "5. HZERO 生态一体化", 2);
AddParagraph(body, "飞搭与 HZERO 生态的深度整合：");
AddBulletPoint(body, "低代码页面与 HZERO 标准功能无缝衔接");
AddBulletPoint(body, "统一的用户体系、权限体系、数据模型");
AddBulletPoint(body, "可复用 HZERO 丰富的企业级组件和业务能力");

// Section 3: Comparison
AddHeading(body, "三、与主流低代码平台对比", 1);

AddTable(body, 
    new[] { "对比维度", "飞搭（HZERO）", "典型低代码平台" },
    new[] {
        new[] { "技术底座", "自研 HZERO 微服务架构", "基于公有云或第三方框架" },
        new[] { "部署模式", "私有化部署为主", "公有云 SaaS 为主" },
        new[] { "\"源码\"本质", "JSON 格式元数据", "通常封闭存储" },
        new[] { "扩展方式", "HZERO 高代码无缝集成", "依赖平台开放接口" },
        new[] { "目标客群", "中大型企业、复杂业务", "中小企业、轻量级应用" },
        new[] { "国产化", "支持信创环境", "有限或需额外配置" }
    });

// Section 4: Application Scenarios
AddHeading(body, "四、适用场景", 1);

AddHeading(body, "飞搭优势场景", 2);
AddBulletPoint(body, "复杂 ERP 类应用开发");
AddBulletPoint(body, "大规模私有化部署需求");
AddBulletPoint(body, "需要与现有 HZERO 系统深度集成");
AddBulletPoint(body, "有信创/国产化合规要求");

AddHeading(body, "其他平台优势场景", 2);
AddBulletPoint(body, "简单表单/审批快速搭建");
AddBulletPoint(body, "快速上线 MVP 验证");
AddBulletPoint(body, "轻量级应用，无复杂集成需求");

// Section 5: Summary
AddHeading(body, "五、总结", 1);
AddParagraph(body, "飞搭的定位是\"面向企业级复杂场景的低代码平台\"，而非\"面向快速搭建简单应用的轻量级工具\"。");
AddParagraph(body, "");
AddParagraph(body, "其核心竞争优势在于：");
AddBulletPoint(body, "HZERO 企业级底座的支撑能力");
AddBulletPoint(body, "低代码与高代码的无缝融合方案");
AddBulletPoint(body, "私有化部署和信创适配能力");
AddBulletPoint(body, "与 HZERO 生态的深度整合");
AddParagraph(body, "");
AddParagraph(body, "飞搭的竞争对手更像是 OutSystems、Mendix 等国际企业级低代码平台，而非钉钉宜搭、腾讯云微搭等轻量工具。");

// Add page setup
var sectPr = new SectionProperties(
    new PageSize { Width = 11906, Height = 16838 },  // A4
    new PageMargin { Top = 1440, Right = 1440, Bottom = 1440, Left = 1440, Header = 720, Footer = 720 }
);
body.Append(sectPr);

// Save
doc.Save();
Console.WriteLine($"Document created: {outputPath}");

// Helper methods
void AddCorporateStyles(MainDocumentPart mainPart)
{
    var stylesPart = mainPart.AddNewPart<StyleDefinitionsPart>();
    var styles = new Styles();
    
    // Normal style
    styles.Append(new Style(
        new StyleName { Val = "Normal" },
        new StyleParagraphProperties(
            new Spacing { After = 160, Line = 276, LineRule = LineSpacingRule.Auto }
        ),
        new StyleRunProperties(
            new RunFonts { Ascii = "Microsoft YaHei", HighAnsi = "Microsoft YaHei", EastAsia = "Microsoft YaHei" },
            new FontSize { Val = "22" },
            new FontSizeComplexScript { Val = "22" }
        )
    ) { Type = StyleValues.Paragraph });
    
    // Title style
    styles.Append(new Style(
        new StyleName { Val = "Title" },
        new BasedOn { Val = "Normal" },
        new StyleParagraphProperties(
            new Justification { Val = JustificationValues.Center },
            new Spacing { Before = 0, After = 400 }
        ),
        new StyleRunProperties(
            new Bold(),
            new FontSize { Val = "44" },
            new FontSizeComplexScript { Val = "44" },
            new Color { Val = "1F4E79" }
        )
    ) { Type = StyleValues.Paragraph, StyleId = "Title" });
    
    // Heading 1
    styles.Append(new Style(
        new StyleName { Val = "Heading 1" },
        new BasedOn { Val = "Normal" },
        new StyleParagraphProperties(
            new Spacing { Before = 400, After = 200 },
            new OutlineLevel { Val = 0 }
        ),
        new StyleRunProperties(
            new Bold(),
            new FontSize { Val = "32" },
            new FontSizeComplexScript { Val = "32" },
            new Color { Val = "1F4E79" }
        )
    ) { Type = StyleValues.Paragraph, StyleId = "Heading1" });
    
    // Heading 2
    styles.Append(new Style(
        new StyleName { Val = "Heading 2" },
        new BasedOn { Val = "Normal" },
        new StyleParagraphProperties(
            new Spacing { Before = 300, After = 160 },
            new OutlineLevel { Val = 1 }
        ),
        new StyleRunProperties(
            new Bold(),
            new FontSize { Val = "26" },
            new FontSizeComplexScript { Val = "26" },
            new Color { Val = "2E74B5" }
        )
    ) { Type = StyleValues.Paragraph, StyleId = "Heading2" });
    
    stylesPart.Styles = styles;
}

void AddTitle(Body body, string text)
{
    body.Append(new Paragraph(
        new ParagraphProperties(new ParagraphStyleId { Val = "Title" }),
        new Run(new Text(text))
    ));
}

void AddHeading(Body body, string text, int level)
{
    var styleId = level == 1 ? "Heading1" : "Heading2";
    body.Append(new Paragraph(
        new ParagraphProperties(new ParagraphStyleId { Val = styleId }),
        new Run(new Text(text))
    ));
}

void AddParagraph(Body body, string text)
{
    if (string.IsNullOrEmpty(text))
    {
        body.Append(new Paragraph());
        return;
    }
    body.Append(new Paragraph(
        new Run(new Text(text))
    ));
}

void AddBulletPoint(Body body, string text)
{
    body.Append(new Paragraph(
        new ParagraphProperties(
            new NumberingProperties(
                new NumberingLevelReference { Val = 0 },
                new NumberingId { Val = 1 }
            )
        ),
        new Run(new Text(text))
    ));
}

void AddTable(Body body, string[] headers, string[][] rows)
{
    var table = new Table();
    
    // Table properties
    table.Append(new TableProperties(
        new TableWidth { Width = "5000", Type = TableWidthUnitValues.Pct },
        new TableBorders(
            new TopBorder { Val = BorderValues.Single, Size = 8, Color = "000000"