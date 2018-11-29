using System;
using System.Collections.Generic;
using System.Xml;
using System.Linq;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using PaperFormatDetection.Tools;
using PaperFormatDetection.Frame;
using System.Text.RegularExpressions;

namespace PaperFormatDetection.Undergraduate
{
    class Catalog : Paperbase.Catalog
    {
        public Catalog(WordprocessingDocument doc)
        {
            Init();
            detectCatalog(doc);
        }
        public void Init()
        {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(@"./Template/Undergraduate/Catalog.xml");
            int m = 0;
            //目录标题
            XmlNodeList TitleNode = xmlDoc.SelectSingleNode("Root").SelectSingleNode("Catalog").SelectSingleNode("Title").ChildNodes;
            m = 0;
            foreach (XmlNode node in TitleNode)
            {
                this.Title[m] = node.InnerText; m++;
            }
            //目录内容
            XmlNodeList TextNode = xmlDoc.SelectSingleNode("Root").SelectSingleNode("Catalog").SelectSingleNode("Text").ChildNodes;
            m = 0;
            foreach (XmlNode node in TextNode)
            {
                this.Text[m] = node.InnerText; m++;
            }
        }
    }
}
