using DocumentFormat.OpenXml.Packaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace PaperFormatDetection.Undergraduate
{
    class Figure : Paperbase.Figure
    {
        public Figure(WordprocessingDocument doc)
        {
            Tools.Util.printError("大连理工大学学位论文图检测");
            Tools.Util.printError("----------------------------------------------");
            try
            {
                Initiation();
                PicDetectionUnder(doc);

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

        }

        //从模板文件中读取表的数据
        public void Initiation()
        {
            string modelPath = System.IO.Directory.GetCurrentDirectory() + "\\Template\\Undergraduate\\Figure.xml";
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
