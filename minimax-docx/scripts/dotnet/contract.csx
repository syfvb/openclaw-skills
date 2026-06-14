#r "nuget: DocumentFormat.OpenXml, 3.2.0"

using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using System.IO;

var outputPath = "/root/.openclaw/workspace-h0assistant/房屋租赁合同.docx";

if (File.Exists(outputPath)) File.Delete(outputPath);

var doc = WordprocessingDocument.Create(outputPath, WordprocessingDocumentType.Document);
var mainPart = doc.AddMainDocumentPart();
mainPart.Document = new Document(new Body());

var body = mainPart.Document.Body;

Action<string, bool, bool> AddPara = (text, bold, center) => {
    var para = new Paragraph();
    if (center) para.ParagraphProperties = new ParagraphProperties(new Justification { Val = JustificationValues.Center });
    var run = new Run();
    if (bold) run.RunProperties = new RunProperties(new Bold(), new FontSize { Val = "24" });
    else run.RunProperties = new RunProperties(new FontSize { Val = "21" });
    run.Append(new Text(text));
    para.Append(run);
    body.Append(para);
};

AddPara("房屋租赁合同", true, true);
AddPara("", false, false);
AddPara("出租方（甲方）：商云方", true, false);
AddPara("承租方（乙方）：陈鸣飞", true, false);
AddPara("", false, false);
AddPara("根据《中国人民共和国合同法》及有关法律、法规的规定，甲方与乙方在平等、自愿的基础上，就房屋租赁的有关事宜达成协议如下：", false, false);
AddPara("", false, false);

AddPara("第一条：租赁房屋坐落位置：常州市新北区高新广场2号楼1409室，建筑面积 95.43 平方。", false, false);
AddPara("第二条：租期 1 年，租赁期限从 2025 年 5 月 16 日至 2026 年 5 月 15 日。", false, false);
AddPara("第三条：承租期内双方商定付款方式为：付 6 月，押 1 个月，每月租金 3000 元，年租金 36000 元。", false, false);
AddPara("第四条：房租金的支付方式：以现金或银行转账方式支付。", false, false);
AddPara("第五条：甲方需将之前该房的物业费、电费、水费结清，并完好交付给乙方。承租期内的水费、电费、物业管理费均由乙方自行承担。", false, false);
AddPara("第六条：乙方如需对承租房装修，必须事先征得甲方同意，装修时不得破坏房屋的主体结构，要服从物业的管理，遵守国家的相关规定，乙方承担房屋装修中的一切意外及风险。", false, false);
AddPara("第七条：风险承担：本协议约定的承租期内，承租房屋内的全部风险包括人身伤害、财务毁损抢盗等一切风险均由乙方自行承担责任，甲方不承担任何意外及风险。", false, false);
AddPara("第八条：合同解除条款：有以下情形之一，甲方有权立即解除本合同，直接收回出租房屋。", false, false);
AddPara("    1. 乙方不按时支付房租金达15日以上。", false, false);
AddPara("    2. 乙方所欠各项费用达 1000 元人民币以上；", false, false);
AddPara("    3. 乙方在承租房内进行违法活动及放置国家明令禁止的危险品之类；", false, false);
AddPara("    4. 在承租期内乙方不得私自转租给第三方；", false, false);
AddPara("如有上述情形出现，甲方可立即收回出租房屋，乙方已支付房租金不予退还。", false, false);

AddPara("第九条：违约责任：", false, false);
AddPara("    1. 本协议条款甲乙双方应严格遵守，一方违约应赔偿守约方的直接经济损失及一个月房租的违约金。", false, false);
AddPara("    2. 乙方逾期支付租金的，除应及时如数补交外，还应另支付滞纳金（滞纳金为年租金的1%/日），逾期不得超过20日，否则甲方有权收回出租房屋。", false, false);

AddPara("第十条：合同争议解决方式：本合同未尽事宜及在履行过程中发生争议，由双方当事人协商解决，协商或调解不成，直接提交房屋所在地法院诉讼解决。", false, false);

AddPara("第十一条：其他约定", false, false);
AddPara("    1. 甲方在乙方承租期间如需出售房屋，需提交1个月通知乙方，乙方在同等条件下享有优先购买权。同时甲方应遵守买卖不破租赁的规定。", false, false);
AddPara("    2. 乙方在承租期间，由于甲方的原因（包括抵押）导致房屋被提前收回，应赔偿乙方实际经济损失及装修费用（装修费用具体金额双方同意由常州市有资质的机构对乙方的装修进行审计及评估，或双方协商解决）。", false, false);
AddPara("    3. 承租期满后，乙方如需续租，需提前一个月通知甲方，甲方在租房市场的同等价格条件下，乙方享有优先承租权。不续租的，也需要提前一个月通知甲方。", false, false);
AddPara("    4. 乙方在租赁期满后应结清物业费、电费、水费等各项费用，并完好移交给甲方，并确保室内空调及办公用设施完好交付给甲方，甲方应全额无息退换房屋押金。", false, false);
AddPara("    5. 甲方应提供产权证复印件及个人身份证复印件。", false, false);
AddPara("    6. 室内附属设置清单：微信群里拍照证明。", false, false);
AddPara("    7. 附加条款（一）：下半年房租金2025年10月16日付。", false, false);

AddPara("第十二条：本协议一式两份。甲乙各执一份。签字、盖章后具有同等效力。", false, false);
AddPara("", false, false);
AddPara("", false, false);

AddPara("出租方（甲方）：商云方", true, false);
AddPara("电话：13651855989", false, false);
AddPara("", false, false);
AddPara("承租方（乙方）：陈鸣飞", true, false);
AddPara("电话：158961379588", false, false);
AddPara("", false, false);
AddPara("签约日期：2025 年 5 月 14 日", true, false);

doc.Dispose();

Console.WriteLine("✅ 已生成: " + outputPath);