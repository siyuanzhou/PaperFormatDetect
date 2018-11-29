using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using System.Xml;
using PaperFormatDetection.Tools;

namespace PaperFormatDetection.Doctor
{
    class HeaderFooter : Paperbase.HeaderFooter
    {

        public HeaderFooter(WordprocessingDocument docx)
        {
            Tools.Util.printError("--------------------------------------");
            Console.WriteLine("论文页眉页脚检测");
            try
            {
            Body body = docx.MainDocumentPart.Document.Body;
            IEnumerable<Paragraph> paras = body.Elements<Paragraph>();
            MainDocumentPart Mpart = docx.MainDocumentPart;
            List<SectionProperties> list = secPrList(body);
            List<int> intlist = secPrListInt(body);     //节的位置

            for (int i = 0; i < list.Count; i++)
            {
                firstPageIsdifferent(list[i]);
            }

            if (evenAndOddHeaders(Mpart) == false)
            {
                Util.printError("论文应设置奇数偶数页不同");
            }
            Initiation(body);

            detectFirstSection(docx);
            detectEven(docx);
            detectDefault(docx);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

        }

        //从模板文件中读取摘要的数据
        public void Initiation(Body body)
        {
            string modelPath = System.IO.Directory.GetCurrentDirectory() + "\\Template\\Doctor\\HeaderFooter.xml";
            IEnumerable<Paragraph> paras = body.Elements<Paragraph>();
            try
            {
                XmlDocument myXmlDoc = new XmlDocument();
                myXmlDoc.Load(modelPath);
                XmlNode rootNode = myXmlDoc.SelectSingleNode("HeaderFooter");

                XmlNode HeaderNode = rootNode.SelectSingleNode("Header");
                this.CNheaderFonts = HeaderNode.SelectSingleNode("CNheaderFonts").InnerText;
                this.headerFontsize = HeaderNode.SelectSingleNode("headerFontsize").InnerText;
                this.headerJustification = HeaderNode.SelectSingleNode("headerJustification").InnerText;

                XmlNode FooterNode = rootNode.SelectSingleNode("Footer");
                this.CNFooterFonts = FooterNode.SelectSingleNode("CNFooterFonts").InnerText;
                this.footerFontsize = FooterNode.SelectSingleNode("footerFontsize").InnerText;
                this.footerJustification = FooterNode.SelectSingleNode("footerJustification").InnerText;

                this.indexOfChineseTitleOfDocx = int.Parse(rootNode.SelectSingleNode("indexOfChineseTitleOfDocx").InnerText);

                this.evenHeaderText = getChineseTitleOfDocx(paras, indexOfChineseTitleOfDocx);

                this.oddHeaderText = "大连理工大学博士学位论文";

                Console.WriteLine(this.evenHeaderText);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }





}


