using System;
using System.Xml;
using System.Collections.Generic;
using PaperFormatDetection.Tools;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using System.Text.RegularExpressions;

namespace PaperFormatDetection.Master
{
    class Text : Paperbase.Text
    {
        public Text(WordprocessingDocument doc)
        {
            Util.printError("正文检测");
            Util.printError("----------------------------------------------");
            try
            {
                Init();
                detectAllText(sectionLoction(doc, "正文", 0), doc);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            Util.printError("----------------------------------------------");
        }
        public void Init()
        {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(Util.environmentDir + "/Template/Master/Text.xml");
            int m, index;
            XmlNodeList tTitleNode = xmlDoc.SelectSingleNode("Text").SelectSingleNode("tTitle").ChildNodes;
            index = 0;
            foreach (XmlNode levelnode in tTitleNode)
            {
                m = 0;
                XmlNodeList levelNode = levelnode.ChildNodes;
                foreach (XmlNode node in levelNode)
                {
                    tTitle[index, m] = node.InnerText;
                    m++;
                }
                index++;
            }
            
            XmlNodeList tTextNode = xmlDoc.SelectSingleNode("Text").SelectSingleNode("tText").ChildNodes;
            m = 0;
            foreach (XmlNode node in tTextNode)
            {
                tText[m] = node.InnerText;
                m++;
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
            int index = Array.IndexOf(type[paperType], section);
            Body body = doc.MainDocumentPart.Document.Body;
            IEnumerable<Paragraph> paras = body.Elements<Paragraph>();
            List<Paragraph> list = new List<Paragraph>();
            Boolean begin = false;
            Boolean end = false;
            if (section == "正文")
            {
                bool haveBookMark = false;

                foreach (Paragraph p in paras)
                {
                    String fullText = Util.getFullText(p).Trim();

                    if (p.GetFirstChild<BookmarkStart>() != null && p.GetFirstChild<BookmarkEnd>() != null)
                        haveBookMark = true;
                    else
                        haveBookMark = false;

                    if (fullText.Length > 0)
                    {
                        Match m = Regex.Match(fullText, @"[0-1]");
                        if (m.Success && m.Index == 0 && haveBookMark)
                        {
                            begin = true;
                        }
                    }
                    for (int i = index + 1; i < type[paperType].Length; i++)
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
            return list;
        }
    }
}
