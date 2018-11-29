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
    class Tabledect : Paperbase.Tabledect
    {
        public Tabledect(WordprocessingDocument doc)
        {
            Tools.Util.printError("表检测");
            Tools.Util.printError("----------------------------------------------");

            try
            {
                Initiation();
                detectTable(doc,"master");
                //detectTableCNName(doc);
                //detectTableENName(doc);
                //detectTableInText(doc);
                //detectTableLocation(doc);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            //detectTable(doc, "master");
            Tools.Util.printError("----------------------------------------------");

        }



        //从模板文件中读取表的数据
        public void Initiation()
        {
            string modelPath = Util.environmentDir + "\\Template\\Master\\TableStyle.xml";
            try
            {
                XmlDocument myXmlDoc = new XmlDocument();
                myXmlDoc.Load(modelPath);
                XmlNode rootNode = myXmlDoc.SelectSingleNode("TableStyle");

                XmlNode tableFontNode = rootNode.SelectSingleNode("TableName");
                this.tableFont = tableFontNode.SelectSingleNode("font").InnerText;
                this.tableFontsize = tableFontNode.SelectSingleNode("fontsize").InnerText;
                this.tableJustification = tableFontNode.SelectSingleNode("justification").InnerText;
                this.spacelnTotableUp = int.Parse(tableFontNode.SelectSingleNode("spacelnTotableUp").InnerText);
                this.MNtoName = int.Parse(tableFontNode.SelectSingleNode("MNtoName").InnerText);


                XmlNode EnglishTableFontNode = rootNode.SelectSingleNode("EnglishTableName");
                this.englishtableFont = EnglishTableFontNode.SelectSingleNode("font").InnerText;
                this.englishtableFontsize = EnglishTableFontNode.SelectSingleNode("fontsize").InnerText;
                this.englishtableJustification = EnglishTableFontNode.SelectSingleNode("justification").InnerText;
                this.englishspacelnTotableUp = int.Parse(EnglishTableFontNode.SelectSingleNode("spacelnTotableUp").InnerText);



                XmlNode InTableNode = rootNode.SelectSingleNode("InTable");
                this.intableFont = InTableNode.SelectSingleNode("font").InnerText;
                this.inEntableFont = InTableNode.SelectSingleNode("enfont").InnerText;
                this.intableFontsize = InTableNode.SelectSingleNode("fontsize").InnerText;
                this.intableJustification = InTableNode.SelectSingleNode("justification").InnerText;


            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }




    }
}