using System;
using Microsoft.Office.Interop.Word;
using System.IO;
using System.Collections.Generic;

namespace PaperFormatDetection.Tools
{
    /**
     * 页码定位器类，用于定位页码
     */
    public class PageLocator
    {
        private List<string> Pages; //用于页定位的页列表
        private bool usePageLocator; //是否使用页定位标记

        public bool UsePageLocator
        {
            get
            {
                return usePageLocator;
            }
        }
        public PageLocator()
        {

        }

        /* 构造器，当usePageLocator为true，初始化页列表Pages */
        public PageLocator(string docxPath, bool usePageLocator)
        {
            Pages = new List<string>();
            this.usePageLocator = usePageLocator;
            Application application = null;
            Document doc = null;
            if (usePageLocator)
            {
                try
                {
                    application = new Application();
                    application.Visible = false;
                    application.DisplayAlerts = WdAlertLevel.wdAlertsNone;
                    FileInfo docxFile = new FileInfo(docxPath);
                    string docxname = docxFile.FullName;
                    object Miss = System.Reflection.Missing.Value;
                    object ReadOnly = false;
                    object Visible = false;
                    doc = application.Documents.Open(docxPath, ref Miss, ref ReadOnly, ref Miss, ref Miss, ref Miss, ref Miss, ref Miss, ref Miss, ref Miss, ref Miss, ref Visible, ref Miss, ref Miss, ref Miss, ref Miss);
                    generatePageList(doc);
                    //printPageList();
                }
                finally
                {
                    doc.Close();
                    doc = null;
                    application.Quit();
                    application = null;
                }
            }

        }
        /* 生成页列表 */
        private void generatePageList(Document doc)
        {
            int NumberOfPreviousPage = -1;
            int NumberOfPage = -1;
            string InnerText = "";
            int pCount = doc.Paragraphs.Count;
            Paragraph CurrentParagraph = null;
            for (int i = 0; i < pCount; i++)
            {             
                CurrentParagraph = doc.Paragraphs[i + 1];
                InnerText = CurrentParagraph.Range.Text;
                NumberOfPage = (int)CurrentParagraph.Range.get_Information(WdInformation.wdActiveEndPageNumber);

               if (NumberOfPage == NumberOfPreviousPage)
                   Pages[Pages.Count - 1] += string.Format("\r\n{0}", InnerText);
               else
               {
                   Pages.Add(InnerText);
                   NumberOfPreviousPage = NumberOfPage;
               }
            }
            //测试，将页列表中按页顺序打印到测试目录的word_page.txt文件
            printPageList();
        }

        /**
         * 查找段落内容所在页码,返回页码有效起始数字为1
         * 不使用页码定位是返回值是-1
         */
        public int findPageNum(string paragraphText)
        {
            if (UsePageLocator == false) return -1;
            for (int i = 0; i < Pages.Count; i++)
            {
                string text = Pages[i];
                if (text.IndexOf(paragraphText) != -1)
                {
                    return i + 1;
                }
            }
            return -1;
        }

        /**
         * 从给定页数开始查找段落内容
         * 不使用页码定位是返回值是-1
         */
        public int findPageNumStartWith(int pageNum, string paragraphText)
        {
            if (UsePageLocator == false) return -1;
            if (pageNum <= 0)
            {
                pageNum = 1;
            }
            for (int i = pageNum - 1; i < Pages.Count; i++)
            {
                string text = Pages[i];
                if (text.IndexOf(paragraphText) != -1)
                {
                    return i + 1;
                }
            }
            return -1;
        }

        /* 输出页列表，用于测试 */
        private void printPageList()
        {
            /*FileStream fs = new FileStream("word_page.txt", FileMode.Create);
            StreamWriter sw = new StreamWriter(fs);*/
            for (int i = 0; i < Pages.Count; i++)
            {
                Console.WriteLine("page ===> " + (i + 1));
                Console.WriteLine(Pages[i]);
            }
            /*sw.Flush();
            sw.Close();
            fs.Close()*/;
        }
    }

}
