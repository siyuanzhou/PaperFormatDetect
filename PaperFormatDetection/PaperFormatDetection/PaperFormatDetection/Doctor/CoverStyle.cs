using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using PaperFormatDetection.Tools;
using System.Xml;

namespace PaperFormatDetection.Doctor
{
    class covUtil
    {
        public static List<Paragraph> CovSectionLoction(WordprocessingDocument doc, int paperType)
        {
            string[] Undergraduate = new string[] { "摘要", "Abstract", "目录", "引言", "正文", "结论", "参考文献", "附录", "致谢" };
            string[] Master = new string[] { "大连理工大学学位论文独创性声明", "摘要", "Abstract", "目录", "引言", "正文", "结论",
                                             "参考文献", "附录", "攻读硕士学位期间发表学术论文情况", "致谢", "大连理工大学学位论文版权使用授权书" };
            string[] Doctor = new string[] { "大连理工大学学位论文独创性声明", "大连理工大学学位论文版权使用授权书", "摘要", "ABSTRACT", "目录", "TABLE OF CONTENTS",
                                             "图目录","表目录","主要符号表","正文", "参考文献", "附录", "攻读博士学位期间科研项目及科研成果", "致谢", "作者简介" };
            string[][] type = new string[][] { Undergraduate, Master, Doctor };
            Body body = doc.MainDocumentPart.Document.Body;
            IEnumerable<Paragraph> paras = body.Elements<Paragraph>();
            List<Paragraph> list = new List<Paragraph>();
            Boolean begin = false;
            Boolean end = false;
            foreach (Paragraph p in paras)
            {
                String fullText = Util.getFullRunText(p);
                begin = true;
                for (int i = 0; i < type[paperType].Length; i++)
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

namespace PaperFormatDetection.Doctor
{
    class DocCoverStyle
    {
        /// <summary>
        /// 未避免变量过多，将同一部分变量放在一个数组里面 注意变量在数组中的顺序 
        /// 这里包含大标题，论文中文题目， 论文英文题目，学生信息，中文LOGO，英文LOGO五个数组
        /// 大标题9个参数 顺序分别是 0该部分是否必要 1标题之间空格数目 2对齐方式 3字体 4字体大小 5是否需要加粗 6段间距 7段前距 8段后距
        /// 论文中文题目8个参数 顺序分别是 0最大字数 1对齐方式 2字体 3是否需要加粗 4字体大小 5段间距 6段前距 7段后距
        /// 论文英文题目7个参数 顺序分别是 0居中方式 1字体 2是否需要加粗 3字体大小 4段间距 5段前距 6段后距
        /// 学生信息8个参数 顺序分别是 0最大字数 1字体 2居中方式 3字体大小 4段间距 5段前距 6段后距
        /// 中文LOGO7个参数 顺序分别是 0字体 1居中方式 2字体大小 3段间距 4段前距 5段后距
        /// 英文LOGO7个参数 顺序分别是 0字体 1居中方式 2字体大小 3段间距 4段前距 5段后距
        /// 若以后需要新增属性一定注意文件中各属性的顺序 因为为了方便 后面初始化赋值就按照这个顺序赋值 这相当于一个约定
        /// </summary>
        protected string[] coverstyleHeadline = new string[9];
        protected string[] coverstyleSubtitleCh = new string[8];
        protected string[] coverstyleSubtitleEn = new string[7];
        protected string[] coverstyleStuInformation = new string[7];
        protected string[] coverstyleSchoolNameCh = new string[6];
        protected string[] coverstyleSchoolNameEn = new string[6];

        public DocCoverStyle()
        {

        }
        //论文封面检测
        public void detectCoverStyle(List<Paragraph> list, WordprocessingDocument doc)
        {
            Tools.Util.printError("论文封面检测");
            Util.printError("----------------------------------------------");
            if (list.Count <= 0)
            {
                if (bool.Parse(coverstyleHeadline[0]))
                    Util.printError("论文缺少封面部分");
            }
            else
            {
                int i = -1;
                //封面大标题
                while (true)
                {
                    i++;
                    if (Util.getFullText(list[i]).Trim().Length != 0) break;
                }
                string CovHeadline = Util.getFullText(list[i]).Replace(" ", "");
                if(CovHeadline != "博士学位论文")
                        Util.printError("论文封面大标题有误，应为“博士学位论文”");
                if (!(Util.getFullText(list[i]).Trim().Length - 6 == int.Parse(coverstyleHeadline[1]))) Util.printError("封面大标题中间不应多余包含空格且不应该缺少字符");
                if (!Util.correctJustification(list[i], doc, coverstyleHeadline[2])) Util.printError("封面大标题对齐方式错误，应为" + coverstyleHeadline[2]);
                if (!Util.correctSpacingBetweenLines_line(list[i], doc, coverstyleHeadline[6])) Util.printError("封面大标题行间距错误，应为" + Util.DSmap[coverstyleHeadline[3]]);
                if (!Util.correctSpacingBetweenLines_Be(list[i], doc, coverstyleHeadline[7])) Util.printError("封面大标题段前间距错误，应为0行");
                if (!Util.correctSpacingBetweenLines_Af(list[i], doc, coverstyleHeadline[8])) Util.printError("封面大标题段后间距错误，应为0行");
                if (!Util.correctfonts(list[i], doc, coverstyleHeadline[3], "Times New Roman")) Util.printError("封面大标题字体错误，应为" + coverstyleHeadline[3]);
                if (!Util.correctsize(list[i], doc, coverstyleHeadline[4])) Util.printError("封面大标题字号错误，应为" + coverstyleHeadline[4]);
                if (!Util.correctBold(list[i], doc, bool.Parse(coverstyleHeadline[5])))
                {
                    if (bool.Parse(coverstyleHeadline[5]))
                        Util.printError("封面大标题未加粗");
                    else
                        Util.printError("封面大标题不需加粗");
                }

                //封面中文小标题
                while (true)
                {
                    i++;
                    if (Util.getFullText(list[i]).Trim().Length != 0) break;
                }
                string SubChText = Util.getFullText(list[i]).Replace(" ", "").Trim();
                int SubChWordNum = Util.getFullText(list[i]).Replace(" ", "").Trim().Length;
                for (int j = 0; j < Util.getFullText(list[i]).Replace(" ", "").Trim().Length; j++)
                    if ((SubChText[j] >= 48 && SubChText[j] <= 57) || (SubChText[j] >= 65 && SubChText[j] <= 90) || (SubChText[j] >= 97 && SubChText[j] <= 122))
                    {
                        int k = 0;
                        int m = j;
                        for (; ; m++)
                        {
                            if (!((SubChText[m] >= 48 && SubChText[m] <= 57) || (SubChText[m] >= 65 && SubChText[m] <= 90) || (SubChText[m] >= 97 && SubChText[m] <= 122))) break;
                            k++;
                        }
                        SubChWordNum = SubChWordNum - k + 1;
                        j = m;
                    }
                if (SubChWordNum > int.Parse(coverstyleSubtitleCh[0])) Util.printError("封面中文小标题字数超过20个");
                if (!Util.correctJustification(list[i], doc, coverstyleSubtitleCh[1])) Util.printError("封面中文小标题对齐方式错误，应为" + coverstyleSubtitleCh[1]);
                if (!Util.correctSpacingBetweenLines_line(list[i], doc, coverstyleSubtitleCh[5])) Util.printError("封面中文小标题行间距错误，应为" + Util.DSmap[coverstyleSubtitleCh[5]]);
                if (!Util.correctSpacingBetweenLines_Be(list[i], doc, coverstyleSubtitleCh[6])) Util.printError("封面中文小标题段前间距错误，应为0行");
                if (!Util.correctSpacingBetweenLines_Af(list[i], doc, coverstyleSubtitleCh[7])) Util.printError("封面中文小标题段后间距错误，应为0行");
                if (!Util.correctfonts(list[i], doc, coverstyleSubtitleCh[2], "Times New Roman")) Util.printError("封面中文小标题字体错误，应为" + coverstyleSubtitleCh[2]);
                if (!Util.correctsize(list[i], doc, coverstyleSubtitleCh[4])) Util.printError("封面中文小标题字号错误，应为" + coverstyleSubtitleCh[4]);
                if (!Util.correctBold(list[i], doc, bool.Parse(coverstyleSubtitleCh[3])))
                {
                    if (bool.Parse(coverstyleSubtitleCh[3]))
                        Util.printError("封面中文小标题未加粗");
                    else
                        Util.printError("封面中文小标题不需加粗");
                }

                //检测封面中文小标题之间是否有多余转行
                while (true)
                {
                    i++;
                    if (Util.getFullText(list[i]).Trim().Length != 0) break;
                }
                string CNorEn_SubtitleText = Util.getFullText(list[i]).Replace(" ", "");
                if (!((CNorEn_SubtitleText[0] >= 65 && CNorEn_SubtitleText[0] <= 90) || (CNorEn_SubtitleText[0] >= 97 && CNorEn_SubtitleText[0] <= 122)))
                    Util.printError("封面中文小标题中间不应该有转行");

                //封面英文小标题
                string EnSubtitleString;
                while (true)
                {
                    EnSubtitleString = Util.getFullText(list[i]).Replace(" ", "");
                    if ((EnSubtitleString[0] >= 65 && EnSubtitleString[0] <= 90) || (EnSubtitleString[0] >= 97 && EnSubtitleString[0] <= 122)) break;
                    i++;
                }
                if (!Util.correctJustification(list[i], doc, coverstyleSubtitleEn[0])) Util.printError("封面英文小标题对齐方式错误，应为" + coverstyleSubtitleEn[0]);
                if (!Util.correctSpacingBetweenLines_line(list[i], doc, coverstyleSubtitleEn[4])) Util.printError("封面英文小标题行间距错误，应为" + Util.DSmap[coverstyleSubtitleEn[4]]);
                if (!Util.correctSpacingBetweenLines_Be(list[i], doc, coverstyleSubtitleEn[5])) Util.printError("封面英文小标题段前间距错误，应为0行");
                if (!Util.correctSpacingBetweenLines_Af(list[i], doc, coverstyleSubtitleEn[6])) Util.printError("封面英文小标题段后间距错误，应为0行");
                if (!Util.correctfonts(list[i], doc, "Times New Roman", coverstyleSubtitleEn[1])) Util.printError("封面英文小标题字体错误，应为" + coverstyleSubtitleEn[1]); 
                if (!Util.correctsize(list[i], doc, coverstyleSubtitleEn[3])) Util.printError("封面英文小标题字号错误，应为" + coverstyleSubtitleEn[3]);
                if (!Util.correctBold(list[i], doc, bool.Parse(coverstyleSubtitleEn[2])))
                {
                    if (bool.Parse(coverstyleSubtitleEn[2]))
                        Util.printError("封面英文小标题未加粗");
                    else
                        Util.printError("封面英文小标题不需加粗");
                }

                //英文小标题字母大小写的检测
                string[] CapitalEmptyWord = { "A", "Above", "An", "As", "Behind", "But", "Before", "If", "The", "At", "For", "From", "Of", "Off", "On", "To", "In", "With", "And", "As", "While", "So" };
                string[] SmallEmptyWord = { "a", "above", "an", "as", "behind", "but", "before", "if", "the", "at", "for", "from", "of", "off", "on", "to", "in", "with", "and", "as", "while", "so" };
                string SubtitleEnText = Util.getFullText(list[i]);
                int SubtitleEnTextLength = Util.getFullText(list[i]).Trim().Length;
                if (!(SubtitleEnText[0] >= 65 && SubtitleEnText[0] <= 90)) Util.printError("论文英文题目句首的单词首字母未大写");
                int LastSpaceInText = SubtitleEnText.LastIndexOf(' ');
                int NumInText = 0;
                bool IsHaveEmptyWord = false;
                for (; NumInText < SubtitleEnTextLength; NumInText++)
                {
                    if (SubtitleEnText[NumInText] == ' ' && NumInText < LastSpaceInText)
                    {
                        string Word = "";
                        bool IsWordEmpty = true;
                        bool WordFlag = true;
                        int a = SubtitleEnText.IndexOf(' ', NumInText + 1);
                        Word = SubtitleEnText.Substring(NumInText + 1, a - NumInText - 1);
                        if (a - NumInText - 1 > 0)
                            IsWordEmpty = false;
                        else
                            IsHaveEmptyWord = true;
                        foreach (string S in CapitalEmptyWord)
                        {
                            if (Word == S) Util.printError("论论文英文题目非句首虚词首字母不应大写" + "  ----" + Word);
                        }
                        foreach (string C in SmallEmptyWord)
                        {
                            if (Word == C) WordFlag = false;
                        }
                        if (WordFlag && !IsWordEmpty)
                        {
                            if (!(Word[0] >= 65 && Word[0] <= 90) && Word.IndexOf("(") == -1) Util.printError("论文英文标题实词首字母未大写" + "  ----" + Word);
                        }
                    }
                    if (SubtitleEnText[NumInText] == ' ' && NumInText == LastSpaceInText)
                    {
                        bool WordFlag = true;
                        string Word = SubtitleEnText.Substring(NumInText + 1);
                        foreach (string S in CapitalEmptyWord)
                        {
                            if (Word == S) Util.printError("论论文英文题目非句首虚词首字母不应大写" + "  ----" + Word);
                        }
                        foreach (string C in SmallEmptyWord)
                        {
                            if (Word == C) WordFlag = false;
                        }
                        if (WordFlag)
                        {
                            if (Word[0] != '\0')
                                if (!(Word[0] >= 65 && Word[0] <= 90) && Word.IndexOf("(") == -1) Util.printError("论文英文标题实词首字母未大写" + "  ----" + Word);
                        }
                    }
                }
                if (IsHaveEmptyWord) Util.printError("论文英文题目里单词间含有多余的空格");

                //检测封面英文小标题之间是否有多余转行
                while (true)
                {
                    i++;
                    if (Util.getFullText(list[i]).Trim().Length != 0) break;
                }
                string SubOrStuInformationText = Util.getFullText(list[i]).Replace(" ", "");
                if ((SubOrStuInformationText[0] >= 65 && SubOrStuInformationText[0] <= 90) || (SubOrStuInformationText[0] >= 97 && SubOrStuInformationText[0] <= 122))
                    Util.printError("封面英文小标题中间不应该有转行");

                //封面学生信息
                string StuInformationString;
                int NumLost = 0;
                while (true)
                {
                    StuInformationString = Util.getFullText(list[i]).Replace(" ", "");
                    if (Util.getFullText(list[i]).Trim().Length != 0 && !((StuInformationString[0] >= 65 && StuInformationString[0] <= 90) || (StuInformationString[0] >= 97 && StuInformationString[0] <= 122))) break;
                    i++;
                }
                int FirstStuInformation = i;
                int StuInformationLine = 0;
                string[] EachStuInformation = new string[] { "作者姓名", "学号", "指导教师", "学科、 专业", "答辩日期" };
                for (; i <= FirstStuInformation + 4; i++)
                {
                    StuInformationLine++;
                    StuInformationString = Util.getFullText(list[i]).Replace(" ", "");
                    if (Util.getFullText(list[i]).Trim().Length != 0)
                    {
                        if (StuInformationString.IndexOf(EachStuInformation[i - FirstStuInformation]) == -1)
                            Util.printError("第" + StuInformationLine + "学生信息应为:" + EachStuInformation[i - FirstStuInformation]);
                        if (!Util.correctJustification(list[i], doc, coverstyleStuInformation[2])) Util.printError("第" + StuInformationLine + "行学生信息" + "(" + StuInformationString + ")" + "对齐方式错误，应为" + coverstyleStuInformation[2]);
                        if (!Util.correctSpacingBetweenLines_line(list[i], doc, coverstyleStuInformation[4])) Util.printError("第" + StuInformationLine + "行学生信息" + "(" + StuInformationString + ")" + "行间距错误，应为" + Util.DSmap[coverstyleStuInformation[4]]);
                        if (!Util.correctSpacingBetweenLines_Be(list[i], doc, coverstyleStuInformation[5])) Util.printError("第" + StuInformationLine + "行学生信息" + "(" + StuInformationString + ")" + "段前间距错误，应为0行");
                        if (!Util.correctSpacingBetweenLines_Af(list[i], doc, coverstyleStuInformation[6])) Util.printError("第" + StuInformationLine + "行学生信息" + "(" + StuInformationString + ")" + "段后间距错误，应为0行");
                        if (!Util.correctfonts(list[i], doc, coverstyleStuInformation[1], "Times New Roman")) Util.printError("第" + StuInformationLine + "行学生信息" + "(" + StuInformationString + ")" + "字体错误，应为" + coverstyleStuInformation[1]);
                        if (!Util.correctsize(list[i], doc, coverstyleStuInformation[3])) Util.printError("第" + StuInformationLine + "行学生信息" + "(" + StuInformationString + ")" + "字号错误，应为" + coverstyleStuInformation[3]);
                    }
                    else NumLost++;
                }
                if (NumLost != 0) Util.printError("学生信息有" + NumLost + "行缺省");

                //封面中文学校名
                while (true)
                {
                    i++;
                    if (Util.getFullText(list[i]).Trim().Length != 0) break;
                }
                if (!Util.correctJustification(list[i], doc, coverstyleSchoolNameCh[1])) Util.printError("封面中文学校名对齐方式错误，应为" + coverstyleSchoolNameCh[1]);
                if (!Util.correctSpacingBetweenLines_line(list[i], doc, coverstyleSchoolNameCh[3])) Util.printError("封面中文学校名行间距错误，应为" + Util.DSmap[coverstyleSchoolNameCh[3]]);
                if (!Util.correctSpacingBetweenLines_Be(list[i], doc, coverstyleSchoolNameCh[4])) Util.printError("封面中文学校名段前间距错误，应为0行");
                if (!Util.correctSpacingBetweenLines_Af(list[i], doc, coverstyleSchoolNameCh[5])) Util.printError("封面中文学校名段后间距错误，应为0行");
                if (!Util.correctfonts(list[i], doc, coverstyleSchoolNameCh[0], "Times New Roman")) Util.printError("封面中文学校名字体错误，应为" + coverstyleSchoolNameCh[0]);
                if (!Util.correctsize(list[i], doc, coverstyleSchoolNameCh[2])) Util.printError("封面中文学校名字号错误，应为" + coverstyleSchoolNameCh[2]);

                //封面英文学校名
                while (true)
                {
                    i++;
                    if (Util.getFullText(list[i]).Trim().Length != 0) break;
                }
                if (!Util.correctJustification(list[i], doc, coverstyleSchoolNameEn[1])) Util.printError("封面英文学校名对齐方式错误，应为" + coverstyleSchoolNameEn[1]);
                if (!Util.correctSpacingBetweenLines_line(list[i], doc, coverstyleSchoolNameEn[3])) Util.printError("封面英文学校名行间距错误，应为" + Util.DSmap[coverstyleSchoolNameEn[3]]);
                if (!Util.correctSpacingBetweenLines_Be(list[i], doc, coverstyleSchoolNameEn[4])) Util.printError("封面英文学校名段前间距错误，应为0行");
                if (!Util.correctSpacingBetweenLines_Af(list[i], doc, coverstyleSchoolNameEn[5])) Util.printError("封面英文学校名段后间距错误，应为0行");
                if (!Util.correctfonts(list[i], doc, coverstyleSchoolNameEn[0], "Times New Roman")) Util.printError("封面英文学校名字体错误，应为" + coverstyleSchoolNameEn[0]);
                if (!Util.correctsize(list[i], doc, coverstyleSchoolNameEn[2])) Util.printError("封面英文学校名字号错误，应为" + coverstyleSchoolNameEn[2]);
            }
            Console.WriteLine("----------------------------------------------");
        }
    }
}

namespace PaperFormatDetection.Doctor
{
    class CoverStyle : Master.MasCoverStyle
    {
        public CoverStyle(WordprocessingDocument doc)
        {
            Init();
            detectCoverStyle(covUtil.CovSectionLoction(doc, 2), doc);
        }
        /// <summary>
        /// 从XML文件给数组变量赋值 一定注意数组与XML文件是否一致对应
        /// </summary>
        public void Init()
        {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(@"./Template/Doctor/Coverstyle.xml");
            int m = 0;
            //封面大标题
            XmlNodeList covHeadlineNode = xmlDoc.SelectSingleNode("Root").SelectSingleNode("Coverstyle").SelectSingleNode("Headline").ChildNodes;
            m = 0;
            foreach (XmlNode node in covHeadlineNode)
            {
                this.coverstyleHeadline[m] = node.InnerText; m++;
            }
            //封面中文小标题
            XmlNodeList covSubChNode = xmlDoc.SelectSingleNode("Root").SelectSingleNode("Coverstyle").SelectSingleNode("SubtitleCh").ChildNodes;
            m = 0;
            foreach (XmlNode node in covSubChNode)
            {
                this.coverstyleSubtitleCh[m] = node.InnerText; m++;
            }
            //封面英文小标题
            XmlNodeList covSubEnNode = xmlDoc.SelectSingleNode("Root").SelectSingleNode("Coverstyle").SelectSingleNode("SubtitleEn").ChildNodes;
            m = 0;
            foreach (XmlNode node in covSubEnNode)
            {
                this.coverstyleSubtitleEn[m] = node.InnerText; m++;
            }
            //封面学生信息
            XmlNodeList covStuInfNode = xmlDoc.SelectSingleNode("Root").SelectSingleNode("Coverstyle").SelectSingleNode("StuInformation").ChildNodes;
            m = 0;
            foreach (XmlNode node in covStuInfNode)
            {
                this.coverstyleStuInformation[m] = node.InnerText; m++;
            }
            //封面中文学校名
            XmlNodeList covSchNamChNode = xmlDoc.SelectSingleNode("Root").SelectSingleNode("Coverstyle").SelectSingleNode("SchoolNameCh").ChildNodes;
            m = 0;
            foreach (XmlNode node in covSchNamChNode)
            {
                this.coverstyleSchoolNameCh[m] = node.InnerText; m++;
            }
            //封面英文学校名
            XmlNodeList covSchNamEnNode = xmlDoc.SelectSingleNode("Root").SelectSingleNode("Coverstyle").SelectSingleNode("SchoolNameEn").ChildNodes;
            m = 0;
            foreach (XmlNode node in covSchNamEnNode)
            {
                this.coverstyleSchoolNameEn[m] = node.InnerText; m++;
            }
        }
    }
}
