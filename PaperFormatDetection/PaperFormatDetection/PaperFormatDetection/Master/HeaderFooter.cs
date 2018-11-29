using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using System.Xml;
using PaperFormatDetection.Tools;

namespace PaperFormatDetection.Master
{
    class HeaderFooter : Paperbase.headerfooters
    {

        string masterType = Util.masterType;
        public HeaderFooter(WordprocessingDocument docx)
        {
            Tools.Util.printError("页眉页脚检测");
            Tools.Util.printError("----------------------------------------------");
            try
            {
                Body body = docx.MainDocumentPart.Document.Body;
                IEnumerable<Paragraph> paras = body.Elements<Paragraph>();
                MainDocumentPart Mpart = docx.MainDocumentPart;
                //List<SectionProperties> list = secPrList(body);
                //List<int> intlist = secPrListInt(body);     //节的位置

                //for(int i=0;i<list.Count;i++)
                //{
                //    if(firstPageIsdifferent(list[i]))
                //    {
                //            PaperFormatDetection.Tools.Util.printError("论文页眉页脚不应设置首页不同");
                //            break;
                //    }
                //}

                if (evenAndOddHeaders(Mpart) == false)
                {
                    Util.printError("论文页眉奇数偶数页应不同");
                }
                //Initiation(docx, masterType);

                //detectFirstSection(docx);
                //detectHeaderFooter(docx);
                //detectEven(docx);
                //detectDefault(docx);
                isTabInTitle(docx);
                Initiation(docx, masterType);
                detectFirstSection(docx);
                detectHeaderFooter(docx);
                detectOddEven(docx);
                detectLastSection(docx);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            Tools.Util.printError("----------------------------------------------");

        }

        //从模板文件中读取数据
        public void Initiation(WordprocessingDocument docx, string masterType)
        {
            Body body= docx.MainDocumentPart.Document.Body;
            string modelPath = Util.environmentDir + "\\Template\\Master\\HeaderFooter.xml";
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

                this.evenHeaderText = Util.getPaperName(docx);
                if(masterType.Equals("学术型硕士"))
                {
                    this.oddHeaderText ="大连理工大学硕士学位论文";
                }
                else if(masterType.Equals("专业学位硕士"))
                {
                    this.oddHeaderText = "大连理工大学专业学位硕士学位论文";
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }





}

