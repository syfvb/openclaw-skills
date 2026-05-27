#r "nuget: DocumentFormat.OpenXml, 3.2.0"
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;

// Color constants
const string CDB = "1F3864";   // dark blue (Title/H1)
const string CMB = "2F5496";   // medium blue (H2/H3/table header)
const string CTG = "333333";   // text gray (body)
const string CLG = "B0B0B0";   // light gray (table inner border)
const string CAR = "EDF2F9";   // alt row (zebra even rows)
const string CWH = "FFFFFF";   // white (header text)

// ====== STYLE DEFINITIONS ======
void DefineStyles(StyleDefinitionsPart sp)
{
    sp.Styles = new Styles();
    var s = sp.Styles;

    // Normal
    s.Append(new Style(
        new StyleName { Val = "Normal" }, new UIPriority { Val = 0 }, new PrimaryStyle(),
        new StyleParagraphProperties(new SpacingBetweenLines { After = "120", Line = "276", LineRule = LineSpacingRuleValues.Auto }),
        new StyleRunProperties(
            new RunFonts { Ascii = "Calibri", HighAnsi = "Calibri", EastAsia = "SimSun", ComplexScript = "Arial" },
            new FontSize { Val = "22" }, new FontSizeComplexScript { Val = "22" },
            new Color { Val = CTG }, new Languages { Val = "en-US", EastAsia = "zh-CN" }
        )
    ) { Type = StyleValues.Paragraph, StyleId = "Normal", Default = true });

    // Title
    s.Append(new Style(
        new StyleName { Val = "Title" }, new BasedOn { Val = "Normal" }, new NextParagraphStyle { Val = "Normal" },
        new UIPriority { Val = 10 }, new PrimaryStyle(),
        new StyleParagraphProperties(new Justification { Val = JustificationValues.Center },
            new SpacingBetweenLines { Before = "0", After = "200", Line = "240", LineRule = LineSpacingRuleValues.Auto }),
        new StyleRunProperties(
            new RunFonts { Ascii = "Calibri", HighAnsi = "Calibri", EastAsia = "SimSun", ComplexScript = "Arial" },
            new FontSize { Val = "36" }, new FontSizeComplexScript { Val = "36" },
            new Bold(), new BoldComplexScript(), new Color { Val = CDB }
        )
    ) { Type = StyleValues.Paragraph, StyleId = "Title" });

    // Heading1
    s.Append(new Style(
        new StyleName { Val = "heading 1" }, new BasedOn { Val = "Normal" }, new NextParagraphStyle { Val = "Normal" },
        new LinkedStyle { Val = "Heading1Char" }, new UIPriority { Val = 9 }, new PrimaryStyle(),
        new StyleParagraphProperties(new KeepNext(), new KeepLines(),
            new SpacingBetweenLines { Before = "360", After = "120", Line = "240", LineRule = LineSpacingRuleValues.Auto },
            new OutlineLevel { Val = 0 }),
        new StyleRunProperties(
            new RunFonts { Ascii = "Calibri", HighAnsi = "Calibri", EastAsia = "SimHei", ComplexScript = "Arial" },
            new FontSize { Val = "32" }, new FontSizeComplexScript { Val = "32" },
            new Bold(), new BoldComplexScript(), new Color { Val = CDB }
        )
    ) { Type = StyleValues.Paragraph, StyleId = "Heading1" });

    // Heading2
    s.Append(new Style(
        new StyleName { Val = "heading 2" }, new BasedOn { Val = "Normal" }, new NextParagraphStyle { Val = "Normal" },
        new LinkedStyle { Val = "Heading2Char" }, new UIPriority { Val = 9 }, new PrimaryStyle(),
        new StyleParagraphProperties(new KeepNext(), new KeepLines(),
            new SpacingBetweenLines { Before = "360", After = "120", Line = "240", LineRule = LineSpacingRuleValues.Auto },
            new OutlineLevel { Val = 1 }),
        new StyleRunProperties(
            new RunFonts { Ascii = "Calibri", HighAnsi = "Calibri", EastAsia = "SimHei", ComplexScript = "Arial" },
            new FontSize { Val = "28" }, new FontSizeComplexScript { Val = "28" },
            new Bold(), new BoldComplexScript(), new Color { Val = CMB }
        )
    ) { Type = StyleValues.Paragraph, StyleId = "Heading2" });

    // Heading3
    s.Append(new Style(
        new StyleName { Val = "heading 3" }, new BasedOn { Val = "Normal" }, new NextParagraphStyle { Val = "Normal" },
        new LinkedStyle { Val = "Heading3Char" }, new UIPriority { Val = 9 }, new PrimaryStyle(),
        new StyleParagraphProperties(new KeepNext(), new KeepLines(),
            new SpacingBetweenLines { Before = "240", After = "120", Line = "240", LineRule = LineSpacingRuleValues.Auto },
            new OutlineLevel { Val = 2 }),
        new StyleRunProperties(
            new RunFonts { Ascii = "Calibri", HighAnsi = "Calibri", EastAsia = "SimHei", ComplexScript = "Arial" },
            new FontSize { Val = "24" }, new FontSizeComplexScript { Val = "24" },
            new Bold(), new BoldComplexScript(), new Color { Val = CMB }
        )
    ) { Type = StyleValues.Paragraph, StyleId = "Heading3" });
}

// ====== PAGE SETUP ======
void SetMargins(Body body)
{
    var sp = body.AppendChild(new SectionProperties());
    // 页边距: 上下2.54cm(1440), 左右2.48cm(1406)
    sp.Append(new PageMargin { Top = 1440, Bottom = 1440, Left = 1406, Right = 1406, Header = 1440, Footer = 1440 });
}

// ====== PARAGRAPH HELPER ======
// styleId: "Title" | "Heading1" | "Heading2" | "Heading3" | "Normal"
void P(Body b, string styleId, string text)
{
    var p = new Paragraph(new ParagraphProperties(new ParagraphStyleId { Val = styleId }));
    var r = new Run();
    if (styleId == "Title" || styleId == "Heading1" || styleId == "Heading2" || styleId == "Heading3")
        r.Append(new RunProperties(new Bold(), new Color { Val = styleId == "Heading2" || styleId == "Heading3" ? CMB : CDB }));
    else
        r.Append(new RunProperties(new Color { Val = CTG }));
    r.Append(new Text(text));
    p.Append(r);
    b.Append(p);
}

// ====== TABLE HELPER ======
// hdrs: column headers (string array)
// rows: data rows (2D string array)
void Table(Body b, string[] hdrs, string[][] rows)
{
    var t = new Table();
    t.Append(new TableProperties(
        new TableWidth { Width = "5000", Type = TableWidthUnitValues.Pct },
        new TableBorders(
            new TopBorder { Val = BorderValues.Single, Size = 12, Space = 0, Color = CMB },
            new BottomBorder { Val = BorderValues.Single, Size = 12, Space = 0, Color = CMB },
            new LeftBorder { Val = BorderValues.None, Size = 0, Space = 0, Color = "auto" },
            new RightBorder { Val = BorderValues.None, Size = 0, Space = 0, Color = "auto" },
            new InsideHorizontalBorder { Val = BorderValues.Single, Size = 4, Space = 0, Color = CLG },
            new InsideVerticalBorder { Val = BorderValues.Single, Size = 4, Space = 0, Color = CLG }
        )));
    var g = new TableGrid();
    int cw = hdrs.Length > 0 ? 9000 / hdrs.Length : 1500;
    foreach (var _ in hdrs) g.Append(new GridColumn { Width = cw.ToString() });
    t.Append(g);

    // header row: medium blue bg, white bold centered text
    var hr = new TableRow();
    foreach (var h in hdrs)
    {
        hr.Append(new TableCell(
            new TableCellProperties(new Shading { Val = ShadingPatternValues.Clear, Color = "auto", Fill = CMB }),
            new Paragraph(
                new ParagraphProperties(new Justification { Val = JustificationValues.Center },
                    new SpacingBetweenLines { After = "0", Line = "276", LineRule = LineSpacingRuleValues.Auto }),
                new Run(new RunProperties(new Bold(), new Color { Val = CWH }),
                    new Text(h) { Space = SpaceProcessingModeValues.Preserve }))));
    }
    t.Append(hr);

    // data rows: zebra striping, first column bold
    for (int i = 0; i < rows.Length; i++)
    {
        var row = new TableRow();
        bool alt = (i % 2 == 1);
        for (int j = 0; j < rows[i].Length; j++)
        {
            var tc = new TableCellProperties();
            if (alt) tc.Append(new Shading { Val = ShadingPatternValues.Clear, Color = "auto", Fill = CAR });
            var cell = new TableCell(tc);
            var para = new Paragraph(new ParagraphProperties(
                new SpacingBetweenLines { After = "0", Line = "276", LineRule = LineSpacingRuleValues.Auto }));
            if (j == 0 && rows[i][j].Length > 0)
                para.Append(new Run(new RunProperties(new Bold(), new Color { Val = CTG }),
                    new Text(rows[i][j]) { Space = SpaceProcessingModeValues.Preserve }));
            else
                para.Append(new Run(new RunProperties(new Color { Val = CTG }),
                    new Text(rows[i][j]) { Space = SpaceProcessingModeValues.Preserve }));
            cell.Append(para);
            row.Append(cell);
        }
        t.Append(row);
    }
    b.Append(t);
    b.Append(new Paragraph());
}

// ====== MAIN: EDIT THIS SECTION ======
string outPath = "/root/.openclaw/workspace/report.docx";
var doc = WordprocessingDocument.Create(outPath, WordprocessingDocumentType.Document);
var mp = doc.AddMainDocumentPart();
mp.Document = new Document(new Body());
var body = mp.Document.Body;

var sp = mp.AddNewPart<StyleDefinitionsPart>();
DefineStyles(sp);
SetMargins(body);

Console.WriteLine("Generating document...");

// === CONTENT START ===
// Replace with actual document content.
// Examples:
//   P(body, "Title", "Document Title");
//   P(body, "Heading1", "Chapter 1");
//   P(body, "Heading2", "Section 1.1");
//   P(body, "Normal", "Body text paragraph.");
//   Table(body, new[] { "Col1", "Col2" }, new[] {
//       new[] { "Row1Col1", "Row1Col2" },
//       new[] { "Row2Col1", "Row2Col2" }
//   });
// === CONTENT END ===

doc.Save();
Console.WriteLine("Done: " + outPath);
