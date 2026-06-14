// Create Yiji Technology Investment Plan Report
#r "nuget: DocumentFormat.OpenXml, 3.2.0"

using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using System.IO;

var outputPath = "/root/.openclaw/workspace-h0assistant/益吉科技入股方案.docx";

// Create document
using var doc = WordprocessingDocument.Create(outputPath, WordprocessingDocumentType.Document);
var mainPart = doc.AddMainDocumentPart();
mainPart.Document = new Document(new Body());

var body = mainPart.Document.Body;

// Title
AddHeading(body, "汉得入股益吉科技方案（简要版）", 1);

// Date
AddParagraph(body, "日期：2026年5月11日");
AddParagraph(body, "");

// Section 1: Background
AddHeading(body, "一、背景", 2);
AddParagraph(body, "益吉科技（HZERO ISV，QMS产品）被塞美特（7亿营收、港股IPO中）拟收购51%。杨总希望汉得入股背书，增强谈判筹码。");
AddParagraph(body, "");

// Section 2: Investment Plan
AddHeading(body, "二、投资方案", 2);
AddSimpleTable(body, new[] {
    ("估值", "1000万（收购前）"),
    ("汉得占股", "3%"),
    ("投入金额", "30万"),
    ("投入方式", "货款抵扣，零现金（益吉应付货款抵扣）"),
    ("分成协议", "ISV协议继续执行，不受影响"),
    ("收购后分成", "延续执行"),
    ("退出机制", "收购失败可退出（待补充条款）")
});
AddParagraph(body, "");

// Section 3: Business Plan
AddHeading(body, "三、益吉商业计划（未来3年）", 2);
AddSimpleTable(body, new[] {
    ("2026年", "营收预估1500万", "塞美特导入客户"),
    ("2027年", "营收预估2000万", "增资1000万扩张"),
    ("2028年", "营收预估3000万+", "市场深耕")
});
AddParagraph(body, "");
AddParagraph(body, "关键举措：");
AddParagraph(body, "• 塞美特收购后增资1000万，支持扩张");
AddParagraph(body, "• 塞美特导入客户资源（半导体、制造业QMS需求）");
AddParagraph(body, "• 继续以HZERO为技术底座，不迁移平台");
AddParagraph(body, "• QMS产品线深耕，补齐研发质量管理模块");
AddParagraph(body, "");

// Section 4: Hand Benefits
AddHeading(body, "四、汉得收益", 2);
AddSimpleTable(body, new[] {
    ("股权背书", "ISV生态合作伙伴身份强化"),
    ("分成继续", "6.5%分成不受影响（营收增长分成增加）"),
    ("增值空间", "收购成功后股权有增值潜力"),
    ("风险可控", "零现金投入，退出机制保障")
});
AddParagraph(body, "");

// Section 5: Pending Clauses
AddHeading(body, "五、待补充条款", 2);
AddParagraph(body, "• 退出机制：收购失败/塞美特不承诺HZERO → 汉得退出股权");
AddParagraph(body, "• 抵扣清单：30万货款具体项目明细");
AddParagraph(body, "• 塞美特承诺：收购后HZERO续用书面确认");
AddParagraph(body, "");

// Section 6: Conclusion
AddHeading(body, "六、结论", 2);
AddParagraph(body, "风险可控，收益保障，建议推进。");

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

void AddSimpleTable(Body body, (string, string)[] rows)
{
    var tbl = new Table();
    
    // Table properties with borders
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
    
    foreach (var row in rows)
    {
        var tr = new TableRow();
        tr.Append(CreateCell(row.Item1, true));  // First column bold (label)
        tr.Append(CreateCell(row.Item2, false)); // Second column normal
        tbl.Append(tr);
    }
    
    body.Append(tbl);
}

void AddSimpleTable(Body body, (string, string, string)[] rows)
{
    var tbl = new Table();
    
    // Table properties with borders
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
    
    foreach (var row in rows)
    {
        var tr = new TableRow();
        tr.Append(CreateCell(row.Item1, true));
        tr.Append(CreateCell(row.Item2, false));
        tr.Append(CreateCell(row.Item3, false));
        tbl.Append(tr);
    }
    
    body.Append(tbl);
}

TableCell CreateCell(string text, bool isBold = false)
{
    var tc = new TableCell();
    var p = new Paragraph();
    var run = new Run();
    
    if (isBold)
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

Console.WriteLine($"Document created: {outputPath}");