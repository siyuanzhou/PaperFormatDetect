using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using System.IO;
using System.Text;
using DocumentFormat.OpenXml;

namespace PaperFormatDetection.Tools
{
    /**
     * 工具类
     * 所有方法都是静态方法，可以直接使用"类名.方法名"方式调用
     */
    public class Tool
    {
        /* 获取字符串的字符个数 */
        public static int GetHanNumFromString(string str)
        {
            int count = 0;
            Regex regex = new Regex(@"^[\u4E00-\u9FA5]{0,}$");
            for (int i = 0; i < str.Length; i++)
            {
                if (regex.IsMatch(str[i].ToString()))
                {
                    count++;
                }
            }
            return count;
        }
        /*常用期刊合集*/
        /* static byte[] byData = new byte[100];
         static char[] charData = new char[1000];
         static string str = null;
         public static int Read()
         {
             try
             {
                 /*string str = System.Environment.CurrentDirectory;
                 str += "\\常用期刊合集.txt";*/

        //FileStream file = new FileStream("C:\\Users\\lenovo\\Desktop\\硕士版V2.2\\PaperFormatDetection\\PaperFormatDetection\\bin\\Debug常用期刊合集.txt", FileMode.Open);
        /*  FileStream file = new FileStream("H:\\本科\\PaperFormatDetection\\PaperFormatDetection\\bin\\Debug常用期刊合集.txt", FileMode.Open);
          file.Seek(0, SeekOrigin.Begin);
          file.Read(byData, 0, 100); //byData传进来的字节数组,用以接受FileStream对象中的数据,第2个参数是字节数组中开始写入数据的位置,它通常是0,表示从数组的开端文件中向数组写数据,最后一个参数规定从文件读多少字符.
          Decoder d = Encoding.Default.GetDecoder();
          d.GetChars(byData, 0, byData.Length, charData, 0);
          //Console.WriteLine(charData);
          file.Close();
          return 1;
      }
      catch (IOException e)
      {
          Console.WriteLine(e.ToString());
          return 0;
      }

  }*/

        /*常用期刊合集*/
        static byte[] byData = new byte[2000];
        static char[] charData = new char[2000];
        static string str = null;
        public static int Read()
        {
            try
            {
                string str = Util.environmentDir;
                str += "\\常用期刊合集.txt";
                FileStream file = new FileStream(str, FileMode.Open);
                file.Seek(0, SeekOrigin.Begin);
                file.Read(byData, 0, 2000); //byData传进来的字节数组,用以接受FileStream对象中的数据,第2个参数是字节数组中开始写入数据的位置,它通常是0,表示从数组的开端文件中向数组写数据,最后一个参数规定从文件读多少字符.
                Decoder d = Encoding.Default.GetDecoder();
                d.GetChars(byData, 0, byData.Length, charData, 0);
                //Console.WriteLine(charData);
                file.Close();
                return 1;
            }
            catch (IOException e)
            {
                Console.WriteLine(e.ToString());
                return 0;
            }

        }
        public static string test()
        {
            int a = Read();
            str = new string(charData);
            return str;
        }
        /* 获取段落完整文本 */
        public static String getFullText(Paragraph p)
        {
            var list = p.ChildElements;
            Run temp_r = new Run();
            SmartTagRun temp_sr = new SmartTagRun();
            InsertedRun temp_ir = new InsertedRun();
            Hyperlink temp_ih = new Hyperlink();
            String text = "";
            foreach (var t in list)
            {
                if (t.GetType() == temp_r.GetType())
                {
                    Text pText = t.GetFirstChild<Text>();
                    if (pText != null)
                    {
                        text += pText.Text;
                    }
                }
                else if (t.GetType() == temp_sr.GetType())
                {
                    IEnumerable<Run> rr = t.Elements<Run>();
                    if (rr != null)
                    {
                        foreach (Run tr in rr)
                        {
                            Text pText = tr.GetFirstChild<Text>();
                            if (pText != null)
                            {
                                text += pText.Text;
                            }
                        }
                    }
                }
                else if (t.GetType() == temp_ir.GetType())
                {
                    Run r = t.GetFirstChild<Run>();
                    if (r != null)
                    {
                        Text pText = r.GetFirstChild<Text>();
                        if (pText != null)
                        {
                            text += pText.Text;
                        }

                    }
                }
                else if (t.GetType() == temp_ih.GetType())
                {
                    IEnumerable<Run> rr = t.Elements<Run>();
                    if (rr != null)
                    {
                        foreach (Run tr in rr)
                        {
                            Text pText = tr.GetFirstChild<Text>();
                            if (pText != null)
                            {
                                text += pText.Text;
                            }
                        }
                    }
                }
            }
            return text;
        }

        public static String getPargraphStyleId(Paragraph p)
        {
            ParagraphProperties pPr = new ParagraphProperties();
            String id = "";
            if (p.GetFirstChild<ParagraphProperties>() != null)
            {
                pPr = p.GetFirstChild<ParagraphProperties>();
            }
            if (pPr.GetFirstChild<ParagraphStyleId>() != null)
            {
                id = pPr.GetFirstChild<ParagraphStyleId>().Val;
            }
            return id;
        }

        public static int getSpaceCount(String str)
        {
            List<int> lst = new List<int>();
            char[] chr = str.ToCharArray();
            int iSpace = 0;
            foreach (char c in chr)
            {
                if (char.IsWhiteSpace(c))
                {
                    iSpace++;
                }
            }
            return iSpace;
        }

        /* 获取文档标题所在位置 */
        public static List<int> getTitlePosition(WordprocessingDocument doc)
        {
            Body body = doc.MainDocumentPart.Document.Body;
            var list = body.ChildElements;
            Paragraph temp_p = new Paragraph();
            List<int> titlePosition = new List<int>();
            int count = 0;
            string text = "";
            List<String> content = new List<string>();
            content = getContent(doc);
            Regex titleOne = new Regex("[1-9]");
            foreach (var p in list)
            {
                count++;
                if (p.GetType() == temp_p.GetType())
                {
                    Paragraph pp = (Paragraph)p;
                    Run r = p.GetFirstChild<Run>();
                    if (r != null)
                    {
                        text = getFullText(pp);
                        //trim去掉字符串左右两段的空格
                        if (text.Trim().Length > 1)
                        {
                            if (isTitle(text, content))
                            {
                                if (getFullText(pp).Trim().Length >= 3)
                                {
                                    if (pp.GetFirstChild<BookmarkStart>() != null || getFullText(pp).Trim().Substring(0, 3).Split('.').Length > 1)
                                    {
                                        titlePosition.Add(count);
                                    }
                                }
                                continue;
                            }
                            if (p.GetFirstChild<BookmarkStart>() != null)
                            {
                                if (titleOne.Match(text.Trim().Substring(0, 1)).Success)
                                {
                                    titlePosition.Add(count);
                                }
                            }
                        }
                    }
                }
            }
            return titlePosition;
        }

        /* 获得章标题位置 */
        public static List<int> getchaptertitleposition(WordprocessingDocument doc)
        {
            Body body = doc.MainDocumentPart.Document.Body;
            var list = body.ChildElements;
            Paragraph temp_p = new Paragraph();
            List<int> titlePosition = new List<int>();
            int count = 0;
            string text = "";
            Tool tool = new Tool();
            List<String> content = new List<string>();
            content = getContent(doc);
            Regex titleOne = new Regex("[1-9]");
            foreach (var p in list)
            {
                count++;
                if (p.GetType() == temp_p.GetType())
                {
                    Paragraph pp = (Paragraph)p;
                    Run r = p.GetFirstChild<Run>();
                    if (r != null)
                    {
                        text = getFullText(pp);
                        //trim去掉字符串左右两端的空格
                        if (text.Trim().Length > 1)
                        {
                            if (p.GetFirstChild<BookmarkStart>() != null)
                            {
                                if (isTitle(text, content))
                                {
                                    if (getFullText(pp).Trim().Length >= 3)
                                    {
                                        if (pp.GetFirstChild<BookmarkStart>() != null || getFullText(pp).Trim().Substring(0, 3).Split('.').Length > 1)
                                        {
                                            if (text.Trim().IndexOf('.') != 3 && text.Trim().IndexOf('.') != 1)
                                            {
                                                titlePosition.Add(count);
                                            }
                                        }
                                    }
                                    continue;
                                }
                                if (titleOne.Match(text.Trim().Substring(0, 1)).Success)
                                {
                                    if (text.Trim().IndexOf('.') != 3 && text.Trim().IndexOf('.') != 1)
                                    {
                                        titlePosition.Add(count);
                                    }
                                }
                            }
                        }
                    }
                }
            }
            return titlePosition;
        }

        /* 获取目录内容 */
        public static List<String> getContent(WordprocessingDocument doc)
        {
            Body body = doc.MainDocumentPart.Document.Body;
            MainDocumentPart mainPart = doc.MainDocumentPart;
            IEnumerable<Paragraph> paras = body.Elements<Paragraph>();
            List<String> contens = new List<string>();
            bool isContents = false;
            Tool tool = new Tool();
            foreach (Paragraph p in paras)
            {
                Run r = p.GetFirstChild<Run>();
                String fullText = "";
                if (r != null)
                {
                    fullText = getFullText(p).Trim();
                }
                if (fullText.Replace(" ", "") == "目录" && !isContents)
                {
                    isContents = true;
                    continue;
                }
                if (isContents)
                {
                    Hyperlink h = p.GetFirstChild<Hyperlink>();
                    if (h != null)
                    {
                        contens.Add(getHyperlinkFullText(h));
                    }
                }
            }
            return contens;
        }

        public static String getHyperlinkFullText(Hyperlink p)
        {
            String text = "";
            IEnumerable<Run> list = p.Elements<Run>();
            foreach (Run r in list)
            {
                Text pText = r.GetFirstChild<Text>();
                if (pText != null)
                {
                    text += pText.Text;
                }
            }
            return text;
        }
        public static bool isTitle(String str, List<String> content)
        {
            foreach (String s in content)
            {
                if (s.Contains(str))
                {
                    return true;
                }
            }
            return false;
        }
        public static List<Paragraph> toList(IEnumerable<Paragraph> paras)
        {
            List<Paragraph> list = new List<Paragraph>();
            foreach (Paragraph p in paras)
            {
                Paragraph t = p as Paragraph;
                if (t != null)
                {
                    list.Add(t);
                }
            }
            return list;
        }


        /* 判断是从某个章开始的第几个表格,图也可用 */
        public static int NumTblCha(List<int> chapter, List<int> table, int location)
        {
            int a = 0;
            int i;
            int index = -1;
            List<int> chaptertable = new List<int>();
            if (chapter.Count != 0)
            {
                for (i = 0; chapter[i] < location; i++)
                {
                    index = i;
                    if (i == chapter.Count - 1)
                        break;
                }
            }
            foreach (int tbl in table)
            {
                if (index != -1 && index <= chapter.Count - 2)
                {
                    if (tbl >= chapter[index] - 1 && tbl <= chapter[index + 1] - 1)
                    {
                        chaptertable.Add(tbl);
                    }
                }
                if (index != -1 && index == chapter.Count - 1)
                {
                    if (tbl >= chapter[index])
                    {
                        chaptertable.Add(tbl);
                    }
                }
            }
            for (int j = 0; j < chaptertable.LongCount(); j++)
            {
                if (chaptertable[j] == location)
                    a = j + 1;
            }
            if (a == 0)
            {
                return a;
            }
            return a;
        }


        /* 判断段落字体是否正确 */
        public static bool correctfonts(Paragraph p, WordprocessingDocument doc, string CNfonts, string ENfonts)
        {
            IEnumerable<Style> style = doc.MainDocumentPart.StyleDefinitionsPart.Styles.Elements<Style>();
            //正文style(default (Default Style))
            Style Normalst = null;
            foreach (Style s in style)
            {
                if (s.Type == StyleValues.Paragraph && s.Default == true)
                {
                    Normalst = s;
                    break;
                }
            }
            //pstyleid
            string pstyleid = null;
            string pbasestyleid = null;
            //段落style
            Style pstyle = null;
            string pstylefonts = null;
            string CNpstylefonts = null;
            // Style pbasestyle = null;
            string pbasestylefonts = null;
            string CNpbasestylefonts = null;
            //正文样式字体(default (Default Style))
            string Normalfonts = null;
            string CNNormalfonts = null;
            //defaults
            string Defaultsfonts = null;
            string CNDefaultsfonts = null;
            IEnumerable<Run> run = p.Elements<Run>();
            foreach (Run r in run)
            {
                string rtext = Regex.Replace(r.InnerText, @"\s*", "");
                //调试用,记得删去
                /*if(rtext!= "资料编目模块测试结果")
                {
                    continue;
                }*/
                if (rtext.Length != 0)
                {
                    bool isChinese = true;
                    Match match = Regex.Match(rtext, @"[\u4e00-\u9fa5]");
                    isChinese = match.Success ? true : false;
                    //过滤数字，数字字体没有硬性要求
                    bool isNumber = false;
                    match = Regex.Match(rtext, @"[0-9]");
                    isNumber = match.Success ? true : false;
                    if (isNumber)
                    {
                        continue;
                    }
                    //英文字母
                    bool isEnglish = false;
                    match = Regex.Match(rtext, @"[a-z]");
                    if (match.Success)
                    {
                        isEnglish = true;
                    }
                    match = Regex.Match(rtext, "@[A-Z]");
                    if (match.Success)
                    {
                        isEnglish = true;
                    }
                    //rstyleid
                    string rstyleid = null;
                    //rBaseonstyleid
                    string rBasestyleid = null;
                    //rfonts
                    string rfonts = null;
                    string CNrfonts = null;
                    //rstylefonts
                    string rstylefonts = null;
                    string CNrstylefonts = null;
                    //rBaseonfonts
                    string rBasefonts = null;
                    string CNrBasefonts = null;
                    //rstyle
                    Style rstyle = null;
                    //rBaseonstyle
                    Style rBasestyle = null;
                    if (r.RunProperties != null)
                    {
                        if (r.RunProperties.RunStyle != null)
                        {
                            if (r.RunProperties.RunStyle.Val != null)
                            {
                                rstyleid = r.RunProperties.RunStyle.Val.ToString();
                                foreach (Style s in style)
                                {
                                    if (s.StyleId == rstyleid)
                                    {
                                        rstyle = s;
                                        if (rstyle.StyleRunProperties != null)
                                        {
                                            if (rstyle.StyleRunProperties.RunFonts != null)
                                            {
                                                if (rstyle.StyleRunProperties.RunFonts.Ascii != null)
                                                {
                                                    rstylefonts = rstyle.StyleRunProperties.RunFonts.Ascii;
                                                }
                                                else if (rstyle.StyleRunProperties.RunFonts.AsciiTheme != null)
                                                {
                                                    rstylefonts = rstyle.StyleRunProperties.RunFonts.AsciiTheme;
                                                }
                                                if (rstyle.StyleRunProperties.RunFonts.EastAsia != null)
                                                {
                                                    CNrstylefonts = rstyle.StyleRunProperties.RunFonts.EastAsia;
                                                }
                                                else if (rstyle.StyleRunProperties.RunFonts.EastAsiaTheme != null)
                                                {
                                                    CNrstylefonts = rstyle.StyleRunProperties.RunFonts.EastAsiaTheme;
                                                }
                                            }
                                        }
                                        break;
                                    }
                                }
                            }
                        }
                    }

                    if (rstyle != null)
                    {
                        if (rstyle.BasedOn != null)
                        {
                            if (rstyle.BasedOn.Val != null)
                            {
                                rBasestyleid = rstyle.BasedOn.Val;
                                foreach (Style s in style)
                                {
                                    if (s.StyleId == rBasestyleid)
                                    {
                                        rBasestyle = s;
                                        if (rBasestyle.StyleRunProperties != null)
                                        {
                                            if (rBasestyle.StyleRunProperties.RunFonts != null)
                                            {
                                                if (rBasestyle.StyleRunProperties.RunFonts.Ascii != null)
                                                {
                                                    rBasefonts = rBasestyle.StyleRunProperties.RunFonts.Ascii;
                                                }
                                                else if (rBasestyle.StyleRunProperties.RunFonts.AsciiTheme != null)
                                                {
                                                    rBasefonts = rBasestyle.StyleRunProperties.RunFonts.AsciiTheme;
                                                }
                                                if (rBasestyle.StyleRunProperties.RunFonts.EastAsia != null)
                                                {
                                                    CNrBasefonts = rBasestyle.StyleRunProperties.RunFonts.EastAsia;
                                                }
                                                else if (rBasestyle.StyleRunProperties.RunFonts.EastAsiaTheme != null)
                                                {
                                                    CNrBasefonts = rBasestyle.StyleRunProperties.RunFonts.EastAsiaTheme;
                                                }
                                            }
                                        }
                                        break;
                                    }
                                }
                            }
                        }
                    }
                    RunProperties rpr = r.RunProperties;
                    if (rpr != null)
                    {
                        if (rpr.RunFonts != null)
                        {
                            if (rpr.RunFonts.Ascii != null)
                            {
                                rfonts = rpr.RunFonts.Ascii;
                            }
                            else if (rpr.RunFonts.AsciiTheme != null)
                            {
                                rfonts = rpr.RunFonts.AsciiTheme;
                            }
                            if (rpr.RunFonts.EastAsia != null)
                            {
                                CNrfonts = rpr.RunFonts.EastAsia;
                            }
                            else if (rpr.RunFonts.EastAsiaTheme != null)
                            {
                                CNrfonts = rpr.RunFonts.EastAsiaTheme;
                            }
                        }
                    }
                    if (isChinese)
                    {
                        if (CNrfonts != null)
                        {
                            if (CNrfonts != CNfonts)
                            {
                                return false;
                            }
                            else
                            {
                                return true;
                            }
                        }
                        else if (CNrstylefonts != null)
                        {
                            if (CNrstylefonts != CNfonts)
                            {
                                return false;
                            }
                            else
                            {
                                return true;
                            }
                        }
                        else if (CNrBasefonts != null)
                        {
                            if (CNrBasefonts != CNfonts)
                            {
                                return false;
                            }
                            else
                            {
                                return true;
                            }
                        }
                        else if (CNpstylefonts != null)
                        {
                            if (CNpstylefonts != CNfonts)
                            {
                                return false;
                            }
                            else
                            {
                                return true;
                            }
                        }
                        else if (CNpbasestylefonts != null)
                        {
                            if (CNpbasestylefonts != CNfonts)
                            {
                                return false;
                            }
                            else
                            {
                                return true;
                            }
                        }
                        else if (CNNormalfonts != null)
                        {
                            if (CNNormalfonts != CNfonts)
                            {
                                return false;
                            }
                            else
                            {
                                return true;
                            }
                        }
                        else if (CNDefaultsfonts != null)
                        {
                            if (CNDefaultsfonts != CNfonts)
                            {
                                return false;
                            }
                            else
                            {
                                return true;
                            }
                        }

                    }
                    if (isEnglish)
                    {
                        if (rfonts != null)
                        {
                            if (rfonts != ENfonts)
                            {
                                return false;
                            }
                            else
                            {
                                return true;
                            }
                        }
                        else if (rstylefonts != null)
                        {
                            if (rstylefonts != ENfonts)
                            {
                                return false;
                            }
                            else
                            {
                                return true;
                            }
                        }
                        else if (rBasefonts != null)
                        {
                            if (rBasefonts != ENfonts)
                            {
                                return false;
                            }
                            else
                            {
                                return true;
                            }
                        }
                        else if (pstylefonts != null)
                        {
                            if (pstylefonts != ENfonts)
                            {
                                return false;
                            }
                            else
                            {
                                return true;
                            }
                        }
                        else if (pbasestylefonts != null)
                        {
                            if (pbasestylefonts != ENfonts)
                            {
                                return false;
                            }
                            else
                            {
                                return true;
                            }
                        }
                        else if (Normalfonts != null)
                        {
                            if (Normalfonts != ENfonts)
                            {
                                return false;
                            }
                            else
                            {
                                return true;
                            }
                        }
                        else if (Defaultsfonts != null)
                        {
                            if (Defaultsfonts != ENfonts)
                            {
                                return false;
                            }
                            else
                            {
                                return true;
                            }
                        }
                    }
                }
                return true;
            }

            if (Normalst != null)
            {
                if (Normalst.StyleRunProperties != null)
                {
                    if (Normalst.StyleRunProperties.RunFonts != null)
                    {
                        if (Normalst.StyleRunProperties.RunFonts.Ascii != null)
                        {
                            Normalfonts = Normalst.StyleRunProperties.RunFonts.Ascii.ToString();
                        }
                        else if (Normalst.StyleRunProperties.RunFonts.AsciiTheme != null)
                        {
                            Normalfonts = Normalst.StyleRunProperties.RunFonts.Ascii.ToString();
                        }
                        if (Normalst.StyleRunProperties.RunFonts.EastAsia != null)
                        {
                            CNNormalfonts = Normalst.StyleRunProperties.RunFonts.EastAsia;
                        }
                        else if (Normalst.StyleRunProperties.RunFonts.EastAsiaTheme != null)
                        {
                            CNNormalfonts = Normalst.StyleRunProperties.RunFonts.EastAsiaTheme;
                        }
                    }
                }
            }

            if (doc.MainDocumentPart.StyleDefinitionsPart.Styles.DocDefaults != null)
            {
                if (doc.MainDocumentPart.StyleDefinitionsPart.Styles.DocDefaults.RunPropertiesDefault != null)
                {
                    if (doc.MainDocumentPart.StyleDefinitionsPart.Styles.DocDefaults.RunPropertiesDefault.RunPropertiesBaseStyle != null)
                    {
                        if (doc.MainDocumentPart.StyleDefinitionsPart.Styles.DocDefaults.RunPropertiesDefault.RunPropertiesBaseStyle.RunFonts != null)
                        {
                            if (doc.MainDocumentPart.StyleDefinitionsPart.Styles.DocDefaults.RunPropertiesDefault.RunPropertiesBaseStyle.RunFonts.Ascii != null)
                            {
                                Defaultsfonts = doc.MainDocumentPart.StyleDefinitionsPart.Styles.DocDefaults.RunPropertiesDefault.RunPropertiesBaseStyle.RunFonts.Ascii;
                            }
                            else if (doc.MainDocumentPart.StyleDefinitionsPart.Styles.DocDefaults.RunPropertiesDefault.RunPropertiesBaseStyle.RunFonts.AsciiTheme != null)
                            {
                                Defaultsfonts = doc.MainDocumentPart.StyleDefinitionsPart.Styles.DocDefaults.RunPropertiesDefault.RunPropertiesBaseStyle.RunFonts.AsciiTheme;
                            }

                            if (doc.MainDocumentPart.StyleDefinitionsPart.Styles.DocDefaults.RunPropertiesDefault.RunPropertiesBaseStyle.RunFonts.EastAsia != null)
                            {
                                CNDefaultsfonts = doc.MainDocumentPart.StyleDefinitionsPart.Styles.DocDefaults.RunPropertiesDefault.RunPropertiesBaseStyle.RunFonts.EastAsia;
                            }
                            else if (doc.MainDocumentPart.StyleDefinitionsPart.Styles.DocDefaults.RunPropertiesDefault.RunPropertiesBaseStyle.RunFonts.EastAsiaTheme != null)
                            {
                                CNDefaultsfonts = doc.MainDocumentPart.StyleDefinitionsPart.Styles.DocDefaults.RunPropertiesDefault.RunPropertiesBaseStyle.RunFonts.EastAsiaTheme;
                            }
                        }
                    }
                }
            }

            if (p.GetFirstChild<ParagraphProperties>() != null)
            {
                if (p.GetFirstChild<ParagraphProperties>().ParagraphStyleId != null)
                {
                    if (p.GetFirstChild<ParagraphProperties>().ParagraphStyleId.Val != null)
                    {
                        pstyleid = p.GetFirstChild<ParagraphProperties>().ParagraphStyleId.Val;
                        foreach (Style s in style)
                        {
                            if (s.StyleId == pstyleid)
                            {
                                pstyle = s;
                                if (pstyle.StyleRunProperties != null)
                                {
                                    if (pstyle.StyleRunProperties.RunFonts != null)
                                    {
                                        if (pstyle.StyleRunProperties.RunFonts.Ascii != null)
                                        {
                                            if (pstyle.StyleRunProperties.RunFonts.Ascii != null)
                                            {
                                                pstylefonts = pstyle.StyleRunProperties.RunFonts.Ascii;
                                            }
                                            else if (pstyle.StyleRunProperties.RunFonts.AsciiTheme != null)
                                            {
                                                pstylefonts = pstyle.StyleRunProperties.RunFonts.AsciiTheme;
                                            }
                                            if (pstyle.StyleRunProperties.RunFonts.EastAsia != null)
                                            {
                                                CNpstylefonts = pstyle.StyleRunProperties.RunFonts.EastAsia;
                                            }
                                            else if (pstyle.StyleRunProperties.RunFonts.EastAsiaTheme != null)
                                            {
                                                CNpstylefonts = pstyle.StyleRunProperties.RunFonts.EastAsiaTheme;
                                            }
                                        }
                                    }
                                }
                                break;
                            }
                        }
                    }
                }
            }
            //pstyle-basedon
            if (pstyle != null)
            {
                while (pstyle.BasedOn != null && (pbasestylefonts == null || CNpbasestylefonts == null))
                {
                    if (pstyle.BasedOn.Val != null)
                    {
                        pbasestyleid = pstyle.BasedOn.Val;
                    }
                    if (pbasestyleid != null)
                    {
                        foreach (Style s in style)
                        {
                            if (s.StyleId == pbasestyleid)
                            {
                                pstyle = s;
                                if (s.StyleRunProperties != null)
                                {
                                    if (s.StyleRunProperties.RunFonts != null)
                                    {
                                        if (s.StyleRunProperties.RunFonts.Ascii != null)
                                        {
                                            if (s.StyleRunProperties.RunFonts.Ascii != null && pbasestylefonts == null)
                                            {
                                                pbasestylefonts = s.StyleRunProperties.RunFonts.Ascii.ToString();
                                            }
                                            else if (s.StyleRunProperties.RunFonts.AsciiTheme != null && pbasestylefonts == null)
                                            {
                                                pbasestylefonts = s.StyleRunProperties.RunFonts.AsciiTheme;
                                            }
                                            if (s.StyleRunProperties.RunFonts.EastAsia != null && CNpbasestylefonts == null)
                                            {
                                                CNpbasestylefonts = s.StyleRunProperties.RunFonts.EastAsia;
                                            }
                                            else if (s.StyleRunProperties.RunFonts.EastAsiaTheme != null && CNpbasestylefonts == null)
                                            {
                                                CNpbasestylefonts = s.StyleRunProperties.RunFonts.EastAsiaTheme;
                                            }
                                        }
                                    }
                                }
                                break;
                            }
                        }
                    }
                }
            }

            return true;
        }

        //判断段落字体是否正确
        //ye.2016/6/10
        public static bool correctsize(Paragraph p, WordprocessingDocument doc, string size)
        {
            IEnumerable<Style> style = doc.MainDocumentPart.StyleDefinitionsPart.Styles.Elements<Style>();
            //段落style id

            //正文style
            Style Normalst = null;
            string Normalsize = null;
            foreach (Style s in style)
            {
                if (s.Type == StyleValues.Paragraph && s.Default == true)
                {
                    Normalst = s;
                    if (Normalst.StyleRunProperties != null)
                    {
                        if (Normalst.StyleRunProperties.FontSize != null)
                        {
                            if (Normalst.StyleRunProperties.FontSize.Val != null)
                            {
                                Normalsize = Normalst.StyleRunProperties.FontSize.Val;
                            }
                        }
                    }
                    break;
                }
            }
            //defaults
            string Defaultssize = null;
            if (doc.MainDocumentPart.StyleDefinitionsPart.Styles.DocDefaults != null)
            {
                if (doc.MainDocumentPart.StyleDefinitionsPart.Styles.DocDefaults.RunPropertiesDefault != null)
                {
                    if (doc.MainDocumentPart.StyleDefinitionsPart.Styles.DocDefaults.RunPropertiesDefault.RunPropertiesBaseStyle != null)
                    {
                        if (doc.MainDocumentPart.StyleDefinitionsPart.Styles.DocDefaults.RunPropertiesDefault.RunPropertiesBaseStyle.FontSize != null)
                        {
                            Defaultssize = doc.MainDocumentPart.StyleDefinitionsPart.Styles.DocDefaults.RunPropertiesDefault.RunPropertiesBaseStyle.FontSize.Val;
                        }
                    }
                }
            }
            //pstyleid
            string pstyleid = null;
            string pbasestyleid = null;
            //段落style
            Style pstyle = null;
            string pstylesize = null;
            Style pbasestyle = null;
            string pbasestylesize = null;
            if (p.GetFirstChild<ParagraphProperties>() != null)
            {
                if (p.GetFirstChild<ParagraphProperties>().ParagraphStyleId != null)
                {
                    if (p.GetFirstChild<ParagraphProperties>().ParagraphStyleId.Val != null)
                    {
                        pstyleid = p.GetFirstChild<ParagraphProperties>().ParagraphStyleId.Val;
                        foreach (Style s in style)
                        {
                            if (s.StyleId == pstyleid)
                            {
                                pstyle = s;
                                if (pstyle.StyleRunProperties != null)
                                {
                                    if (pstyle.StyleRunProperties.FontSize != null)
                                    {
                                        if (pstyle.StyleRunProperties.FontSize.Val != null)
                                        {
                                            pstylesize = pstyle.StyleRunProperties.FontSize.Val;
                                        }
                                    }
                                }
                                break;
                            }
                        }
                    }
                }
            }
            if (pstyle != null)
            {
                if (pstyle.BasedOn != null)
                {
                    string Basestyleid = null;//Baseonstyleid
                    if (pstyle.BasedOn.Val != null)
                    {
                        Basestyleid = pstyle.BasedOn.Val;
                        foreach (Style s in style)
                        {
                            if (s.StyleId == Basestyleid)
                            {
                                pbasestyle = s;
                                if (pbasestyle.StyleRunProperties != null)
                                {
                                    if (pbasestyle.StyleRunProperties.FontSize != null)
                                    {
                                        if (pbasestyle.StyleRunProperties.FontSize.Val != null)
                                        {
                                            pbasestylesize = pbasestyle.StyleRunProperties.FontSize.Val;
                                        }
                                    }
                                }
                                break;
                            }
                        }
                    }
                }
            }
            IEnumerable<Run> run = p.Elements<Run>();
            foreach (Run r in run)
            {
                if (r.InnerText != null)
                {
                    //rstyleid
                    string rstyleid = null;
                    //rBaseonstyleid
                    string rBasestyleid = null;
                    //rfonts
                    string rsize = null;
                    //rBaseonfonts
                    string rBasesize = null;
                    //rstyle
                    Style rstyle = null;
                    string rstylesize = null;
                    //rbasestyle
                    Style rBasestyle = null;
                    if (r.RunProperties != null)
                    {
                        if (r.RunProperties.RunStyle != null)
                        {
                            if (r.RunProperties.RunStyle.Val != null)
                            {
                                rstyleid = r.RunProperties.RunStyle.Val.ToString();
                                foreach (Style s in style)
                                {
                                    if (s.StyleId == rstyleid)
                                    {
                                        rstyle = s;
                                        if (rstyle.StyleRunProperties != null)
                                        {
                                            if (rstyle.StyleRunProperties.FontSize != null)
                                            {
                                                if (rstyle.StyleRunProperties.FontSize.Val != null)
                                                {
                                                    rstylesize = rstyle.StyleRunProperties.FontSize.Val;
                                                }
                                            }
                                        }
                                        break;
                                    }
                                }
                            }
                        }
                    }
                    if (rstyle != null)
                    {
                        if (rstyle.BasedOn != null)
                        {
                            if (rstyle.BasedOn.Val != null)
                            {
                                rBasestyleid = rstyle.BasedOn.Val;
                                foreach (Style s in style)
                                {
                                    if (s.StyleId == rBasestyleid)
                                    {
                                        rBasestyle = s;
                                        if (rBasestyle.StyleRunProperties != null)
                                        {
                                            if (rBasestyle.StyleRunProperties.FontSize != null)
                                            {
                                                if (rBasestyle.StyleRunProperties.FontSize.Val != null)
                                                {
                                                    rBasesize = rBasestyle.StyleRunProperties.FontSize.Val;
                                                }
                                            }
                                        }
                                        break;
                                    }
                                }
                            }
                        }
                    }
                    RunProperties rpr = r.RunProperties;
                    if (rpr != null)
                    {
                        if (rpr.FontSize != null)
                        {
                            if (rpr.FontSize.Val != null)
                            {
                                rsize = rpr.FontSize.Val;
                            }
                        }
                    }
                    if (rsize != null)
                    {
                        if (rsize != size)
                        {
                            return false;
                        }
                        else { return true; }
                    }
                    else if (rstylesize != null)
                    {
                        if (rstylesize != size)
                        {
                            return false;
                        }
                        else { return true; }
                    }
                    else if (rBasesize != null)
                    {
                        if (rBasesize != size)
                        {
                            return false;
                        }
                        else { return true; }
                    }
                    else if (pstylesize != null)
                    {
                        if (pstylesize != size)
                        {
                            return false;
                        }
                        else { return true; }
                    }
                    else if (pbasestylesize != null)
                    {
                        if (pbasestylesize != size)
                        {
                            return false;
                        }
                        else
                        {
                            return true;
                        }
                    }
                    else if (Normalsize != null)
                    {
                        if (Normalsize != size)
                        {
                            return false;
                        }
                        else
                        {
                            return true;
                        }
                    }
                    else if (Defaultssize != null)
                    {
                        if (Defaultssize != size)
                        {
                            return false;
                        }
                        else { return true; }
                    }

                }
                return true;
            }
            return true;
        }
        public static bool correctJustification(Paragraph p, WordprocessingDocument doc, string justification)
        {
            ParagraphProperties ppr = p.GetFirstChild<ParagraphProperties>();
            IEnumerable<Style> style = doc.MainDocumentPart.StyleDefinitionsPart.Styles.Elements<Style>();
            string pstyleid = null;
            Style pstyle = null;
            string pbasestyleid = null;
            //Style pbasestyle = null;
            if (ppr != null)
            {
                if (ppr.GetFirstChild<Justification>() != null)
                {
                    Justification tj = ppr.GetFirstChild<Justification>();
                    if (tj.Val != justification)
                    {
                        return false;
                    }
                    else
                    {
                        return true;
                    }
                }
                if (p.GetFirstChild<ParagraphProperties>().ParagraphStyleId != null)
                {
                    if (p.GetFirstChild<ParagraphProperties>().ParagraphStyleId.Val != null)
                    {
                        pstyleid = p.GetFirstChild<ParagraphProperties>().ParagraphStyleId.Val;
                        foreach (Style st in style)
                        {
                            pstyle = st;
                            if (st.StyleId == pstyleid)
                            {
                                if (st.StyleParagraphProperties != null)
                                {
                                    if (st.StyleParagraphProperties.Justification != null)
                                    {
                                        if (st.StyleParagraphProperties.Justification.Val.ToString() != justification)
                                        {
                                            return false;
                                        }
                                        else
                                        {
                                            return true;
                                        }
                                    }
                                }
                                break;
                            }
                        }
                        while (pstyle.BasedOn != null)
                        {
                            if (pstyle.BasedOn.Val != null)
                            {
                                pbasestyleid = pstyle.BasedOn.Val;
                                foreach (Style st in style)
                                {
                                    if (st.StyleId == pbasestyleid)
                                    {
                                        pstyle = st;
                                        if (st.StyleParagraphProperties != null)
                                        {
                                            if (st.StyleParagraphProperties.Justification != null)
                                            {
                                                if (st.StyleParagraphProperties.Justification.Val.ToString() != justification)
                                                { return false; }
                                                else { return true; }
                                            }
                                        }
                                        break;
                                    }
                                }
                            }
                        }
                    }
                }
            }
            return true;
        }
        public static bool correctSpacingBetweenLines_line(Paragraph p, WordprocessingDocument doc, string spacing_line)
        {
            IEnumerable<Style> style = doc.MainDocumentPart.StyleDefinitionsPart.Styles.Elements<Style>();

            if (p.ParagraphProperties != null)
            {
                if (p.ParagraphProperties.ParagraphPropertiesChange == null)
                {
                    if (p.ParagraphProperties.SpacingBetweenLines != null)
                    {
                        if (p.ParagraphProperties.SpacingBetweenLines.Line != null)
                        {
                            if (p.ParagraphProperties.SpacingBetweenLines.Line.Value != spacing_line)
                            {
                                return false;
                            }
                            else
                            {
                                return true;
                            }
                        }
                    }
                }
            }
            else
            {
                if (p.ParagraphProperties.ParagraphPropertiesChange.ParagraphPropertiesExtended != null)
                {
                    if (p.ParagraphProperties.ParagraphPropertiesChange.ParagraphPropertiesExtended.SpacingBetweenLines != null)
                    {
                        if (p.ParagraphProperties.ParagraphPropertiesChange.ParagraphPropertiesExtended.SpacingBetweenLines.Line != null)
                        {
                            if (p.ParagraphProperties.ParagraphPropertiesChange.ParagraphPropertiesExtended.SpacingBetweenLines.Line.Value != spacing_line)
                            {
                                return false;
                            }
                            else
                            {
                                return true;
                            }
                        }
                    }
                }
            }
            if (p.GetFirstChild<ParagraphProperties>().ParagraphStyleId != null)
            {
                if (p.GetFirstChild<ParagraphProperties>().ParagraphStyleId.Val != null)
                {
                    string pstyleid = p.GetFirstChild<ParagraphProperties>().ParagraphStyleId.Val;
                    foreach (Style s in style)
                    {
                        if (s.StyleId == pstyleid)
                        {
                            if (s.StyleParagraphProperties != null)
                            {
                                if (s.StyleParagraphProperties.SpacingBetweenLines != null)
                                {
                                    if (s.StyleParagraphProperties.SpacingBetweenLines.Line != null)
                                    {
                                        if (s.StyleParagraphProperties.SpacingBetweenLines.Line.Value != spacing_line)
                                        {
                                            return false;
                                        }
                                        else
                                        {
                                            return true;
                                        }
                                    }
                                }
                            }
                            if (s.BasedOn != null)
                            {
                                if (s.BasedOn.Val != null)
                                {
                                    string pbaseid = s.BasedOn.Val;
                                    foreach (Style s2 in style)
                                    {
                                        if (s2.StyleId == pbaseid)
                                        {
                                            if (s2.StyleParagraphProperties != null)
                                            {
                                                if (s2.StyleParagraphProperties.SpacingBetweenLines != null)
                                                {
                                                    if (s2.StyleParagraphProperties.SpacingBetweenLines.Line != null)
                                                    {
                                                        if (s2.StyleParagraphProperties.SpacingBetweenLines.Line.Value != spacing_line)
                                                        {
                                                            return false;
                                                        }
                                                        else
                                                        {
                                                            return true;
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                            break;
                        }
                    }
                }
            }
            return true;
        }
        public static bool correctSpacingBetweenLines_Be(Paragraph p, WordprocessingDocument doc, string spacing_Before)
        {
            IEnumerable<Style> style = doc.MainDocumentPart.StyleDefinitionsPart.Styles.Elements<Style>();
            if (p.ParagraphProperties != null)
            {
                if (p.ParagraphProperties.ParagraphPropertiesChange == null)
                {
                    if (p.ParagraphProperties.SpacingBetweenLines != null)
                    {

                        if (p.ParagraphProperties.SpacingBetweenLines.Before != null)
                        {
                            if (Convert.ToInt32(p.ParagraphProperties.SpacingBetweenLines.Before.Value) < Convert.ToInt32(spacing_Before) - 10 ||
                            Convert.ToInt32(p.ParagraphProperties.SpacingBetweenLines.Before.Value) > Convert.ToInt32(spacing_Before) + 10)
                            {
                                return false;
                            }
                            else
                            {
                                return true;
                            }
                        }
                    }
                }
                else
                {
                    if (p.ParagraphProperties.ParagraphPropertiesChange.ParagraphPropertiesExtended != null)
                    {
                        if (p.ParagraphProperties.ParagraphPropertiesChange.ParagraphPropertiesExtended.SpacingBetweenLines != null)
                        {

                            if (p.ParagraphProperties.ParagraphPropertiesChange.ParagraphPropertiesExtended.SpacingBetweenLines.Before != null)
                            {
                                if (Convert.ToInt32(p.ParagraphProperties.ParagraphPropertiesChange.ParagraphPropertiesExtended.SpacingBetweenLines.Before.Value)
                                < Convert.ToInt32(spacing_Before) - 10 ||
                                Convert.ToInt32(p.ParagraphProperties.ParagraphPropertiesChange.ParagraphPropertiesExtended.SpacingBetweenLines.Before.Value)
                                > Convert.ToInt32(spacing_Before) + 10)
                                {
                                    return false;
                                }
                                else
                                {
                                    return true;
                                }
                            }
                        }
                    }
                }
                if (p.GetFirstChild<ParagraphProperties>().ParagraphStyleId != null)
                {
                    if (p.GetFirstChild<ParagraphProperties>().ParagraphStyleId.Val != null)
                    {
                        string pstyleid = p.GetFirstChild<ParagraphProperties>().ParagraphStyleId.Val;
                        foreach (Style s in style)
                        {
                            if (s.StyleId == pstyleid)
                            {
                                if (s.StyleParagraphProperties != null)
                                {
                                    if (s.StyleParagraphProperties.SpacingBetweenLines != null)
                                    {
                                        if (s.StyleParagraphProperties.SpacingBetweenLines.Before != null)
                                        {
                                            if (Convert.ToInt32(s.StyleParagraphProperties.SpacingBetweenLines.Before.Value)
                                            < Convert.ToInt32(spacing_Before) - 10 ||
                                            Convert.ToInt32(s.StyleParagraphProperties.SpacingBetweenLines.Before.Value)
                                            > Convert.ToInt32(spacing_Before) + 10)
                                            {
                                                return false;
                                            }
                                            else
                                            {
                                                return true;
                                            }
                                        }
                                    }
                                }
                                if (s.BasedOn != null)
                                {
                                    if (s.BasedOn.Val != null)
                                    {
                                        string pbaseid = s.BasedOn.Val;
                                        foreach (Style s2 in style)
                                        {
                                            if (s2.StyleId == pbaseid)
                                            {
                                                if (s2.StyleParagraphProperties != null)
                                                {
                                                    if (s2.StyleParagraphProperties.SpacingBetweenLines != null)
                                                    {
                                                        if (s2.StyleParagraphProperties.SpacingBetweenLines.Before != null)
                                                        {
                                                            if (Convert.ToInt32(s2.StyleParagraphProperties.SpacingBetweenLines.Before.Value)
                                                            < Convert.ToInt32(spacing_Before) - 10 ||
                                                            Convert.ToInt32(s2.StyleParagraphProperties.SpacingBetweenLines.Before.Value)
                                                            > Convert.ToInt32(spacing_Before) + 10)
                                                            {
                                                                return false;
                                                            }
                                                            else
                                                            {
                                                                return true;
                                                            }
                                                        }
                                                    }
                                                }
                                                break;
                                            }
                                        }
                                    }
                                }
                                break;
                            }
                        }
                    }
                }
            }
            return true;
        }
        public static bool correctSpacingBetweenLines_Af(Paragraph p, WordprocessingDocument doc, string spacing_After)
        {
            IEnumerable<Style> style = doc.MainDocumentPart.StyleDefinitionsPart.Styles.Elements<Style>();
            if (p.ParagraphProperties != null)
            {
                if (p.ParagraphProperties.ParagraphPropertiesChange == null)
                {
                    if (p.ParagraphProperties.SpacingBetweenLines != null)
                    {
                        if (p.ParagraphProperties.SpacingBetweenLines.After != null)
                        {
                            if (Convert.ToInt32(p.ParagraphProperties.SpacingBetweenLines.After.Value) <
                                Convert.ToInt32(spacing_After) - 10 ||
                                Convert.ToInt32(p.ParagraphProperties.SpacingBetweenLines.After.Value) >
                                Convert.ToInt32(spacing_After) + 10)
                            {
                                return false;
                            }
                            else
                            {
                                return true;
                            }
                        }
                    }
                }
                else
                {
                    if (p.ParagraphProperties.ParagraphPropertiesChange.ParagraphPropertiesExtended != null)
                    {
                        if (p.ParagraphProperties.ParagraphPropertiesChange.ParagraphPropertiesExtended.SpacingBetweenLines != null)
                        {
                            if (p.ParagraphProperties.ParagraphPropertiesChange.ParagraphPropertiesExtended.SpacingBetweenLines.After != null)
                            {
                                if (Convert.ToInt32(p.ParagraphProperties.ParagraphPropertiesChange.ParagraphPropertiesExtended.SpacingBetweenLines.After.Value) <
                                    Convert.ToInt32(spacing_After) - 10 ||
                                    Convert.ToInt32(p.ParagraphProperties.ParagraphPropertiesChange.ParagraphPropertiesExtended.SpacingBetweenLines.After.Value) >
                                    Convert.ToInt32(spacing_After) + 10
                                    )
                                {
                                    return false;
                                }
                                else
                                {
                                    return true;
                                }
                            }
                        }
                    }
                }
                if (p.GetFirstChild<ParagraphProperties>().ParagraphStyleId != null)
                {
                    if (p.GetFirstChild<ParagraphProperties>().ParagraphStyleId.Val != null)
                    {
                        string pstyleid = p.GetFirstChild<ParagraphProperties>().ParagraphStyleId.Val;
                        foreach (Style s in style)
                        {
                            if (s.StyleId == pstyleid)
                            {
                                if (s.StyleParagraphProperties != null)
                                {
                                    if (s.StyleParagraphProperties.SpacingBetweenLines != null)
                                    {

                                        if (s.StyleParagraphProperties.SpacingBetweenLines.After != null)
                                        {
                                            if (Convert.ToInt32(s.StyleParagraphProperties.SpacingBetweenLines.After.Value)
                                                < Convert.ToInt32(spacing_After) - 10 ||
                                                Convert.ToInt32(s.StyleParagraphProperties.SpacingBetweenLines.After.Value)
                                                > Convert.ToInt32(spacing_After) + 10
                                                    )
                                            {
                                                return false;
                                            }
                                            else
                                            {
                                                return true;
                                            }
                                        }
                                    }
                                }
                                if (s.BasedOn != null)
                                {
                                    if (s.BasedOn.Val != null)
                                    {
                                        string pbaseid = s.BasedOn.Val;
                                        foreach (Style s2 in style)
                                        {
                                            if (s2.StyleId == pbaseid)
                                            {
                                                if (s2.StyleParagraphProperties != null)
                                                {
                                                    if (s2.StyleParagraphProperties.SpacingBetweenLines != null)
                                                    {

                                                        if (s2.StyleParagraphProperties.SpacingBetweenLines.After != null)
                                                        {
                                                            if (Convert.ToInt32(s2.StyleParagraphProperties.SpacingBetweenLines.After.Value) <
                                                                Convert.ToInt32(spacing_After) - 10 ||
                                                                Convert.ToInt32(s2.StyleParagraphProperties.SpacingBetweenLines.After.Value) >
                                                                Convert.ToInt32(spacing_After) + 10
                                                                )
                                                            {
                                                                return false;
                                                            }
                                                            else
                                                            {
                                                                return true;
                                                            }
                                                        }
                                                    }
                                                }
                                                break;
                                            }
                                        }
                                    }
                                }
                                break;
                            }
                        }
                    }
                }

            }
            return true;
        }

        //获得表(或其他)所在章节号
        public static string Chapter(List<int> titlePosition, int location, Body body)
        {
            string chapter = "";
            int titlelocation = -1;
            int i = 0;
            if (titlePosition.Count != 0)
            {
                for (i = 0; titlePosition[i] < location; i++)
                {
                    titlelocation = i;
                    if (i == titlePosition.Count - 1)
                        break;
                }
            }
            DocumentFormat.OpenXml.Wordprocessing.Paragraph p = null;
            if (titlelocation >= 0)
            {
                if (titlePosition[titlelocation] - 1 >= 0)
                {
                    p = (DocumentFormat.OpenXml.Wordprocessing.Paragraph)body.ChildElements.GetItem(titlePosition[titlelocation] - 1);
                }
            }
            if (p != null)
            {
                chapter = Tool.getFullText(p);
            }
            return chapter;
        }
        public static void change_fontsize(Paragraph p, string val)
        {
            IEnumerable<Run> runs = p.Elements<Run>();
            foreach (Run r in runs)
                if (r.RunProperties != null)
                {
                    if (r.RunProperties.GetFirstChild<FontSize>() != null)
                    {
                        //字号改成21，此处要分清要改的是其中的属性，还是其中的内容
                        r.RunProperties.GetFirstChild<FontSize>().Val = val;
                    }
                    else
                    {
                        //添加字号标签
                        FontSize fs = new FontSize();
                        fs.Val = val;
                        r.RunProperties.AppendChild<FontSize>(fs);
                    }
                }
        }
        public static void change_center(Paragraph p)
        {
            if (p.ParagraphProperties != null)
            {
                if (p.ParagraphProperties.GetFirstChild<Justification>() != null)
                {
                    p.ParagraphProperties.GetFirstChild<Justification>().Val = JustificationValues.Center;
                }
                else
                {

                    Justification jc = new Justification();
                    jc.Val = JustificationValues.Center;
                    p.ParagraphProperties.AppendChild<Justification>(jc);
                }
            }
        }
        public static void change_rfonts(Paragraph p, string val)
        {
            IEnumerable<Run> runs = p.Elements<Run>();
            foreach (Run r in runs)

                if (r.RunProperties != null)
                {
                    if (r.RunProperties.GetFirstChild<RunFonts>() != null)
                    {
                        //字体改，此处要分清要改的是其中的属性，还是其中的内容
                        r.RunProperties.GetFirstChild<RunFonts>().Hint = FontTypeHintValues.EastAsia;
                        r.RunProperties.GetFirstChild<RunFonts>().Ascii = val;
                        r.RunProperties.GetFirstChild<RunFonts>().EastAsia = val;
                    }
                    else
                    {
                        //添加字号标签
                        RunFonts rfont = new RunFonts() { Hint = FontTypeHintValues.EastAsia, Ascii = val, HighAnsi = val, EastAsia = val };

                        r.RunProperties.AppendChild<RunFonts>(rfont);
                    }
                }
        }
        public static void change_pfonts(Paragraph p, string val)
        {
            if (p.ParagraphProperties != null)
            {
                if (p.ParagraphProperties.ParagraphMarkRunProperties.GetFirstChild<RunFonts>() != null)
                {
                    p.ParagraphProperties.ParagraphMarkRunProperties.GetFirstChild<RunFonts>().Ascii = val;
                    p.ParagraphProperties.ParagraphMarkRunProperties.GetFirstChild<RunFonts>().EastAsia = val;
                    p.ParagraphProperties.ParagraphMarkRunProperties.GetFirstChild<RunFonts>().HighAnsi = val;
                }
                else
                {

                    RunFonts jc = new RunFonts() { Ascii = val, HighAnsi = val, EastAsia = val };
                    p.ParagraphProperties.ParagraphMarkRunProperties.AppendChild<RunFonts>(jc);
                }
            }

        }
        public static Paragraph Generatespaceline()
        {
            Paragraph paragraph1 = new Paragraph() { };
            ParagraphProperties paragraphProperties1 = new ParagraphProperties();
            WidowControl widowControl1 = new WidowControl() { Val = false };
            paragraphProperties1.Append(widowControl1);
            paragraph1.Append(paragraphProperties1);
            return paragraph1;
        }
        //添加run
        public static void GenerateRun(Paragraph p, string text)
        {
            Run run1 = new Run();

            RunProperties runProperties1 = new RunProperties();
            RunFonts runFonts1 = new RunFonts() { Hint = FontTypeHintValues.EastAsia, Ascii = "宋体", HighAnsi = "黑体", EastAsia = "宋体" };
            FontSize fontsize1 = new FontSize() { Val = "21" };
            runProperties1.Append(runFonts1);
            runProperties1.Append(fontsize1);
            Text text1 = new Text();
            text1.Text = text;
            run1.Append(runProperties1);
            run1.Append(text1);
            p.AppendChild<Run>(run1);
        }
        public static string ReplaceChar(string str, int index, char c)
        {
            if (index < 0 || index > str.Length - 1) return str;
            char[] carr = str.ToCharArray();
            carr[index] = c;
            return new string(carr);
        }
        /***************加批注*****************/
        /*缺少表名、图名时无法直接改动所以需要有个提示，或者无法确定怎么修改时使用*/
        /*paragraph p为待添加批注的段落*/
        public static void addComment(WordprocessingDocument document, Paragraph p, string comment)
        {
            Comments comments;
            int id = 1;
            if (document.MainDocumentPart.GetPartsCountOfType<WordprocessingCommentsPart>() > 0)
            {
                comments =
                    document.MainDocumentPart.WordprocessingCommentsPart.Comments;
                if (comments.HasChildren)
                {
                    if (comments.Descendants<Comment>().Select(e => Convert.ToInt32(e.Id.Value)).Max<int>() == 10)
                        id = comments.Descendants<Comment>().Select(e => Convert.ToInt32(e.Id.Value)).Max<int>();
                    else
                        id = comments.Descendants<Comment>().Select(e => Convert.ToInt32(e.Id.Value)).Max<int>();
                    id = id + 1;
                }
            }
            else
            {
                WordprocessingCommentsPart commentPart =
                            document.MainDocumentPart.AddNewPart<WordprocessingCommentsPart>();
                commentPart.Comments = new Comments();
                comments = commentPart.Comments;
            }
            Paragraph p1 = new Paragraph(new Run(new Text(comment)));
            Comment cmt = new Comment()
            {
                Id = id.ToString(),
                Author = "Red Ant",
                Initials = "yxy",
                Date = DateTime.Now.AddHours(8)
            };
            cmt.AppendChild(p1);
            comments.AppendChild(cmt);
            comments.Save();
            /************4/16新加*/
            if (p.Elements<Run>().Count<Run>() == 0)
            {
                p.AppendChild<Run>(new Run(new Text("")));
            }
            /***************/
            p.InsertBefore(new CommentRangeStart() { Id = id.ToString() }, p.GetFirstChild<Run>());

            var cmtEnd = p.InsertAfter(new CommentRangeEnd() { Id = id.ToString() }, p.Elements<Run>().Last());

            p.InsertAfter(new Run(new CommentReference() { Id = id.ToString() }), cmtEnd);

        }
        /*替换段落内容*/
        public static void replaceText(Paragraph p, string newString)
        {
            IEnumerable<Run> runs = p.Elements<Run>();
            bool first = true;
            foreach (Run run in runs)
            {
                Text te = run.GetFirstChild<Text>();
                if (te != null)
                {
                    if (first)
                    {
                        if (te.Text.Length != 0)
                        {
                            te.Text = newString;
                            first = false;
                        }
                    }
                    else
                    {
                        te.Text = "";
                    }
                }
            }
        }

        /*改成三线表*/
        public static void threeLineTable(Table t)
        {
            //获取行数

            IEnumerable<TableRow> trs = t.Elements<TableRow>();
            int len = trs.Count<TableRow>();
            if (t.GetFirstChild<TableProperties>() != null)
            {
                TableProperties tblPr = t.GetFirstChild<TableProperties>();
                if (tblPr.TableBorders != null)
                {
                    tblPr.RemoveChild<TableBorders>(tblPr.TableBorders);
                }
                TableBorders tableBorders1 = new TableBorders();
                TopBorder topBorder1 = new TopBorder() { Val = BorderValues.None, Color = "auto", Size = (UInt32Value)0U, Space = (UInt32Value)0U };
                LeftBorder leftBorder1 = new LeftBorder() { Val = BorderValues.None, Color = "auto", Size = (UInt32Value)0U, Space = (UInt32Value)0U };
                BottomBorder bottomBorder1 = new BottomBorder() { Val = BorderValues.None, Color = "auto", Size = (UInt32Value)0U, Space = (UInt32Value)0U };
                RightBorder rightBorder1 = new RightBorder() { Val = BorderValues.None, Color = "auto", Size = (UInt32Value)0U, Space = (UInt32Value)0U };
                InsideHorizontalBorder insideHorizontalBorder1 = new InsideHorizontalBorder() { Val = BorderValues.None, Color = "auto", Size = (UInt32Value)0U, Space = (UInt32Value)0U };
                InsideVerticalBorder insideVerticalBorder1 = new InsideVerticalBorder() { Val = BorderValues.None, Color = "auto", Size = (UInt32Value)0U, Space = (UInt32Value)0U };

                tableBorders1.Append(topBorder1);
                tableBorders1.Append(leftBorder1);
                tableBorders1.Append(bottomBorder1);
                tableBorders1.Append(rightBorder1);
                tableBorders1.Append(insideHorizontalBorder1);
                tableBorders1.Append(insideVerticalBorder1);

                tblPr.AppendChild<TableBorders>(tableBorders1);
            }
            int i = 0;
            foreach (TableRow tr in trs)
            {
                i++;
                /**************格式化第一行开始**********************/
                if (i == 1)
                {
                    IEnumerable<TableCell> tcs = tr.Elements<TableCell>();
                    foreach (TableCell tc in tcs)
                    {
                        if (tc.TableCellProperties != null)
                        {
                            TableCellProperties tcPr = tc.TableCellProperties;
                            if (tcPr.TableCellBorders != null)
                            {
                                tcPr.RemoveChild<TableCellBorders>(tcPr.TableCellBorders);
                            }
                            TableCellBorders tableCellBorders1 = new TableCellBorders();
                            TopBorder topBorder1 = new TopBorder() { Val = BorderValues.Single, Color = "auto", Size = (UInt32Value)4U, Space = (UInt32Value)0U };
                            BottomBorder bottomBorder1 = new BottomBorder() { Val = BorderValues.Single, Color = "auto", Size = (UInt32Value)4U, Space = (UInt32Value)0U };

                            tableCellBorders1.Append(topBorder1);
                            tableCellBorders1.Append(bottomBorder1);
                            tcPr.Append(tableCellBorders1);
                        }
                    }
                }
                /**************格式化最后一行开始**********************/
                else if (i == len)
                {
                    IEnumerable<TableCell> tcs = tr.Elements<TableCell>();
                    foreach (TableCell tc in tcs)
                    {
                        if (tc.TableCellProperties != null)
                        {
                            TableCellProperties tcPr = tc.TableCellProperties;
                            if (tcPr.TableCellBorders != null)
                            {
                                tcPr.RemoveChild<TableCellBorders>(tcPr.TableCellBorders);
                            }
                            TableCellBorders tableCellBorders1 = new TableCellBorders();
                            if (len == 2)
                            {
                                TopBorder topBorder1 = new TopBorder() { Val = BorderValues.Single, Color = "auto", Size = (UInt32Value)4U, Space = (UInt32Value)0U };
                                BottomBorder bottomBorder1 = new BottomBorder() { Val = BorderValues.Single, Color = "auto", Size = (UInt32Value)4U, Space = (UInt32Value)0U };
                                tableCellBorders1.Append(topBorder1);
                                tableCellBorders1.Append(bottomBorder1);
                                tcPr.Append(tableCellBorders1);
                                break;
                            }
                            else
                            {
                                BottomBorder bottomBorder1 = new BottomBorder() { Val = BorderValues.Single, Color = "auto", Size = (UInt32Value)4U, Space = (UInt32Value)0U };
                                tableCellBorders1.Append(bottomBorder1);
                                tcPr.Append(tableCellBorders1);
                            }
                        }
                    }
                }
                /**************格式化第2行开始（且总行数大于2）**********************/
                else if (i == 2)
                {
                    IEnumerable<TableCell> tcs = tr.Elements<TableCell>();
                    foreach (TableCell tc in tcs)
                    {
                        if (tc.TableCellProperties != null)
                        {
                            TableCellProperties tcPr = tc.TableCellProperties;
                            if (tcPr.TableCellBorders != null)
                            {
                                tcPr.RemoveChild<TableCellBorders>(tcPr.TableCellBorders);
                            }
                            TableCellBorders tableCellBorders1 = new TableCellBorders();
                            TopBorder topBorder1 = new TopBorder() { Val = BorderValues.Single, Color = "auto", Size = (UInt32Value)4U, Space = (UInt32Value)0U };

                            tableCellBorders1.Append(topBorder1);
                            tcPr.Append(tableCellBorders1);
                        }
                    }
                }
                /**************格式化中间行开始（且总行数大于2）**********************/
                else
                {
                    IEnumerable<TableCell> tcs = tr.Elements<TableCell>();
                    foreach (TableCell tc in tcs)
                    {
                        if (tc.TableCellProperties != null)
                        {
                            TableCellProperties tcPr = tc.TableCellProperties;
                            if (tcPr.TableCellBorders != null)
                            {
                                tcPr.RemoveChild<TableCellBorders>(tcPr.TableCellBorders);
                            }
                        }
                    }
                }
            }

        }
        public static void changeFontSize(Paragraph p, string size)
        {

            //Run r= p.GetFirstChild<Run>();
            IEnumerable<Run> runs = p.Elements<Run>();
            foreach (Run r in runs)
            {
                //修改run对应的字体
                if (r.RunProperties != null)
                {
                    if (r.RunProperties.GetFirstChild<FontSize>() != null)
                    {
                        //字号改成21，此处要分清要改的是其中的属性，还是其中的内容
                        r.RunProperties.GetFirstChild<FontSize>().Val = size;
                    }
                    else
                    {
                        //添加字号标签
                        FontSize fs = new FontSize();
                        fs.Val = size;
                        r.RunProperties.AppendChild<FontSize>(fs);
                    }
                    /* if (r.RunProperties.GetFirstChild<FontSizeComplexScript>() != null)
                     {
                         FontSizeComplexScript fontcs = r.RunProperties.GetFirstChild<FontSizeComplexScript>();
                        r.RunProperties.RemoveChild<FontSizeComplexScript>(fontcs);
                     }
                     r.RunProperties.AppendChild<FontSizeComplexScript>(new FontSizeComplexScript() { Val = "size" });*/
                }
            }
        }

        /*改变中英文字体*/
        /*CNFont为中文字体，ENFont为英文字体*/
        public static void changeFont(Paragraph p, string CNfont, string ENfont)
        {
            IEnumerable<Run> runs = p.Elements<Run>();
            foreach (Run r in runs)
            {
                if (r.GetFirstChild<RunFonts>() != null)
                {
                    r.RemoveChild<RunFonts>(r.GetFirstChild<RunFonts>());
                }

                r.AppendChild<RunFonts>(new RunFonts() { Ascii = ENfont, EastAsia = CNfont });
            }
        }
        public static void remmovejiachu(Paragraph p)
        {
            IEnumerable<Run> runs = p.Elements<Run>();
            foreach (Run r in runs)
            {
                RunProperties rpr = r.GetFirstChild<RunProperties>();
                if (rpr != null && rpr.GetFirstChild<Bold>() != null)
                {
                    rpr.RemoveAllChildren<Bold>();
                }
            }
        }
        public static bool correctSpacingBetweenLines_line(Paragraph p, int indexOfParagraph, List<int> list, List<SectionProperties> seclist, WordprocessingDocument doc, string spacing_line, bool isTypeOfPoint)
        {
            double spacing = Convert.ToDouble(spacing_line);
            double points = 0;
            SectionProperties secPr = null;
            for (int i = 0; i < list.Count; i++)
            {
                if (indexOfParagraph <= list[i])
                {
                    secPr = seclist[i];
                }
                else
                {
                    break;
                }
            }
            if (secPr != null)
            {
                if (secPr.GetFirstChild<DocGrid>() != null)
                {
                    if (secPr.GetFirstChild<DocGrid>().LinePitch != null)
                    {
                        //计算行高(单位：point)
                        points = secPr.GetFirstChild<DocGrid>().LinePitch.Value / 20.0;
                    }
                }
            }
            if (points == 0)
            {
                return true;
            }
            IEnumerable<Style> style = doc.MainDocumentPart.StyleDefinitionsPart.Styles.Elements<Style>();
            if (p.ParagraphProperties != null)
            {
                if (p.ParagraphProperties.ParagraphPropertiesChange == null)
                {
                    if (p.ParagraphProperties.SpacingBetweenLines != null)
                    {
                        if (p.ParagraphProperties.SpacingBetweenLines.LineRule == null || p.ParagraphProperties.SpacingBetweenLines.LineRule.InnerText == "auto")
                        {
                            if (p.ParagraphProperties.SpacingBetweenLines.Line != null)// =value/240 行
                            {
                                double value = Convert.ToDouble(p.ParagraphProperties.SpacingBetweenLines.Line);
                                if (isTypeOfPoint)
                                {
                                    if (spacing < (value / 240.0) * points - 1 || spacing > (value / 240.0) * points + 1)
                                    {
                                        return false;
                                    }
                                    return true;
                                }
                                else
                                {
                                    if (spacing < value / 240.0 - 0.1 || spacing > value / 240.0 + 1)
                                    {
                                        return false;
                                    }
                                    return true;
                                }
                            }
                        }
                        else if (p.ParagraphProperties.SpacingBetweenLines.LineRule.InnerText == "atLeast" || p.ParagraphProperties.SpacingBetweenLines.LineRule.InnerText == "exactly")
                        {
                            if (p.ParagraphProperties.SpacingBetweenLines.Line != null)//单位point
                            {
                                double value = Convert.ToDouble(p.ParagraphProperties.SpacingBetweenLines.Line);
                                if (isTypeOfPoint)
                                {
                                    if (spacing < value - 1 || spacing > value + 1)
                                    {
                                        return false;
                                    }
                                    return true;
                                }
                                else
                                {
                                    if (spacing * points < value - 1 || spacing * points > value + 1)
                                    {
                                        return false;
                                    }
                                    return true;
                                }
                            }
                        }
                    }
                }
                if (p.ParagraphProperties.ParagraphPropertiesChange != null)
                {
                    if (p.ParagraphProperties.ParagraphPropertiesChange.ParagraphPropertiesExtended != null)
                    {
                        if (p.ParagraphProperties.ParagraphPropertiesChange.ParagraphPropertiesExtended.SpacingBetweenLines != null)
                        {
                            if (p.ParagraphProperties.ParagraphPropertiesChange.ParagraphPropertiesExtended.SpacingBetweenLines.LineRule == null || p.ParagraphProperties.SpacingBetweenLines.LineRule.InnerText == "auto")
                            {
                                if (p.ParagraphProperties.ParagraphPropertiesChange.ParagraphPropertiesExtended.SpacingBetweenLines.Line != null)// =value/240 行
                                {
                                    double value = Convert.ToDouble(p.ParagraphProperties.ParagraphPropertiesChange.ParagraphPropertiesExtended.SpacingBetweenLines.Line);
                                    if (isTypeOfPoint)
                                    {
                                        if (spacing < (value / 240.0) * points - 1 || spacing > (value / 240.0) * points + 1)
                                        {
                                            return false;
                                        }
                                        return true;
                                    }
                                    else
                                    {
                                        if (spacing < value / 240.0 - 0.1 || spacing > value / 240.0 + 1)
                                        {
                                            return false;
                                        }
                                        return true;
                                    }
                                }
                            }
                            else if (p.ParagraphProperties.ParagraphPropertiesChange.ParagraphPropertiesExtended.SpacingBetweenLines.LineRule.InnerText == "atLeast" || p.ParagraphProperties.SpacingBetweenLines.LineRule.InnerText == "exactly")
                            {
                                if (p.ParagraphProperties.SpacingBetweenLines.Line != null)//单位point
                                {
                                    double value = Convert.ToDouble(p.ParagraphProperties.ParagraphPropertiesChange.ParagraphPropertiesExtended.SpacingBetweenLines.Line);
                                    if (isTypeOfPoint)
                                    {
                                        if (spacing < value - 1 || spacing > value + 1)
                                        {
                                            return false;
                                        }
                                        return true;
                                    }
                                    else
                                    {
                                        if (spacing * points < value - 1 || spacing * points > value + 1)
                                        {
                                            return false;
                                        }
                                        return true;
                                    }
                                }
                            }
                        }
                    }
                }
                if (p.GetFirstChild<ParagraphProperties>().ParagraphStyleId != null)
                {
                    if (p.GetFirstChild<ParagraphProperties>().ParagraphStyleId.Val != null)
                    {
                        string pstyleid = p.GetFirstChild<ParagraphProperties>().ParagraphStyleId.Val;
                        foreach (Style s in style)
                        {
                            if (s.StyleId == pstyleid)
                            {
                                if (s.StyleParagraphProperties != null)
                                {
                                    if (s.StyleParagraphProperties.SpacingBetweenLines != null)
                                    {
                                        if (s.StyleParagraphProperties.SpacingBetweenLines.LineRule == null || p.ParagraphProperties.SpacingBetweenLines.LineRule.InnerText == "auto")
                                        {
                                            if (s.StyleParagraphProperties.SpacingBetweenLines.Line != null)// =value/240 行
                                            {
                                                double value = Convert.ToDouble(s.StyleParagraphProperties.SpacingBetweenLines.Line);
                                                if (isTypeOfPoint)
                                                {
                                                    if (spacing < (value / 240.0) * points - 1 || spacing > (value / 240.0) * points + 1)
                                                    {
                                                        return false;
                                                    }
                                                    return true;
                                                }
                                                else
                                                {
                                                    if (spacing < value / 240.0 - 0.1 || spacing > value / 240.0 + 1)
                                                    {
                                                        return false;
                                                    }
                                                    return true;
                                                }
                                            }
                                        }
                                        else if (s.StyleParagraphProperties.SpacingBetweenLines.LineRule.InnerText == "atLeast" || p.ParagraphProperties.SpacingBetweenLines.LineRule.InnerText == "exactly")
                                        {
                                            if (s.StyleParagraphProperties.SpacingBetweenLines.Line != null)//单位point
                                            {
                                                double value = Convert.ToDouble(s.StyleParagraphProperties.SpacingBetweenLines.Line);
                                                if (isTypeOfPoint)
                                                {
                                                    if (spacing < value - 1 || spacing > value + 1)
                                                    {
                                                        return false;
                                                    }
                                                    return true;
                                                }
                                                else
                                                {
                                                    if (spacing * points < value - 1 || spacing * points > value + 1)
                                                    {
                                                        return false;
                                                    }
                                                    return true;
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                            if (s.BasedOn != null)
                            {
                                if (s.BasedOn.Val != null)
                                {
                                    string pbaseid = s.BasedOn.Val;
                                    foreach (Style s2 in style)
                                    {
                                        if (s2.StyleId == pbaseid)
                                        {
                                            if (s2.StyleParagraphProperties != null)
                                            {
                                                if (s2.StyleParagraphProperties.SpacingBetweenLines != null)
                                                {
                                                    if (s2.StyleParagraphProperties.SpacingBetweenLines.LineRule == null || p.ParagraphProperties.SpacingBetweenLines.LineRule.InnerText == "auto")
                                                    {
                                                        if (s2.StyleParagraphProperties.SpacingBetweenLines.Line != null)// =value/240 行
                                                        {
                                                            double value = Convert.ToDouble(s2.StyleParagraphProperties.SpacingBetweenLines.Line);
                                                            if (isTypeOfPoint)
                                                            {
                                                                if (spacing < (value / 240.0) * points - 1 || spacing > (value / 240.0) * points + 1)
                                                                {
                                                                    return false;
                                                                }
                                                                return true;
                                                            }
                                                            else
                                                            {
                                                                if (spacing < value / 240.0 - 0.1 || spacing > value / 240.0 + 1)
                                                                {
                                                                    return false;
                                                                }
                                                                return true;
                                                            }
                                                        }
                                                    }
                                                    else if (s2.StyleParagraphProperties.SpacingBetweenLines.LineRule.InnerText == "atLeast" || p.ParagraphProperties.SpacingBetweenLines.LineRule.InnerText == "exactly")
                                                    {
                                                        if (s2.StyleParagraphProperties.SpacingBetweenLines.Line != null)//单位point
                                                        {
                                                            double value = Convert.ToDouble(s2.StyleParagraphProperties.SpacingBetweenLines.Line);
                                                            if (isTypeOfPoint)
                                                            {
                                                                if (spacing < value - 1 || spacing > value + 1)
                                                                {
                                                                    return false;
                                                                }
                                                                return true;
                                                            }
                                                            else
                                                            {
                                                                if (spacing * points < value - 1 || spacing * points > value + 1)
                                                                {
                                                                    return false;
                                                                }
                                                                return true;
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                            break;
                        }
                    }
                }
            }
            return true;
        }
        // 参数：isTypeOfPoint，是不是判断多少磅，若是则为true,若是行则为false
        public static bool correctSpacingBetweenLines_Be(Paragraph p, int indexOfParagraph, List<int> list, List<SectionProperties> seclist, WordprocessingDocument doc, string spacing_Before, bool isTypeOfPoint)
        {
            double spacing = Convert.ToDouble(spacing_Before);
            double points = 0;
            SectionProperties secPr = null;

            for (int i = 0; i < list.Count; i++)
            {
                if (indexOfParagraph <= list[i])
                {
                    secPr = seclist[i];
                }
                else
                {
                    break;
                }
            }
            if (secPr != null)
            {
                if (secPr.GetFirstChild<DocGrid>() != null)
                {
                    if (secPr.GetFirstChild<DocGrid>().LinePitch != null)
                    {
                        //计算行高(单位：point)
                        points = secPr.GetFirstChild<DocGrid>().LinePitch.Value / 20.0;
                    }
                }
            }
            if (points == 0)
            {
                return true;
            }
            IEnumerable<Style> style = doc.MainDocumentPart.StyleDefinitionsPart.Styles.Elements<Style>();
            if (p.ParagraphProperties != null)
            {
                if (p.ParagraphProperties.SpacingBetweenLines != null)
                {
                    if (p.ParagraphProperties.SpacingBetweenLines.BeforeLines != null)//单位line
                    {
                        double value = p.ParagraphProperties.SpacingBetweenLines.BeforeLines.Value / 100.0;
                        if (isTypeOfPoint)
                        {
                            if (value * points < spacing - 1 || value > spacing + 1)
                            {
                                return false;
                            }
                            return true;
                        }
                        else
                        {
                            if (spacing < (value - 0.1) || spacing > (value + 0.1))
                            {
                                return false;
                            }
                            return true;
                        }
                    }
                    else if (p.ParagraphProperties.SpacingBetweenLines.Before != null)
                    {
                        double value = Convert.ToDouble(p.ParagraphProperties.SpacingBetweenLines.Before.Value) / 20.0;
                        if (isTypeOfPoint)
                        {
                            if (spacing < value - 1 || spacing > value + 1)
                            {
                                return false;
                            }
                            return true;
                        }
                        else
                        {
                            if (spacing * points < value - 1 || spacing * points > points + 1)
                            {
                                return false;
                            }
                            return true;
                        }
                    }
                    //应该还有一种
                }
            }
            string pstyleid = null;
            if (p.GetFirstChild<ParagraphProperties>() != null)
            {
                if (p.GetFirstChild<ParagraphProperties>().ParagraphStyleId != null)
                {
                    if (p.GetFirstChild<ParagraphProperties>().ParagraphStyleId.Val != null)
                    {
                        pstyleid = p.GetFirstChild<ParagraphProperties>().ParagraphStyleId.Val;
                        foreach (Style s in style)
                        {
                            if (s.StyleId == pstyleid)
                            {
                                if (s.StyleParagraphProperties != null)
                                {
                                    if (s.StyleParagraphProperties.SpacingBetweenLines != null)
                                    {
                                        if (s.StyleParagraphProperties.SpacingBetweenLines.BeforeLines != null)
                                        {
                                            double value = s.StyleParagraphProperties.SpacingBetweenLines.BeforeLines.Value / 100.0;
                                            if (isTypeOfPoint)
                                            {
                                                if (value * points < spacing - 1 || value * points > spacing + 1)
                                                {
                                                    return false;
                                                }
                                                return true;
                                            }
                                            else
                                            {
                                                if (spacing < value - 0.1 || spacing > value + 0.1)
                                                {
                                                    return false;
                                                }
                                                return true;
                                            }
                                        }
                                        else if (s.StyleParagraphProperties.SpacingBetweenLines.Before != null)
                                        {
                                            double value = Convert.ToDouble(s.StyleParagraphProperties.SpacingBetweenLines.Before) / 20.0;
                                            if (isTypeOfPoint)
                                            {
                                                if (spacing < value - 1 || spacing > value + 1)
                                                {
                                                    return false;
                                                }
                                                return true;
                                            }
                                            else
                                            {
                                                if (spacing * points < value - 1 || spacing * points > value + 1)
                                                {
                                                    return false;
                                                }
                                                return true;
                                            }
                                        }
                                        //还有一种auto
                                    }
                                }
                            }
                        }
                    }
                }
            }
            return true;
        }

        //list为sectionProperties的位置
        public static bool correctSpacingBetweenLines_Af(Paragraph p, int indexOfParagraph, List<int> list, List<SectionProperties> seclist, WordprocessingDocument doc, string spacing_After, bool isTypeOfPoint)
        {
            double spacing = Convert.ToDouble(spacing_After);
            double points = 0;
            SectionProperties secPr = null;
            for (int i = 0; i < list.Count; i++)
            {
                if (indexOfParagraph <= list[i])
                {
                    secPr = seclist[i];
                }
                else
                {
                    break;
                }
            }
            if (secPr != null)
            {
                if (secPr.GetFirstChild<DocGrid>() != null)
                {
                    if (secPr.GetFirstChild<DocGrid>().LinePitch != null)
                    {
                        //计算行高(单位：point)
                        points = secPr.GetFirstChild<DocGrid>().LinePitch.Value / 20.0;
                    }
                }
            }
            if (points == 0)
            {
                return true;
            }
            IEnumerable<Style> style = doc.MainDocumentPart.StyleDefinitionsPart.Styles.Elements<Style>();
            if (p.ParagraphProperties != null)
            {
                if (p.ParagraphProperties.SpacingBetweenLines != null)
                {
                    if (p.ParagraphProperties.SpacingBetweenLines.AfterLines != null)//单位line
                    {
                        double value = p.ParagraphProperties.SpacingBetweenLines.AfterLines.Value / 100.0;
                        if (isTypeOfPoint)
                        {
                            if (value * points < spacing - 1 || value * points > spacing + 1)
                            {
                                return false;
                            }
                            return true;
                        }
                        else
                        {
                            if (spacing < (value - 0.1) || spacing > (value + 0.1))
                            {
                                return false;
                            }
                            return true;
                        }
                    }
                    else if (p.ParagraphProperties.SpacingBetweenLines.After != null)
                    {
                        double value = Convert.ToDouble(p.ParagraphProperties.SpacingBetweenLines.After.Value) / 20.0;
                        if (isTypeOfPoint)
                        {
                            if (spacing < value - 1 || spacing > value + 1)
                            {
                                return false;
                            }
                            return true;
                        }
                        else
                        {
                            if (spacing * points < value - 1 || spacing * points > points + 1)
                            {
                                return false;
                            }
                            return true;
                        }
                    }
                    //应该还有一种
                }
                return true;
            }
            string pstyleid = null;
            if (p.GetFirstChild<ParagraphProperties>() != null)
            {
                if (p.GetFirstChild<ParagraphProperties>().ParagraphStyleId != null)
                {
                    if (p.GetFirstChild<ParagraphProperties>().ParagraphStyleId.Val != null)
                    {
                        pstyleid = p.GetFirstChild<ParagraphProperties>().ParagraphStyleId.Val;
                        foreach (Style s in style)
                        {
                            if (s.StyleId == pstyleid)
                            {
                                if (s.StyleParagraphProperties != null)
                                {
                                    if (s.StyleParagraphProperties.SpacingBetweenLines != null)
                                    {
                                        if (s.StyleParagraphProperties.SpacingBetweenLines.AfterLines != null)
                                        {
                                            double value = s.StyleParagraphProperties.SpacingBetweenLines.AfterLines.Value / 100.0;
                                            if (isTypeOfPoint)
                                            {
                                                if (value * points < spacing - 1 || value * points > spacing + 1)
                                                {
                                                    return false;
                                                }
                                                return true;
                                            }
                                            else
                                            {
                                                if (spacing < value - 0.1 || spacing > value + 0.1)
                                                {
                                                    return false;
                                                }
                                                return true;
                                            }
                                        }
                                        else if (s.StyleParagraphProperties.SpacingBetweenLines.After != null)
                                        {
                                            double value = Convert.ToDouble(s.StyleParagraphProperties.SpacingBetweenLines.After) / 20.0;
                                            if (isTypeOfPoint)
                                            {
                                                if (spacing < value - 1 || spacing > value + 1)
                                                {
                                                    return false;
                                                }
                                                return true;
                                            }
                                            else
                                            {
                                                if (spacing * points < value - 1 || spacing * points > value + 1)
                                                {
                                                    return false;
                                                }
                                                return true;
                                            }
                                        }
                                        //还有一种auto
                                    }
                                }
                            }
                        }
                    }
                }
            }
            return true;
        }
        //获取所有节属性用list保存
        public static List<SectionProperties> secPrList(Body body)
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
        //获取所有节属性SecPr的位置，用list保存
        static public List<int> secPrListInt(Body body)
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

        /*改变行距*/
        public static void changeSpacingLine(Paragraph p, double line)
        {
            int value = (int)(line * 240);
            string temp = value.ToString();
            ParagraphProperties ppr = p.ParagraphProperties;
            if (ppr != null)
            {
                if (ppr.GetFirstChild<SpacingBetweenLines>() != null)
                {
                    if (ppr.GetFirstChild<SpacingBetweenLines>().Line != null)
                    {
                        ppr.GetFirstChild<SpacingBetweenLines>().Line.Value = temp;
                    }
                }
            }
        }
        /*改变段前距离*/
        /*before以行为单位，例如1行则before应为100*/
        public static void changeSpacingBefore(Paragraph p, int before)
        {
            ParagraphProperties ppr = p.ParagraphProperties;
            if (before != 0)//要设置的段前距离不为0
            {
                if (ppr != null)
                {
                    SpacingBetweenLines sbl = ppr.SpacingBetweenLines;
                    if (sbl != null)
                    {
                        sbl.BeforeLines = before;
                    }
                    else
                    {
                        SpacingBetweenLines sbl2 = new SpacingBetweenLines() { BeforeLines = before };
                        ppr.AppendChild<SpacingBetweenLines>(sbl2);
                    }
                }
            }
            else//要设置的段前距离为0
            {
                if (ppr != null)
                {
                    SpacingBetweenLines sbl = ppr.SpacingBetweenLines;
                    if (sbl != null)
                    {
                        if (sbl.Before != null)
                            sbl.Before.Value = "0";
                        if (sbl.BeforeLines != null)
                            sbl.BeforeLines = before;
                    }
                    else
                    {

                        SpacingBetweenLines sbl2 = new SpacingBetweenLines() { AfterLines = 100, BeforeLines = before, Before = "0" };
                        ppr.AppendChild<SpacingBetweenLines>(sbl2);
                    }
                }
            }
        }

        /*改变段后距离*/
        /*after以行为单位，例如1行则after应为100*/
        public static void changeSpacingAfter(Paragraph p, int after)
        {
            ParagraphProperties ppr = p.ParagraphProperties;
            if (after != 0)//要设置的段前距离不为0
            {
                if (ppr != null)
                {
                    SpacingBetweenLines sbl = ppr.SpacingBetweenLines;
                    if (sbl != null)
                    {
                        sbl.AfterLines = after;
                    }
                    else
                    {
                        SpacingBetweenLines sbl2 = new SpacingBetweenLines() { AfterLines = after };
                        ppr.AppendChild<SpacingBetweenLines>(sbl2);
                    }
                }
            }
            else//要设置的段前距离为0
            {
                if (ppr != null)
                {
                    SpacingBetweenLines sbl = ppr.SpacingBetweenLines;
                    if (sbl != null)
                    {
                        if (sbl.After != null)
                            sbl.After.Value = "0";
                        if (sbl.AfterLines != null)
                            sbl.AfterLines = after;
                    }
                    else
                    {
                        SpacingBetweenLines sbl2 = new SpacingBetweenLines() { AfterLines = after, After = "0" };
                        ppr.AppendChild<SpacingBetweenLines>(sbl2);
                    }
                }
            }
        }
        //更改段后间距，以磅为单位。after为磅值
        public static void changeSpacingAfter_point(Paragraph p, int after)
        {
            ParagraphProperties ppr = p.ParagraphProperties;
            int value = after * 20;
            string temp = value.ToString();
            if (ppr != null)
            {
                if (ppr.SpacingBetweenLines != null)
                {
                    ppr.SpacingBetweenLines.Remove();
                }
                SpacingBetweenLines sbl = new SpacingBetweenLines() { After = temp };
            }
        }
        //改变首行缩进
        //p为待改变首行缩进的段落
        //charNumebr为需要缩进的字符数，如为2，则说明设置首行缩进2字符
        //
        public static void changeIntent(Paragraph p, int charNumber)
        {
            charNumber = charNumber * 100;
            ParagraphProperties ppr = p.ParagraphProperties;
            if (charNumber != 0)//要设置的段前缩进不为0
            {
                if (ppr != null)
                {
                    Indentation indent = ppr.Indentation;
                    if (indent != null)
                    {
                        indent.FirstLineChars = charNumber;
                    }
                    else
                    {
                        Indentation indent2 = new Indentation() { FirstLineChars = charNumber };
                        ppr.AppendChild<Indentation>(indent2);
                    }
                }
            }
            else//要设置的段前缩进为0
            {
                if (ppr != null)
                {
                    Indentation indent = ppr.Indentation;
                    if (indent != null)
                    {
                        indent.FirstLine.Value = "0";
                        indent.FirstLineChars = charNumber;
                    }
                    else
                    {
                        Indentation indent2 = new Indentation() { FirstLineChars = charNumber, FirstLine = "0" };
                        ppr.AppendChild<Indentation>(indent2);
                    }
                }
            }
        }


        //在改变缩进之后，检测段落首行是否有空格，若有删去
        public static void deleteSpacingChar(Paragraph p)
        {
            string s = p.InnerText;
            //计算段前空格数目
            Match match = Regex.Match(s, @"\S{0,0}(\ )*");
            int number = match.Groups.Count;

            if (number == 0)
            {
                Console.WriteLine(number);
                return;
            }
            Run r = null;
            if (p.GetFirstChild<Run>() != null)
                r = p.Elements<Run>().First();
            // Set the text for the run.
            IEnumerable<Run> runs = p.Elements<Run>();
            foreach (Run run in runs)
            {
                Text text = run.GetFirstChild<Text>();
                if (text != null)
                {
                    Match match1 = Regex.Match(text.Text, @"\S{0,0}(\ )*");
                    text.Text = text.Text.Substring(match1.Length);
                    if (text.Text.Length > 0)
                    {
                        break;
                    }
                }
            }
        }
        /*检测正文段落字体*/
        public static bool CorrectfontsPS(Paragraph p)
        {
            IEnumerable<Run> runs = p.Elements<Run>();
            foreach (Run r in runs)
            {

                if (r.GetFirstChild<RunProperties>() != null)
                {
                    if (r.GetFirstChild<RunProperties>().GetFirstChild<RunFonts>() != null)
                    {
                        if (r.GetFirstChild<RunProperties>().GetFirstChild<RunFonts>().Ascii != null
                             ||
                            r.GetFirstChild<RunProperties>().GetFirstChild<RunFonts>().EastAsia != null ||
                            r.GetFirstChild<RunProperties>().GetFirstChild<RunFonts>().ComplexScript != null)
                            return false;

                        if (r.GetFirstChild<RunProperties>().GetFirstChild<RunFonts>().HighAnsi != null)
                        {
                            if (r.GetFirstChild<RunProperties>().GetFirstChild<RunFonts>().HighAnsi != "宋体")
                                return false;
                        }
                    }
                }
            }
            return true;
        }

        /*检测正文小标题字体*/
        public static bool CorrectfontsPStittle(Paragraph p)
        {
            IEnumerable<Run> runs = p.Elements<Run>();
            foreach (Run r in runs)
            {

                if (r.GetFirstChild<RunProperties>() != null)
                {
                    if (r.GetFirstChild<RunProperties>().GetFirstChild<RunFonts>() != null)
                    {
                        if (r.GetFirstChild<RunProperties>().GetFirstChild<RunFonts>().Ascii != null
                             ||
                            r.GetFirstChild<RunProperties>().GetFirstChild<RunFonts>().EastAsia != null ||
                            r.GetFirstChild<RunProperties>().GetFirstChild<RunFonts>().ComplexScript != null)
                            return false;

                        if (r.GetFirstChild<RunProperties>().GetFirstChild<RunFonts>().HighAnsi != null)
                        {
                            if (r.GetFirstChild<RunProperties>().GetFirstChild<RunFonts>().HighAnsi != "黑体")
                                return false;
                        }
                    }
                }
            }
            return true;
        }

        /*修正正文段落和小标题字体*/
        public static void ChangefontsPS(Paragraph p)
        {
            IEnumerable<Run> runs = p.Elements<Run>();
            foreach (Run r in runs)
            {
                if (r.GetFirstChild<RunProperties>() != null)
                {
                    if (r.GetFirstChild<RunProperties>().GetFirstChild<RunFonts>() != null)
                    {
                        if (r.GetFirstChild<RunProperties>().GetFirstChild<RunFonts>().Ascii != null)
                        {
                            r.GetFirstChild<RunProperties>().GetFirstChild<RunFonts>().Ascii = null;
                        }
                        if (r.GetFirstChild<RunProperties>().GetFirstChild<RunFonts>().HighAnsi != null)
                        {
                            r.GetFirstChild<RunProperties>().GetFirstChild<RunFonts>().HighAnsi = null;
                        }
                        if (r.GetFirstChild<RunProperties>().GetFirstChild<RunFonts>().EastAsia != null)
                        {
                            r.GetFirstChild<RunProperties>().GetFirstChild<RunFonts>().EastAsia = null;
                        }
                        if (r.GetFirstChild<RunProperties>().GetFirstChild<RunFonts>().ComplexScript != null)
                        {
                            r.GetFirstChild<RunProperties>().GetFirstChild<RunFonts>().ComplexScript = null;
                        }
                    }
                }
            }
        }
        /*检测正文段落和三级小标题字号*/
        public static bool CorrectfontsizePS(Paragraph p)
        {
            IEnumerable<Run> runs = p.Elements<Run>();
            foreach (Run r in runs)
            {

                if (r.GetFirstChild<RunProperties>() != null)
                {
                    if (r.GetFirstChild<RunProperties>().GetFirstChild<FontSize>() != null)
                    {
                        if (r.GetFirstChild<RunProperties>().GetFirstChild<FontSize>().Val != null)
                        {
                            if (r.GetFirstChild<RunProperties>().GetFirstChild<FontSize>().Val != "24")

                                return false;
                        }
                    }
                }
            }
            return true;
        }
        /*修正正文段落和小标题字号*/
        public static void ChangefontsizePS(Paragraph p)
        {
            IEnumerable<Run> runs = p.Elements<Run>();
            foreach (Run r in runs)
            {
                if (r.GetFirstChild<RunProperties>() != null)
                {
                    if (r.GetFirstChild<RunProperties>().GetFirstChild<FontSize>() != null)
                    {
                        r.GetFirstChild<RunProperties>().GetFirstChild<FontSize>().Remove();
                    }
                }
            }
        }
        /*检测正文小标题位置*/
        public static bool CorrectpositiPS(Paragraph p)
        {
            if (p.GetFirstChild<ParagraphProperties>() != null)
            {
                if (p.GetFirstChild<ParagraphProperties>().GetFirstChild<Justification>() != null)
                {
                    if (p.GetFirstChild<ParagraphProperties>().GetFirstChild<Justification>().Val.ToString() != "left")
                        return false;
                }
                if (p.GetFirstChild<ParagraphProperties>().GetFirstChild<Indentation>() != null)
                {
                    return false;
                }

            }

            return true;
        }
        /*修正正文小标题位置*/
        public static void ChangepositiPS(Paragraph p)
        {
            if (p.GetFirstChild<ParagraphProperties>() != null)
            {
                if (p.GetFirstChild<ParagraphProperties>().GetFirstChild<Justification>() != null)
                {
                    p.GetFirstChild<ParagraphProperties>().GetFirstChild<Justification>().Remove();
                }

                if (p.GetFirstChild<ParagraphProperties>().GetFirstChild<Indentation>() != null)
                {

                    if (p.GetFirstChild<ParagraphProperties>().GetFirstChild<Indentation>().Left != null)
                    {
                        p.GetFirstChild<ParagraphProperties>().GetFirstChild<Indentation>().Left = null;
                    }

                    if (p.GetFirstChild<ParagraphProperties>().GetFirstChild<Indentation>().FirstLine != null)
                    {
                        p.GetFirstChild<ParagraphProperties>().GetFirstChild<Indentation>().FirstLine = null;
                    }
                    p.GetFirstChild<ParagraphProperties>().GetFirstChild<Indentation>().Remove();
                    /* if (p.GetFirstChild<ParagraphProperties>().GetFirstChild<Indentation>() == null)
                     {
                         Console.WriteLine("ppp");
                     }*/
                }
            }


        }

        public static bool CorrectfontsizePSsecond(Paragraph p)
        {
            IEnumerable<Run> runs = p.Elements<Run>();
            foreach (Run r in runs)
            {

                if (r.GetFirstChild<RunProperties>() != null)
                {
                    if (r.GetFirstChild<RunProperties>().GetFirstChild<FontSize>() != null)
                    {
                        if (r.GetFirstChild<RunProperties>().GetFirstChild<FontSize>().Val != null)
                        {
                            if (r.GetFirstChild<RunProperties>().GetFirstChild<FontSize>().Val != "28")

                                return false;
                        }
                    }
                }
            }
            return true;
        }






    }
}