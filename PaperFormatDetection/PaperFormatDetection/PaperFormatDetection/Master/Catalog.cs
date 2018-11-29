using System;
using System.Collections.Generic;
using System.Xml;
using System.Linq;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using PaperFormatDetection.Tools;
using PaperFormatDetection.Frame;
using System.Text.RegularExpressions;

namespace PaperFormatDetection.Master
{
    class Catalog : Paperbase.Catalog
    {
        public Catalog(WordprocessingDocument doc)
        {
            Tools.Util.printError("目录检测");
            Util.printError("----------------------------------------------");
            try
            {
                Init();
                detectCatalog(doc);
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
            xmlDoc.Load(Util.environmentDir + "/Template/Master/Catalog.xml");
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
