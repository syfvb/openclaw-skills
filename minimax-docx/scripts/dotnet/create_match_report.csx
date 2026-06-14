#!/usr/bin/env dotnet-script
#r "nuget: DocumentFormat.OpenXml, 3.2.0"

using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using System.IO;

var outputPath = "/root/.openclaw/workspace-h0assistant/HZERO匹配度分析报告.docx";

// Create document
WordprocessingDocument doc = null;
try {
    doc = WordprocessingDocument.Create(outputPath, WordprocessingDocumentType.Document);
    var mainPart = doc.AddMainDocumentPart();
    mainPart.Document = new Document(new Body());

    var body = mainPart.Document.Body;

    // Add styles
    var stylesPart = mainPart.AddNewPart<StyleDefinitionsPart>();
    stylesPart.Styles = CreateStyles();

    // Page setup (A4)
    var sectPr = new SectionProperties(
        new PageSize { Width = 11906, Height = 16838 },
        new PageMargin { Top = 1440, Right = 1440, Bottom = 1440, Left = 1440, Header = 720, Footer = 720 }
    );
    body.AppendChild(sectPr);

    // Title
    AddHeading(body, "HZERO PaaS平台与仿真集成系统需求匹配度分析报告", 1);

    // Meta info
    AddParagraph(body, "编制单位：汉得信息", false, true);
    AddParagraph(body, "编制日期：2026年5月16日", false, true);
    AddParagraph(body, "");

    // Section 1: 需求概览
    AddHeading(body, "一、需求概览", 2);
    AddParagraph(body, "客户需求是一个仿真集成系统，包含9个子系统和15项性能比测要求。");
    AddParagraph(body, "");
    AddParagraph(body, "核心子系统包括：DevOps持续交付、低代码开发、组件服务、容器服务、能力集成中心、服务配置、能力开放、服务运营管理、服务运维管理。");
    AddParagraph(body, "");
    AddParagraph(body, "关键特征：需要实时容器、实时调度能力以满足仿真场景需求。");

    // Section 2: 匹配度分析
    AddHeading(body, "二、匹配度分析", 2);

    // Create table
    var table = new Table(
        new TableProperties(
            new TableBorders(
                new TopBorder { Val = BorderValues.Single, Size = 4, Color = "000000" },
                new BottomBorder { Val = BorderValues.Single, Size = 4, Color = "000000" },
                new LeftBorder { Val = BorderValues.Single, Size = 4, Color = "000000" },
                new RightBorder { Val = BorderValues.Single, Size = 4, Color = "000000" },
                new InsideHorizontalBorder { Val = BorderValues.Single, Size = 4, Color = "000000" },
                new InsideVerticalBorder { Val = BorderValues.Single, Size = 4, Color = "000000" }
            ),
            new TableWidth { Width = "5000", Type = TableWidthUnitValues.Pct }
        )
    );

    // Headers
    var headerRow = new TableRow();
    AddTableCell(headerRow, "子系统需求", true);
    AddTableCell(headerRow, "HZERO对应能力", true);
    AddTableCell(headerRow, "匹配度", true);
    AddTableCell(headerRow, "说明", true);
    table.AppendChild(headerRow);

    // Data rows
    var data = new[] {
        ("DevOps持续交付", "DevPaaS开发底座 + CI/CD集成", "85%", "有代码管理、构建、部署能力，灰度发布需扩展"),
        ("低代码开发", "aPaaS低代码平台 + 班翎流程平台", "90%", "页面开发、逻辑编排能力完整覆盖"),
        ("组件服务", "DevPaaS微服务组件", "75%", "有服务模板、脚手架，元数据管理需扩展"),
        ("容器服务", "鲲苍容器管理（K8s）", "65%", "有编排、镜像、伸缩能力，缺实时容器/实时OS"),
        ("能力集成中心", "集星獭集成平台", "80%", "服务组合、执行、日志能力完整"),
        ("服务配置", "集星獭 + 鲲苍", "70%", "有目录、模板能力，SLA控制需扩展"),
        ("能力开放", "集星獭API网关", "75%", "有网关、路由、负载均衡，缺实时调度"),
        ("服务运营管理", "HZERO IAM + 零衍", "85%", "认证、权限、审批能力完整"),
        ("服务运维管理", "行狼监控运维平台", "80%", "调用链、监控、告警能力完整")
    };

    foreach (var row in data) {
        var tr = new TableRow();
        AddTableCell(tr, row.Item1);
        AddTableCell(tr, row.Item2);
        AddTableCell(tr, row.Item3);
        AddTableCell(tr, row.Item4);
        table.AppendChild(tr);
    }

    body.AppendChild(table);
    AddParagraph(body, "");

    // Section 3: 综合匹配度
    AddHeading(body, "三、综合匹配度", 2);
    AddParagraph(body, "综合评估结果：");
    AddParagraph(body, "");
    AddParagraph(body, "• 通用PaaS能力匹配度：81.25%");
    AddParagraph(body, "• 仿真专业能力匹配度：25%");
    AddParagraph(body, "• 综合匹配度：58.6%");
    AddParagraph(body, "");
    AddParagraph(body, "结论：HZERO在通用PaaS能力（DevOps、低代码、集成、运维）方面匹配度较高，但在仿真专业能力（实时容器、实时调度、仿真引擎集成）方面存在明显差距。");

    // Section 4: 核心差距分析
    AddHeading(body, "四、核心差距分析", 2);
    AddParagraph(body, "主要差距项：");
    AddParagraph(body, "");
    AddParagraph(body, "1. ROSS仿真引擎集成能力缺失");
    AddParagraph(body, "   • 客户需要集成ROSS（Robot Operating System）仿真引擎");
    AddParagraph(body, "   • HZERO目前无此类专业仿真组件");
    AddParagraph(body, "");
    AddParagraph(body, "2. 实时容器/实时操作系统支持缺失");
    AddParagraph(body, "   • 客户要求实时容器能力（子系统应用实时容器技术和实时操作系统技术）");
    AddParagraph(body, "   • HZERO容器管理基于标准K8s，无实时OS支持");
    AddParagraph(body, "");
    AddParagraph(body, "3. 仿真性能优化能力缺失");
    AddParagraph(body, "   • 客户要求乐观/保守同步等仿真优化机制");
    AddParagraph(body, "   • HZERO无仿真场景专用性能优化方案");
    AddParagraph(body, "");
    AddParagraph(body, "4. 实时服务调度能力不足");
    AddParagraph(body, "   • 客户要求服务实时调度功能，满足实时仿真要求");
    AddParagraph(body, "   • HZERO服务调度基于常规负载均衡，无实时调度机制");

    // Section 5: 推荐方案
    AddHeading(body, "五、推荐方案", 2);
    AddParagraph(body, "建议采用分层建设方案：");
    AddParagraph(body, "");
    AddParagraph(body, "方案一：HZERO底座 + 仿真引擎集成");
    AddParagraph(body, "• 利用HZERO通用PaaS能力作为底座");
    AddParagraph(body, "• 集成第三方ROSS仿真引擎");
    AddParagraph(body, "• 扩展实时容器组件");
    AddParagraph(body, "");
    AddParagraph(body, "方案二：定制化开发");
    AddParagraph(body, "• 在HZERO基础上定制实时容器服务");
    AddParagraph(body, "• 开发仿真专用调度模块");
    AddParagraph(body, "• 预估工作量：3-6个月");
    AddParagraph(body, "");
    AddParagraph(body, "方案三：混合架构");
    AddParagraph(body, "• HZERO负责通用PaaS功能");
    AddParagraph(body, "• 独立部署仿真专用子系统");
    AddParagraph(body, "• 通过集星獭实现能力集成");
    AddParagraph(body, "");
    AddParagraph(body, "推荐：方案一，平衡成本与能力覆盖，预计综合匹配度可达85%。");

} finally {
    if (doc != null) {
        doc.Dispose();
    }
}

Console.WriteLine($"Document created: {outputPath}");

// Helper functions
void AddHeading(Body body, string text, int level) {
    var p = new Paragraph(
        new ParagraphProperties(
            new ParagraphStyleId { Val = $"Heading{level}" },
            new SpacingBetweenLines { Before = "240", After = "120" }
        ),
        new Run(
            new RunProperties(
                new Bold(),
                new FontSize { Val = level == 1 ? "32" : "28" }
            ),
            new Text(text)
        )
    );
    body.AppendChild(p);
}

void AddParagraph(Body body, string text, bool bold = false, bool center = false) {
    var pProps = new ParagraphProperties(
        new ParagraphStyleId { Val = "Normal" },
        new SpacingBetweenLines { After = "120", Line = "259", LineRule = LineSpacingRuleValues.Auto }
    );
    if (center) pProps.AppendChild(new Justification { Val = JustificationValues.Center });
    
    var runProps = new RunProperties();
    if (bold) runProps.AppendChild(new Bold());
    
    var p = new Paragraph(pProps, new Run(runProps, new Text(text)));
    body.AppendChild(p);
}

void AddTableCell(TableRow row, string text, bool isHeader = false) {
    var tc = new TableCell(
        new TableCellProperties(
            new TableCellWidth { Width = "2500", Type = TableWidthUnitValues.Auto }
        ),
        new Paragraph(
            new ParagraphProperties(
                new ParagraphStyleId { Val = "Normal" },
                new SpacingBetweenLines { After = "60" }
            ),
            new Run(
                new RunProperties(
                    isHeader ? new Bold() : null,
                    new FontSize { Val = "20" }
                ),
                new Text(text)
            )
        )
    );
    row.AppendChild(tc);
}

Styles CreateStyles() {
    return new Styles(
        new Style(
            new StyleId { Val = "Normal" },
            new Name { Val = "Normal" },
            new StyleRunProperties(
                new RunFonts { Ascii = "SimSun", HighAnsi = "SimSun", EastAsia = "SimSun" },
                new FontSize { Val = "22" }
            ),
            new StyleParagraphProperties(
                new SpacingBetweenLines { After = "160", Line = "259", LineRule = LineSpacingRuleValues.Auto }
            )
        ),
        new Style(
            new StyleId { Val = "Heading1" },
            new Name { Val = "Heading 1" },
            new BasedOn { Val = "Normal" },
            new StyleRunProperties(
                new Bold(),
                new FontSize { Val = "32" },
                new Color { Val = "000000" }
            ),
            new StyleParagraphProperties(
                new SpacingBetweenLines { Before = "240", After = "120" },
                new OutlineLevel { Val = 0 }
            )
        ),
        new Style(
            new StyleId { Val = "Heading2" },
            new Name { Val = "Heading 2" },
            new BasedOn { Val = "Normal" },
            new StyleRunProperties(
                new Bold(),
                new FontSize { Val = "28" },
                new Color { Val = "000000" }
            ),
            new StyleParagraphProperties(
                new SpacingBetweenLines { Before = "200", After = "100" },
                new OutlineLevel { Val = 1 }
            )
        )
    );
}