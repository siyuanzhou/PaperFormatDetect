using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using PaperFormatDetection.Tools;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using System.Xml;

namespace PaperFormatDetection.Undergraduate
{
    class Reference : Paperbase.Reference
    {
        public Reference(WordprocessingDocument d)
        {
            try
            {
                Init();
                SelectandCheckRef(d);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }


        }

        public void Init()
        {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(@"./Template/Undergraduate/Reference.xml");

            XmlNode justi = xmlDoc.SelectSingleNode("/Root/Reference/Title/justification");//.SelectSingleNode("Justification");
            this.TitleJustifi = justi.InnerText;
            //Console.WriteLine(TitleJustifi);

            XmlNode bef = xmlDoc.SelectSingleNode("/Root/Reference/Title/linebefore");
            this.TitleBef = bef.InnerText;
            //Console.WriteLine(TitleBef);

            XmlNode aft = xmlDoc.SelectSingleNode("/Root/Reference/Title/lineafter");
            this.TitleAft = aft.InnerText;
           // Console.WriteLine(TitleAft);

            XmlNode fonts = xmlDoc.SelectSingleNode("/Root/Reference/Title/fonts");//.SelectSingleNode("Justification");
            this.TitleFonts = fonts.InnerText;
           // Console.WriteLine(TitleFonts);


            XmlNode size = xmlDoc.SelectSingleNode("/Root/Reference/Title/fontsize");
            this.TitleSize = size.InnerText;
          //  Console.WriteLine(TitleSize);

            XmlNode spacing = xmlDoc.SelectSingleNode("/Root/Reference/Title/spacing");
            this.TitleSpacing = spacing.InnerText;
           // Console.WriteLine(TitleSpacing);


            XmlNode cjusti = xmlDoc.SelectSingleNode("/Root/Reference/Content/justification");//.SelectSingleNode("Justification");
            this.ContentJustifi = cjusti.InnerText;
          //  Console.WriteLine(ContentJustifi);

            XmlNode cbef = xmlDoc.SelectSingleNode("/Root/Reference/Content/linebefore");
            this.ContentBef = cbef.InnerText;
           // Console.WriteLine(ContentBef);

            XmlNode caft = xmlDoc.SelectSingleNode("/Root/Reference/Content/lineafter");
            this.ContentAft = caft.InnerText;
          //  Console.WriteLine(ContentAft);

            XmlNode cfonts = xmlDoc.SelectSingleNode("/Root/Reference/Content/fonts");//.SelectSingleNode("Justification");
            this.ContentFonts = cfonts.InnerText;
           // Console.WriteLine(ContentFonts);

            XmlNode csize = xmlDoc.SelectSingleNode("/Root/Reference/Content/fontsize");
            this.ContentSize = csize.InnerText;
           // Console.WriteLine(ContentSize);

            XmlNode cspacing = xmlDoc.SelectSingleNode("/Root/Reference/Content/spacing");
            this.ContentSpacing= cspacing.InnerText;
          //  Console.WriteLine(ContentSpacing);

            XmlNode refcount = xmlDoc.SelectSingleNode("/Root/Reference/Count/Refcount");
            this.Refcount = int.Parse(refcount.InnerText);

            XmlNode refcountj = xmlDoc.SelectSingleNode("/Root/Reference/Count/RefcountJ");
            this.Refcount_J = int.Parse(refcountj.InnerText);

            XmlNode refcounten = xmlDoc.SelectSingleNode("/Root/Reference/Count/RefcountEN");
            this.RefcountEn = int.Parse(refcounten.InnerText);



        }
    }
}

