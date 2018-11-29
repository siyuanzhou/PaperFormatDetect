using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Office.Interop.Word;
using System.IO;
using System.Text.RegularExpressions;
using System.Data;
using System.ComponentModel;
using System.Drawing;
using System.Runtime.InteropServices;
using Word = Microsoft.Office.Interop.Word;
namespace PaperFormatDetection.Tools
{
    class ErrorReport
    {
        private Object Nothing = null;
        private Object filename = null;
        private Application WordApp = null;
        private Document WordDoc = null;
        private Paragraph oPara1;
        public ErrorReport(string paperName, List<string> eLists)
        {
            try
            {
                Nothing = System.Reflection.Missing.Value;
                string name = paperName;
                //报告生成路径与论文路径在同级路径上
                filename = Util.paperPath.Replace("Papers", "Reports");
                //Console.WriteLine("3333   " + filename);

                WriteTXT(filename.ToString().Replace("docx", "txt"), eLists);

                //创建Word文档
                WordApp = new ApplicationClass();
                WordDoc = WordApp.Documents.Add(ref Nothing, ref Nothing, ref Nothing, ref Nothing);
                //设置页眉
                WordApp.ActiveWindow.View.Type = Microsoft.Office.Interop.Word.WdViewType.wdOutlineView;
                WordApp.ActiveWindow.View.SeekView = Microsoft.Office.Interop.Word.WdSeekView.wdSeekPrimaryHeader;
                WordApp.ActiveWindow.ActivePane.Selection.InsertAfter(paperName.Replace(".docx", ""));
                WordApp.Selection.ParagraphFormat.Alignment = Microsoft.Office.Interop.Word.WdParagraphAlignment.wdAlignParagraphCenter;//设置右对齐
                WordApp.ActiveWindow.View.SeekView = Microsoft.Office.Interop.Word.WdSeekView.wdSeekMainDocument;//跳出页眉设置

                oPara1 = WordDoc.Content.Paragraphs.Add(ref Nothing);
                writeError(eLists);
                //Console.WriteLine("正在转为PDF...");
                //string PDF_filename = dir + "\\PDF_report\\" + name;
                //if (WordToPDF(filename.ToString(), PDF_filename))
                //    Console.WriteLine("检测报告转为PDF完成");
                //else
                //    Console.WriteLine("检测报告转PDF失败！");

                WordDoc.SaveAs(ref filename, ref Nothing, ref Nothing, ref Nothing,
                ref Nothing, ref Nothing, ref Nothing, ref Nothing, ref Nothing,
                ref Nothing, ref Nothing, ref Nothing, ref Nothing, ref Nothing,
                ref Nothing, ref Nothing);

                WordDoc.Close();
                WordDoc = null;
                WordApp.Quit();
                WordApp = null;

                Console.WriteLine("正在将报告转为PDF...");
                if (WordToPdf(filename.ToString()))
                    Console.WriteLine("报告转换成功！");
                else
                    Console.WriteLine("报告转换失败！");
            }
            catch (Exception ex)
            {
                killWinWord();
                Console.WriteLine(ex.Message);
                Console.WriteLine("生成报告出错！");
            }
            finally
            {
            }
        }
        public void writeError(List<string> eLists)
        {
            foreach (string str in eLists)
            {
                oPara1.Range.Text = str;
                if (str.EndsWith("检测"))
                {
                    oPara1.Range.Font.Bold = 1;
                    oPara1.Range.Font.Name = "黑体";
                    oPara1.Range.Font.Size = 12;
                    oPara1.Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphLeft;
                    oPara1.Format.SpaceAfter = 2;  //24 pt spacing after paragraph.
                    oPara1.Range.InsertParagraphAfter();
                }
                else
                {
                    oPara1.Range.Font.Name = "宋体";
                    oPara1.Range.Font.Size = 10;
                    oPara1.Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphLeft;
                    oPara1.Range.InsertParagraphAfter();
                }
            }
        }

        public bool WordToPdf(string sourcePath)
        {
            object docxPath = sourcePath;
            string pdfPath = null;
            //docxPath = @"C:\Users\Zhang_weiwei\Desktop\第四周测试论文\基于eLAMP的Linux安全加固方案设计与实现张俊霞论文.docx";
            //pdfPath = @"C:\Users\Zhang_weiwei\Desktop\第四周测试论文\基于eLAMP的Linux安全加固方案设计与实现张俊霞论文.pdf";
            pdfPath = sourcePath.Replace("docx", "pdf");
            bool result = false;
            WdExportFormat wdExportFormatPDF = WdExportFormat.wdExportFormatPDF;
            object missing = Type.Missing;
            Microsoft.Office.Interop.Word.ApplicationClass applicationClass = null;
            Document document = null;
            try
            {
                applicationClass = new Microsoft.Office.Interop.Word.ApplicationClass();
                document = applicationClass.Documents.Open(ref docxPath, ref missing, ref missing, ref missing, ref missing, ref missing, ref missing, ref missing, ref missing, ref missing, ref missing, ref missing, ref missing, ref missing, ref missing, ref missing);
                if (document != null)
                {
                    document.ExportAsFixedFormat(pdfPath, wdExportFormatPDF, false, WdExportOptimizeFor.wdExportOptimizeForPrint, WdExportRange.wdExportAllDocument, 0, 0, WdExportItem.wdExportDocumentContent, true, true, WdExportCreateBookmarks.wdExportCreateWordBookmarks, true, true, false, ref missing);
                }
                result = true;


                if (document != null)
                {
                    document.Close();
                    document = null;
                }
                if (applicationClass != null)
                {
                    applicationClass.Quit();
                    applicationClass = null;
                }
            }
            catch (Exception e)
            {
                killWinWord();
                Console.WriteLine(e.Message);
                result = false;
            }
            finally
            {
            }
            return result;
        }
        public void WriteTXT(string path, List<string> eLists)
        {
            FileStream fs = new FileStream(path, FileMode.Create, FileAccess.ReadWrite);
            //Console.WriteLine("1111  " + path);
            StreamWriter sw = new StreamWriter(fs);
            //开始写入
            foreach (string str in eLists)
            {
                sw.WriteLine(str);
            }
            //清空缓冲区
            sw.Flush();
            //关闭流
            sw.Close();
            fs.Close();
        }

        public void killWinWord()
        {
            System.Diagnostics.Process[] processes = System.Diagnostics.Process.GetProcessesByName("WINWORD");
            foreach (System.Diagnostics.Process process in processes)
            {
                bool b = process.MainWindowTitle == "";
                if (process.MainWindowTitle == "")
                {
                    process.Kill();
                }
            }
        }
    }
}
