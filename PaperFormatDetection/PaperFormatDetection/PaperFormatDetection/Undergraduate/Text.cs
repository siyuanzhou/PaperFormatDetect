using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using DocumentFormat.OpenXml.Wordprocessing;
using DocumentFormat.OpenXml.Packaging;
using PaperFormatDetection.Tools;

namespace PaperFormatDetection.Undergraduate
{

    class Text
    {
        /// <summary>
        /// tTitle数组为正文中标题的属性
        /// tfulltext数组为正文中段落的属性
        /// 标题6个参数 顺序分别是 0字体 1字号 2对齐方式 3段前间距 4断后间距 5行间距
        /// 段落7个参数 顺序分别是 0缩进 1字体 2字号 3对齐方式 4段前间距 5断后间距 6行间距
        /// </summary>
        protected string[,] tTitle = new string[3, 6];
        protected string[,] show = new string[3, 6];
        protected string[] tText = new string[7];
        bool ChineseNumbering = false;
        bool EnglishNumbering = false;
        string proTwo = "①②③④⑤⑥⑦⑧⑨⑩";
        string one_level = "1",
               two_level = "1.1",
               three_level = "1.1.1";

        public Text()
        {

        }

        public void detectAllText(List<Paragraph> list, WordprocessingDocument doc)
        {
            //记录当前标题文本
            string curTitle = null;
            //记录当前段落文本
            string paratext = null;
            List<string> content = getContent(doc);
            foreach (Paragraph p in list)
            {
                paratext = Tool.getFullText(p).Trim();
                if (isTitle(paratext, content))
                {
                    Match m = Regex.Match(paratext, @"\S");
                    if (m.Success)
                    {
                        curTitle = paratext;
                        detectTitle(p, doc);
                    }
                }
                //过滤公式
                else if (p.GetFirstChild<DocumentFormat.OpenXml.Math.OfficeMath>() != null)
                {

                }
                else
                {
                    Match match;
                    //过滤中文图名表名
                    if (paratext.Length > 7)
                    {
                        match = Regex.Match(paratext.Substring(0, 6), @"[\u4E00-\u9FA5]\ *[1-9][0-9]*\.*\-*[1-9][0-9]*");
                        if (match.Success) continue;
                    }
                    //过滤英文表名
                    match = Regex.Match(paratext, @"Tab\.*\ *[1-9][0-9]*\.[1-9][0-9]*");
                    if (match.Success) continue;
                    //过滤英文图名
                    match = Regex.Match(paratext, @"Fig\.*\ *[1-9][0-9]*\.[1-9][0-9]*");
                    if (match.Success) continue;
                    //过滤代码，代码格式必须正确
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
            reg[0] = new Regex(@"[1-9][0-9]*");
            //二级标题
            reg[1] = new Regex(@"[1-9][0-9]*\.[1-9][0-9]*");
            //三级标题
            reg[2] = new Regex(@"[1-9][0-9]*\.[1-9][0-9]*\.[1-9][0-9]*");

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
                    if (title.IndexOf("  ") < 0)
                    {
                        Util.printError("该标题序号后缺少两个空格----" + title);
                    }
                    else if (Util.getSubstrCount(title.Substring(0, m.Value.Length + 3), " ") > 2)
                    {
                        Util.printError("该标题序号后多于两个空格----" + title);
                    }

                    order = m.Value;
                    if (Util.getSubstrCount(order, ".") == 0)
                    {
                        if (order != one_level)
                        {
                            Util.printError("该标题序号错误，序号编号应为" + one_level + "----" + title);
                        }
                        two_level = one_level + ".1";
                        one_level = (Convert.ToInt32(one_level) + 1).ToString();
                    }
                    else if (Util.getSubstrCount(order, ".") == 1)
                    {
                        if (order != two_level)
                        {
                            Util.printError("该标题序号错误，序号编号应为" + two_level + "----" + title);
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
                            Util.printError("该标题序号错误，序号编号应为" + three_level + "----" + title);
                        }
                        int i = three_level.LastIndexOf(".");
                        //三级标题加一
                        three_level = three_level.Substring(0, i + 1) + (Convert.ToInt32(three_level.Substring(i + 1, three_level.Length - i - 1)) + 1).ToString();
                    }
                    else
                    {
                        Util.printError("该标题序号错误，不能超过三级标题" + "----" + title);
                    }
                }
                else
                {
                    return;
                }
                /* index = 0 对应一级标题
                    index = 1 对应二级标题
                    index = 2 对应三级标题 */
                if (!Util.correctfonts(p, doc, tTitle[index, 0], "Cambria")) Util.printError("该标题字体错误，应为" + show[index, 0] + "----" + title);
                if (!Util.correctsize(p, doc, tTitle[index, 1])) Util.printError("该标题字号错误，应为" + show[index, 1] + "----" + title);
                if (!Util.correctJustification(p, doc, tTitle[index, 2]) && !Util.correctJustification(p, doc, "两端对齐")) Util.printError("该标题位置错误，应为" + show[index, 2] + "----" + title);
                if (!Util.correctSpacingBetweenLines_Be(p, doc, tTitle[index, 3])) Util.printError("该标题段前间距错误，应为" + show[index, 3] + "----" + title);
                if (!Util.correctSpacingBetweenLines_Af(p, doc, tTitle[index, 4])) Util.printError("该标题段后间距错误，应为" + show[index, 4] + "----" + title);
                if (!Util.correctSpacingBetweenLines_line(p, doc, tTitle[index, 5])) Util.printError("该标题行距错误，应为" + show[index, 5] + "----" + title);
            }
            else
            {
                if (p.ParagraphProperties != null)
                {
                    if (p.ParagraphProperties.NumberingProperties == null)
                    {
                        if (title.Length != 0)
                            Util.printError("标题序号应为阿拉伯数字----" + title);
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
                                            Util.printError("标题序号应为阿拉伯数字----" + title);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                else
                {
                    Util.printError("标题序号应为阿拉伯数字----" + title);
                }
            }
        }

        public void detectText(Paragraph p, string curTitle, WordprocessingDocument doc)
        {
            IEnumerable<Run> runList = p.Elements<Run>();
            string paratext = p.InnerText.Trim();
            //手动输入的项目符号检测
            if (paratext.Length > 3)
            {
                int position;
                if (paratext.Substring(0, 4).IndexOf("）") > 0)
                {
                    ChineseNumbering = true;
                    if (EnglishNumbering)
                    {
                        Util.printError("一级项目标题圆括号格式不统一：" + curTitle + "----" + (paratext.Length < 10 ? paratext : (paratext.Substring(0, 10) + "...")));
                    }
                    position = paratext.Substring(0, 4).IndexOf("）");
                    if (paratext.Substring(0, 4).IndexOf("（") < 0)
                    {
                        Util.printError("一级项目编号必须是一对圆括号，内为一个阿拉伯数字：" + curTitle + "----" + (paratext.Length < 10 ? paratext : (paratext.Substring(0, 10) + "...")));
                    }
                    else
                    {
                        Match match = Regex.Match(paratext.Substring(0, position), @"[1-9][0-9]*");
                        if (!match.Success)
                        {
                            Util.printError("一级项目编号必须是一对圆括号，内为一个阿拉伯数字：" + curTitle + "----" + (paratext.Length < 10 ? paratext : (paratext.Substring(0, 10) + "...")));
                        }
                    }
                }
                else if (paratext.Substring(0, 4).IndexOf(")") > 0)
                {
                    EnglishNumbering = true;
                    if (ChineseNumbering)
                    {
                        Util.printError("一级项目标题圆括号格式不统一：" + curTitle + "----" + (paratext.Length < 10 ? paratext : (paratext.Substring(0, 10) + "...")));
                    }
                    position = paratext.IndexOf(")");
                    if (position > 0)
                    {
                        if (position < 4)
                        {
                            Match match = Regex.Match(paratext.Substring(0, position), @"[1-9][0-9]*");
                            if (!match.Success)
                            {
                                Util.printError("一级项目编号必须是一对圆括号，内为一个阿拉伯数字：" + curTitle + "----" + (paratext.Length < 10 ? paratext : (paratext.Substring(0, 10) + "...")));
                            }
                        }
                    }
                    else
                    {
                        Util.printError("一级项目编号必须是一对圆括号，内为一个阿拉伯数字：" + curTitle + "----" + (paratext.Length < 10 ? paratext : (paratext.Substring(0, 10) + "...")));
                    }
                }
            }
            //自动生成的项目符号检测
            if (p.ParagraphProperties != null)
            {
                if (p.ParagraphProperties.NumberingProperties != null)
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
                                        Util.printError("一级项目编号必须是一对圆括号，内为一个阿拉伯数字：" + curTitle + "----" + (paratext.Length < 10 ? paratext : (paratext.Substring(0, 10) + "...")));
                                    }
                                    else if (level.GetFirstChild<LevelText>().Val.InnerText != "（%1）" || level.GetFirstChild<LevelText>().Val.InnerText != "(%1)")
                                    {
                                        Util.printError("一级项目编号必须是一对圆括号，内为一个阿拉伯数字：" + curTitle + "----" + (paratext.Length < 10 ? paratext : (paratext.Substring(0, 10) + "...")));
                                    }
                                }
                            }
                        }
                    }
                }
            }
            if (runList != null && p.ParagraphProperties != null)
            {
                if (!Util.correctIndentation(p, doc, tText[0])) Util.printError("此段落段落缩进应为2字：" + curTitle + "----" + (paratext.Length < 10 ? paratext : (paratext.Substring(0, 10) + "...")));
                if (!Util.correctfonts(p, doc, tText[1], "Times New Roman")) Util.printError("此段落存在字体错误，应为中文宋体，英文Times New Roman：" + curTitle + "----" + (paratext.Length < 10 ? paratext : (paratext.Substring(0, 10) + "...")));
                if (!Util.correctsize(p, doc, tText[2])) Util.printError("此段落存在字号错误，应为小四：" + curTitle + "----" + (paratext.Length < 10 ? paratext : (paratext.Substring(0, 10) + "...")));
                if (!Util.correctJustification(p, doc, tText[3])) Util.printError("此段落对齐方式应为两端对齐：" + curTitle + "----" + (paratext.Length < 10 ? paratext : (paratext.Substring(0, 10) + "...")));
                if (!Util.correctSpacingBetweenLines_Be(p, doc, tText[4])) Util.printError("此段落段前间距应为0行：" + curTitle + "----" + (paratext.Length < 10 ? paratext : (paratext.Substring(0, 10) + "...")));
                if (!Util.correctSpacingBetweenLines_Af(p, doc, tText[5])) Util.printError("此段落段后间距应为0行：" + curTitle + "----" + (paratext.Length < 10 ? paratext : (paratext.Substring(0, 10) + "...")));
                if (!Util.correctSpacingBetweenLines_line(p, doc, tText[6])) Util.printError("此段落行间距应为1.25倍：" + curTitle + "----" + (paratext.Length < 10 ? paratext : (paratext.Substring(0, 10) + "...")));
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
            Match match = Regex.Match(str, @"[\u4e00-\u9fa5]");
            int index = str.IndexOf("//") != -1 ? str.IndexOf("//") : str.IndexOf("/*");
            if (match.Success && index == -1)
                return false;
            return true;
        }
        public static bool isTitle(String str, List<String> content)
        {
            Match m1 = Regex.Match(str, @"[\u4E00-\u9FA5]"),
                  m2 = Regex.Match(str, @"[A-Z]");
            string tmp = null;
            if (m1.Success)
                tmp = str.Substring(m1.Index, str.Length - m1.Index);
            else if (m2.Success)
                tmp = str.Substring(m2.Index, str.Length - m2.Index);
            else
                return false;
            foreach (String s in content)
            {
                if (s.Contains(tmp))
                {
                    return true;
                }
            }
            return false;
        }
    }
}
