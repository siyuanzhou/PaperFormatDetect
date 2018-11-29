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
    class AuthorIntroduction
    {
        private string[] authorTitle = new string[9];
        private string[] authorText = new string[6];
        public AuthorIntroduction(WordprocessingDocument doc)
        {
            Init(@"./Template/Doctor/AuthorIntroduction.xml");
            detectAuthorIntroduction(Util.sectionLoction(doc, "作者简介", 2), doc);
        }
        public void Init(string path)
        {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(path);
            int m = 0;
            //结论标题
            XmlNodeList conTitleNode = xmlDoc.SelectSingleNode("Root").SelectSingleNode("AuthorIntroduction").SelectSingleNode("Title").ChildNodes;
            m = 0;
            foreach (XmlNode node in conTitleNode)
            {
                this.authorTitle[m] = node.InnerText; m++;
            }
            //结论正文
            XmlNodeList conTextNode = xmlDoc.SelectSingleNode("Root").SelectSingleNode("AuthorIntroduction").SelectSingleNode("Text").ChildNodes;
            m = 0;
            foreach (XmlNode node in conTextNode)
            {
                this.authorText[m] = node.InnerText; m++;
            }
        }
        public void detectAuthorIntroduction(List<Paragraph> list, WordprocessingDocument doc)
        {
            Console.WriteLine("作者简介检测");
            Console.WriteLine("---------------------------------");
            if (list.Count <= 0)
            {
                if (bool.Parse(authorTitle[0]))
                    Util.printError("论文缺少作者简介部分");
            }
            else
            {
                //作者简介标题
                if (Util.getFullText(list[0]).Trim().Length - 4 != Int16.Parse(authorTitle[1])) Util.printError("作者简介标题之间应包含" + authorTitle[1] + "个空格");
                if (!Util.correctJustification(list[0], doc, authorTitle[2])) Util.printError("作者简介标题对齐方式错误，应为" + authorTitle[2]);
                if (!Util.correctSpacingBetweenLines_line(list[0], doc, authorTitle[3])) Util.printError("作者简介标题行间距错误，应为" + Util.DSmap[authorTitle[3]]);
                if (!Util.correctSpacingBetweenLines_Be(list[0], doc, authorTitle[4])) Util.printError("作者简介标题段前间距错误，应为3行");
                if (!Util.correctSpacingBetweenLines_Af(list[0], doc, authorTitle[5])) Util.printError("作者简介标题断后间距错误，应为1行");
                if (!Util.correctfonts(list[0], doc, authorTitle[6], "Times New Roman")) Util.printError("作者简介标题字体错误，应为" + authorTitle[6]);
                if (!Util.correctsize(list[0], doc, authorTitle[7])) Util.printError("作者简介标题字号错误，应为" + authorTitle[7]);
                if (!Util.correctBold(list[0], doc, bool.Parse(authorTitle[8])))
                {
                    if (bool.Parse(authorTitle[8]))
                        Util.printError("作者简介标题未加粗");
                    else
                        Util.printError("作者简介标题不需加粗");
                }
                //作者简介正文
                for (int i = 1; i < list.Count; i++)
                {
                    string temp = Tool.getFullText(list[i]);
                    if (temp.Trim().Length == 0) continue;
                    temp = "     " + (temp.Length > 10 ? temp.Substring(0, 10) : temp) + "......";
                    if (!Util.correctIndentation(list[i], doc, authorText[0])) Util.printError("作者简介正文段落缩进错误，应为每段落首行缩进" + authorText[0] + "字符" + temp);
                    if (!Util.correctSpacingBetweenLines_line(list[i], doc, authorText[1])) Util.printError("作者简介正文行间距错误，应为1.5倍行距" + temp);
                    if (!Util.correctSpacingBetweenLines_Be(list[i], doc, authorText[2])) Util.printError("作者简介正文段前间距错误，应为段前0行" + temp);
                    if (!Util.correctSpacingBetweenLines_Af(list[i], doc, authorText[3])) Util.printError("作者简介正文段后间距错误，应为段后0行" + temp);
                    if (!Util.correctfonts(list[i], doc, authorText[4], "Times New Roman")) Util.printError("作者简介正文字体错误，应为" + authorText[4] + temp);
                    if (!Util.correctsize(list[i], doc, authorText[5])) Util.printError("作者简介正文字号错误，应为" + authorText[5] + temp);
                }
            }
            detectContent(list);
            Console.WriteLine("---------------------------------");
        }
        public void detectContent(List<Paragraph> list)
        {
            string[] info = { "姓名", "性别", "出生年月", "民族","籍贯","研究方向","简历" };
            for(int i=1;i<list.Count;i++)
            {
                string content = Util.getFullText(list[i]);
                if (content == "")
                    continue;
                content = content.Replace(":","：");
                int index = content.IndexOf("：");
                if (index>0)
                {
                    for(int j=0;j<info.Length;j++)
                    {
                        if (info[j] == content.Substring(0, index))
                            info[j] = "";
                    }
                }
            }
            for (int j = 0; j < info.Length; j++)
            {
                if (info[j] != "")
                    Util.printError("作者简介缺少" + info[j]+"信息");
            }
        }
    }
}
