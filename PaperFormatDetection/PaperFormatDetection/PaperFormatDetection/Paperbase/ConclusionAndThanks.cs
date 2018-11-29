using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using PaperFormatDetection.Tools;

namespace PaperFormatDetection.Paperbase
{
    class ConclusionAndThanks
    {
        /*protected string titleCharNUm;
        protected string titleJustification;
        protected string titleFont;
        protected string titleFontSize;
        protected string titleLineDis;
        protected string titleBefore;
        protected string titleAfter;

        protected string textIndent;
        protected string textJustification;
        protected string textFont;
        protected string textFontSize;
        protected bool isBold=false;
        protected string textLineDis;
        protected string textBefore;
        protected string textAfter;*/

        /// <summary>
        /// 未避免变量过多，将同一部分变量放在一个数组里面 注意变量在数组中的顺序 
        /// 这里包含结论标题，结论正文， 致谢标题和致谢正文四个数组
        /// 标题9个参数 顺序分别是 0该部分是否必要 1标题之间空格数目 2对齐方式 3行间距 4段前间距 5段后间距 6字体 7字号 8是否加粗
        /// 正文7个参数 顺序分别是 0缩进 1对齐方式 2行间距 3段前间距 4段后间距 5字体 6字号
        /// 若以后需要新增属性一定注意文件中各属性的顺序 因为为了方便 后面初始化赋值就按照这个顺序赋值 这相当于一个约定
        /// </summary>
        protected string[] conTitle=new string[9];
        protected string[] conText = new string[7];
        protected string[] thanksTitle = new string[9];
        protected string[] thanksText = new string[7];

        public ConclusionAndThanks()
        {

        }
        //结论检测
        public void detectConclusion(List<Paragraph> list, WordprocessingDocument doc)
        {
            if (list.Count<=0)
            {
                if (bool.Parse(conTitle[0]))
                    Util.printError("论文缺少结论部分");
            }
            else
            {
                //结论标题
                if (list[0].InnerText.Trim().Length - 2 != int.Parse(conTitle[1]))
                    Util.printError("结论标题“结论”两字之间应有" + conTitle[1] + "个空格");
                if (!Util.correctJustification(list[0], doc, conTitle[2])) 
                    Util.printError("结论标题未" + conTitle[2]);
                if (!Util.correctSpacingBetweenLines_line(list[0], doc, conTitle[3]))
                    Util.printError("结论标题行间距错误，应为" + Util.DSmap[conTitle[3]]);
                if (!Util.correctSpacingBetweenLines_Be(list[0], doc, conTitle[4]))
                    Util.printError("结论标题段前距错误，应为段前" + Util.getLine(conTitle[4])+"行");
                if (!Util.correctSpacingBetweenLines_Af(list[0], doc, conTitle[5]))
                    Util.printError("结论标题段后距错误，应为段后" + Util.getLine(conTitle[5])+"行");
                if (!Util.correctfonts(list[0], doc, conTitle[6], "Times New Roman"))
                    Util.printError("结论标题字体错误，应为" + conTitle[6]);
                if (!Util.correctsize(list[0], doc, conTitle[7])) 
                    Util.printError("结论标题字号错误，应为" + conTitle[7]); 
                if (!Util.correctBold(list[0], doc, bool.Parse(conTitle[8])))
                {
                    if (bool.Parse(conTitle[8]))
                        Util.printError("结论标题未加粗");
                    else
                        Util.printError("结论标题不需加粗");
                }
                //结论正文
                for(int i=1;i<list.Count;i++)
                {
                    string temp = Tool.getFullText(list[i]);
                    if (temp.Trim().Length == 0) continue;
                    temp = "  ----" + (temp.Length > 10 ? temp.Substring(0, 10) : temp) + "......";
                    if (!Util.correctIndentation(list[i], doc, conText[0]))
                        Util.printError("结论缩进错误，应为首行缩进" + conText[0] + "字符" + temp);
                    //if (Util.correctJustification(list[i], doc, conText[1])) Util.printError("结论正文段落对齐方式错误，应为两端对齐" + temp);
                    if (!Util.correctSpacingBetweenLines_line(list[i], doc, conText[2]))
                        Util.printError("结论行间距错误，应为" + Util.DSmap[conText[2]] + temp);
                    if (!Util.correctSpacingBetweenLines_Be(list[i], doc, conText[3]))
                        Util.printError("结论段前距错误，应为段前" + Util.getLine(conText[3])+"行" + temp);
                    if (!Util.correctSpacingBetweenLines_Af(list[i], doc, conText[4]))
                        Util.printError("结论段后距错误，应为段后" + Util.getLine(conText[4])+"行" + temp);
                    if (!Util.correctfonts(list[i], doc, conText[5], "Times New Roman"))
                        Util.printError("结论字体错误，应为" + conText[5] + temp);
                    if (!Util.correctsize(list[i], doc, conText[6])) 
                        Util.printError("结论字号错误，应为" + conText[6] + temp);
                }
            }
        }
        //致谢检测
        public void detectThanks(List<Paragraph> list, WordprocessingDocument doc)
        {
            if (list.Count <= 0)
            {
                if (bool.Parse(thanksTitle[0]))
                    Util.printError("论文缺少致谢部分");
            }
            else
            {
                //致谢标题
                if (list[0].InnerText.Trim().Length - 2 != int.Parse(thanksTitle[1]))
                    Util.printError("致谢标题“致谢”两字之间应有" + thanksTitle[1] + "个空格");
                if (!Util.correctJustification(list[0], doc, thanksTitle[2])) 
                    Util.printError("致谢标题未" + thanksTitle[2]);
                if (!Util.correctSpacingBetweenLines_line(list[0], doc, thanksTitle[3]))
                    Util.printError("致谢标题行间距错误，应为" + Util.DSmap[thanksTitle[3]]);
                if (!Util.correctSpacingBetweenLines_Be(list[0], doc, thanksTitle[4]))
                    Util.printError("致谢标题段前距错误，应为段前" + Util.getLine(thanksTitle[4])+"行");
                if (!Util.correctSpacingBetweenLines_Af(list[0], doc, thanksTitle[5]))
                    Util.printError("致谢标题段后距错误，应为段后" + Util.getLine(thanksTitle[5])+"行");
                if (!Util.correctfonts(list[0], doc, thanksTitle[6], "Times New Roman")) 
                    Util.printError("致谢标题字体错误，应为" + thanksTitle[6]);
                if (!Util.correctsize(list[0], doc, thanksTitle[7])) 
                    Util.printError("致谢标题字号错误，应为" + thanksTitle[7]);
                if (!Util.correctBold(list[0], doc, bool.Parse(thanksTitle[8])))
                {
                    if (bool.Parse(thanksTitle[8]))
                        Util.printError("致谢标题未加粗");
                    else
                        Util.printError("致谢标题不需加粗");
                }
                //致谢正文
                for (int i = 1; i < list.Count; i++)
                {
                    string temp = Util.getFullText(list[i]);
                    if (temp.Trim().Length == 0) continue;
                    temp = "  ----" + (temp.Length > 10 ? temp.Substring(0, 10) : temp) + "......";
                    if (!Util.correctIndentation(list[i], doc, thanksText[0]))
                        Util.printError("致谢缩进错误，应为首行缩进" + thanksText[0] + "字符" + temp);
                    if (!Util.correctSpacingBetweenLines_line(list[i], doc, thanksText[2]))
                        Util.printError("致谢行间距错误，应为" + Util.DSmap[thanksText[2]] + temp);
                    if (!Util.correctSpacingBetweenLines_Be(list[i], doc, thanksText[3]))
                        Util.printError("致谢段前距错误，应为段前" + Util.getLine(thanksText[3])+"行" + temp);
                    if (!Util.correctSpacingBetweenLines_Af(list[i], doc, thanksText[4]))
                        Util.printError("致谢段后距错误，应为段后" + Util.getLine(thanksText[4])+"行" + temp);
                    if (!Util.correctfonts(list[i], doc, thanksText[5], "Times New Roman")) 
                        Util.printError("致谢字体错误，应为" + thanksText[5] + temp);
                    if (!Util.correctsize(list[i], doc, thanksText[6])) 
                        Util.printError("致谢字号错误，应为" + thanksText[6] + temp);
                }
            }
        }
    }
}
