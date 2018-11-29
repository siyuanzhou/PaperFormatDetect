using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using DocumentFormat.OpenXml.Wordprocessing;
using DocumentFormat.OpenXml.Packaging;
using PaperFormatDetection.Tools;

namespace PaperFormatDetection.Paperbase
{

    class Text
    {
        /// <summary>
        /// tTitle数组为正文中标题的属性
        /// tfulltext数组为正文中段落的属性
        /// 标题6个参数 顺序分别是 0字体 1字号 2对齐方式 3段前间距 4断后间距 5行间距 6加粗
        /// 段落7个参数 顺序分别是 0缩进 1字体 2字号 3对齐方式 4段前间距 5断后间距 6行间距 7加粗
        /// </summary>
        protected string[,] tTitle = new string[3, 7];
        protected string[] tText = new string[8];
        string one_level = "1",
               two_level = "1.1",
               three_level = "1.1.1";
        static string[] NotesPattern = { "[-][-].*", "[/][*].*?[*][/]", "[/][/].*", "[#].*", "<!--.*?-->", "[\"].+?[\"]", "[^\u4E00-\u9FA5][-][-].*", "[\'][\u4E00-\u9FA5]*[\']" };
        static string[] ProNumbering = { "[（].*?[）]", "[(].*?[)]", "[①②③④⑤⑥⑦⑧⑨⑩]"};
        bool ChineseNumbering = false;
        bool EnglishNumbering = false;
        bool isParagraphBreak = false;
        int count = 0;

        public Text()
        {

        }

        public void detectAllText(List<Paragraph> list, WordprocessingDocument doc)
        {
            //记录当前标题文本
            string curTitle = null;
            //记录当前段落文本
            string paratext = null;
            //记录空段落
            List<string> content = getContent(doc);
            foreach (Paragraph p in list)
            {
                paratext = Util.getFullText(p).Trim();
                Match m = Regex.Match(paratext, @"\S");
                if (!m.Success)
                {
                    continue;
                }
                if (isTitle(paratext, content))
                {
                    curTitle = paratext;
                    detectTitle(p, doc);
                }
                //过滤公式
                else if (p.GetFirstChild<DocumentFormat.OpenXml.Math.OfficeMath>() != null)
                {

                }
                else
                {
                    if (paratext.Length > 7)
                    {
                        //过滤中文图名表名
                        m = Regex.Match(paratext.Substring(0, 6), @"[\u4E00-\u9FA5]\ *[1-9][0-9]*\.*\-*[1-9][0-9]*");
                        if (m.Success) continue;
                        //过滤英文表名
                        m = Regex.Match(paratext, @"Tab\.*\ *[1-9][0-9]*\.[1-9][0-9]*");
                        if (m.Success) continue;
                        //过滤英文图名
                        m = Regex.Match(paratext, @"Fig\.*\ *[1-9][0-9]*\.[1-9][0-9]*");
                        if (m.Success) continue;
                    }
                    //过滤代码
                    if (isCode(paratext)) continue;

                    detectText(p, curTitle, doc);
                }
            }
        }

        public void detectTitle(Paragraph p, WordprocessingDocument doc)
        {
            //匹配标题的正则数组
            Regex[] reg = new Regex[3];
            //一级标题
            reg[0] = new Regex(@".+?");
            //二级标题
            reg[1] = new Regex(@"[1-9][0-9]*\..+?");
            //三级标题
            reg[2] = new Regex(@"[1-9][0-9]*\.[1-9][0-9]*\..+?");
            string title = Tool.getFullText(p).Trim();
            string order = "";
            IEnumerable<Run> run = p.Elements<Run>();
            int index;
            Match m = reg[2].Match(title);

            for (index = 2; index > -1; index--)
            {
                m = reg[index].Match(title);
                if (m.Success == true)
                    break;
            }
            if (index > -1)
            {
                if (m.Index == 0)
                {
                    if (title.IndexOf("   ") > 0)
                    {
                        Util.printError("正文标题序号与内容之间应有两个空格----" + title);
                    }
                    else if (title.IndexOf("  ") < 0)
                    {
                        Util.printError("正文标题序号与内容之间应有两个空格----" + title);
                    }

                    order = m.Value;
                    if (Util.getSubstrCount(order, ".") == 0)
                    {
                        //章标题另起一页
                        if (Util.pageDic.ContainsKey(title))
                        {
                            string page = Util.pageDic[title];
                            int index1 = page.IndexOf("_");
                            int index2 = page.LastIndexOf("_");
                            if (page.Substring(0, index1) == page.Substring(index1 + 1, index2 - index1 - 1))
                            {
                                Util.printError("正文章标题需另起一页" + "----" + title);
                            }
                        }
                        if (order != one_level)
                        {
                            Util.printError("正文标题序号顺序错误，序号应为" + one_level + "----" + title);
                        }
                        two_level = one_level + ".1";
                        one_level = (Convert.ToInt32(one_level) + 1).ToString();
                    }
                    else if (Util.getSubstrCount(order, ".") == 1)
                    {
                        if (order != two_level)
                        {
                            Util.printError("正文标题序号顺序错误，序号应为" + two_level + "----" + title);
                        }
                        three_level = two_level + ".1";
                        int i = two_level.IndexOf(".");
                        //二级标题加一
                        two_level = two_level.Substring(0, i + 1) + (Convert.ToInt32(two_level.Substring(i + 1, two_level.Length - i - 1)) + 1).ToString();
                    }
                    else if (Util.getSubstrCount(order, ".") == 2)
                    {
                        if (order != three_level)
                        {
                            Util.printError("正文标题序号顺序错误，序号应为" + three_level + "----" + title);
                        }
                        int i = three_level.LastIndexOf(".");
                        //三级标题加一
                        three_level = three_level.Substring(0, i + 1) + (Convert.ToInt32(three_level.Substring(i + 1, three_level.Length - i - 1)) + 1).ToString();
                    }
                    else
                    {
                        Util.printError("正文标题序号错误，不能超过三级标题" + "----" + title);
                    }
                }
                else
                {
                    return;
                }
                /* index = 0 对应一级标题
                    index = 1 对应二级标题
                    index = 2 对应三级标题 */
                if (!Util.correctfonts(p, doc, tTitle[index, 0], "Cambria"))
                    Util.printError("正文标题字体错误，应为" + tTitle[index, 0] + "----" + title);
                if (!Util.correctsize(p, doc, tTitle[index, 1]))
                    Util.printError("正文标题字号错误，应为" + tTitle[index, 1] + "----" + title);
                if (!Util.correctJustification(p, doc, tTitle[index, 2]) && !Util.correctJustification(p, doc, "两端对齐"))
                    Util.printError("正文标题未" + tTitle[index, 2] + "----" + title);
                if (!Util.correctSpacingBetweenLines_Be(p, doc, tTitle[index, 3]))
                    Util.printError("正文标题段前距错误，应为" + Convert.ToDouble(tTitle[index, 3])/240 + "行" + "----" + title);
                if (!Util.correctSpacingBetweenLines_Af(p, doc, tTitle[index, 4]))
                    Util.printError("正文标题段后距错误，应为" + Convert.ToDouble(tTitle[index, 4])/240 + "行" + "----" + title);
                if (!Util.correctSpacingBetweenLines_line(p, doc, tTitle[index, 5]))
                    Util.printError("正文标题行间距错误，应为" + Convert.ToDouble(tTitle[index, 5]) / 240 + "倍行距" + "----" + title);
                if (!Util.correctBold(p, doc, Convert.ToBoolean(tText[7])))
                    Util.printError("正文标题" + (Convert.ToBoolean(tText[7]) ? "需要" : "不需") + "加粗" + "----" + title);
            }
            else
            {
                if (p.ParagraphProperties != null)
                {
                    if (p.ParagraphProperties.NumberingProperties == null)
                    {
                        if (title.Length != 0)
                            Util.printError("正文标题序号错误，应为阿拉伯数字----" + title);
                    }
                    //检测是否自动编号
                    else
                    {
                        string numberingId = p.ParagraphProperties.NumberingProperties.NumberingId.Val;
                        string ilvl = p.ParagraphProperties.NumberingProperties.NumberingLevelReference.Val;
                        Numbering numbering1 = doc.MainDocumentPart.NumberingDefinitionsPart.Numbering;
                        IEnumerable<NumberingInstance> nums = numbering1.Elements<NumberingInstance>();
                        IEnumerable<AbstractNum> abstractNums = numbering1.Elements<AbstractNum>();
                        foreach (NumberingInstance num in nums)
                        {
                            if (num.NumberID == numberingId)
                            {
                                Int32 abstractNumId1 = num.AbstractNumId.Val;
                                foreach (AbstractNum abstractNum in abstractNums)
                                {
                                    if (abstractNum.AbstractNumberId == abstractNumId1)
                                    {
                                        Level level = abstractNum.GetFirstChild<Level>();
                                        if (level.GetFirstChild<NumberingFormat>().Val != "decimal")
                                        {
                                            Util.printError("正文标题序号错误，应为阿拉伯数字----" + title);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                else
                {
                    Util.printError("正文标题序号错误，应为阿拉伯数字----" + title);
                }
            }
        }

        public void detectText(Paragraph p, string curTitle, WordprocessingDocument doc)
        {
            IEnumerable<Run> runList = p.Elements<Run>();
            string paratext = Util.getFullText(p).Trim();
            bool isProTitle = false;
            //手动输入的项目符号检测
            if (paratext.Length > 3)
            {
                Match m = Regex.Match(paratext, "[（].*?[）]");
                if(m.Success && m.Index == 0)
                {
                    isProTitle = true;
                    ChineseNumbering = true;
                    if (count < 1 && EnglishNumbering)
                    {
                        Util.printError("正文段落项目编号错误，应为全文统一的中文圆括号：" + curTitle + "----" + (paratext.Length < 10 ? paratext : (paratext.Substring(0, 10) + "...")));
                        count++;
                    }
                    if (!Regex.Match(m.Value.Substring(1, m.Length - 2), "[0-9][1-9]*").Success)
                        Util.printError("正文段落项目编号错误，应为一对圆括号，内为一个阿拉伯数字：" + curTitle + "----" + (paratext.Length < 10 ? paratext : (paratext.Substring(0, 10) + "...")));
                    if (paratext.Substring(m.Length, 1) != " " || paratext.Substring(m.Length, 2) == "  ")
                        Util.printError("正文段落项目编号与标题之间应为一个空格：" + curTitle + "----" + (paratext.Length < 10 ? paratext : (paratext.Substring(0, 10) + "...")));
                }
                m = Regex.Match(paratext, "[(].*?[)]");
                if (m.Success && m.Index == 0)
                {
                    isProTitle = true;
                    EnglishNumbering = true;
                    if (count < 1 && ChineseNumbering)
                    {
                        Util.printError("正文段落项目编号错误，应为全文统一的中文圆括号：" + curTitle + "----" + (paratext.Length < 10 ? paratext : (paratext.Substring(0, 10) + "...")));
                        count++;
                    }
                    if (!Regex.Match(m.Value.Substring(1, m.Length - 2), "[0-9][1-9]*").Success)
                        Util.printError("正文段落项目编号错误，应为一对圆括号，内为一个阿拉伯数字：" + curTitle + "----" + (paratext.Length < 10 ? paratext : (paratext.Substring(0, 10) + "...")));
                    if (paratext.Substring(m.Length, 1) != " " || paratext.Substring(m.Length, 2) == "  ")
                        Util.printError("正文段落项目编号与标题之间应为一个空格：" + curTitle + "----" + (paratext.Length < 10 ? paratext : (paratext.Substring(0, 10) + "...")));
                }
                m = Regex.Match(paratext, "[①②③④⑤⑥⑦⑧⑨⑩]");
                if (m.Success && m.Index == 0)
                {
                    isProTitle = true;
                    if (paratext.Substring(1, 1) != " " || paratext.Substring(1, 2) == "  ")
                        Util.printError("正文段落项目编号与标题之间应为一个空格：" + curTitle + "----" + (paratext.Length < 10 ? paratext : (paratext.Substring(0, 10) + "...")));
                }
                m = Regex.Match(paratext, ".[）)、.．]");
                if (m.Success && m.Index == 0)
                {
                    isProTitle = true;
                    Util.printError("正文段落项目编号错误，应为一对圆括号，内为一个阿拉伯数字：" + curTitle + "----" + (paratext.Length < 10 ? paratext : (paratext.Substring(0, 10) + "...")));
                }
                m = Regex.Match(paratext, "[◆●]");
                if(m.Success && m.Index == 0)
                {
                    isProTitle = true;
                    Util.printError("正文段落项目编号错误，应为一对圆括号，内为一个阿拉伯数字：" + curTitle + "----" + (paratext.Length < 10 ? paratext : (paratext.Substring(0, 10) + "...")));
                }
            }
            //自动生成的项目符号检测
            if (p.ParagraphProperties != null)
            {
                if (p.ParagraphProperties.NumberingProperties != null)
                {
                    isProTitle = true;
                    string numberingId = p.ParagraphProperties.NumberingProperties.NumberingId.Val;
                    string ilvl = p.ParagraphProperties.NumberingProperties.NumberingLevelReference.Val;
                    Numbering numbering1 = doc.MainDocumentPart.NumberingDefinitionsPart.Numbering;
                    IEnumerable<NumberingInstance> nums = numbering1.Elements<NumberingInstance>();
                    IEnumerable<AbstractNum> abstractNums = numbering1.Elements<AbstractNum>();
                    foreach (NumberingInstance num in nums)
                    {
                        if (num.NumberID == numberingId)
                        {
                            Int32 abstractNumId1 = num.AbstractNumId.Val;
                            foreach (AbstractNum abstractNum in abstractNums)
                            {
                                if (abstractNum.AbstractNumberId == abstractNumId1)
                                {
                                    Level level = abstractNum.GetFirstChild<Level>();
                                    if (level.GetFirstChild<NumberingFormat>().Val != "decimal")
                                    {
                                        Util.printError("正文段落项目编号错误，应为一对圆括号，内为一个阿拉伯数字：" + curTitle + "----" + (paratext.Length < 10 ? paratext : (paratext.Substring(0, 10) + "...")));
                                    }
                                    else if (level.GetFirstChild<LevelText>().Val.InnerText != "（%1）" && level.GetFirstChild<LevelText>().Val.InnerText != "(%1)")
                                    {
                                        Util.printError("正文段落项目编号错误，应为一对圆括号，内为一个阿拉伯数字：" + curTitle + "----" + (paratext.Length < 10 ? paratext : (paratext.Substring(0, 10) + "...")));
                                    }
                                }
                            }
                        }
                    }
                }
            }
            if (runList != null && p.ParagraphProperties != null)
            {
                Match m = Regex.Match(Util.getFullText(p), @"\s+\S");
                if (!Util.correctIndentation(p, doc, tText[0]))
                {
                    if (Util.getFullText(p).IndexOf("    ") == 0)
                    {
                        m = Regex.Match(Util.getFullText(p).Substring(4), @"\s+\S");
                        if (m.Success && m.Index == 0 && !isProTitle)
                            Util.printError("正文段落前存在多余空格：" + curTitle + "----" + (paratext.Length < 10 ? paratext : (paratext.Substring(0, 10) + "...")));
                    }
                    else
                        Util.printError("正文段落缩进错误，应为首行缩进" + tText[0] + "字符：" + curTitle + "----" + (paratext.Length < 10 ? paratext : (paratext.Substring(0, 10) + "...")));
                }
                else
                {
                    if (m.Success && m.Index == 0 && !isProTitle)
                        Util.printError("正文段落前存在多余空格：" + curTitle + "----" + (paratext.Length < 10 ? paratext : (paratext.Substring(0, 10) + "...")));
                }
                if (!Util.correctfonts(p, doc, tText[1], "Times New Roman"))
                    Util.printError("正文段落字体错误，应为" + tText[1] + "：" + curTitle + "----" + (paratext.Length < 10 ? paratext : (paratext.Substring(0, 10) + "...")));
                if (!Util.correctsize(p, doc, tText[2]))
                {
                    if (isParagraphBreak)
                    {
                        isParagraphBreak = false;
                    }
                    else
                    {
                        if (isProTitle)
                            Util.printError("正文段落字号错误，应为" + tText[2] + "：" + curTitle + "----" + (paratext.Length < 10 ? paratext : (paratext.Substring(0, 10) + "...")));
                        else
                        {
                            m = Regex.Match(paratext.Substring(paratext.Length - 1, 1), @"[，、“；0-9a-zA-Z\u4E00-\u9FA5]");
                            if (m.Success)
                            {
                                isParagraphBreak = false;
                                Util.printError("正文段落字号错误，应为" + tText[2] + "：" + curTitle + "----" + (paratext.Length < 10 ? paratext : (paratext.Substring(0, 10) + "...")));
                            }
                            else isParagraphBreak = true;
                        }
                    }
                }
                //if (!Util.correctJustification(p, doc, tText[3]) && !Util.correctJustification(p, doc, "两端对齐"))
                    //Util.printError("正文段落未" + tText[3] + "：" + curTitle + "----" + (paratext.Length < 10 ? paratext : (paratext.Substring(0, 10) + "...")));
                if (!Util.correctSpacingBetweenLines_Be(p, doc, tText[4]))
                    Util.printError("正文段落段前距应为" + Convert.ToDouble(tText[4]) / 240 + "行：" + curTitle + "----" + (paratext.Length < 10 ? paratext : (paratext.Substring(0, 10) + "...")));
                if (!Util.correctSpacingBetweenLines_Af(p, doc, tText[5]))
                    Util.printError("正文段落段后距应为" + Convert.ToDouble(tText[5]) / 240 + "行：" + curTitle + "----" + (paratext.Length < 10 ? paratext : (paratext.Substring(0, 10) + "...")));
                if (!Util.correctSpacingBetweenLines_line(p, doc, tText[6]))
                    Util.printError("正文段落行间距应为" + Convert.ToDouble(tText[6]) / 240 + "倍行距：" + curTitle + "----" + (paratext.Length < 10 ? paratext : (paratext.Substring(0, 10) + "...")));
                if (!Util.correctBold(p, doc, Convert.ToBoolean(tText[7])))
                    Util.printError("正文段落" + (Convert.ToBoolean(tText[7])? "需要":"不需") + "加粗：" + curTitle + "----" + (paratext.Length < 10 ? paratext : (paratext.Substring(0, 10) + "...")));

            }
        }
        public static List<string> getContent(WordprocessingDocument doc)
        {
            Body body = doc.MainDocumentPart.Document.Body;
            IEnumerable<Paragraph> para = body.Elements<Paragraph>();
            List<string> list = new List<string>();
            bool begin = false;

            foreach (Paragraph p in para)
            {
                Run r = p.GetFirstChild<Run>();
                Hyperlink h = p.GetFirstChild<Hyperlink>();
                FieldChar f = null;
                String paratext = "";
                if (r != null)
                {
                    paratext = Util.getFullText(p).Trim();
                    f = r.GetFirstChild<FieldChar>();
                }
                else if (h != null)
                {
                    paratext = Util.getFullText(p);
                }
                else
                {
                    continue;
                }
                if (paratext.Replace(" ", "") == "目录")
                {
                    begin = true;
                    continue;
                }
                if (begin)
                {
                    if (f == null && h == null)
                    {
                        return list;
                    }
                    else
                    {
                        list.Add(paratext);
                    }
                }
            }
            return list;
        }
        //判断是否为代码，用于正文中的代码过滤
        public static bool isCode(string str)
        {
            Match m = Regex.Match(str, @"[\u4E00-\u9FA5]");
            if(!m.Success)
                return true;
            else
            {
                m = Regex.Match(str, @NotesPattern[0]);
                if (m.Success && m.Index == 0)
                    return true;
                for(int i = 1; i < NotesPattern.Length; i++)
                {
                    m = Regex.Match(str, @NotesPattern[i]);
                    if (m.Success)
                        return true;
                }
            }
            return false;
        }
        public static bool isTitle(String str, List<String> content)
        {
            Match m = Regex.Match(str, "[第][一二三四五六七八九十][章]");
            string title = str;
            if (m.Success)
                title = title.Substring(m.Length);

            for (int i = 0; i < ProNumbering.Length; i++)
            {
                m = Regex.Match(title, ProNumbering[i]);
                if (m.Success && m.Index == 0) return false;
            }

            m = Regex.Match(title, @"[a-zA-Z\u4E00-\u9FA5]");
            if (m.Success)
                title = title.Substring(m.Index);
            else
                return false;

            foreach (String s in content)
            {
                if (s.Contains(title))
                {
                    return true;
                }
            }
            return false;
        }
    }
}
