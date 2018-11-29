using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using PaperFormatDetection.Tools;
using System.Xml;
namespace PaperFormatDetection.Undergraduate
{
    class Abstract : Paperbase.Abstract
    {
        public Abstract(WordprocessingDocument doc)
        {
            Init();
            detectabstitle(Util.sectionLoction(doc, "ժҪ", 1), doc);
            detectabstitleE(Util.sectionLoction(doc, "Abstract", 1), doc);
        }
        public void Init()
        {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(@"./Template/Master/Abstract.xml");
            int m = 0;
            XmlNodeList abstitleNode = xmlDoc.SelectSingleNode("Root").SelectSingleNode("Abstract").SelectSingleNode("Title").ChildNodes;
            m = 0;
            foreach (XmlNode node in abstitleNode)
            {
                this.abstitle[m] = node.InnerText; m++;
            }
            XmlNodeList abstextNode = xmlDoc.SelectSingleNode("Root").SelectSingleNode("Abstract").SelectSingleNode("Text").ChildNodes;
            m = 0;
            foreach (XmlNode node in abstextNode)
            {
                this.abstext[m] = node.InnerText; m++;
            }
            XmlNodeList abskeywordNode = xmlDoc.SelectSingleNode("Root").SelectSingleNode("Abstract").SelectSingleNode("keyword").ChildNodes;
            m = 0;
            foreach (XmlNode node in abskeywordNode)
            {
                this.abskeyword[m] = node.InnerText; m++;
            }
            XmlNodeList abstitleENode = xmlDoc.SelectSingleNode("Root").SelectSingleNode("Abstract").SelectSingleNode("TitleE").ChildNodes;
            m = 0;
            foreach (XmlNode node in abstitleENode)
            {
                this.abstitleE[m] = node.InnerText; m++;
            }
            XmlNodeList abstextENode = xmlDoc.SelectSingleNode("Root").SelectSingleNode("Abstract").SelectSingleNode("TextE").ChildNodes;
            m = 0;
            foreach (XmlNode node in abstextENode)
            {
                this.abstextE[m] = node.InnerText; m++;
            }
            XmlNodeList abskeywordENode = xmlDoc.SelectSingleNode("Root").SelectSingleNode("Abstract").SelectSingleNode("keywordE").ChildNodes;
            m = 0;
            foreach (XmlNode node in abskeywordENode)
            {
                this.abskeywordE[m] = node.InnerText; m++;
            }
        }
    }
}