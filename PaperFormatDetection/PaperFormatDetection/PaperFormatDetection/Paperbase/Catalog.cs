using System;
using System.Collections.Generic;
using System.Xml;
using System.Linq;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using PaperFormatDetection.Tools;
using PaperFormatDetection.Frame;
using System.Text.RegularExpressions;

namespace PaperFormatDetection.Paperbase
{
    class Catalog
    {
        /// 目录标题10个参数 顺序分别是 0该部分是否必要 1标题之间空格数目 2对齐方式 3行间距 4段前间距 5段后间距 6中文字体 7英文字体 8字号 9是否加粗
        /// 目录内容7个参数 顺序分别是 0对齐方式 1行间距 2段前间距 3段后间距 4中文字体 5英文字体 6字号
        protected string[] Title = new string[10];
        protected string[] Text = new string[7];
        public Catalog()
        {

        }
        public void detectCatalog(WordprocessingDocument doc)
        {
            Body body = doc.MainDocumentPart.Document.Body;
            MainDocumentPart mainPart = doc.MainDocumentPart;
            IEnumerable<Paragraph> paras = body.Elements<Paragraph>();
            bool isContents = false;
            foreach (Paragraph p in paras)
            {
                Run r = p.GetFirstChild<Run>();
                String fullText = "";
                if (r != null)
                {
                    fullText = Util.getFullText(p).Trim();
                }
                //检测目录标题
                if (fullText.Replace(" ", "") == "目录" && !isContents)
                {
                    isContents = true;
                    if (Util.getFullText(p).Trim().Length - 2 != int.Parse(Title[1]))
                        Util.printError("目录标题“目录”两字之间应有" + Title[1] + "个空格");
                    if (!Util.correctJustification(p, doc, Title[2]))
                        Util.printError("目录标题未" + Title[2]);
                    if (!Util.correctSpacingBetweenLines_line(p, doc, Title[3]))
                        Util.printError("目录标题行间距错误，应为" + Util.DSmap[Title[3]]);
                    if (!Util.correctSpacingBetweenLines_Be(p, doc, Title[4]))
                        Util.printError("目录标题段前距错误，应为段前" + Util.getLine(Title[4]) + "行");
                    if (!Util.correctSpacingBetweenLines_Af(p, doc, Title[5]))
                        Util.printError("目录标题段后距错误，应为段后" + Util.getLine(Title[5]) + "行");
                    if (!Util.correctfonts(p, doc, Title[6], Title[6]))
                        Util.printError("目录标题字体错误，应为" + Title[6]);
                    if (!Util.correctsize(p, doc, Title[8]))
                        Util.printError("目录标题字号错误，应为" + Title[8]);
                    if (!Util.correctBold(p, doc, bool.Parse(Title[9])))
                    {
                        if (bool.Parse(Title[9]))
                            Util.printError("目录标题未加粗");
                        else
                            Util.printError("目录标题不需加粗");
                    }
                    continue;
                }
                if (isContents)
                {
                    //hyperlink
                    Hyperlink h = p.GetFirstChild<Hyperlink>();
                    IEnumerable<Run> runList = p.Elements<Run>();
                    //fieldchar
                    bool flag = false;
                    foreach (Run run in runList)
                    {
                        if (run.GetFirstChild<FieldChar>() != null)
                        {
                            flag = true;
                        }
                    }
                    //判断是更新过的   
                    if (h != null || flag)
                    {
                        String str = Util.getFullText(p).Replace(" ","");
                        if (str.Length > 0)
                        {
                            if (str.Trim().Substring(str.Trim().Length - 1) == "-")
                                Util.printError("目录页码两端不应有\"-\"" + "  ----" + str);
                            if (!Util.correctSpacingBetweenLines_line(p, doc, Text[1]))
                                Util.printError("目录行间距错误，应为" + Util.DSmap[Text[1]] + "  ----" + str);
                            if (!Util.correctSpacingBetweenLines_Be(p, doc, Text[2]))
                                Util.printError("目录段前距错误，应为段前" + Util.getLine(Text[2]) + "行" + "  ----" + str);
                            if (!Util.correctSpacingBetweenLines_Af(p, doc, Text[3]))
                                Util.printError("目录段后距错误，应为段后" + Util.getLine(Text[3]) + "行" + "  ----" + str);
                            if (!Util.correctHyperlinkFonts(p, doc, Text[4], Text[5]))
                                Util.printError("目录字体错误，中文应为" + Text[4] + " 英文和数字应为" + Text[5] + "  ----" + str);
                            if (!correctSize(p, doc, Text[6]))
                                Util.printError("目录字号错误，应为" + Text[6] + "  ----" + str);
                            if (str.Length > 1 && Regex.IsMatch(str.Substring(0, 1), @"[0-9]"))
                            {
                                calatogOrderDetect(p, doc);
                            }
                            else
                            {
                                if(!getIndentation(p))
                                    Util.printError("目录一级标题不需缩进" + "  ----" + str);
                            }
                        }

                        IEnumerable<Run> hyperRun = null;
                        if (h != null)
                            hyperRun = h.Elements<Run>();
                        else
                        {
                            hyperRun = runList;
                        }
                        foreach (Run run in hyperRun)
                        {
                            if (run.GetFirstChild<TabChar>() != null)
                            {
                                RunProperties rPr = run.GetFirstChild<RunProperties>();
                                if (rPr != null)
                                {
                                    if (rPr.GetFirstChild<RunFonts>() != null)
                                    {
                                        //if (rPr.GetFirstChild<RunFonts>().Ascii != "Times New Roman")
                                        //    Util.printError("目录中标题与页码之间的小圆点字体应为Times New Roman" + "  ----" + str);
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        break;
                    }
                }
            }
        }
        List<string> allOrder = new List<string>();
        int INDEX = 0;
        //目录标题序号初始化
        string one_level = "1";
        string two_level = "1.1";
        string three_level = "1.1.1";
        public void calatogOrderDetect(Paragraph p, WordprocessingDocument doc)
        {
            string str = Util.getFullText(p).Trim();
            //包含序号
            string order = "";
            MatchCollection mc = Regex.Matches(str, "^[0-9]+([.][0-9]+)*");
            if (mc.Count > 0)
            {
                foreach (Match m in mc)
                {
                    order = m.ToString();
                    char[] title=str.Substring(order.Length).ToCharArray();
                    if (!((title[0]==' ') && (title[1]==' ') && (title[2]!=' ')))
                    {
                        Util.printError("目录序号与内容之间应有两个空格" + "  ----" + str);
                    }
                    //一级标题
                    if(Util.getSubstrCount(order,".")==0)
                    {
                        if (order != one_level)
                        {
                            Util.printError("目录序号错误，应为" + one_level + "  ----" + str);
                        }
                        two_level = one_level + ".1";
                        one_level = (Convert.ToInt32(one_level) + 1).ToString();
                        if (!getIndentation(p))
                            Util.printError("目录一级标题不需缩进" + "  ----" + str);
                    }
                    //二级标题
                    else if (Util.getSubstrCount(order, ".") == 1)
                    {
                        if (order != two_level)
                        {
                            Util.printError("目录序号错误，应为" + two_level + "  ----" + str);
                        }
                        three_level = two_level + ".1";
                        int i = two_level.IndexOf(".");
                        //二级标题加一
                        two_level = two_level.Substring(0, i + 1) + (Convert.ToInt32(two_level.Substring(i + 1, two_level.Length - i - 1)) + 1).ToString();
                        if (!Util.correctIndentation(p, doc, "2"))
                        {
                            Util.printError("目录二级标题缩进错误，应缩进 2 字符" + "  ----" + str);
                        }
                    }
                    //三级标题
                    else if (Util.getSubstrCount(order, ".") == 2)
                    {
                        if (order != three_level)
                        {
                            Util.printError("目录序号错误，应为" + three_level + "  ----" + str);
                        }
                        int ii = three_level.LastIndexOf(".");
                        //三级标题加一
                        three_level = three_level.Substring(0, ii + 1) + (Convert.ToInt32(three_level.Substring(ii + 1, three_level.Length - ii - 1)) + 1).ToString();

                        if (!Util.correctIndentation(p, doc, "4"))
                        {
                            Util.printError("目录三级标题缩进错误，应缩进 4 字符" + "  ----" + str);
                        }
                    }
                    //超过三级标题
                    else
                    {
                        Util.printError("目录不应超过三级标题" + "  ----" + str);
                    }
                }
            }
            INDEX++;
        }

        public bool correctSize(Paragraph p, WordprocessingDocument doc, string size)
        {
            Dictionary<string, string> FSmap = new Dictionary<string, string>() { { "小六","13" }, { "六号","15" },{"小五", "18" },{ "五号","21" },
                                                                                  {"小四", "24" }, { "四号","28" },{"小三" ,"30"}, { "三号","32" },
                                                                                  {"小二", "36" }, { "二号","44" },{"小一", "48" }, {"一号","52" } };
            size = FSmap[size];
            string each = null;
            int T = 0; int F = 0;
            IEnumerable<Run> runlist = p.Elements<Run>();
            if (p.GetFirstChild<Hyperlink>() != null)
            {
                runlist = p.GetFirstChild<Hyperlink>().Elements<Run>();
            }
            foreach (Run run in runlist)
            {
                if (Regex.IsMatch(run.InnerText, @"^([ ])+$") || run.InnerText.Length == 0)
                    continue;
                each = Util.getFromRunPpr(run, 6); //从Run属性中查找
                if (each == null)
                {
                    if (run.RunProperties != null)
                    {
                        RunStyle rs = run.RunProperties.RunStyle;
                        if (rs != null)
                        {
                            each = Util.getFromStyle(doc, rs.Val, 6);//从Runstyle中查找
                        }
                    }
                }

                if (each == null && p.ParagraphProperties != null)
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
            if (F == 0)
                return true;
            else
                return false;
        }
        public bool getIndentation(Paragraph p)
        {
            ParagraphProperties ppr = p.ParagraphProperties;
            double fc = 0;
            if (ppr != null)
            {
                Indentation ind = ppr.Indentation;
                if (ind != null)
                {
                    if (ind.FirstLineChars != null && ind.FirstLine != null && ind.FirstLineChars == "0")
                        fc += double.Parse(ind.FirstLine);
                    else if (ind.FirstLineChars != null)
                        fc += ind.FirstLineChars;
                    else if (ind.FirstLine != null)
                        fc += double.Parse(ind.FirstLine);
                }
            }
            if (fc == 0) return true;
            else
                return false;
        }
    }
}
