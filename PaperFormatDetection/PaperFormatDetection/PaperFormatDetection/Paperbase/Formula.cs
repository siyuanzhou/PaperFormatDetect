using System;
using System.Collections.Generic;
using System.Xml;
using System.Linq;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using PaperFormatDetection.Tools;
using PaperFormatDetection.Frame;
using System.Text.RegularExpressions;

namespace PaperFormatDetection.Paperbase
{
    class Formula
    {
        protected string Justifi;
        protected string textBef;
        protected string textAft;
        protected int Sequencenum;//记录当前章节数
        protected int Chapternum;//记录当前章节中的公式的序号

        public Formula()
        {
            Sequencenum = 0;
            Chapternum = 1;
        }

        //首先找到公式
        public void SelectandCheckFormula(WordprocessingDocument doc)
        {
            Body body = doc.MainDocumentPart.Document.Body;
            IEnumerable<Paragraph> paras = body.Elements<Paragraph>();
            List<Paragraph> plist = Tool.toList(paras);
            var list = body.ChildElements;
            List<int> titleposition = Tool.getTitlePosition(doc);
            Paragraph temp = new Paragraph();
            int count = 0;
            int number = -1;
            foreach (var t in list)
            {
                count++;
                if (t.GetType() == temp.GetType())
                {
                    Paragraph p = (Paragraph)t;
                    Run r = t.GetFirstChild<Run>();
                    number++;
                    if (r != null)
                    {

                        EmbeddedObject ob = r.GetFirstChild<EmbeddedObject>();
                        OfficeMath om = r.GetFirstChild<OfficeMath>();
                        Picture pic = null;
                        IEnumerable<Run> runs = p.Elements<Run>();
                        foreach (Run rr in runs)
                        {
                            if (rr.GetFirstChild<Picture>() != null)
                                pic = rr.GetFirstChild<Picture>();
                        }

                        if (ob != null || om != null || pic != null)
                        {
                            if (plist != null && number < plist.Count - 1)
                            {


                                /*
                                 string chaper = Chapter(titleposition, count, body);
                                 if (CharpterNum != chaper.Trim().Substring(0, 1))
                                 {
                                     //正确的章节号
                                     CharpterNum = chaper.Trim().Substring(0, 1);
                                     //每章节的公式序号
                                     CharpterFormulaN = 0;
                                 }*/
                                string content = Util.getFullText(p);
                                string showcontent = content.Trim();
                                int firstnum = 0;
                                bool formula = false;
                                bool rightformula = false;
                                bool special = false;
                                bool samep = false;
                                bool haspicname = false;
                                bool ispicture = false;
                                int i;

                                //是图
                                for (i = 0; i < content.Length; i++)
                                {
                                    if (content[i] == '图')
                                        samep = true;
                                }

                                //一般来说  公式的图片和后面的序号在同一paragraph里  可据此判断是否为公式
                                for (i = 0; i < content.Length; i++)
                                {
                                    if (content[i] > 48 && content[i] < 58 && firstnum == 0 && samep == false)
                                    {

                                        // rightformula = true;
                                        formula = true;
                                        break;
                                    }
                                }


                                if (!rightformula)
                                {

                                    Paragraph nextl = plist[number + 1];
                                    string nl = Util.getFullText(nextl);

                                    //图片的下一paragraph里有“图”字  可判断此图片是图
                                    for (i = 0; i < nl.Length; i++)
                                    {
                                        if (nl[i] == '图' || nl[i] == 'F')
                                        {
                                            haspicname = true;

                                        }
                                    }

                                    if (number < plist.Count - 3)
                                    {
                                        if (plist[number + 2].InnerText.IndexOf('.') >= 0 && plist[number + 2].InnerText.IndexOf('图') >= 0)
                                            haspicname = true;

                                        //图片的上一行有“如图x.x”字样  则图片是图
                                        if (plist[number - 1].InnerText.IndexOf("如图") >= 0 || /*(plist[number - 1].InnerText == null &&*/ plist[number - 2].InnerText.IndexOf("如图") >= 0)
                                        {
                                            ispicture = true;
                                        }
                                    }

                                }

                                if (Regex.IsMatch(p.InnerText, @"[\u4e00-\u9fa5]"))
                                    special = true;
                                if (!samep && !haspicname && !ispicture && !special)
                                {

                                    // Console.WriteLine(plist[number+1].InnerText);
                                    // Console.WriteLine(plist[number - 2].InnerText);
                                    //  Console.WriteLine(p.InnerText);
                                    WhetherJustif(p);//公式部分是否居中


                                    HasNumber(p, doc, count, titleposition);//是否有编号
                                    Checkbracket(p);//括号是否正确
                                    if (!Util.correctJustification(p, doc, Justifi))
                                        Util.printError("公式整行未右对齐  ----" + "第" + Chapternum + "章" + "第" + Sequencenum + "个公式");//是否右对齐
                                    if (!Util.correctSpacingBetweenLines_Be(p, doc, textBef))
                                        Util.printError("公式的段前间距错误，应为段前0.5行  ----" + "第" + Chapternum + "章" + "第" + Sequencenum + "个公式");
                                    if (!Util.correctSpacingBetweenLines_Af(p, doc, textAft))
                                        Util.printError("公式的段后间距错误，应为段后0.5行  ----" + "第" + Chapternum + "章" + "第" + Sequencenum + "个公式");
                                }
                                else
                                    continue;
                            }
                        }
                    }
                }
            }

        }


        public void WhetherJustif(Paragraph p)
        {
            ParagraphProperties ppr = p.GetFirstChild<ParagraphProperties>();
            if (ppr != null)
            {

                if (ppr.GetFirstChild<Justification>() != null)
                {
                    if (ppr.GetFirstChild<Justification>().Val != null)
                    {
                        if (ppr.GetFirstChild<Justification>().Val == "right")
                        {
                            if (p.InnerText.ToString().Length - p.InnerText.ToString().Replace(" ", "").Length < 15)
                                Util.printError("公式部分疑似未居中  ----" + "第" + Chapternum + "章" + "第" + Sequencenum + "个公式");
                        }
                    }
                }
            }

        }
        //检测是否有编号
        public void HasNumber(Paragraph p, WordprocessingDocument doc, int count, List<int> titleposition)
        {
            bool hasnumber = false;
            string chaper = Tool.Chapter(titleposition, count, doc.MainDocumentPart.Document.Body);
            if (Chapternum.ToString() != chaper.Substring(0, 1))
            {
                Chapternum = Convert.ToInt32(chaper.Substring(0, 1));
                Sequencenum = 1;
            }
            else
            {
                Sequencenum++;
            }
            string content = Util.getFullText(p);
            int i;

            //一般来说  公式的图片和后面的序号在同一paragraph里  可据此判断是否为公式
            for (i = 0; i < content.Length; i++)
            {
                if (content[i] > 48 && content[i] < 58)
                {

                    hasnumber = true;
                    break;
                }
            }


            if (!hasnumber)
            {
                Util.printError("公式缺少编号  ----" + "第" + Chapternum + "章" + "第" + Sequencenum + "个公式");
            }
            else
            {
                CheckNum(p);
            }

        }

        public void Checkbracket(Paragraph p)
        {
            string content = Util.getFullText(p);
            if (content.IndexOf("(") >= 0 || content.IndexOf(")") >= 0)
                Util.printError("公式编号使用的括号错误，应为中文括号  ----" + "第" + Chapternum + "章第" + Sequencenum + "个公式");


        }

        public void CheckNum(Paragraph p)
        {
            string content = Util.getFullText(p);
            int i;
            int firstnum = 0;

            //一般来说  公式的图片和后面的序号在同一paragraph里  可据此判断是否为公式
            for (i = 0; i < content.Length; i++)
            {
                if (content[i] > 48 && content[i] < 58)
                {
                    firstnum = i;
                    break;
                }
            }

            if (content[firstnum].ToString() != Chapternum.ToString())
            {
                Util.printError("公式编号的章节数错误  ----" + "第" + Chapternum + "章第" + Sequencenum + "个公式");
            }

            for (i = firstnum; i < content.Length; i++)
                if (content[i] == '.')
                    break;

            if (content[i + 1].ToString() != Sequencenum.ToString())
            {
                Util.printError("公式编号的序号错误  ----" + "第" + Chapternum + "章第" + Sequencenum + "个公式");
            }

        }

    }
}