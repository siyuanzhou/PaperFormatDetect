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
    class ConclusionAndThanks:Paperbase.ConclusionAndThanks
    {
        public ConclusionAndThanks(WordprocessingDocument doc)
        {
           // Console.WriteLine("本科论文结论检测");
            //Console.WriteLine("----------本科论文结论检测------------");
            Init();
            detectConclusion(Util.sectionLoction(doc, "结论", 0), doc);
            detectThanks(Util.sectionLoction(doc, "致谢", 0), doc);
        }
        /// <summary>
        /// 从XML文件给数组变量赋值 一定注意数组与XML文件是否一致对应
        /// </summary>
        public void Init()
        {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(@"./Template/Undergraduate/ConclusionAndThanks.xml");
            int m = 0;
            //结论标题
            XmlNodeList conTitleNode = xmlDoc.SelectSingleNode("Root").SelectSingleNode("Conclusion").SelectSingleNode("Title").ChildNodes;
            m = 0;
            foreach(XmlNode node in conTitleNode)
            {
                this.conTitle[m] = node.InnerText; m++;
            }
            //结论正文
            XmlNodeList conTextNode = xmlDoc.SelectSingleNode("Root").SelectSingleNode("Conclusion").SelectSingleNode("Text").ChildNodes;
            m = 0;
            foreach (XmlNode node in conTextNode)
            {
                this.conText[m] = node.InnerText; m++;
            }
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
    }
}
