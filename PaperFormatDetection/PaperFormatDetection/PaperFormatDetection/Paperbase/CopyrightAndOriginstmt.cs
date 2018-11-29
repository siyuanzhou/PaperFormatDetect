using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using PaperFormatDetection.Tools;
using System.Xml;
using System.Text.RegularExpressions;

namespace PaperFormatDetection.Paperbase
{
    class CopyrightAndOriginstmt
    {
        /// <summary>
        /// 未避免变量过多，将同一部分变量放在一个数组里面 注意变量在数组中的顺序 
        /// 这里包含版权标题，版权正文， 独创性标题和独创性正文四个数组
        /// 标题9个参数 顺序分别是 0该部分是否必要 1标题之间空格数目 2对齐方式 3行间距 4段前间距 5段后间距 6字体 7字号 8是否加粗
        /// 正文7个参数 顺序分别是 0缩进 1对齐方式 2行间距 3段前间距 4段后间距 5字体 6字号 7签名部分的缩进 8签名部分的行间距
        /// 若以后需要新增属性一定注意文件中各属性的顺序 因为为了方便 后面初始化赋值就按照这个顺序赋值 这相当于一个约定
        /// </summary>
        protected string[] copyrightTitle=new string[9];
        protected string[] copyrightText = new string[9];
        protected string[] originTitle = new string[9];
        protected string[] originText = new string[9];

        public CopyrightAndOriginstmt()
        {

        }
        //版权说明书检测
        public void detectCopyright(List<Paragraph> list, WordprocessingDocument doc)
        {
            if(list.Count<=0)
            {
                if (bool.Parse(copyrightTitle[0]))
                    Util.printError("论文缺少版权使用授权书部分");
            }
            else
            {
                //版权说明书标题
                if (Util.getFullText(list[0]).Trim().Length - 17 != int.Parse(copyrightTitle[1]))
                    Util.printError("版权使用授权书标题之间不应包含空格");
                if (!Util.correctJustification(list[0], doc, copyrightTitle[2]))
                    Util.printError("版权使用授权书标题未" + copyrightTitle[2]);
                if (!Util.correctSpacingBetweenLines_line(list[0], doc, copyrightTitle[3]))
                    Util.printError("版权使用授权书标题行间距错误，应为" + Util.DSmap[copyrightTitle[3]]);
                if (!Util.correctSpacingBetweenLines_Be(list[0], doc, copyrightTitle[4]))
                    Util.printError("版权使用授权书标题段前距错误，应为段前" + Util.getLine(copyrightTitle[4]) + "行");
                if (!Util.correctSpacingBetweenLines_Af(list[0], doc, copyrightTitle[5]))
                    Util.printError("版权使用授权书标题段后距错误，应为段后" + Util.getLine(copyrightTitle[5]) + "行");
                if (!Util.correctfonts(list[0], doc, copyrightTitle[6], "Times New Roman")) 
                    Util.printError("版权使用授权书标题字体错误，应为" + copyrightTitle[6]);
                if (!Util.correctsize(list[0], doc, copyrightTitle[7])) 
                    Util.printError("版权使用授权书标题字号错误，应为" + copyrightTitle[7]);
                if (!Util.correctBold(list[0], doc, bool.Parse(copyrightTitle[8])))
                {
                    if (bool.Parse(copyrightTitle[8]))
                        Util.printError("版权使用授权书标题未加粗");
                    else
                        Util.printError("版权使用授权书标题不需加粗");
                }
                //版权说明书正文
                bool isSignal=false;
                for(int i=1;i<list.Count;i++)
                {
                    string temp = Tool.getFullText(list[i]);
                    if (temp.Trim().Length == 0) continue;
                    if (temp.IndexOf("学位论文题目") >= 0)
                    {
                        isSignal = true;
                        if (temp.Substring(7).Trim().Length > 0 && temp.Substring(7).Trim() != Util.getPaperName(doc).Trim())
                            Util.printError("学位论文题目与封面论文标题不一致  ----" + temp.Substring(7).Trim());
                    } 
                    temp = "  ----" + (temp.Length > 10 ? temp.Substring(0, 10) : temp) + "......";
                    //if (!Util.correctJustification(list[i], doc, copyrightText[1])) 
                        //Util.printError("版权说明书正文段落对齐方式错误，应为" + copyrightText[1] + temp);
                    if (!Util.correctSpacingBetweenLines_Be(list[i], doc, copyrightText[3]))
                        Util.printError("版权使用授权书段前距错误，应为段前" + Util.getLine(copyrightText[3]) + "行"+temp);
                    if (!Util.correctSpacingBetweenLines_Af(list[i], doc, copyrightText[4]))
                        Util.printError("版权使用授权书段后距错误，应为段后" + Util.getLine(copyrightText[4]) + "行"+temp);
                    if (!Util.correctfonts(list[i], doc, copyrightText[5], "Times New Roman"))
                        Util.printError("版权使用授权书字体错误，应为" + copyrightText[5] + temp);
                    if (!Util.correctsize(list[i], doc, copyrightText[6]))
                        Util.printError("版权使用授权书字号错误，应为" + copyrightText[6] + temp);
                    if(!isSignal)
                    {
                        if (!Util.correctIndentation(list[i], doc, copyrightText[0]))
                            Util.printError("版权使用授权书缩进错误，应为首行缩进" + copyrightText[0] + "字符" + temp);
                        if (!Util.correctSpacingBetweenLines_line(list[i], doc, copyrightText[2]))
                            Util.printError("版权使用授权书行间距错误，应为" + Util.DSmap[copyrightText[2]] + temp);
                    }
                    else
                    {
                        if (!Util.correctIndentation(list[i], doc, copyrightText[7]))
                            Util.printError("版权使用授权书缩进错误，应为首行缩进" + copyrightText[7] + "字符" + temp);
                        if (!Util.correctSpacingBetweenLines_line(list[i], doc, copyrightText[8]))
                            Util.printError("版权使用授权书行间距错误，应为" + Util.DSmap[copyrightText[8]] + temp);
                    }
                }
            }
        }
        //独创性声明检测
        public void detectOriginstmt(List<Paragraph> list, WordprocessingDocument doc)
        {
            if (list.Count <= 0)
            {
                if (bool.Parse(originTitle[0]))
                    Util.printError("论文缺少学位论文独创性声明");
            }
            else
            {
                Paragraph prePar = Util.getPreParagraph(doc, list[0]);
                if (prePar!=null && Util.getFullText(prePar).Trim().Length > 0)
                {
                    Util.printError("论文独创性声明标题之前应空一行");
                }
                //独创性声明标题
                if (Util.getFullText(list[0]).Trim().Length - 15 != int.Parse(originTitle[1])) 
                    Util.printError("论文独创性声明标题之间不应包含空格");
                if (!Util.correctJustification(list[0], doc, originTitle[2])) 
                    Util.printError("论文独创性声明标题未" + originTitle[2]);
                if (!Util.correctSpacingBetweenLines_line(list[0], doc, originTitle[3]))
                    Util.printError("论文独创性声明标题行间距错误，应为" + Util.DSmap[originTitle[3]]);
                if (!Util.correctSpacingBetweenLines_Be(list[0], doc, originTitle[4]))
                    Util.printError("论文独创性声明标题段前距错误，应为段前" + Util.getLine(originTitle[4])+"行");
                if (!Util.correctSpacingBetweenLines_Af(list[0], doc, originTitle[5]))
                    Util.printError("论文独创性声明标题段后距错误，应为段后" + Util.getLine(originTitle[5])+"行");
                if (!Util.correctfonts(list[0], doc, originTitle[6], "Times New Roman")) 
                    Util.printError("论文独创性声明标题字体错误，应为" + originTitle[6]);
                if (!Util.correctsize(list[0], doc, originTitle[7])) 
                    Util.printError("论文独创性声明标题字号错误，应为" + originTitle[7]);
                if (!Util.correctBold(list[0], doc, bool.Parse(copyrightTitle[8])))
                {
                    if (bool.Parse(originTitle[8]))
                        Util.printError("论文独创性声明标题未加粗");
                    else
                        Util.printError("论文独创性声明标题不需加粗");
                }
                //独创性声明正文
                bool isSignal = false;
                for (int i = 1; i < list.Count; i++)
                {
                    string temp = Tool.getFullText(list[i]);
                    //段后空一行
                    if (i == 1 && temp.Trim().Length>0)
                    {
                        Util.printError("论文独创性声明标题之后应空一行");
                    }
                    if (temp.Trim().Length == 0) continue;
                    if (temp.IndexOf("学位论文题目") >= 0)
                    {
                        isSignal = true;
                        if (temp.Substring(7).Trim().Length>0 && temp.Substring(7).Trim() != Util.getPaperName(doc).Trim())
                            Util.printError("学位论文题目与封面论文标题不一致  ----" + temp.Substring(7).Trim());
                    } 
                    temp = "     " + (temp.Length > 10 ? temp.Substring(0, 10) : temp) + "......";
                    if (temp.Contains("作者郑重声明"))
                    {
                        if (!Util.correctSpacingBetweenLines_Be(list[i], doc, "240"))
                            Util.printError("论文独创性声明段前距错误，应为段前1行" + temp);
                    }
                    if (temp.Contains("若有不实之处"))
                    {
                        if (!Util.correctSpacingBetweenLines_Be(list[i], doc, "0"))
                            Util.printError("论文独创性声明段前距错误，应为段前0行" + temp);
                    }
                    if (temp.Contains("学位论文题目") || temp.Replace(" ", "").Contains("作者签名"))
                    {
                        if (!Util.correctSpacingBetweenLines_Be(list[i], doc, "0"))
                            Util.printError("论文独创性声明段前距错误，应为段前0行" + temp);
                        if (!Util.correctSpacingBetweenLines_Af(list[i], doc, "0"))
                            Util.printError("论文独创性声明段后距错误，应为段后0行" + temp);
                    }
                    if (!Util.correctfonts(list[i], doc, originText[5], "Times New Roman")) 
                        Util.printError("论文独创性声明字体错误，应为" + originText[5] + temp);
                    if (!Util.correctsize(list[i], doc, originText[6])) 
                        Util.printError("论文独创性声明字号错误，应为" + originText[6] + temp);
                    if (!isSignal)
                    {
                        if (!Util.correctIndentation(list[i], doc, originText[0]))
                            Util.printError("论文独创性声明缩进错误，应为首行缩进" + originText[0] + "字符" + temp);
                        if (!Util.correctSpacingBetweenLines_line(list[i], doc, originText[2]))
                            Util.printError("论文独创性声明行间距错误，应为" + Util.DSmap[originText[2]] + temp);
                    }
                    else
                    {
                        if (!Util.correctIndentation(list[i], doc, originText[7]))
                            Util.printError("论文独创性声明缩进错误，应为首行缩进" + originText[7] + "字符" + temp);
                        if (!Util.correctSpacingBetweenLines_line(list[i], doc, originText[8]))
                            Util.printError("论文独创性声明行间距错误，应为" + Util.DSmap[originText[8]] + temp);
                    }
                }
            }
        }
        public void Init(string path)
        {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(path);
            int m = 0;
            //结论标题
            XmlNodeList conTitleNode = xmlDoc.SelectSingleNode("Root").SelectSingleNode("Copyright").SelectSingleNode("Title").ChildNodes;
            m = 0;
            foreach (XmlNode node in conTitleNode)
            {
                this.copyrightTitle[m] = node.InnerText; m++;
            }
            //结论正文
            XmlNodeList conTextNode = xmlDoc.SelectSingleNode("Root").SelectSingleNode("Copyright").SelectSingleNode("Text").ChildNodes;
            m = 0;
            foreach (XmlNode node in conTextNode)
            {
                this.copyrightText[m] = node.InnerText; m++;
            }
            //结论标题
            XmlNodeList originTitleeNode = xmlDoc.SelectSingleNode("Root").SelectSingleNode("Originstmt").SelectSingleNode("Title").ChildNodes;
            m = 0;
            foreach (XmlNode node in originTitleeNode)
            {
                this.originTitle[m] = node.InnerText; m++;
            }
            //结论正文
            XmlNodeList originTextNode = xmlDoc.SelectSingleNode("Root").SelectSingleNode("Originstmt").SelectSingleNode("Text").ChildNodes;
            m = 0;
            foreach (XmlNode node in originTextNode)
            {
                this.originText[m] = node.InnerText; m++;
            }
        }
    }
}
