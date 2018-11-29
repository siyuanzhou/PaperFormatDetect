using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using PaperFormatDetection.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace PaperFormatDetection.Paperbase
{
    class headerfooters
    {

        protected int indexOfChineseTitleOfDocx;//中文標題在第几段


        protected string CNheaderFonts;           //中文页眉字体
        protected string ENHeaderFonts = "Times New Roman";
        protected string headerFontsize;              //页眉字体大小
        protected string headerJustification;        //页眉对齐




        protected string CNFooterFonts;           //中文页脚字体
        protected string ENFooterFonts = "Times New Roman";
        protected string footerFontsize;              //页脚字体大小
        protected string footerJustification;        //页脚对齐


        protected string oddHeaderText;         //奇数页眉内容
        protected string evenHeaderText;        //偶数页眉内容

        protected bool flag = false;             //标题是否空行

        public headerfooters()
        {

        }

        /*
        function:the first page and the second should have no header
        params: list:the list of sectPrs
        intlist:the location of sectPrs in body
        */
        protected void detectFirstSection(WordprocessingDocument docx)
        {
            Body body = docx.MainDocumentPart.Document.Body;
            IEnumerable<Paragraph> paras = body.Elements<Paragraph>();
            MainDocumentPart Mpart = docx.MainDocumentPart;
            List<SectionProperties> list = secPrList(body);
            SectionProperties s = null;

            if (list.Count == 0)
            {
                return;
            }
            s = list[0];


            IEnumerable<HeaderReference> headrs = s.Elements<HeaderReference>();
            IEnumerable<FooterReference> footrs = s.Elements<FooterReference>();
            foreach (HeaderReference headr in headrs)
            {
                string ID = headr.Id.ToString();
                HeaderPart hp = (HeaderPart)Mpart.GetPartById(ID);
                Header header = hp.Header;
                if (header!=null&&header.InnerText != null)
                {
                    if (header.InnerText.Trim().Length > 0)
                    {
                        PaperFormatDetection.Tools.Util.printError("论文封面和独创性声明应无页眉");
                        break;
                    }
                }
            }
            foreach (FooterReference footr in footrs)
            {
                string ID = footr.Id.ToString();
                FooterPart fp = (FooterPart)Mpart.GetPartById(ID);
                Footer footer = fp.Footer;
                if (footer != null && footer.InnerText != null)
                {
                    if (footer.InnerText.Trim().Length > 0)
                    {
                        PaperFormatDetection.Tools.Util.printError("论文封面和独创性声明应无页脚");
                        break;
                    }
                }
            }
        }



        protected void detectHeaderFooter(WordprocessingDocument docx)
        {
            Body body = docx.MainDocumentPart.Document.Body;
            IEnumerable<Paragraph> paras = body.Elements<Paragraph>();
            MainDocumentPart Mpart = docx.MainDocumentPart;
            List<SectionProperties> list = secPrList(body);
            List<int> intlist = secPrListInt(body);
            SectionProperties scetpr = body.GetFirstChild<SectionProperties>();
            //list.Add(scetpr);
            //intlist.Add(body.ChildElements.Count() - 1);
            SectionProperties s = null;
            for (int i = 2; i <= list.Count<SectionProperties>(); i++)
            {
                s = list[i - 1];
                string chapter = null;
                if(intlist[i - 1]<body.ChildElements.Count)
                {
                    chapter = getPicMassage(intlist[i - 1],body)+"---所在章的";
                }
               
                //页眉
                IEnumerable<HeaderReference> headrs = s.Elements<HeaderReference>();
                
                foreach (HeaderReference headr in headrs)
                {
                    string headertype = null;
                    if (headr.Type == HeaderFooterValues.Default)
                    {
                        headertype = "奇数页";
                    }
                    if (headr.Type == HeaderFooterValues.Even)
                    {
                        headertype = "偶数页";
                    }
                    string ID = headr.Id.ToString();
                    HeaderPart hp = (HeaderPart)Mpart.GetPartById(ID);
                    Header header = hp.Header;
                    Paragraph p = header.GetFirstChild<Paragraph>();
                    if (header.InnerText != null)
                    {
                        if (Util.correctfonts(p, docx, CNheaderFonts, ENHeaderFonts) == false)
                        {
                            PaperFormatDetection.Tools.Util.printError("页眉字体错误，应为:" + CNheaderFonts+ "----"+chapter+ headertype);
                        }
                        if (Util.correctsize(p, docx, headerFontsize) == false)
                        {
                            PaperFormatDetection.Tools.Util.printError("页眉字号错误，应为：" + headerFontsize+ "----" + headertype);
                        }
                        if (JustificationCenter(p, Mpart) == false)
                        {
                            PaperFormatDetection.Tools.Util.printError("页眉未居中" + "----" + chapter + headertype);
                        }
                    }
                }
                //页脚
                IEnumerable<FooterReference> footrs = s.Elements<FooterReference>();
                foreach (FooterReference footr in footrs)
                {
                    string type = null;
                    if (footr.Type == HeaderFooterValues.Default)
                    {
                        type = "奇数页";
                    }
                    if (footr.Type == HeaderFooterValues.Even)
                    {
                        type = "偶数页";
                    }
                    string ID = footr.Id.ToString();
                    FooterPart fp = (FooterPart)Mpart.GetPartById(ID);
                    Footer footer = fp.Footer;
                    Paragraph p = footer.GetFirstChild<Paragraph>();

                    if (footer.InnerText.Trim() != "")
                    {
                        if (Util.correctfonts(p, docx, CNFooterFonts, ENFooterFonts) == false)
                        {
                            Util.printError("页脚字体错误，应为：" + CNFooterFonts + "----" + chapter+ type);
                        }
                        if (Util.correctsize(p, docx, footerFontsize) == false)
                        {
                            Util.printError( "页脚字号错误，应为：" + footerFontsize +"----" + chapter+ type);
                        }
                        if (JustificationCenter(p, Mpart) == false)
                        {
                            Util.printError("页脚未居中" +"----" + chapter+ type);
                        }
                    }
                }
            }
        }

        protected void detectLastSection(WordprocessingDocument docx)
        {
            Body body = docx.MainDocumentPart.Document.Body;
            MainDocumentPart Mpart = docx.MainDocumentPart;
            int l = body.ChildElements.Count;
            if (body.ChildElements.GetItem(l - 1).GetType().ToString() == "DocumentFormat.OpenXml.Wordprocessing.SectionProperties")
            {
                SectionProperties s=body.GetFirstChild<SectionProperties>();
                IEnumerable<FooterReference> footrs = s.Elements<FooterReference>();
                foreach (FooterReference footr in footrs)
                {
                    string ID = footr.Id.ToString();
                    FooterPart fp = (FooterPart)Mpart.GetPartById(ID);
                    Footer footer = fp.Footer;
                    if (footer!=null&&footer.InnerText != null)
                    {
                        if (footer.InnerText.Trim().Length > 0)
                        {
                            Util.printError("论文版权使用授权书应无页脚");
                            break;
                        }
                    }
                }
                IEnumerable<HeaderReference> headrs = s.Elements<HeaderReference>();
                foreach (HeaderReference headr in headrs)
                {
                    string ID = headr.Id.ToString();
                    HeaderPart hp = (HeaderPart)Mpart.GetPartById(ID);
                    Header header = hp.Header;
                    if (header != null&&header.InnerText != null)
                    {
                        if(header.InnerText.Trim().Length > 0)
                        {
                            if(header.InnerText.Trim()!=oddHeaderText)
                            {
                                Util.printError("论文版权使用授权书内容错误，应为："+ oddHeaderText);
                                break;
                            }
                        }
                    }
                }

            }
        }


        protected void detectOddEven(WordprocessingDocument docx)
        {
            Body body = docx.MainDocumentPart.Document.Body;
            MainDocumentPart Mpart = docx.MainDocumentPart;
            List<SectionProperties> list = secPrList(body);
            List<int> intlist = secPrListInt(body);
            SectionProperties s = null;
            for (int i = 2; i <= list.Count<SectionProperties>(); i++)
            {
                s = list[i - 1];
                string chapter = null;
                if (intlist[i - 1] < body.ChildElements.Count)
                {
                    chapter = getPicMassage(intlist[i - 1],body) + "----所在章的";
                }
                IEnumerable<HeaderReference> headrs = s.Elements<HeaderReference>();
                HeaderReference headereven = null;
                HeaderReference headerdefault = null;
                foreach (HeaderReference headr in headrs)
                {
                    if (headr.Type == HeaderFooterValues.Default)
                    {
                        headerdefault = headr;
                    }
                    if (headr.Type == HeaderFooterValues.Even)
                    {
                        headereven = headr;
                    }
                }
                if (headerdefault != null)
                {
                    //奇数页
                    string ID = headerdefault.Id.ToString();
                    HeaderPart hp = (HeaderPart)Mpart.GetPartById(ID);
                    Header header = hp.Header;
                    Paragraph p = header.GetFirstChild<Paragraph>();
                    if (header.InnerText != null)
                    {
                        string headername = header.InnerText.Trim();
                        if (headername != oddHeaderText)
                        {
                            Util.printError("页眉内容错误，应为：" +  oddHeaderText + "----" + chapter+"奇数页");
                        }
                    }
                }
                //偶数页
                if (headereven != null)
                {
                    string ID = headereven.Id.ToString();
                    HeaderPart hp = (HeaderPart)Mpart.GetPartById(ID);
                    Header header = hp.Header;
                    Paragraph p = header.GetFirstChild<Paragraph>();
                    if (header.InnerText != null)
                    {
                        if(header.InnerText!="")
                        {
                            string headername = header.InnerText.Trim();
                            if (headername != evenHeaderText&&!flag)
                            {
                               // Util.printError("页眉内容错误，应为：" + evenHeaderText + "----" + chapter + "奇数页");
                            }
                        }
                    }
                }                            
            }
        }



        protected void isTabInTitle(WordprocessingDocument docx)
        {
            Body body = docx.MainDocumentPart.Document.Body;
            IEnumerable<Paragraph> paras = body.Elements<Paragraph>();
            foreach(Paragraph p in paras)
            {
                string temp = Tool.getFullText(p);
                if (temp.Trim().Length == 0) continue;
                if (temp.IndexOf("学位论文题目") >= 0)
                {
                    if (temp.Substring(7).Trim().Length > 0 && temp.Substring(7).Trim() != Util.getPaperName(docx).Trim())
                        flag = true;
                }
            }
        }
        /*
        function:judge document whether is set to evenandodd//判断是否奇数偶数页不同
        true 表示设置了奇数偶数页不同
        */
        protected bool evenAndOddHeaders(MainDocumentPart Mpart)
        {
            DocumentSettingsPart dsp = Mpart.DocumentSettingsPart;
            Settings setting = dsp.Settings;
            if (setting.GetFirstChild<EvenAndOddHeaders>() == null)
            {
                return false;
            }
            return true;
        }



        //判断是否设置了首页不同
        //true 表示设置了首页不同
        protected bool firstPageIsdifferent(SectionProperties s)
        {
            TitlePage tp = s.GetFirstChild<TitlePage>();
            if (tp != null)
            {
                return true;
            }
            return false;
        }


        /*
        function：judge a paragraph's position center
                    判断居中
        params: p:paragraph
                Mpart:MainDocumentPart
        return:
                真表示居中
        */
        public static bool JustificationCenter(Paragraph p, MainDocumentPart Mpart)
        {
            Justification jc = null;
            ParagraphStyleId pid = null;
            if (p.ParagraphProperties != null)
            {
                if ((jc = p.ParagraphProperties.Justification) != null)
                {
                    if (jc.Val != JustificationValues.Center)
                    { return false; }
                }
                if (jc != null)
                {
                    if ((pid = p.ParagraphProperties.ParagraphStyleId) != null)
                    {
                        Styles styles = Mpart.StyleDefinitionsPart.Styles;
                        Style style = null;
                        IEnumerable<Style> stys = styles.OfType<Style>();
                        foreach (Style sty in stys)
                        {
                            if (sty.StyleId.ToString() == pid.ToString())
                            {
                                style = sty;
                                break;
                            }
                        }
                        if (style != null)
                        {
                            if (style.StyleParagraphProperties != null)
                            {
                                if ((jc = style.StyleParagraphProperties.Justification) != null)
                                {
                                    if (jc.Val != JustificationValues.Center)
                                    { return false; }
                                }
                            }
                        }
                    }
                }
            }
            return true;
        }




        //获取所有章节属性SecPr的位置，用list保存
        protected List<int> secPrListInt(Body body)
            {
                List<int> list = new List<int>(20);
                int l = body.ChildElements.Count();
                for (int i = 0; i < l; i++)
                {
                    if (body.ChildElements.GetItem(i).GetType().ToString() == "DocumentFormat.OpenXml.Wordprocessing.Paragraph")
                    {
                        Paragraph p = (Paragraph)body.ChildElements.GetItem(i);
                        if (p.ParagraphProperties != null)
                        {
                            if (p.ParagraphProperties.SectionProperties != null)
                            {
                                list.Add(i);
                            }
                        }
                    }
                }
                return list;
            }
        protected List<SectionProperties> secPrList(Body body)
            {
                List<SectionProperties> list = new List<SectionProperties>(20);
                int l = body.ChildElements.Count();
                for (int i = 0; i < l; i++)
                {
                    if (body.ChildElements.GetItem(i).GetType().ToString() == "DocumentFormat.OpenXml.Wordprocessing.Paragraph")
                    {
                        Paragraph p = (Paragraph)body.ChildElements.GetItem(i);
                        if (p.ParagraphProperties != null)
                        {
                            if (p.ParagraphProperties.SectionProperties != null)
                            {
                                list.Add(p.ParagraphProperties.SectionProperties);
                            }
                        }
                    }
                }
                return list;
            }
        protected string getPicMassage(int i, Body body)
        {
            string message = "";
            for (int j = i + 1; j < i + 3; j++)
            {
                if(j< body.ChildElements.Count())
                {
                    if (body.ChildElements.GetItem(j).GetType().ToString() == "DocumentFormat.OpenXml.Wordprocessing.Paragraph")
                    {
                        message = Util.getFullText((Paragraph)body.ChildElements.GetItem(j)).Trim();
                        if (message != "")
                        {
                            if (message.Length >= 15)
                            {
                                message = message.Substring(0, 14);
                                break;
                            }
                        }
                    }
                }
            }
            return message;
        }
    }
}
