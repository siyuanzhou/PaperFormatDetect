using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using System.IO;
using System.Text;
using DocumentFormat.OpenXml;

namespace PaperFormatDetection.Tools
{
    class Util
    {
        public static List<string> errorLists = new List<string>();

        public static string environmentDir = "D:\\PaperFormatDetection\\PaperFormatDetection\\PaperFormatDetection\\PaperFormatDetection\\bin\\Debug";
        public static string paperPath = null;
        public static FileStream fs = null;
        public static StreamWriter errorReport = null;
        public static Dictionary<string, string> pageDic = new Dictionary<string, string>();
        public static string masterType = "专业学位硕士";// "学术型硕士";//专业学位硕士
        public static Dictionary<string, string> DSmap = new Dictionary<string, string>() { { "0", "0倍行距" },{ "120", "0.5倍行距" }, { "240", "1倍行距" },{ "300", "1.25倍行距" }, { "360", "1.5倍行距" },
                                                                                            { "480", "二倍行距" }, { "720", "三倍行距" }, { "220", "11磅" }, { "500", "25磅" } };
        public static int firstCharSize = 24;
        //错误输出,为调试方便，直接在命令行输出，后期可以将错误提示输出到文件中，只用修改此方法即可
        public static void printError(string str)
        {
            //errorReport.WriteLine(str);
            errorLists.Add(str);
            Console.WriteLine(str);
        }
        public static bool IsInt(string value)
        {
            return Regex.IsMatch(value, @"^[+-]?\d*$");
        }
        public static int getSubstrCount(string str1, string str2)
        {
            int count = 0;
            int index = str1.IndexOf(str2);
            while (index > 0)
            {
                count++;
                str1 = str1.Substring(index + 1, str1.Length - index - 1);
                index = str1.IndexOf(str2);
            }
            return count;
        }
        public static String getFullRunText(Paragraph p)
        {
            string text = "";
            IEnumerable<Run> allRun = p.Elements<Run>();
            foreach (Run r in allRun)
            {
                Text pText = r.GetFirstChild<Text>();
                if (pText != null)
                {
                    text += pText.Text;
                }
            }
            return text;
        }
        public static String getFullText(Paragraph p)
        {
            var list = p.ChildElements;
            Run temp_r = new Run();
            SmartTagRun temp_sr = new SmartTagRun();
            InsertedRun temp_ir = new InsertedRun();
            Hyperlink temp_ih = new Hyperlink();
            String text = "";
            foreach (var t in list)
            {
                if (t.GetType() == temp_r.GetType())
                {
                    Text pText = t.GetFirstChild<Text>();
                    if (pText != null)
                    {
                        text += pText.Text;
                    }
                }
                else if (t.GetType() == temp_sr.GetType())
                {
                    IEnumerable<Run> rr = t.Elements<Run>();
                    if (rr != null)
                    {
                        foreach (Run tr in rr)
                        {
                            Text pText = tr.GetFirstChild<Text>();
                            if (pText != null)
                            {
                                text += pText.Text;
                            }
                        }
                    }
                }
                else if (t.GetType() == temp_ir.GetType())
                {
                    Run r = t.GetFirstChild<Run>();
                    if (r != null)
                    {
                        Text pText = r.GetFirstChild<Text>();
                        if (pText != null)
                        {
                            text += pText.Text;
                        }

                    }
                }
                else if (t.GetType() == temp_ih.GetType())
                {
                    IEnumerable<Run> rr = t.Elements<Run>();
                    if (rr != null)
                    {
                        foreach (Run tr in rr)
                        {
                            Text pText = tr.GetFirstChild<Text>();
                            if (pText != null)
                            {
                                text += pText.Text;
                            }
                        }
                    }
                }
            }
            return text;
        }
        public static Paragraph getPreParagraph(WordprocessingDocument doc,Paragraph curPar)
        {
            Body body = doc.MainDocumentPart.Document.Body;
            IEnumerable<Paragraph> paras = body.Elements<Paragraph>();
            Paragraph prePar=null;
            foreach (Paragraph p in paras)
            {
                if (p.Equals(curPar))
                    break;
                else 
                    prePar = p;
            }
            return prePar;
        }
        public static string getPaperName(WordprocessingDocument doc)
        {
            string paperName = "";
            Body body = doc.MainDocumentPart.Document.Body;
            IEnumerable<Paragraph> paras = body.Elements<Paragraph>();
            foreach (Paragraph p in paras)
            {
                string text = Util.getFullRunText(p).Trim();
                if (text.Length == 0 || text == "硕士学位论文" || text == "专业学位硕士学位论文")
                    continue; ;
                if (Regex.IsMatch(text, @"^[^\u4e00-\u9fa5]+$"))
                    break;
                else
                    paperName = text;
            }
            return paperName;
        }
        /// <summary>
        /// 定位到某个部分 section为名称 如下面数组所示 1代表正文 传入时为“正文” paperType为int 0代表本科 1代表硕士 2代表博士
        /// 正文 附录以及其上部分的定位为特殊情况 暂时未写
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="section"></param>
        /// <param name="paperType"></param>
        /// <returns></returns>
        public static List<Paragraph> sectionLoction(WordprocessingDocument doc, string section, int paperType)
        {
            string[] Undergraduate = new string[] { "摘要", "Abstract", "目录", "引言", "正文", "结论", "参考文献", "附录", "致谢" };
            string[] Master = new string[] { "大连理工大学学位论文独创性声明", "摘要", "Abstract", "目录", "引言", "正文", "结论",
                                             "参考文献", "附录", "攻读硕士学位期间发表学术论文情况", "致谢", "大连理工大学学位论文版权使用授权书" };
            string[] Doctor = new string[] { "大连理工大学学位论文独创性声明", "大连理工大学学位论文版权使用授权书", "摘要", "ABSTRACT", "目录", "TABLE OF CONTENTS",
                                             "图目录","表目录","主要符号表","正文", "参考文献", "附录", "攻读博士学位期间科研项目及科研成果", "致谢", "作者简介" };
            string[][] type = new string[][] { Undergraduate, Master, Doctor };
            int index = Array.IndexOf(type[paperType], section);
            Body body = doc.MainDocumentPart.Document.Body;
            IEnumerable<Paragraph> paras = body.Elements<Paragraph>();
            List<Paragraph> list = new List<Paragraph>();
            Boolean begin = false;
            Boolean end = false;
            foreach (Paragraph p in paras)
            {
                String fullText = getFullRunText(p);
                if (fullText.Replace(" ", "").Equals(section))
                {
                    begin = true;
                }
                for (int i = 0; i < type[paperType].Length; i++)
                {
                    if (i == index) continue;
                    if (begin && fullText.Replace(" ", "").Length < 40 && fullText.Replace(" ", "").Equals(type[paperType][i]))
                    {
                        if (i < index)
                            printError(section+" 在论文中的位置不正确");
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
        //参数 indentation代表缩进字符数 如2代表2个字符
        public static bool correctIndentation(Paragraph p, WordprocessingDocument doc, string indentation)
        {
            //得到这段第一个字符的字号
            firstCharSize = int.Parse(getFirstcharSize(p, doc));
            ////////////////////////
            indentation = (int.Parse(indentation) * 100).ToString();
            string ind = getFromPpr(p, 0);//从段落属性中获取
            if (ind == null && p.ParagraphProperties != null)//段落属性中没找到
            {
                ParagraphStyleId style_id = p.ParagraphProperties.ParagraphStyleId;
                if (style_id != null)//从style中获取
                {
                    ind = getFromStyle(doc, style_id.Val, 0);
                }
            }
            if (ind == null)//style没找到
            {
                ind = getFromDefault(doc, 0);
            }
            if (ind == null)//default没找到
            {
                ind = "0";
            }
            if (Math.Abs(double.Parse(ind) - double.Parse(indentation)) <= 10)
                return true;
            else
                return false;
        }
        //参数justification代表对齐方式 如居中，居左，两端对齐 在函数里面转为center,left等
        public static bool correctJustification(Paragraph p, WordprocessingDocument doc, string justification)
        {
            Dictionary<string, string> JCmap = new Dictionary<string, string>() { { "居中", "center" }, { "居左", "left" }, { "居右", "right" }, { "两端对齐", "both" } };
            justification = JCmap[justification];
            string jc = getFromPpr(p, 1);//从段落属性中获取
            if (jc == null && p.ParagraphProperties != null)//段落属性中没找到
            {
                ParagraphStyleId style_id = p.ParagraphProperties.ParagraphStyleId;
                if (style_id != null)//从style中获取
                {
                    jc = getFromStyle(doc, style_id.Val, 1);
                }
            }
            if (jc == null)//style没找到
            {
                jc = getFromDefault(doc, 1);
            }
            if (jc == null)//default没找到
            {
                jc = "both";
            }
            if (jc == justification)
                return true;
            else
                return false;
        }
        //before 代表段前间距，240是一行 以此类推  如段前1.5行 则传入360
        public static bool correctSpacingBetweenLines_Be(Paragraph p, WordprocessingDocument doc, string before)
        {
            string be = getFromPpr(p, 2);//从段落属性中获取
            if (be == null && p.ParagraphProperties != null)//段落属性中没找到
            {
                ParagraphStyleId style_id = p.ParagraphProperties.ParagraphStyleId;
                if (style_id != null)//从style中获取
                {
                    be = getFromStyle(doc, style_id.Val, 2);
                }
            }
            if (be == null)//style没找到
            {
                be = getFromDefault(doc, 2);//default没找到
            }
            if (be == null)
            {
                be = "0";
            }
            if (be == before)
                return true;
            else
                return false;
        }
        //after 代表段后间距，240是一行 以此类推  如段后1.5行 则传入360
        public static bool correctSpacingBetweenLines_Af(Paragraph p, WordprocessingDocument doc, string after)
        {
            string af = getFromPpr(p, 3);//从段落属性中获取
            if (af == null && p.ParagraphProperties != null)//段落属性中没找到
            {
                ParagraphStyleId style_id = p.ParagraphProperties.ParagraphStyleId;
                if (style_id != null)//从style中获取
                {
                    af = getFromStyle(doc, style_id.Val, 3);
                }
            }
            if (af == null)//style没找到
            {
                af = getFromDefault(doc, 3);
            }
            if (af == null)//default没找到
            {
                af = "0";
            }
            //if (af == after)
            if (Math.Abs(double.Parse(af) - double.Parse(after))<=20)
                return true;
            else
                return false;
        }
        //line 代表行间距，240是一倍行距 以此类推  如1.25倍行距 则传入300
        public static bool correctSpacingBetweenLines_line(Paragraph p, WordprocessingDocument doc, string line)
        {
            string af = getFromPpr(p, 4);//从段落属性中获取
            if (af == null && p.ParagraphProperties != null)//段落属性中没找到
            {
                ParagraphStyleId style_id = p.ParagraphProperties.ParagraphStyleId;
                if (style_id != null)//从style中获取
                {
                    af = getFromStyle(doc, style_id.Val, 4);
                }
            }
            if (af == null)//style没找到
            {
                af = getFromDefault(doc, 4);
            }
            if (af == null)//default没找到
            {
                af = "300";
            }
            if (af == line)
                return true;
            else
                return false;
        }
        //isBold代表是否加粗，注意整段加粗
        public static bool correctBold(Paragraph p, WordprocessingDocument doc, bool isBold)
        {
            string b = null;
            string each = null;
            int T = 0; int F = 0;
            IEnumerable<Run> runlist = p.Elements<Run>();
            foreach (Run run in runlist)
            {
                if (Regex.IsMatch(run.InnerText, @"^([ ])+$") || run.InnerText.Length==0)
                    continue;
                each = getFromRunPpr(run, 5); //从Run属性中查找
                if (each == null)
                {
                    if (run.RunProperties != null)
                    {
                        RunStyle rs = run.RunProperties.RunStyle;
                        if (rs != null)
                        {
                            each = getFromStyle(doc, rs.Val, 5);//从Runstyle中查找
                        }
                    }
                }
                if (each == null)
                {
                    //each = getFromPpr(p, 5);//从段落属性中找
                    if (each == null)
                    {
                        ParagraphStyleId style_id = p.ParagraphProperties.ParagraphStyleId;
                        if (style_id != null)//从paragraphstyle中获取
                        {
                            each = getFromStyle(doc, style_id.Val, 5);
                        }
                    }
                }
                if (each == null)//style没找到
                {
                    each = getFromDefault(doc, 5);//从段落默认style中找
                }
                if (each == null)//default没找到
                {
                    each = "false";
                }
                if (each == "true") T++;
                else F++;
            }
            int num = runlist.Count();
            if (F==0) b = "true";
            else b = "false";
            if (Boolean.Parse(b) == isBold)
                return true;
            else
                return false;
        }
        public static bool correctsize(Paragraph p, WordprocessingDocument doc, string size)
        {
            Dictionary<string, string> FSmap = new Dictionary<string, string>() { { "小六","13" }, { "六号","15" },{"小五", "18" },{ "五号","21" },
                                                                                  {"小四", "24" }, { "四号","28" },{"小三" ,"30"}, { "三号","32" },
                                                                                  {"小二", "36" }, { "二号","44" },{"小一", "48" }, {"一号","52" } };
            size = FSmap[size];
            string each = null;
            int T = 0; int F = 0;
            IEnumerable<Run> runlist = p.Elements<Run>();
            foreach (Run run in runlist)
            {
                if (Regex.IsMatch(run.InnerText, @"^([ ])+$") || run.InnerText.Length==0)
                    continue;
                each = getFromRunPpr(run, 6); //从Run属性中查找
                if (each == null)
                {
                    if (run.RunProperties != null)
                    {
                        RunStyle rs = run.RunProperties.RunStyle;
                        if (rs != null)
                        {
                            each = getFromStyle(doc, rs.Val, 6);//从Runstyle中查找
                        }
                    }
                }
                if (each == null)
                {
                    each = getFromPpr(p, 6);//从段落属性中找
                    if (each == null && p.ParagraphProperties != null)
                    {
                        ParagraphStyleId style_id = p.ParagraphProperties.ParagraphStyleId;
                        if (style_id != null)//从paragraphstyle中获取
                        {
                            each = getFromStyle(doc, style_id.Val, 6);
                        }
                        if (each == null)//style没找到
                        {
                            each = getFromDefault(doc, 6);//从段落默认style中找
                            if (each == null)//default没找到
                            {
                                each = "0";
                            }
                        }
                    }
                }
                if (each=="0"||each == size) T++;
                else F++;
            }
            if (F == 0)
                return true;
            else
                return false;
        }
        /// <summary>
        /// 字体检测  检测run的字体 
        /// 如果run里面全是数字或者字符 则将它的字体与ENfont作比较
        /// 否则的话 将它字体与CNfont作比较
        /// 比较结果一致 返回true,反之 返回false;
        /// 如果run里面全是空格 不检查 直接返回true,表示没错
        /// </summary>
        /// <param name="run"></param>
        /// <param name="p"></param>
        /// <param name="doc"></param>
        /// <param name="CNfont"></param>
        /// <param name="ENfont"></param>
        /// <returns></returns>
        public static bool correctRunFonts(Run run, Paragraph p,WordprocessingDocument doc, string CNfont,string ENfont)
        {
            string text = run.InnerText;
            if (Regex.IsMatch(text, @"^[ ]+$") || run.GetFirstChild<Text>()==null)
                return true;
            string font = null;
            int type = 8;
            if (Regex.IsMatch(text, @"^[^\u4e00-\u9fa5]+$"))
                type = 7;
            if(font==null)
            {
                font = getFromRunPpr(run,type);
            }
            if(font==null)
            {
                if (run.RunProperties != null)
                {
                    RunStyle rs = run.RunProperties.RunStyle;
                    if (rs != null)
                    {
                        font = getFromStyle(doc, rs.Val, type);//从Runstyle中查找
                    } 
                }
            }
            if(font==null && p.GetFirstChild<Hyperlink>()==null)
            {
                font = getFromPpr(p,type);
            }
            if(font==null)
            {
                if (p.ParagraphProperties != null)
                {
                    ParagraphStyleId style_id = p.ParagraphProperties.ParagraphStyleId;
                    if (style_id != null)//从paragraphstyle中获取
                    {
                        font = getFromStyle(doc, style_id.Val, type);
                    }
                }
            }
            if(font==null)
            {
                font = getFromDefault(doc,type);
            }
            if(font==null)
            {
                if (type == 7)
                    font = "Times New Roman";
                else
                    font = "宋体";
            }
            //中文中的英文可以是Times New Roman也可以和中文一样
            if ((type == 7 && (font == ENfont || font == CNfont)) || (type == 8 && font == CNfont))
                return true;
            else
                return false;
        }
        public static bool correctfonts(Paragraph p, WordprocessingDocument doc, string CNfonts, string ENfonts)
        {
            IEnumerable<Run>  runlist = p.Elements<Run>();
            foreach (Run run in runlist)
            {
                if (!correctRunFonts(run, p, doc, CNfonts, ENfonts))
                    return false;
            }
            return true;
        }
        public static bool correctHyperlinkFonts( Paragraph p, WordprocessingDocument doc, string CNfont, string ENfont)
        {
            IEnumerable<Run> runlist;
            if(p.GetFirstChild<Hyperlink>()!=null)
            {
                runlist = p.GetFirstChild<Hyperlink>().Elements<Run>();
            }
            else
            {
                runlist =  p.Elements<Run>();
            }
            foreach(Run run in runlist)
            {
                if (!correctRunFonts(run, p, doc, CNfont, ENfont))
                    return false;
            }
            return true;
        }
        /// <summary>
        /// 以下三个函数分别从段落属性，style和默认style中查找属性
        /// 参数type 0缩进 1对齐方式 2段前 3段后 4行间距 5加粗 6字号
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        public static string getFromPpr(Paragraph p, int type)
        {
            ParagraphProperties ppr = p.ParagraphProperties;
            if (ppr != null)
            {
                /////缩进
                if (type == 0)
                {
                    Indentation ind = ppr.Indentation;
                    if (ind != null)
                    {
                        double lc = 0;
                        double fc = 0;
                        if (ind.LeftChars != null)
                            lc+=ind.LeftChars;
                        else if (ind.Left != null)
                            lc += double.Parse(PointToChars(ind.Left));
                        if (ind.FirstLineChars != null && ind.FirstLine != null && ind.FirstLineChars == "0")
                            fc += double.Parse(PointToChars(ind.FirstLine));
                        else if (ind.FirstLineChars != null)
                            fc+=ind.FirstLineChars;
                        else if (ind.FirstLine != null)
                            fc += double.Parse(PointToChars(ind.FirstLine));
                        return (lc + fc).ToString();
                    }
                }
                /////对齐方式
                if (type == 1)
                {
                    Justification jc = ppr.Justification;
                    if (jc != null)
                    {
                        return jc.Val;
                    }
                }
                /////段前
                if (type == 2)
                {
                    SpacingBetweenLines line = ppr.SpacingBetweenLines;
                    if (line != null)
                    {
                        if (line.BeforeLines != null && line.Before != null && line.BeforeLines=="0")
                            return line.Before.Value;
                        if (line.BeforeLines != null)
                            return lineToPoint(line.BeforeLines);
                        if (line.Before != null)
                            return line.Before.Value;
                    }
                }
                /////段后
                if (type == 3)
                {
                    SpacingBetweenLines line = ppr.SpacingBetweenLines;
                    if (line != null)
                    {
                        if (line.AfterLines != null && line.After != null && line.AfterLines=="0")
                            return line.After.Value;
                        if (line.AfterLines != null)
                            return lineToPoint(line.AfterLines);
                        if (line.After != null)
                            return line.After.Value;
                    }
                }
                /////行间距
                if (type == 4)
                {
                    SpacingBetweenLines line = ppr.SpacingBetweenLines;
                    if (line != null)
                    {
                        if (line.Line != null)
                            return line.Line.Value;
                    }
                }
                //加粗
                if (type == 5)
                {
                    ParagraphMarkRunProperties rpr = ppr.ParagraphMarkRunProperties;
                    if (rpr != null)
                    {
                        if (rpr.GetFirstChild<Bold>() != null)
                        {
                            if (rpr.GetFirstChild<Bold>().Val == "0")
                                return "false";
                            else
                                return "true";
                        }
                    }
                }
                //字号
                if (type == 6)
                {
                    ParagraphMarkRunProperties rpr = ppr.ParagraphMarkRunProperties;
                    if (rpr != null)
                    {
                        if (rpr.GetFirstChild<FontSize>() != null)
                        {
                            return rpr.GetFirstChild<FontSize>().Val;
                        }
                    }
                }
                //英文字体
                if (type == 7)
                {
                    ParagraphMarkRunProperties rpr = ppr.ParagraphMarkRunProperties;
                    if (rpr != null)
                    {
                        if (rpr.GetFirstChild<RunFonts>() != null && rpr.GetFirstChild<RunFonts>().Ascii != null)
                        {
                            return rpr.GetFirstChild<RunFonts>().Ascii;
                        }
                    }
                }
                //字体
                if (type ==8)
                {
                    ParagraphMarkRunProperties rpr = ppr.ParagraphMarkRunProperties;
                    if (rpr != null)
                    {
                        if (rpr.GetFirstChild<RunFonts>() != null && rpr.GetFirstChild<RunFonts>().EastAsia != null)
                        {
                            return rpr.GetFirstChild<RunFonts>().EastAsia;
                        }
                    }
                }
            }
            return null;
        }
        public static string getFromStyle(WordprocessingDocument doc, string style_id, int type)
        {
            IEnumerable<Style> style = doc.MainDocumentPart.StyleDefinitionsPart.Styles.Elements<Style>();
            Style _style = null;
            foreach (Style st in style)
            {
                if (st.StyleId == style_id)
                {
                    _style = st;
                    StyleParagraphProperties sppr = st.StyleParagraphProperties;
                    StyleRunProperties srpr = st.StyleRunProperties;
                    if (sppr != null)
                    {
                        /////缩进
                        if (type == 0)
                        {
                            Indentation ind = sppr.Indentation;
                            if (ind != null)
                            {
                                double lc = 0;
                                double hc = 0;
                                double fc = 0;
                                if (ind.LeftChars != null)
                                    lc += ind.LeftChars;
                                else if (ind.Left != null)
                                    lc += double.Parse(PointToChars(ind.Left));
                                if (ind.HangingChars != null)
                                    hc += ind.HangingChars;
                                else if (ind.Hanging != null)
                                    hc += double.Parse(PointToChars(ind.Hanging));
                                if (ind.FirstLineChars != null)
                                    fc += ind.FirstLineChars;
                                else if (ind.FirstLine != null)
                                    fc += double.Parse(PointToChars(ind.FirstLine));
                                return (lc + hc + fc).ToString();
                            }
                        }
                        /////对齐方式
                        if (type == 1)
                        {
                            Justification jc = sppr.Justification;
                            if (jc != null)
                            {
                                if (jc != null)
                                    return jc.Val;
                            }
                        }
                        /////段前
                        if (type == 2)
                        {
                            SpacingBetweenLines line = sppr.SpacingBetweenLines;
                            if (line != null)
                            {
                                if (line.BeforeLines != null)
                                    return lineToPoint(line.BeforeLines);
                                else if (line.Before != null)
                                    return line.Before.Value;
                            }
                        }
                        /////段后
                        if (type == 3)
                        {
                            SpacingBetweenLines line = sppr.SpacingBetweenLines;
                            if (line != null)
                            {
                                if (line.AfterLines != null)
                                    return lineToPoint(line.AfterLines);
                                else if (line.After != null)
                                    return line.After.Value;
                            }
                        }
                        /////行间距
                        if (type == 4)
                        {
                            SpacingBetweenLines line = sppr.SpacingBetweenLines;
                            if (line != null)
                            {
                                if (line.Line != null)
                                    return line.Line.Value;
                            }
                        }
                    }
                    if (srpr != null)
                    {
                        //加粗
                        if (type == 5)
                        {
                            if (srpr != null)
                            {
                                if (srpr.GetFirstChild<Bold>() != null)
                                {
                                    if (srpr.GetFirstChild<Bold>().Val == "0")
                                        return "false";
                                    else
                                        return "true";
                                }
                            }
                        }
                        //字号
                        if (type == 6)
                        {
                            if (srpr != null)
                            {
                                if (srpr.GetFirstChild<FontSize>() != null)
                                {
                                    return srpr.GetFirstChild<FontSize>().Val;
                                }
                            }
                        }
                        //英文字体
                        if (type == 7)
                        {
                            if (srpr != null)
                            {
                                if (srpr.GetFirstChild<RunFonts>() != null)
                                {
                                    if (srpr.GetFirstChild<RunFonts>().Ascii!=null)
                                        return srpr.GetFirstChild<RunFonts>().Ascii;
                                }
                            }
                        }
                        //中文字体
                        if (type == 8)
                        {
                            if (srpr != null)
                            {
                                if (srpr.GetFirstChild<RunFonts>() != null)
                                {
                                    if (srpr.GetFirstChild<RunFonts>().EastAsia!=null)
                                        return srpr.GetFirstChild<RunFonts>().EastAsia;
                                }
                            }
                        }
                    }
                }
            }
            if (_style.BasedOn == null)
                return null;
            else
            {
                //递归查询
                return getFromStyle(doc, _style.BasedOn.Val, type);
            }
        }
        public static string getFromDefault(WordprocessingDocument doc, int type)
        {
            IEnumerable<Style> style = doc.MainDocumentPart.StyleDefinitionsPart.Styles.Elements<Style>();
            foreach (Style st in style)
            {
                if (st.Type == StyleValues.Paragraph && st.Default != null && st.Default)
                {
                    StyleParagraphProperties sppr = st.StyleParagraphProperties;
                    StyleRunProperties srpr = st.StyleRunProperties;
                    if (sppr != null)
                    {
                        /////缩进
                        if (type == 0)
                        {
                            Indentation ind = sppr.Indentation;
                            if (ind != null)
                            {
                                double lc = 0;
                                double hc = 0;
                                double fc = 0;
                                if (ind.LeftChars != null)
                                    lc += ind.LeftChars;
                                else if (ind.Left != null)
                                    lc += double.Parse(PointToChars(ind.Left));
                                if (ind.HangingChars != null)
                                    hc += ind.HangingChars;
                                else if (ind.Hanging != null)
                                    hc += double.Parse(PointToChars(ind.Hanging));
                                if (ind.FirstLineChars != null)
                                    fc += ind.FirstLineChars;
                                else if (ind.FirstLine != null)
                                    fc += double.Parse(PointToChars(ind.FirstLine));
                                return (lc + hc + fc).ToString();
                            }
                        }
                        /////对齐方式
                        if (type == 1)
                        {
                            Justification jc = sppr.Justification;
                            if (jc != null)
                            {
                                if (jc != null)
                                    return jc.Val;
                            }
                        }
                        /////段前
                        if (type == 2)
                        {
                            SpacingBetweenLines line = sppr.SpacingBetweenLines;
                            if (line != null)
                            {
                                if (line.BeforeLines != null)
                                    return lineToPoint(line.BeforeLines);
                                else if (line.Before != null)
                                    return line.Before.Value;
                            }
                        }
                        /////段后
                        if (type == 3)
                        {
                            SpacingBetweenLines line = sppr.SpacingBetweenLines;
                            if (line != null)
                            {
                                if (line.AfterLines != null)
                                    return lineToPoint(line.AfterLines);
                                else if (line.After != null)
                                    return line.After.Value;
                            }
                        }
                        /////行间距
                        if (type == 4)
                        {
                            SpacingBetweenLines line = sppr.SpacingBetweenLines;
                            if (line != null)
                            {
                                if (line.Line != null)
                                    return line.Line.Value;
                            }
                        }
                    }
                    if (srpr != null)
                    {
                        //加粗
                        if (type == 5)
                        {
                            if (srpr != null)
                            {
                                if (srpr.GetFirstChild<Bold>() != null)
                                {
                                    if (srpr.GetFirstChild<Bold>().Val == "0")
                                        return "false";
                                    else
                                        return "true";
                                }
                            }
                        }
                        //字号
                        if (type == 6)
                        {
                            if (srpr != null)
                            {
                                if (srpr.GetFirstChild<FontSize>() != null)
                                {
                                    return srpr.GetFirstChild<FontSize>().Val;
                                }
                            }
                        }
                        //英文字体
                        if (type == 7)
                        {
                            if (srpr != null)
                            {
                                if (srpr.GetFirstChild<RunFonts>() != null && srpr.GetFirstChild<RunFonts>().Ascii != null)
                                {
                                    return srpr.GetFirstChild<RunFonts>().Ascii;
                                }
                            }
                        }
                        //中文字体
                        if (type == 8)
                        {
                            if (srpr != null)
                            {
                                if (srpr.GetFirstChild<RunFonts>() != null && srpr.GetFirstChild<RunFonts>().EastAsia != null)
                                {
                                    return srpr.GetFirstChild<RunFonts>().EastAsia;
                                }
                            }
                        }
                    }
                }
            }
            return null;
        }
        public static string getFromRunPpr(Run r, int type)
        {
            RunProperties rpr = r.RunProperties;
            if (rpr != null)
            {
                //加粗
                if (type == 5)
                {
                    if (rpr != null)
                    {
                        if (rpr.GetFirstChild<Bold>() != null)
                        {
                            if (rpr.GetFirstChild<Bold>().Val == "0")
                                return "false";
                            else
                                return "true";
                        }
                    }
                }
                //字号
                if (type == 6)
                {
                    if (rpr != null)
                    {
                        if (rpr.GetFirstChild<FontSize>() != null)
                        {
                            return rpr.GetFirstChild<FontSize>().Val;
                        }
                    }
                }
                //英文字体
                if (type == 7)
                {
                    if (rpr != null)
                    {
                        if (rpr.GetFirstChild<RunFonts>() != null && rpr.GetFirstChild<RunFonts>().Ascii != null)
                        {
                            return rpr.GetFirstChild<RunFonts>().Ascii;
                        }
                    }
                }
                //中文字体
                if (type == 8)
                {
                    if (rpr != null)
                    {
                        if (rpr.GetFirstChild<RunFonts>() != null && rpr.GetFirstChild<RunFonts>().EastAsia != null)
                        {
                            return rpr.GetFirstChild<RunFonts>().EastAsia;
                        }
                    }
                }
            }
            return null;
        }
        public static string lineToPoint(string line)
        {
            string l = (int.Parse(line) * 24/10).ToString();
            return int.Parse(l).ToString();
        }
        public static string PointToChars(string point)
        {
            string l = (double.Parse(point) * 10 / firstCharSize).ToString();
            return l;
        }
        public static string getFirstcharSize(Paragraph p, WordprocessingDocument doc)
        {
            Dictionary<string, string> FSmap = new Dictionary<string, string>() { { "小六","13" }, { "六号","15" },{"小五", "18" },{ "五号","21" },
                                                                                  {"小四", "24" }, { "四号","28" },{"小三" ,"30"}, { "三号","32" },
                                                                                  {"小二", "36" }, { "二号","44" },{"小一", "48" }, {"一号","52" } };
            string size = null;
            Run run = p.GetFirstChild<Run>();
            //这里就没有考虑目录了
            if (run == null)
                return "24";
            else
            {
                size = getFromRunPpr(run, 6); //从Run属性中查找
                if (size == null)
                {
                    if (run.RunProperties != null)
                    {
                        RunStyle rs = run.RunProperties.RunStyle;
                        if (rs != null)
                        {
                            size = getFromStyle(doc, rs.Val, 6);//从Runstyle中查找
                        }
                    }
                }
                if (size == null)
                {
                    size = getFromPpr(p, 6);//从段落属性中找
                }
                if (size == null && p.ParagraphProperties != null)
                {
                    ParagraphStyleId style_id = p.ParagraphProperties.ParagraphStyleId;
                    if (style_id != null)
                    {
                        size = getFromStyle(doc, style_id.Val, 6);//从paragraphstyle中获取
                    }
                }
                if (size == null)//style没找到
                {
                    size = getFromDefault(doc, 6);//从段落默认style中找
                    if (size == null)//default没找到
                    {
                        size = "24";
                    }
                }
            }
            return size;
        }
        public static string getLine(string px)
        {
            string l = (double.Parse(px) / 240).ToString();
            return l;
        }












































































        //获得表所在章节号,图也可用 location为第几段
        public static string Chapter(List<int> titlePosition, int location, Body body)
        {
            string chapter = "";
            int titlelocation = -1;
            int i = 0;
            if (titlePosition.Count != 0)
            {
                for (i = 0; titlePosition[i] < location; i++)
                {
                    titlelocation = i;
                    if (i == titlePosition.Count - 1)
                        break;
                }
            }
            Paragraph p = null;
            if (titlelocation >= 0)
            {
                if (titlePosition[titlelocation] - 1 >= 0)
                {
                    p = (Paragraph)body.ChildElements.GetItem(titlePosition[titlelocation] - 1);
                }
            }
            if (p != null)
            {
                chapter = getFullText(p);
            }
            return chapter;
        }




        /* 判断是从某个章开始的第几个表格,图也可用 */
        public static int NumTblCha(List<int> chapter, List<int> table, int location)
        {
            int a = 0;
            int i;
            int index = -1;
            List<int> chaptertable = new List<int>();
            if (chapter.Count != 0)
            {
                for (i = 0; chapter[i] < location; i++)
                {
                    index = i;
                    if (i == chapter.Count - 1)
                        break;
                }
            }
            foreach (int tbl in table)
            {
                if (index != -1 && index <= chapter.Count - 2)
                {
                    if (tbl >= chapter[index] - 1 && tbl <= chapter[index + 1] - 1)
                    {
                        chaptertable.Add(tbl);
                    }
                }
                if (index != -1 && index == chapter.Count - 1)
                {
                    if (tbl >= chapter[index])
                    {
                        chaptertable.Add(tbl);
                    }
                }
            }
            for (int j = 0; j < chaptertable.LongCount(); j++)
            {
                if (chaptertable[j] == location)
                    a = j + 1;
            }
            if (a == 0)
            {
                return a;
            }
            return a;
        }







        /* 获取文档标题所在位置 */
        public static List<int> getTitlePosition(WordprocessingDocument doc)
        {
            Body body = doc.MainDocumentPart.Document.Body;
            var list = body.ChildElements;
            Paragraph temp_p = new Paragraph();
            List<int> titlePosition = new List<int>();
            int count = 0;
            string text = "";
            List<String> content = new List<string>();
            content = getContent(doc);
            Regex titleOne = new Regex("[1-9]");
            foreach (var p in list)
            {
                count++;
                if (p.GetType() == temp_p.GetType())
                {
                    Paragraph pp = (Paragraph)p;
                    Run r = p.GetFirstChild<Run>();
                    if (r != null)
                    {
                        text = Util.getFullText(pp);
                        //trim去掉字符串左右两段的空格
                        if (text.Trim().Length > 1)
                        {
                            if (isTitle(text, content))
                            {
                                if (Util.getFullText(pp).Trim().Length >= 3)
                                {
                                    if (pp.GetFirstChild<BookmarkStart>() != null || Util.getFullText(pp).Trim().Substring(0, 3).Split('.').Length > 1)
                                    {
                                        titlePosition.Add(count);
                                    }
                                }
                                continue;
                            }
                            if (p.GetFirstChild<BookmarkStart>() != null)
                            {
                                if (titleOne.Match(text.Trim().Substring(0, 1)).Success)
                                {
                                    titlePosition.Add(count);
                                }
                            }
                        }
                    }
                }
            }
            return titlePosition;
        }

        /* 获得章标题位置 */
        public static List<int> getchaptertitleposition(WordprocessingDocument doc)
        {
            Body body = doc.MainDocumentPart.Document.Body;
            var list = body.ChildElements;
            Paragraph temp_p = new Paragraph();
            List<int> titlePosition = new List<int>();
            int count = 0;
            string text = "";
            Tool tool = new Tool();
            List<String> content = new List<string>();
            content = getContent(doc);
            Regex titleOne = new Regex("[1-9]");
            foreach (var p in list)
            {
                count++;
                if (p.GetType() == temp_p.GetType())
                {
                    Paragraph pp = (Paragraph)p;
                    Run r = p.GetFirstChild<Run>();
                    if (r != null)
                    {
                        text = getFullText(pp);
                        //trim去掉字符串左右两端的空格
                        if (text.Trim().Length > 1)
                        {
                            if (p.GetFirstChild<BookmarkStart>() != null)
                            {
                                if (isTitle(text, content))
                                {
                                    if (getFullText(pp).Trim().Length >= 3)
                                    {
                                        if (pp.GetFirstChild<BookmarkStart>() != null || getFullText(pp).Trim().Substring(0, 3).Split('.').Length > 1)
                                        {
                                            if (text.Trim().IndexOf('.') != 3 && text.Trim().IndexOf('.') != 1)
                                            {
                                                titlePosition.Add(count);
                                            }
                                        }
                                    }
                                    continue;
                                }
                                if (titleOne.Match(text.Trim().Substring(0, 1)).Success)
                                {
                                    if (text.Trim().IndexOf('.') != 3 && text.Trim().IndexOf('.') != 1)
                                    {
                                        titlePosition.Add(count);
                                    }
                                }
                            }
                        }
                    }
                }
            }
            return titlePosition;
        }


        //判断是否是标题
        public static bool isTitle(String str, List<String> content)
        {
            foreach (String s in content)
            {
                if (s.Contains(str))
                {
                    return true;
                }
            }
            return false;
        }


        /* 获取目录内容 */
        public static List<String> getContent(WordprocessingDocument doc)
        {
            Body body = doc.MainDocumentPart.Document.Body;
            MainDocumentPart mainPart = doc.MainDocumentPart;
            IEnumerable<Paragraph> paras = body.Elements<Paragraph>();
            List<String> contens = new List<string>();
            bool isContents = false;
            foreach (Paragraph p in paras)
            {
                Run r = p.GetFirstChild<Run>();
                String fullText = "";
                if (r != null)
                {
                    fullText = Util.getFullText(p).Trim();
                }
                if (fullText.Replace(" ", "") == "目录" && !isContents)
                {
                    isContents = true;
                    continue;
                }
                if (isContents)
                {
                    Hyperlink h = p.GetFirstChild<Hyperlink>();
                    if (h != null)
                    {
                        contens.Add(getHyperlinkFullText(h));
                    }
                }
            }
            return contens;
        }

        //得到Hyperlink p的内容
        public static String getHyperlinkFullText(Hyperlink p)
        {
            String text = "";
            IEnumerable<Run> list = p.Elements<Run>();
            foreach (Run r in list)
            {
                Text pText = r.GetFirstChild<Text>();
                if (pText != null)
                {
                    text += pText.Text;
                }
            }
            return text;
        }




        //提取表和图中的M.N
        public static string number(string title)
        {
            if (title != null)
            {
                string num = null;
                int l = -1;
                int i = -1;
                int j = 0;
                int len = title == null ? -1 : title.Length;
                //获得第一个数字位置用l记录
                foreach (char c in title)
                {
                    i++;
                    if ((c <= '9' && c >= '0'))
                    {
                        l = i;
                        break;
                    }
                }
                for (j = 0; j < 5; j++)
                {
                    if (j + l < len && j + l >= 0)
                    {
                        if ((title[j + l] >= '0' && title[j + l] <= '9') || title[j + l] == '.')
                        {
                            num += title[j + l];
                        }
                        else
                        {
                            break;
                        }
                    }
                }
                return num;
            }
            else { return null; }
        }

        //表和图编号中的m应与章号一致
        public static bool correctm(string num, string chapter)
        {
            char m1 = '\0';
            char m2 = '\0';
            if (chapter != "")
            {
                m1 = chapter[0];
            }
            if (num != "")
            {
                m2 = num[0];
            }
            //带章节号的比对
            if (m1 >= '0' && m1 <= '9')
            {
                if (m1 == m2)
                {
                }
                else
                {
                    return false;
                }
            }
            else if (m1 == '附')
            {
                //附录X
                if (chapter.Length >= 3)
                {
                    if (chapter[2] != m2)
                    {
                        return false;
                    }
                }
            }
            return true;
        }


        //表和图中序号n连续  numbertable为第几个表，continued为续表个数
        public static bool correctn(string num, int numbertable, int continued)
        {
            int i = num.IndexOf('.');
            string n = "";
            if (i < 0)
            {
                return false;
            }
            else
            {
                if (i + 1 < num.Length)
                {
                    if (i + 2 < num.Length)
                    {
                        if (num[i + 1] >= '1' && num[i + 1] <= '9')
                        {
                            if (num[i + 2] >= '0' && num[i + 2] <= '9')
                            {
                                n = num.Substring(i + 1, 2);
                            }
                            else
                            {
                                n = num.Substring(i + 1, 1);
                            }
                        }
                        else
                        {
                            return false;
                        }
                    }
                    else
                    {

                        if (num[i + 1] >= '1' && num[i + 1] <= '9')
                        {
                            n = num.Substring(i + 1, 1);
                        }
                        else
                        {
                            return false;
                        }
                    }
                }
            }
            if (n != "")
            {
                if (n != (numbertable - continued).ToString())
                {
                    return false;
                }
            }
            return true;
        }



        //中文图表名
        //*******5.24新增 序号检测
        //检测项  1.序号前无空格  
        //       2.序号后g空格
        //       3.是否满足m.n格式
        public static int[] numberstyle(string title, int numlen, int g)
        {
            int l = -1;
            int i = -1;
            int[] a = new int[3] { 1, 1, 1 };
            foreach (char c in title)//寻找第一个数字字母位置
            {
                i++;
                if ((c <= '9' && c >= '0') || (c >= 'A' && c <= 'Z') || (c >= 'a' && c <= 'z'))
                {
                    l = i;
                    break;
                }
            }
            if (l == -1)//没写序号的情况
            {
                a[2] = 0;
                //加批注
                return a;
            }
            //序号前无空格  
            if (l - 1 >= 0)
            {
                if (title[l - 1] == ' ')
                {
                    a[0] = 0;
                }
            }
            //序号后g空格
            if (l + numlen + 1 < title.Length)
            {
                for (int j = l + numlen; j < numlen + l + g; j++)
                {
                    if (title[j] != ' ')
                    {
                        a[1] = 0;
                        break;
                    }
                }
                if (title[l + numlen + g] == ' ')
                {
                    a[1] = 0;
                }
            }
            //m.n格式
            if (l + 2 < title.Length && l >= 0)
            {
                if (title[l + 1] == '.')//m为一位数
                {
                    for (int j = 2; j < numlen; j++)
                    {
                        if (title[l + j] < '0' || title[l + j] > '9')
                        {
                            a[2] = 0;
                        }
                    }
                }
                else if (title[l + 2] == '.')//m为两位数
                {
                    for (int j = 3; j < numlen; j++)
                    {
                        if (title[l + j] <= '0' || title[l + j] >= '9')
                        {
                            a[2] = 0;
                        }
                    }
                }
                else if (title[l + 2] != '.'|| title[l + 1] != '.')//m为两位数
                {
                    a[2] = 0;
                }

            }

            return a;
        }
    }
}
