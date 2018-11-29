using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions; 
using System.Threading.Tasks;
using PaperFormatDetection.Tools;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using System.Xml;

namespace PaperFormatDetection.Doctor
{
    class ConclusionAndThanks : Paperbase.ConclusionAndThanks
    {
        WordprocessingDocument doc = null;
        Body body = null;
        IEnumerable<Paragraph> paras = null;
        public ConclusionAndThanks(WordprocessingDocument d)
        {
            this.doc = d;
            body = doc.MainDocumentPart.Document.Body;
            paras = body.Elements<Paragraph>();
            List<Paragraph> list = new List<Paragraph>();
            //找到结论与展望部分
            Boolean flag = false;
            foreach (Paragraph p in paras)
            {
                String fullText = Tool.getFullText(p);
                if (Regex.IsMatch(fullText, "^[0-9]+[ ]*结论与展望$"))
                {
                    flag = true;
                }
                if (fullText.Replace(" ","").Equals("参考文献"))
                {
                    flag = false; break;
                }
                if (flag == true)
                {
                    list.Add(p);
                }     
            }
            ConclusionandProspect(list);
            Init();
            //致谢
            detectThanks(Util.sectionLoction(doc, "致谢", 2), doc);
            //////////////////////////////////
        }
        public void Init()
        {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(@"./Template/Doctor/ConclusionAndThanks.xml");
            int m = 0;
            //致谢标题
            XmlNodeList thanksTitleNode = xmlDoc.SelectSingleNode("Root").SelectSingleNode("Thanks").SelectSingleNode("Title").ChildNodes;
            m = 0;
            foreach (XmlNode node in thanksTitleNode)
            {
                this.thanksTitle[m] = node.InnerText; m++;
            }
            //致谢正文
            XmlNodeList thanksTextNode = xmlDoc.SelectSingleNode("Root").SelectSingleNode("Thanks").SelectSingleNode("Text").ChildNodes;
            m = 0;
            foreach (XmlNode node in thanksTextNode)
            {
                this.thanksText[m] = node.InnerText; m++;
            }
        }
        //结论与展望检测
        public void ConclusionandProspect(List<Paragraph> list)
        {
            if(list.Count==0)
            {
                Util.printError("论文缺少结论与展望部分"); 
                return;
            }
            string order = list[0].InnerText.Substring(0, list[0].InnerText.Length - 5).Trim();
            string nextOrder = (int.Parse(order) + 1).ToString();
            List<string> sectionTitle = new List<string>();
            foreach (Paragraph p in list)
            {
                string str = Tool.getFullText(p);
                //最后一节
                if (str.Trim().IndexOf(nextOrder) == 0)
                {
                    Util.printError("结论与展望应作为论文的最后一章出现"); break;
                }
                if (str.Trim().IndexOf(order+".") == 0)
                {
                    sectionTitle.Add(str);
                }
            }
            if(sectionTitle.Count!=3)
            {
                Util.printError("结论与展望包含且仅包含结论，创新点和展望三小节");
            }
            else
            {
                if(sectionTitle[0].Replace(" ","") != order+".1"+"结论")
                {
                    Util.printError("结论与展望第一节应为结论");
                }
                if (sectionTitle[1].Replace(" ", "") != order + ".2" + "创新点")
                {
                    Util.printError("结论与展望第二节应为创新点");
                }
                if (sectionTitle[2].Replace(" ", "") != order + ".3" + "展望")
                {
                    Util.printError("结论与展望第三节应为展望");
                }
            }
        }
    }
}
