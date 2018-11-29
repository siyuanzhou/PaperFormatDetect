using DocumentFormat.OpenXml.Packaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using PaperFormatDetection.Tools;

namespace PaperFormatDetection.Master
{
    class Figure : Paperbase.Figure
    {

        public Figure(WordprocessingDocument doc)
        {
            Tools.Util.printError("图检测");
            Tools.Util.printError("----------------------------------------------");
            try
            {
                Initiation();
                PicDetection(doc);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            Tools.Util.printError("----------------------------------------------");

        }

        //从模板文件中读取表的数据
        public void Initiation()
        {
            string modelPath = Util.environmentDir + "\\Template\\Master\\Figure.xml";
            try
            {
                XmlDocument myXmlDoc = new XmlDocument();
                myXmlDoc.Load(modelPath);
                XmlNode rootNode = myXmlDoc.SelectSingleNode("FigureStyle");

                XmlNode FigureFontNode = rootNode.SelectSingleNode("FigureCNName");
                this.PicCNFont = FigureFontNode.SelectSingleNode("font").InnerText;
                this.PicCNFontSize = FigureFontNode.SelectSingleNode("fontsize").InnerText;
                this.PicCNFontJustification = FigureFontNode.SelectSingleNode("justification").InnerText;
                this.CNFontToPicLns = int.Parse(FigureFontNode.SelectSingleNode("CNFontToPicLns").InnerText);



                this.MNtoName = int.Parse(FigureFontNode.SelectSingleNode("MNtoName").InnerText);


                XmlNode FigureENFontNode = rootNode.SelectSingleNode("FigureENName");
                this.PicENFont = FigureENFontNode.SelectSingleNode("font").InnerText;
                this.PicENFontSize = FigureENFontNode.SelectSingleNode("fontsize").InnerText;
                this.PicENFontJustification = FigureENFontNode.SelectSingleNode("justification").InnerText;
                this.ENFontToCNFont = int.Parse(FigureENFontNode.SelectSingleNode("ENFontToCNFont").InnerText);


                this.PicJustification = rootNode.SelectSingleNode("PicJustification").InnerText;
                this.UpBlankToPicLns = int.Parse(rootNode.SelectSingleNode("UpBlankToPicLns").InnerText);
                this.DownBlankToPicLns = int.Parse(rootNode.SelectSingleNode("DownBlankToPicLns").InnerText);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }

}
