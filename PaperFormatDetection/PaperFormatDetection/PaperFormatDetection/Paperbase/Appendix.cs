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
    class Appendix
    {
        /// <summary>
        /// 未避免变量过多，将同一部分变量放在一个数组里面 注意变量在数组中的顺序 
        /// 这里包含附录标题，附录正文两个数组
        /// 标题9个参数 顺序分别是 0该部分是否必要 1标题之间空格数目 2对齐方式 3行间距 4段前间距 5段后间距 6字体 7字号 8是否加粗
        /// 正文7个参数 顺序分别是 0缩进 1对齐方式 2行间距 3段前间距 4段后间距 5字体 6字号
        /// 若以后需要新增属性一定注意文件中各属性的顺序 因为为了方便 后面初始化赋值就按照这个顺序赋值 这相当于一个约定
        /// </summary>
        protected string[] conTitle = new string[9];
        protected string[] conText = new string[7];
        static string[] NotesPattern = { "[-][-].*", "[/][*].*?[*][/]", "[/][/].*", "[#].*", "<!--.*?-->", "[\"].+?[\"]", "[^\u4E00-\u9FA5][-][-].*", "[\'][\u4E00-\u9FA5]*[\']" };

        public Appendix()
        {

        }

        public static bool IsCode(string str)
        {
            Match m = Regex.Match(str, @"[\u4E00-\u9FA5]");
            if (!m.Success)
                return true;
            else
            {
                m = Regex.Match(str, @NotesPattern[0]);
                if (m.Success && m.Index == 0)
                    return true;
                for (int i = 1; i < NotesPattern.Length; i++)
                {
                    m = Regex.Match(str, @NotesPattern[i]);
                    if (m.Success)
                        return true;
                }
            }
            return false;
        }
        //结论检测
        public void detectConclusion(List<Paragraph> list, WordprocessingDocument doc)
        {
            if (list.Count <= 0)
            {
                if (bool.Parse(conTitle[0]))
                    Util.printError("缺少附录部分");
            }
            else
            {
                
                char flag = 'A';
                //附录正文
                for (int i = 0; i < list.Count; i++)
                {
                    bool isCode = false;
                    string s1 = "^附录[A-Z]?";
                    string s2 = "[^0-9]$";
                    string s3 = "（续）$";
                    string temp = Tool.getFullText(list[i]);
                    string t = temp;
                    isCode = IsCode(temp);
                    temp = "     " + (temp.Length > 10 ? temp.Substring(0, 10) : temp) + "......";
                    if (t.Length < 20 && Regex.IsMatch(t.Replace(" ", ""), s1) && Regex.IsMatch(t.Replace(" ", ""), s2))
                    {
                        //附录标题
                        if (!(t[3] == ' ' && t[4] == ' ' && t[5] != ' ')) Util.printError("附录标题之间应间隔" + conTitle[1] + "个空格");
                        if (!Util.correctJustification(list[i], doc, conTitle[2])) Util.printError("附录标题对齐方式错误，应为"+conTitle[2]);
                        if (!Util.correctSpacingBetweenLines_line(list[i], doc, conTitle[3])) Util.printError("附录标题行间距错误，应为"+Util.DSmap[conTitle[3]]);
                        if (!Util.correctSpacingBetweenLines_Be(list[i], doc, conTitle[4])) Util.printError("附录标题段前间距错误，应为"+ Util.DSmap[conTitle[4]]);
                        if (!Util.correctSpacingBetweenLines_Af(list[i], doc, conTitle[5])) Util.printError("附录标题段后间距错误，应为"+ Util.DSmap[conTitle[5]]);
                        if (!Util.correctfonts(list[i], doc, conTitle[6], "Cambria")) Util.printError("附录标题字体错误，应为"+conTitle[6]);
                        if (!Util.correctsize(list[i], doc, conTitle[7])) Util.printError("附录标题字号错误，应为"+conTitle[7]);
                        if (!Util.correctBold(list[i], doc, bool.Parse(conTitle[8])))
                        {
                            if (bool.Parse(conTitle[8]))
                                Util.printError("附录标题未加粗");
                            else
                                Util.printError("附录标题不需加粗");
                        }
                        if (i == 0)
                        {
                            if (!(flag == t[2]))
                            {
                                Util.printError("附录标题字母错误，应为" + flag);

                            }
                        }
                        else
                        {
                            if (!(flag == t[2]) && Regex.IsMatch(t, s3))
                            {
                                Util.printError("附录标题字母错误，应为" + flag);
                            }
                            else if (!Regex.IsMatch(t, s3))
                            {
                                flag++;
                                if (!(flag == t[2]))
                                {
                                    Util.printError("附录标题字母错误，应为" + flag);

                                }

                            }
                            else
                            {
                                continue;
                            }
                        }
                        continue;

                    }
                    if (isCode)
                    {
                        continue;
                    }
                    if (!Util.correctIndentation(list[i], doc, conText[0])) Util.printError("附录正文段落缩进错误，应为每段落首行缩进2字符" + temp);
                    //if (Util.correctJustification(list[i], doc, conText[1])) Util.printError("结论正文段落对齐方式错误，应为两端对齐" + temp);
                    if (!Util.correctSpacingBetweenLines_line(list[i], doc, conText[2])) Util.printError("附录正文行距错误，应为多倍行距" + Util.DSmap[conTitle[2]] +"  " + temp);
                    if (!Util.correctSpacingBetweenLines_Be(list[i], doc, conText[3])) Util.printError("附录正文段前间距错误，应为" + Util.DSmap[conTitle[3]]  + temp);
                    if (!Util.correctSpacingBetweenLines_Af(list[i], doc, conText[4])) Util.printError("附录正文段后间距错误，应为" + Util.DSmap[conTitle[4]]  + temp);
                    if (!Util.correctfonts(list[i], doc, conText[5], "Times New Roman")) Util.printError("附录正文字体错误，应为"+conText[5]+"   " + temp);
                    if (!Util.correctsize(list[i], doc, conText[6])) Util.printError("附录正文字号错误，应为小四" + temp);
                }
            }
        }

        public static List<Paragraph> sectionLoction(WordprocessingDocument doc, string section, int paperType)
        {
            string[] Undergraduate = new string[] { "摘要", "Abstract", "目录", "引言", "正文", "结论", "参考文献", "附录", "致谢" };
            string[] Master = new string[] { "大连理工大学学位论文独创性声明", "摘要", "Abstract", "目录", "引言", "正文", "结论",
                                             "参考文献", "附录", "攻读硕士学位期间发表学术论文情况", "致谢", "大连理工大学学位论文版权使用授权书" };
            string[] Doctor = new string[] { "大连理工大学学位论文独创性声明", "大连理工大学学位论文版权使用授权书", "摘要", "ABSTRACT", "目录", "TABLE OF CONTENTS",
                                             "图目录","表目录","主要符号表","正文", "参考文献", "附录", "攻读博士学位期间科研项目及科研成果", "致谢", "作者简介" };
            string[][] type = new string[][] { Undergraduate, Master, Doctor };
            string s1 = "^附录[A-Z]?";
            string s2 = "[^0-9]$";
            int it = 0;
            int flag = 0;
            int index = Array.IndexOf(type[paperType], section);
            Body body = doc.MainDocumentPart.Document.Body;
            IEnumerable<Paragraph> paras = body.Elements<Paragraph>();
            List<Paragraph> list = new List<Paragraph>();

            Boolean begin = false;
            Boolean end = false;
            foreach (Paragraph p in paras)
            {
                String fullText = Util.getFullText(p);

                if (fullText.Replace(" ", "") == "目录" || it < 100 && flag == 1)
                {
                    it++;
                    flag = 1;
                    continue;
                }
                if (Regex.IsMatch(fullText.Replace(" ", ""), s1) /*&& Regex.IsMatch(fullText.Replace(" ", ""), s2)*/)
                {

                    begin = true;

                }

                for (int i = index + 1; i < type[paperType].Length; i++)
                {
                    if (fullText.Replace(" ", "").Length < 40 && fullText.Replace(" ", "").Equals(type[paperType][i]))
                    {
                        begin = false; end = true; break;
                    }
                }
                if (begin /* (fullText.Replace(" ", "").Length < 18 || Regex.IsMatch(fullText.Replace(" ", ""), s1))&&*/ )
                {
                    list.Add(p);
                }
                if (end)
                {
                    break;
                }
               // lasttext = fullText.Replace(" ", "");

            }
            return list;
        }
    }
}
