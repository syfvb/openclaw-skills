// Blue Tech Style Word Report Generator
// Execute with: dotnet-script scripts/dotnet/blue_report.csx
#r "nuget: DocumentFormat.OpenXml, 3.2.0"

using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using System.IO;

// === Constants for Blue Tech Style ===
const string COLOR_TITLE = "1F3864";      // Deep navy
const string COLOR_H1 = "1F3864";         // Deep navy
const string COLOR_H2 = "2F5496";         // Medium blue
const string COLOR_H3 = "2F5496";         // Medium blue
const string COLOR_BODY = "333333";       // Dark gray (not pure black)
const string COLOR_TABLE_HEADER = "2F5496"; // Medium blue (not deep)
const string COLOR_TABLE_BORDER = "2F5496"; // Medium blue
const string COLOR_TABLE_INSIDE = "B0B0B0"; // Light gray
const string COLOR_INSIGHT_BG = "E6F0FA";   // Light blue background
const string COLOR_INSIGHT_BORDER = "2F5496"; // Medium blue left border
const string COLOR_ODD_ROW = "FFFFFF";      // White
const string COLOR_EVEN_ROW = "EDF2F9";     // Light blue-gray

// Font names
const string FONT_EN = "Calibri";
const string FONT_CN_BODY = "SimSun";      // 宋体
const string FONT_CN_HEADING = "SimHei";   // 黑体

// Page margin: 2.54cm = ~1440 DXA (1 inch)
const int PAGE_MARGIN_DXA = 1440;

// Output path
var outputPath = "/root/.openclaw/media/outbound/OpenClaw_Token_Analysis_Blue_Report.docx";

// === Create Document ===
var doc = WordprocessingDocument.Create(outputPath, WordprocessingDocumentType.Document);
var mainPart = doc.AddMainDocumentPart();
mainPart.Document = new Document();

// === Create Styles ===
var stylesPart = mainPart.AddNewPart<StyleDefinitionsPart>();
stylesPart.Styles = new Styles();
CreateBlueTechStyles(stylesPart.Styles);
stylesPart.Styles.Save();

// === Set Page Margins ===
var body = new Body();
var sectPr = new SectionProperties();
sectPr.Append(new PageMargin
{
    Top = PAGE_MARGIN_DXA,
    Bottom = PAGE_MARGIN_DXA,
    Left = PAGE_MARGIN_DXA,
    Right = PAGE_MARGIN_DXA,
    Header = 720,
    Footer = 720,
    Gutter = 0
});
body.Append(sectPr);

// === Build Document Content ===
// Title
body.Append(CreateTitleParagraph("OpenClaw Token 消耗分析报告"));

// 一、概述
body.Append(CreateH1Paragraph("一、概述"));
body.Append(CreateBodyParagraph("本报告基于 OpenClaw 今日使用数据（2026年5月25日），结合阿里云百炼平台的计费规则，分析 Token 消耗情况，并对 Token Plan 团队版套餐的适用性进行评估。"));

// 二、今日使用数据
body.Append(CreateH1Paragraph("二、今日使用数据"));
body.Append(CreateBodyParagraph("以下为今日 OpenClaw 系统的使用统计数据："));

// Data table
var dataRows = new string[][]
{
    new[] { "总 Token 消耗", "30.7M" },
    new[] { "消息数量", "461 条" },
    new[] { "会话数量", "14 个" },
    new[] { "工具调用", "281 次" },
    new[] { "缓存命中率", "80.2%（显著降低成本）" },
    new[] { "错误率", "0.00%（系统运行稳定）" },
    new[] { "吞吐量", "68.2K tok/min" }
};
body.Append(CreateDataTable(new[] { "指标", "数值" }, dataRows));

// 三、阿里云按量付费价格分析
body.Append(CreateH1Paragraph("三、阿里云按量付费价格分析"));
body.Append(CreateH2Paragraph("3.1 GLM-5 模型价格"));
body.Append(CreateBodyParagraph("GLM-5 为智谱 AI 提供的文本生成模型，在阿里云百炼平台的计费标准如下："));

var priceRows1 = new string[][]
{
    new[] { "输入 Token 范围 0-32K", "输入单价 ¥4/百万，输出单价 ¥18/百万" },
    new[] { "输入 Token 范围 32K-198K", "输入单价 ¥6/百万，输出单价 ¥22/百万" }
};
body.Append(CreateDataTable(new[] { "范围", "价格" }, priceRows1));

body.Append(CreateH2Paragraph("3.2 上下文缓存价格"));
body.Append(CreateBodyParagraph("阿里云百炼支持上下文缓存功能，缓存命中后的 Token 价格大幅降低："));

var cacheRows = new string[][]
{
    new[] { "隐式缓存（自动）", "创建缓存按输入单价 100%，命中缓存按输入单价 20%" },
    new[] { "显式缓存（需开启）", "创建缓存按输入单价 125%，命中缓存按输入单价 10%" }
};
body.Append(CreateDataTable(new[] { "缓存类型", "计费规则" }, cacheRows));

// 四、Token Plan 团队版套餐对比
body.Append(CreateH1Paragraph("四、Token Plan 团队版套餐对比"));
body.Append(CreateBodyParagraph("阿里云百炼 Token Plan 团队版提供三档套餐，以 Credits 统一计量："));

var planRows = new string[][]
{
    new[] { "标准坐席", "¥198/月", "25,000 Credits", "适合轻度使用" },
    new[] { "高级坐席", "¥698/月", "100,000 Credits", "适合日常高频使用" },
    new[] { "尊享坐席", "¥1,398/月", "250,000 Credits", "适合重度依赖用户" }
};
body.Append(CreateDataTable(new[] { "套餐", "价格", "Credits", "适用场景" }, planRows));

// 五、Credits 消耗计算示例
body.Append(CreateH1Paragraph("五、Credits 消耗计算示例"));
body.Append(CreateBodyParagraph("根据阿里云官方示例（以 qwen3.6-plus 为例），单次请求消耗明细如下："));

var creditRows = new string[][]
{
    new[] { "输入 tokens", "8,349", "1.67 Credits" },
    new[] { "缓存 tokens", "40,794", "0.82 Credits" },
    new[] { "输出 tokens", "573", "0.69 Credits" },
    new[] { "合计", "约 50K tokens", "3.18 Credits" }
};
body.Append(CreateDataTable(new[] { "类型", "数量", "Credits" }, creditRows));

body.Append(CreateInsightBox("注意：不同模型的单价不同，实际消耗以账单为准。GLM-5 的 Credits 消耗可能比 qwen3.6-plus 高 1.5-2 倍。"));

// 六、成本估算
body.Append(CreateH1Paragraph("六、成本估算"));
body.Append(CreateH2Paragraph("6.1 按量付费估算"));
body.Append(CreateBodyParagraph("基于 80.2% 缓存命中率，输入:输出约 10:1 的比例："));

var costRows = new string[][]
{
    new[] { "缓存输入（约 24M）", "¥0.8/百万", "约 ¥19" },
    new[] { "非缓存输入（约 5.5M）", "¥4/百万", "约 ¥22" },
    new[] { "输出（约 2.8M）", "¥18/百万", "约 ¥50" },
    new[] { "总计", "", "约 ¥90" }
};
body.Append(CreateDataTable(new[] { "类型", "单价", "估算" }, costRows));

body.Append(CreateH2Paragraph("6.2 Credits 消耗估算"));
body.Append(CreateBodyParagraph("基于阿里云官方示例推算："));
body.Append(CreateBodyParagraph("• 30.7M tokens ÷ 50K/次 ≈ 614 次调用"));
body.Append(CreateBodyParagraph("• 614 次 × 3.18 Credits ≈ 1,945 Credits（qwen3.6-plus）"));
body.Append(CreateBodyParagraph("• GLM-5 较高，实际约 3,000-4,000 Credits"));

// 七、结论与建议
body.Append(CreateH1Paragraph("七、结论与建议"));
body.Append(CreateH2Paragraph("7.1 套餐适用性分析"));

var suitRows = new string[][]
{
    new[] { "¥198 标准坐席", "25,000 Credits", "够用约 6-8 天（高频使用）" },
    new[] { "¥698 高级坐席", "100,000 Credits", "适合日常高频使用" },
    new[] { "¥1,398 尊享坐席", "250,000 Credits", "适合重度依赖用户" }
};
body.Append(CreateDataTable(new[] { "套餐", "Credits", "适用性" }, suitRows));

body.Append(CreateH2Paragraph("7.2 建议"));
body.Append(CreateBodyParagraph("1. 建议先试用一个月 ¥198 标准坐席，观察实际 Credits 消耗"));
body.Append(CreateBodyParagraph("2. 若日均消耗超过 3,000 Credits，需升级至 ¥698 高级坐席"));
body.Append(CreateBodyParagraph("3. 高缓存命中率（80%+）显著降低成本，建议保持良好上下文习惯"));
body.Append(CreateBodyParagraph("4. Token Plan 相比按量付费更适合高频稳定使用场景"));

// Footer info
body.Append(CreateSpacerParagraph());
body.Append(CreateBodyParagraph("报告日期：2026年5月25日"));
body.Append(CreateBodyParagraph("生成工具：微信ClawBot（OpenClaw）"));

// === Save ===
mainPart.Document.Append(body);
mainPart.Document.Save();
doc.Dispose();

Console.WriteLine($"✅ Document generated: {outputPath}");

// === Helper Functions ===

void CreateBlueTechStyles(Styles styles)
{
    // DocDefaults
    var docDefaults = new DocDefaults(
        new RunPropertiesDefault(
            new RunPropertiesBaseStyle(
                new RunFonts { Ascii = FONT_EN, HighAnsi = FONT_EN, EastAsia = FONT_CN_BODY, ComplexScript = FONT_EN },
                new FontSize { Val = "22" },
                new FontSizeComplexScript { Val = "22" },
                new Languages { Val = "en-US", EastAsia = "zh-CN" }
            )
        ),
        new ParagraphPropertiesDefault(
            new ParagraphPropertiesBaseStyle(
                new SpacingBetweenLines { After = "120", Line = "276", LineRule = LineSpacingRuleValues.Auto }
            )
        )
    );
    styles.Append(docDefaults);
    
    // Normal style
    var normalStyle = new Style(
        new StyleName { Val = "Normal" },
        new UIPriority { Val = 0 },
        new PrimaryStyle(),
        new StyleParagraphProperties(
            new SpacingBetweenLines { After = "120", Line = "276", LineRule = LineSpacingRuleValues.Auto }
        ),
        new StyleRunProperties(
            new RunFonts { Ascii = FONT_EN, HighAnsi = FONT_EN, EastAsia = FONT_CN_BODY, ComplexScript = FONT_EN },
            new FontSize { Val = "22" },
            new FontSizeComplexScript { Val = "22" },
            new Color { Val = COLOR_BODY }
        )
    ) { Type = StyleValues.Paragraph, StyleId = "Normal", Default = true };
    styles.Append(normalStyle);
    
    // Title style
    var titleStyle = new Style(
        new StyleName { Val = "Title" },
        new BasedOn { Val = "Normal" },
        new UIPriority { Val = 10 },
        new PrimaryStyle(),
        new StyleParagraphProperties(
            new Justification { Val = JustificationValues.Center },
            new SpacingBetweenLines { Before = "0", After = "200", Line = "240", LineRule = LineSpacingRuleValues.Auto }
        ),
        new StyleRunProperties(
            new RunFonts { Ascii = FONT_EN, HighAnsi = FONT_EN, EastAsia = FONT_CN_BODY, ComplexScript = FONT_EN },
            new FontSize { Val = "36" },
            new FontSizeComplexScript { Val = "36" },
            new Bold(),
            new BoldComplexScript(),
            new Color { Val = COLOR_TITLE }
        )
    ) { Type = StyleValues.Paragraph, StyleId = "Title" };
    styles.Append(titleStyle);
    
    // Heading 1 style
    var h1Style = new Style(
        new StyleName { Val = "heading 1" },
        new BasedOn { Val = "Normal" },
        new NextParagraphStyle { Val = "Normal" },
        new UIPriority { Val = 9 },
        new PrimaryStyle(),
        new StyleParagraphProperties(
            new KeepNext(),
            new KeepLines(),
            new SpacingBetweenLines { Before = "360", After = "120", Line = "240", LineRule = LineSpacingRuleValues.Auto },
            new OutlineLevel { Val = 0 }
        ),
        new StyleRunProperties(
            new RunFonts { Ascii = FONT_EN, HighAnsi = FONT_EN, EastAsia = FONT_CN_HEADING, ComplexScript = FONT_EN },
            new FontSize { Val = "32" },
            new FontSizeComplexScript { Val = "32" },
            new Bold(),
            new BoldComplexScript(),
            new Color { Val = COLOR_H1 }
        )
    ) { Type = StyleValues.Paragraph, StyleId = "Heading1" };
    styles.Append(h1Style);
    
    // Heading 2 style
    var h2Style = new Style(
        new StyleName { Val = "heading 2" },
        new BasedOn { Val = "Normal" },
        new NextParagraphStyle { Val = "Normal" },
        new UIPriority { Val = 9 },
        new PrimaryStyle(),
        new StyleParagraphProperties(
            new KeepNext(),
            new KeepLines(),
            new SpacingBetweenLines { Before = "360", After = "120", Line = "240", LineRule = LineSpacingRuleValues.Auto },
            new OutlineLevel { Val = 1 }
        ),
        new StyleRunProperties(
            new RunFonts { Ascii = FONT_EN, HighAnsi = FONT_EN, EastAsia = FONT_CN_HEADING, ComplexScript = FONT_EN },
            new FontSize { Val = "28" },
            new FontSizeComplexScript { Val = "28" },
            new Bold(),
            new BoldComplexScript(),
            new Color { Val = COLOR_H2 }
        )
    ) { Type = StyleValues.Paragraph, StyleId = "Heading2" };
    styles.Append(h2Style);
    
    // Heading 3 style
    var h3Style = new Style(
        new StyleName { Val = "heading 3" },
        new BasedOn { Val = "Normal" },
        new NextParagraphStyle { Val = "Normal" },
        new UIPriority { Val = 9 },
        new PrimaryStyle(),
        new StyleParagraphProperties(
            new KeepNext(),
            new KeepLines(),
            new SpacingBetweenLines { Before = "240", After = "60", Line = "240", LineRule = LineSpacingRuleValues.Auto },
            new OutlineLevel { Val = 2 }
        ),
        new StyleRunProperties(
            new RunFonts { Ascii = FONT_EN, HighAnsi = FONT_EN, EastAsia = FONT_CN_HEADING, ComplexScript = FONT_EN },
            new FontSize { Val = "24" },
            new FontSizeComplexScript { Val = "24" },
            new Bold(),
            new BoldComplexScript(),
            new Color { Val = COLOR_H3 }
        )
    ) { Type = StyleValues.Paragraph, StyleId = "Heading3" };
    styles.Append(h3Style);
    
    // InsightBox style
    var insightStyle = new Style(
        new StyleName { Val = "InsightBox" },
        new BasedOn { Val = "Normal" },
        new UIPriority { Val = 50 },
        new StyleParagraphProperties(
            new ParagraphBorders(
                new LeftBorder { Val = BorderValues.Single, Size = 24, Space = 0, Color = COLOR_INSIGHT_BORDER }
            ),
            new Shading { Val = ShadingPatternValues.Clear, Color = "auto", Fill = COLOR_INSIGHT_BG },
            new SpacingBetweenLines { Before = "120", After = "120", Line = "276", LineRule = LineSpacingRuleValues.Auto },
            new Indentation { Left = "216" }
        ),
        new StyleRunProperties(
            new RunFonts { Ascii = FONT_EN, HighAnsi = FONT_EN, EastAsia = FONT_CN_BODY, ComplexScript = FONT_EN },
            new FontSize { Val = "22" },
            new Color { Val = COLOR_BODY }
        )
    ) { Type = StyleValues.Paragraph, StyleId = "InsightBox", CustomStyle = true };
    styles.Append(insightStyle);
}

Paragraph CreateTitleParagraph(string text)
{
    var para = new Paragraph(
        new ParagraphProperties(
            new ParagraphStyleId { Val = "Title" }
        ),
        new Run(new Text(text) { Space = SpaceProcessingModeValues.Preserve })
    );
    return para;
}

Paragraph CreateH1Paragraph(string text)
{
    var para = new Paragraph(
        new ParagraphProperties(
            new ParagraphStyleId { Val = "Heading1" }
        ),
        new Run(new Text(text) { Space = SpaceProcessingModeValues.Preserve })
    );
    return para;
}

Paragraph CreateH2Paragraph(string text)
{
    var para = new Paragraph(
        new ParagraphProperties(
            new ParagraphStyleId { Val = "Heading2" }
        ),
        new Run(new Text(text) { Space = SpaceProcessingModeValues.Preserve })
    );
    return para;
}

Paragraph CreateH3Paragraph(string text)
{
    var para = new Paragraph(
        new ParagraphProperties(
            new ParagraphStyleId { Val = "Heading3" }
        ),
        new Run(new Text(text) { Space = SpaceProcessingModeValues.Preserve })
    );
    return para;
}

Paragraph CreateBodyParagraph(string text)
{
    var para = new Paragraph(
        new ParagraphProperties(
            new ParagraphStyleId { Val = "Normal" }
        ),
        new Run(
            new RunProperties(
                new RunFonts { Ascii = FONT_EN, HighAnsi = FONT_EN, EastAsia = FONT_CN_BODY, ComplexScript = FONT_EN },
                new FontSize { Val = "22" },
                new Color { Val = COLOR_BODY }
            ),
            new Text(text) { Space = SpaceProcessingModeValues.Preserve }
        )
    );
    return para;
}

Paragraph CreateInsightBox(string text)
{
    var para = new Paragraph(
        new ParagraphProperties(
            new ParagraphStyleId { Val = "InsightBox" }
        ),
        new Run(new Text(text) { Space = SpaceProcessingModeValues.Preserve })
    );
    return para;
}

Paragraph CreateSpacerParagraph()
{
    return new Paragraph(
        new ParagraphProperties(
            new SpacingBetweenLines { Before = "240" }
        )
    );
}

Table CreateDataTable(string[] headers, string[][] data)
{
    var table = new Table();
    
    // Table properties
    var tblPr = new TableProperties(
        new TableWidth { Width = "5000", Type = TableWidthUnitValues.Pct },
        new TableBorders(
            new TopBorder { Val = BorderValues.Single, Size = 8, Space = 0, Color = COLOR_TABLE_BORDER },
            new BottomBorder { Val = BorderValues.Single, Size = 8, Space = 0, Color = COLOR_TABLE_BORDER },
            new LeftBorder { Val = BorderValues.None },
            new RightBorder { Val = BorderValues.None },
            new InsideHorizontalBorder { Val = BorderValues.Single, Size = 4, Space = 0, Color = COLOR_TABLE_INSIDE },
            new InsideVerticalBorder { Val = BorderValues.Single, Size = 4, Space = 0, Color = COLOR_TABLE_INSIDE }
        ),
        new TableLook { FirstRow = true, NoHorizontalBand = false }
    );
    table.Append(tblPr);
    
    // Grid
    var grid = new TableGrid();
    foreach (var _ in headers)
    {
        grid.Append(new GridColumn { Width = (9360 / headers.Length).ToString() });
    }
    table.Append(grid);
    
    // Header row
    var headerRow = new TableRow();
    headerRow.Append(new TableHeader());
    foreach (var h in headers)
    {
        var cell = new TableCell(
            new TableCellProperties(
                new Shading { Val = ShadingPatternValues.Clear, Color = "auto", Fill = COLOR_TABLE_HEADER },
                new TableCellVerticalAlignment { Val = TableVerticalAlignmentValues.Center }
            ),
            new Paragraph(
                new ParagraphProperties(
                    new Justification { Val = JustificationValues.Center },
                    new SpacingBetweenLines { After = "0", Line = "276", LineRule = LineSpacingRuleValues.Auto }
                ),
                new Run(
                    new RunProperties(
                        new Bold(),
                        new BoldComplexScript(),
                        new Color { Val = "FFFFFF" },
                        new RunFonts { Ascii = FONT_EN, HighAnsi = FONT_EN, EastAsia = FONT_CN_HEADING, ComplexScript = FONT_EN },
                        new FontSize { Val = "22" }
                    ),
                    new Text(h) { Space = SpaceProcessingModeValues.Preserve }
                )
            )
        );
        headerRow.Append(cell);
    }
    table.Append(headerRow);
    
    // Data rows with zebra striping
    for (int i = 0; i < data.Length; i++)
    {
        var row = new TableRow();
        var fillColor = (i % 2 == 1) ? COLOR_EVEN_ROW : COLOR_ODD_ROW;
        
        for (int j = 0; j < data[i].Length; j++)
        {
            var cellText = data[i][j];
            var isFirstCol = (j == 0);
            
            var tcPr = new TableCellProperties(
                new Shading { Val = ShadingPatternValues.Clear, Color = "auto", Fill = fillColor }
            );
            
            var rPr = new RunProperties(
                new RunFonts { Ascii = FONT_EN, HighAnsi = FONT_EN, EastAsia = isFirstCol ? FONT_CN_HEADING : FONT_CN_BODY, ComplexScript = FONT_EN },
                new FontSize { Val = "22" },
                new Color { Val = COLOR_BODY }
            );
            if (isFirstCol)
            {
                rPr.Append(new Bold());
                rPr.Append(new BoldComplexScript());
            }
            
            var para = new Paragraph(
                new ParagraphProperties(
                    new SpacingBetweenLines { After = "0", Line = "276", LineRule = LineSpacingRuleValues.Auto }
                ),
                new Run(rPr, new Text(cellText) { Space = SpaceProcessingModeValues.Preserve })
            );
            
            var cell = new TableCell(tcPr, para);
            row.Append(cell);
        }
        table.Append(row);
    }
    
    return table;
}