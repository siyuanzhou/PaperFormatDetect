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

namespace PaperFormatDetection.Doctor
{
    class Reference : Paperbase.Reference
    {
        public Reference(WordprocessingDocument d)
        {
            Init();
            SelectandCheckRef(d);

        }

        public void Init()
        {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(@"./Template/Doctor/Reference.xml");

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
            this.ContentSpacing = cspacing.InnerText;
            //  Console.WriteLine(ContentSpacing);

            XmlNode refcount = xmlDoc.SelectSingleNode("/Root/Reference/Count/Refcount");
            this.Refcount = int.Parse(refcount.InnerText);

            XmlNode refcountj = xmlDoc.SelectSingleNode("/Root/Reference/Count/RefcountJ");
            this.Refcount_J = int.Parse(refcountj.InnerText);

            /*XmlNode refcounten = xmlDoc.SelectSingleNode("/Root/Reference/Count/RefcountEN");
            this.RefcountEn = int.Parse(refcounten.InnerText);*/

        }

        public void SelectandCheckRef(WordprocessingDocument doc)
        {
            Tools.Util.printError("");
            Tools.Util.printError("------------------论文参考文献检测--------------------");
            Body body = doc.MainDocumentPart.Document.Body;
            IEnumerable<Paragraph> paras = body.Elements<Paragraph>();
            List<Paragraph> plist = Tool.toList(paras);
            int number = -1;
            Paragraph referenceTitle = new Paragraph();


            //标记参考文献的开始
            bool isRefBegin = false;
            //标记参考文献的结束
            bool isRefEnd = false;
            //标记编号开始
            /*bool NumberingStart = false;

           
            string numberingId = null;
            string ilvl = null;*/
            //标记电子文献段落是否可能分成两段

            //遍历每一个Paragraph
            foreach (Paragraph para in paras)
            {
                number++;
                Run r = para.GetFirstChild<Run>();
                if (r == null) continue;
                string fullText = para.InnerText;
                if (fullText.Trim().Length == 0)
                {

                    continue;//无内容
                }
                //判断参考文献检测起始位置，检测参考文献标题
                if (fullText.Replace(" ", "").Equals("参考文献") || fullText.Replace(" ", "").Equals("参考文") ||
                    fullText.Replace(" ", "").Equals("考文献"))
                {
                    referenceTitle = para;
                    isRefBegin = true;
                    checkReferenceTitle(para, doc, fullText);
                    continue;
                }
                //判断参考文献检测结束
                if (isRefBegin && (fullText.Replace(" ", "").IndexOf("附录") != -1 ||
                    fullText.Replace(" ", "").Equals("攻读硕士学位期间发表学术论文情况") || fullText.Replace(" ", "").Equals("致谢")
                    || fullText.Replace(" ", "").Equals("攻读博士学位期间科研项目及科研成果")))
                {
                    isRefEnd = true;
                    //检测参考文献总数量
                    if (countRef < Refcount)
                    {
                        Util.printError("参考文献总量少于" + Refcount + "篇");
                    }
                    //检测期刊数量
                    if (countRef_J < countRef*Refcount_J/100)
                    {
                        Util.printError("期刊类参考文献少于参考文献总数的"+Refcount_J+"%");
                    }
                    //检测外文参考文献数量
                  /*  if (countRef - countCnRef < RefcountEn)
                    {

                        Util.printError("外文参考文献数量少于" + RefcountEn + "篇");

                    }*/
                    break;
                }

                //检测参考文献内容的每个Paragraph
                if (isRefBegin == true && isRefEnd == false)
                {

                    CheckContent(para, doc, plist, number);
                }
            }

            isNumberingCorrectinContents(countRef, doc);
    
        }
    }
}