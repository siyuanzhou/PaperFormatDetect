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
    class ChartCatalog
    {/// <summary>
     /// 未避免变量过多，将同一部分变量放在一个数组里面 注意变量在数组中的顺序 
     /// 这里包含目录标题，目录两个数组
     /// 标题9个参数 顺序分别是 0该部分是否必要 1标题之间空格数目 2对齐方式 3行间距 4段前间距 5段后间距 6字体 7字号 8是否加粗
     /// 目录7个参数 顺序分别是 0缩进 1对齐方式 2行间距 3段前间距 4段后间距 5字体 6字号
     /// 若以后需要新增属性一定注意文件中各属性的顺序 因为为了方便 后面初始化赋值就按照这个顺序赋值 这相当于一个约定
     /// </summary>
        protected string[] conTitle = new string[9];
        protected string[] conText = new string[7];
        public ChartCatalog()
        {
        }
        public void detectConclusion(List<Paragraph> list, WordprocessingDocument doc)
        {

            if (list.Count <= 0)
            {
                if (bool.Parse(conTitle[0]))
                    Util.printError("缺少图目录表目录部分");
            }
            else
            {


                //附录正文
                for (int i = 0; i < list.Count; i++)
                {
                    string s1 = "^图目录$";
                    string s2 = "^表目录$";
                    string temp = Tool.getFullText(list[i]);
                    string t = temp;
                    temp = "     " + (temp.Length > 10 ? temp.Substring(0, 10) : temp) + "......";
                    if (Regex.IsMatch(t.Replace(" ", ""), s1) || Regex.IsMatch(t.Replace(" ", ""), s2))
                    {
                        //目录标题
                        //if (list[0].InnerText.Trim().Length - 2 != int.Parse(conTitle[1])) Util.printError("附录标题之间应间隔四个空格");
                        if (!Util.correctJustification(list[i], doc, conTitle[2])) Util.printError("标题对齐方式错误，应为居中");
                        if (!Util.correctSpacingBetweenLines_line(list[i], doc, conTitle[3])) Util.printError("标题行间距错误，应为1.5倍行距");
                        if (!Util.correctSpacingBetweenLines_Be(list[i], doc, conTitle[4])) Util.printError("标题段前间距错误，应为0行");
                        if (!Util.correctSpacingBetweenLines_Af(list[i], doc, conTitle[5])) Util.printError("标题断后间距错误，应为1行");
                        if (!Util.correctfonts(list[i], doc, conTitle[6], "Times New Roman")) Util.printError("标题字体错误，应为黑体");
                        if (!Util.correctsize(list[i], doc, conTitle[7])) Util.printError("标题字号错误，应为小三");
                        if (!Util.correctBold(list[i], doc, bool.Parse(conTitle[8])))
                        {
                            if (bool.Parse(conTitle[8]))
                                Util.printError("标题未加粗");
                            else
                                Util.printError("标题不需加粗");
                        }

                        continue;

                    }

                    //if (!Util.correctIndentation(list[i], doc, conText[0])) Util.printError("目录缩进错误，应为每段落首行缩进2字符" + temp);
                    //if (Util.correctJustification(list[i], doc, conText[1])) Util.printError("结论正文段落对齐方式错误，应为两端对齐" + temp);
                    if (!Util.correctSpacingBetweenLines_line(list[i], doc, conText[2])) Util.printError("目录行距错误，应为多倍行距1.25" + temp);
                    if (!Util.correctSpacingBetweenLines_Be(list[i], doc, conText[3])) Util.printError("目录段前间距错误，应为段前0行" + temp);
                    if (!Util.correctSpacingBetweenLines_Af(list[i], doc, conText[4])) Util.printError("目录段后间距错误，应为段后0行" + temp);
                    if (!Util.correctHyperlinkFonts(list[i], doc, conText[5], "Times New Roman")) Util.printError("目录字体错误，中文应为"+conText[5]+ ",英文应为 Times New Roman" + temp);
                    if (!correctCatlogsize(list[i], doc, conText[6])) Util.printError("目录字号错误，应为小四" + temp);
                }
            }
            Console.WriteLine("---------------------------------");
        }
        public static bool correctCatlogsize(Paragraph p, WordprocessingDocument doc, string size)
        {
            Dictionary<string, string> FSmap = new Dictionary<string, string>() { { "小六","13" }, { "六号","15" },{"小五", "18" },{ "五号","21" },
                                                                                  {"小四", "24" }, { "四号","28" },{"小三" ,"30"}, { "三号","32" },
                                                                                  {"小二", "36" }, { "二号","44" },{"小一", "48" }, {"一号","52" } };
            string tempSize = size;
            size = FSmap[size];

            string each = null;
            int T = 0; int F = 0;
            IEnumerable<Run> runlist = p.Elements<Run>();
            //runlist=p.GetFirstChild<Hyperlink>().Elements<Run>();
            if (p.GetFirstChild<Hyperlink>() != null)
            {
                runlist = p.GetFirstChild<Hyperlink>().Elements<Run>();

            }
            else
            {
                runlist = p.Elements<Run>();

            }
            foreach (Run run in runlist)
            {
                each = Util.getFromRunPpr(run, 6); //从Run属性中查找
                
                if (each == null)
                {
                    
                    ParagraphStyleId style_id = p.ParagraphProperties.ParagraphStyleId;
                    if (style_id != null)//从paragraphstyle中获取
                    {
                        each = Util.getFromStyle(doc, style_id.Val, 6);
                    }
                    if (each == null)//style没找到
                    {
                        each = Util.getFromDefault(doc, 6);//从段落默认style中找
                        if (each == null)//default没找到
                        {
                            each = "0";
                        }
                    }
                }
                if (each == size) T++;
                else F++;
            }
            int num = runlist.Count();
            if (T == num)
                return true;
            else
                return false;
        }

        public static List<Paragraph> sectionLoction(WordprocessingDocument doc, string section, int paperType)
        {
            string[] Undergraduate = new string[] { "摘要", "Abstract", "目录", "引言", "正文", "结论", "参考文献", "附录", "致谢" };
            string[] Master = new string[] { "大连理工大学学位论文独创性声明", "摘要", "Abstract", "目录", "引言", "正文", "结论",
                                             "参考文献", "附录", "攻读硕士学位期间发表学术论文情况", "致谢", "大连理工大学学位论文版权使用授权书" };
            string[] Doctor = new string[] { "大连理工大学学位论文独创性声明", "大连理工大学学位论文版权使用授权书", "摘要", "ABSTRACT", "目录", "TABLE OF CONTENTS",
                                             "图目录","表目录","主要符号表","正文", "参考文献", "附录", "攻读博士学位期间科研项目及科研成果", "致谢", "作者简介" };
            string[][] type = new string[][] { Undergraduate, Master, Doctor };
            string s1 = "^图目录$";
            string s2 = "^表目录$";

            int index = Array.IndexOf(type[paperType], section);
            Body body = doc.MainDocumentPart.Document.Body;
            IEnumerable<Paragraph> paras = body.Elements<Paragraph>();
            List<Paragraph> list = new List<Paragraph>();

            Boolean begin = false;
            Boolean end = false;
            foreach (Paragraph p in paras)
            {
                String fullText = Util.getFullText(p);
                if (fullText.Replace(" ", "").Length == 0)
                {
                    continue;
                }
                if (Regex.IsMatch(fullText.Replace(" ", ""), s1))
                {

                    begin = true;

                }
                if (Regex.IsMatch(fullText.Replace(" ", ""), s2))
                {

                    begin = true;

                }
                for (int i = index + 2; i < type[paperType].Length; i++)
                {
                    if (fullText.Replace(" ", "").Length < 40 && fullText.Replace(" ", "").Equals(type[paperType][i]))
                    {
                        begin = false; end = true; break;
                    }
                }
                if (begin)
                {
                    list.Add(p);
                }
                if (end)
                {
                    break;
                }
            }
            return list;
        }

    }
}
