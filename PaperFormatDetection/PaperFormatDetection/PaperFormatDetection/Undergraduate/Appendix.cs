using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DocumentFormat.OpenXml.Packaging;

using System.Xml;
using PaperFormatDetection.Tools;
namespace PaperFormatDetection.Undergraduate
{
    class Appendix : Paperbase.Appendix
    {
        public Appendix(WordprocessingDocument doc)
        {
            Init();
            detectConclusion(sectionLoction(doc, "附录", 0), doc);
        }
        public void Init()
        {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(@"./Template/Undergraduate/Appendix.xml");
            int m = 0;
            //附录标题
            XmlNodeList conTitleNode = xmlDoc.SelectSingleNode("Root").SelectSingleNode("Appendix").SelectSingleNode("Title").ChildNodes;
            m = 0;
            foreach (XmlNode node in conTitleNode)
            {
                this.conTitle[m] = node.InnerText; m++;
            }
            //附录正文
            XmlNodeList conTextNode = xmlDoc.SelectSingleNode("Root").SelectSingleNode("Appendix").SelectSingleNode("Text").ChildNodes;
            m = 0;
            foreach (XmlNode node in conTextNode)
            {
                this.conText[m] = node.InnerText; m++;
            }

        }
    }
}
