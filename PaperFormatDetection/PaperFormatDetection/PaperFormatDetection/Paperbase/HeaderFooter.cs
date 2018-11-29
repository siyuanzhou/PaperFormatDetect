using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using PaperFormatDetection.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaperFormatDetection.Paperbase
{
    class HeaderFooter
    {

        protected int indexOfChineseTitleOfDocx;//中文標題在第幾段
       // protected string no2pageTitle;           //独创性声明的标题



        protected string CNheaderFonts;           //中文页眉字体
        protected string ENHeaderFonts= "Times New Roman";
        protected string headerFontsize;              //页眉字体大小
        protected string headerJustification;        //页眉对齐




        protected string CNFooterFonts;           //中文页脚字体
        protected string ENFooterFonts= "Times New Roman";
        protected string footerFontsize;              //页脚字体大小
        protected string footerJustification;        //页脚对齐


        protected string oddHeaderText;         //奇数页眉内容
        protected string evenHeaderText;        //偶数页眉内容




        /* 构造函数 */
        public HeaderFooter()
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
                if (header != null&& header.InnerText!=null)
                {
                    if (header.InnerText.Trim().Length > 0)
                    {
                        PaperFormatDetection.Tools.Util.printError("论文封面和独创性声明应无页眉");
                    }
                }
            }
            foreach (FooterReference footr in footrs)
            {
                string ID = footr.Id.ToString();
                FooterPart fp = (FooterPart)Mpart.GetPartById(ID);
                Footer footer = fp.Footer;
                if (footer != null&&footer.InnerText != null)
                {
                    if (footer.InnerText.Trim().Length > 0)
                    {
                        PaperFormatDetection.Tools.Util.printError("论文封面和独创性声明应无页脚");
                    }
                }
            }
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



        //判断奇数页
        protected void detectDefault(WordprocessingDocument docx)
        {
            Body body = docx.MainDocumentPart.Document.Body;
            IEnumerable<Paragraph> paras = body.Elements<Paragraph>();
            MainDocumentPart Mpart = docx.MainDocumentPart;
            SectionProperties scetpr = body.GetFirstChild<SectionProperties>();
            List<SectionProperties> list = secPrList(body);
            list.Add(scetpr);
            string docname = getChineseTitleOfDocx(paras, indexOfChineseTitleOfDocx);  //论文中文题目

            HeaderReference headerdefault = null;
            FooterReference footerdefault = null;
            if (list.Count == 0)
                return;
            bool[] flag = { false, false, false, false }; //终止循环
            bool[] flagyj = { false, false, false}; //终止循环
            for (int i = 2; i <= list.Count<SectionProperties>(); i++)
            {

                SectionProperties s = list[i - 1];
                IEnumerable<HeaderReference> headrs = s.Elements<HeaderReference>();

                foreach (HeaderReference headr in headrs)
                {
                    if (headr.Type == HeaderFooterValues.Default)
                    {
                        headerdefault = headr;
                    }
                }

                if (headerdefault != null)
                {
                    string ID = headerdefault.Id.ToString();
                    HeaderPart hp = (HeaderPart)Mpart.GetPartById(ID);
                    Header header = hp.Header;
                    Paragraph p = header.GetFirstChild<Paragraph>();
                   
                    if (header.InnerText != null)
                    {
                        string headername = header.InnerText.Trim();
                        if (flag[0]==false&&Util.correctfonts(p, docx, CNheaderFonts, ENHeaderFonts) == false)
                        {
                            PaperFormatDetection.Tools.Util.printError("奇数页页眉字体错误，应为:" + CNheaderFonts);
                            flag[0] = true;
                        }

                        if (flag[1] == false && Util.correctsize(p, docx, headerFontsize) == false)
                        {
                            PaperFormatDetection.Tools.Util.printError("奇数页页眉字号错误，应为：" + headerFontsize);
                            flag[1] = true;
                        }
                        if (flag[2] == false && JustificationCenter(p, Mpart) == false)
                        {
                            PaperFormatDetection.Tools.Util.printError("奇数页页眉页眉未居中");
                            flag[2] = true;
                        }
                        if (flag[3] == false && headername != oddHeaderText)
                        {
                            PaperFormatDetection.Tools.Util.printError("奇数页页眉内容错误，应为：" + oddHeaderText);
                            flag[3] = true;
                        }

                    }



                }
                //页脚
                IEnumerable<FooterReference> footrs = s.Elements<FooterReference>();
                foreach (FooterReference footr in footrs)
                {
                    if (footr.Type == HeaderFooterValues.Default)
                    {
                        footerdefault = footr;
                    }
                }
                if (footerdefault != null)
                {
                    string ID = footerdefault.Id.ToString();
                    FooterPart fp = (FooterPart)Mpart.GetPartById(ID);
                    Footer footer = fp.Footer;
                    Paragraph p = footer.GetFirstChild<Paragraph>();

                    if (footer.InnerText.Trim() != "")
                    {
                        if (flagyj[0]==false&&Util.correctfonts(p, docx, CNFooterFonts, ENFooterFonts) == false)
                        {
                            Util.printError("奇数页页脚字体错误，应为：" + CNFooterFonts);
                            flagyj[0] = true;
                        }

                        if (flagyj[1] == false&&Util.correctsize(p, docx, footerFontsize) == false)
                        {
                            PaperFormatDetection.Tools.Util.printError("奇数页页脚字号错误，应为：" + footerFontsize);
                            flagyj[1] = true;
                        }
                        if (flagyj[2] == false && JustificationCenter(p, Mpart) == false)
                        {
                            PaperFormatDetection.Tools.Util.printError("奇数页页脚未居中");
                            flagyj[2] = true;
                        }
                    }
                }
            }

        }





        //判断偶数页
        protected void detectEven(WordprocessingDocument docx)
        {
            Body body = docx.MainDocumentPart.Document.Body;
            IEnumerable<Paragraph> paras = body.Elements<Paragraph>();
            MainDocumentPart Mpart = docx.MainDocumentPart;
            List<SectionProperties> list = secPrList(body);
            SectionProperties scetpr = body.GetFirstChild<SectionProperties>();
            list.Add(scetpr);


            HeaderReference headereven = null;
            FooterReference footereven = null;
            if (list.Count == 0)
                return;
            bool[] flag = { false, false, false, false }; //终止循环
            bool[] flagyj = { false, false, false }; //终止循环
            for (int i = 2; i <= list.Count<SectionProperties>(); i++)
            {
                SectionProperties s = list[i - 1];
                IEnumerable<HeaderReference> headrs = s.Elements<HeaderReference>();

                foreach (HeaderReference headr in headrs)
                {
                    if (headr.Type == HeaderFooterValues.Even)
                    {
                        headereven = headr;
                    }
                }

                if (headereven != null)
                {
                    string ID = headereven.Id.ToString();
                    HeaderPart hp = (HeaderPart)Mpart.GetPartById(ID);
                    Header header = hp.Header;
                    Paragraph p = header.GetFirstChild<Paragraph>();

                    if (header.InnerText != null)
                    {
                        string headername = header.InnerText.Trim();
                        if (flag[0]==false&&Util.correctfonts(p, docx, CNheaderFonts, ENHeaderFonts) == false)
                        {
                            PaperFormatDetection.Tools.Util.printError("偶数页页眉字体错误，应为：" + CNheaderFonts );
                            flag[0] = true;
                        }

                        if (flag[1] == false && Util.correctsize(p, docx, headerFontsize) == false)
                        {
                            PaperFormatDetection.Tools.Util.printError("偶数页页眉字号错误，应为：" + headerFontsize);
                            flag[1] = true;
                        }
                        if (flag[2] == false && JustificationCenter(p, Mpart) == false)
                        {
                            PaperFormatDetection.Tools.Util.printError("偶数页页眉未居中");
                            flag[2] = true;
                        }
                        if (flag[3] == false && headername != evenHeaderText)
                        {
                            PaperFormatDetection.Tools.Util.printError("偶数页页眉内容错误，应为：" + evenHeaderText);
                            flag[3] = true;
                        }

                    }



                }
                //页脚
                IEnumerable<FooterReference> footrs = s.Elements<FooterReference>();
                foreach (FooterReference footr in footrs)
                {
                    if (footr.Type == HeaderFooterValues.Default)
                    {
                        footereven = footr;
                    }

                }
                if (footereven != null)
                {
                    string ID = footereven.Id.ToString();
                    FooterPart fp = (FooterPart)Mpart.GetPartById(ID);
                    Footer footer = fp.Footer;
                    Paragraph p = footer.GetFirstChild<Paragraph>();

                    if (footer.InnerText != null)
                    {
                        if (flagyj[0]==false&&Util.correctfonts(p, docx, CNFooterFonts, ENFooterFonts) == false)
                        {
                            PaperFormatDetection.Tools.Util.printError( "偶数页页脚字体错误，应为" + CNFooterFonts );
                            flagyj[0] = true;
                        }

                        if (flagyj[1] == false && Util.correctsize(p, docx, footerFontsize) == false)
                        {
                            PaperFormatDetection.Tools.Util.printError( "偶数页页脚字号错误，应为：" + footerFontsize);
                            flagyj[1] = true;
                        }
                        if (flagyj[2] == false && JustificationCenter(p,Mpart) == false)
                        {
                            PaperFormatDetection.Tools.Util.printError( "偶数页页脚未居中");
                            flagyj[2] = true;
                        }
                    }
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






        //判断独创性声明的位置 在location前
        //protected void whetherNo2page(WordprocessingDocument docx, int location, string no2pageTitle)
        //{
        //    Body body = docx.MainDocumentPart.Document.Body;
        //    if (!no2PageInlocation(location, body, no2pageTitle))
        //    {
        //        PaperFormatDetection.Tools.Util.printError("独创性声明不存在或位置不正确");
        //    }
        //}
























        //获取中文标题
        protected string getChineseTitleOfDocx(IEnumerable<Paragraph> paras, int indexOfChineseTitleOfDocx)
        {
            string name = "";
            int count = 0;
            foreach (Paragraph p in paras)
            {
                if (p.InnerText.Trim().Length > 0)
                {
                    count++;
                }
                if (count == indexOfChineseTitleOfDocx)
                {
                    name = p.InnerText;
                    break;
                }
            }
            return name;

        }



        //获取所有章节属性SecPr的位置，用list保存
        static protected List<int> secPrListInt(Body body)
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
            if (body.ChildElements.GetItem(l - 1).GetType().ToString() == "DocumentFormat.OpenXml.Wordprocessing.Paragraph")
            {
                if (((Paragraph)body.ChildElements.GetItem(l - 1)).ParagraphProperties != null)
                {
                    if (((Paragraph)body.ChildElements.GetItem(l - 1)).ParagraphProperties.SectionProperties != null)
                    {
                        list.Add(l - 1);
                    }
                }

            }
            return list;
        }




        //获取所有章节属性SecPr的屬性，用list保存
        static protected List<SectionProperties> secPrList(Body body)
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
            if (body.ChildElements.GetItem(l - 1).GetType().ToString() == "DocumentFormat.OpenXml.Wordprocessing.Paragraph")
            {
                if (((Paragraph)body.ChildElements.GetItem(l - 1)).ParagraphProperties != null)
                {
                    if (((Paragraph)body.ChildElements.GetItem(l - 1)).ParagraphProperties.SectionProperties != null)
                    {
                        list.Add(((Paragraph)body.ChildElements.GetItem(l - 1)).ParagraphProperties.SectionProperties);
                    }
                }

            }
            return list;
        }


    }
}
