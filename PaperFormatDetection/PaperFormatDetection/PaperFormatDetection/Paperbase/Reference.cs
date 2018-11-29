using System;
using System.Collections.Generic;
using System.Xml;
using System.Linq;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using PaperFormatDetection.Tools;
using PaperFormatDetection.Frame;
using System.Text.RegularExpressions;
using System.IO;
using System.Text;

namespace PaperFormatDetection.Paperbase
{
    class Reference
    {
        protected string TitleJustifi;
        protected string TitleFonts;
        protected string TitleSize;
        protected string TitleBef;
        protected string TitleAft;
        protected string TitleSpacing;

        protected string ContentJustifi;
        protected string ContentFonts;
        protected string ContentSize;
        protected string ContentBef;
        protected string ContentAft;
        protected string ContentSpacing;

        protected int countRef;
        protected int countRef_J;
        protected int countCnRef;
        protected bool thesecond;
        protected bool has2p = false;

        protected int Refcount;
        protected int Refcount_J;
        protected int RefcountEn;

        public Reference()
        {
            countCnRef = 0;
            countRef = 0;
            countRef_J = 0;

            thesecond = false;
            has2p = false;
        }

        public enum RefTypes
        {
            M,//普通图书,
            J, //期刊
            C, //论文集、会议录
            D, //学位论文
            P, //专利文献
            P_OL, //专利文献包含链接地址, 标志为"[P/OL]"
            R, //科技报告
            N, //报纸
            S, //标准
            G, //汇编
            J_OL, //电子文献一种,标志为"[J/OL]"
            EB_OL, //电子文献一种,电子公告，标志为"[EB/OL]"
            C_OL, //电子文献一种,标志为"[C/OL]"
            M_OL, //电子文献一种,标志为"[M/OL]",
            Error,
            None //不是参考文献的类型
        };

        private RefTypes getRefType(string paraText)
        {

            Match match = Regex.Match(paraText.Substring(5), @"\[[A-Z]*(/OL)?\]");///////////有问题
            // Console.WriteLine(match.Value);
            if (match.Success)
            {
                string type = match.Groups[0].Value;
                string typenormal = null;
                typenormal = type.Substring(1, type.Length - 2);
                switch (typenormal)
                {
                    case "M": return RefTypes.M; //普通图书,
                    case "J": return RefTypes.J; //期刊
                    case "C": return RefTypes.C; //论文集、会议录
                    case "D": return RefTypes.D; //学位论文
                    case "P": return RefTypes.P; //专利文献
                    case "P_OL": return RefTypes.P_OL; //专利文献包含链接地址, 标志为"[P/OL]"
                    case "R": return RefTypes.R; //科技报告
                    case "N": return RefTypes.N; //报纸
                    case "S": return RefTypes.S; //标准
                    case "G": return RefTypes.G; //汇编
                    case "J/OL": return RefTypes.J_OL; //电子文献一种,标志为"[J/OL]"
                    case "EB/OL": return RefTypes.EB_OL; //电子文献一种,电子公告，标志为"[EB/OL]"
                    case "C/OL": return RefTypes.C_OL; //电子文献一种,标志为"[C/OL]"
                    case "M/OL": return RefTypes.M_OL; //电子文献一种,标志为"[M/OL]",
                    case "A":
                    case "B":
                    case "E":
                    case "F":
                    case "H":
                    case "I":
                    case "K":
                    case "L":
                    case "O":
                    case "Q":
                    case "T":
                    case "U":
                    case "V":
                    case "W":
                    case "X":
                    case "Y":
                    case "Z":
                        return RefTypes.Error;
                    default: return RefTypes.None; //不是参考文献的类型
                }
            }
            return RefTypes.None;
        }

        public void SelectandCheckRef(WordprocessingDocument doc)
        {
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
                        Util.printError("参考文献总量少于"+Refcount+"篇");
                    }
                    //检测期刊数量
                    if (countRef_J < Refcount_J)
                    {
                        Util.printError("期刊类参考文献数量少于"+Refcount_J+"篇");
                    }
                    //检测外文参考文献数量
                   /* if (countRef - countCnRef < RefcountEn)
                    {

                        Util.printError("警告：外文参考文献数量少于"+RefcountEn+"篇");

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

        public void checkReferenceTitle(Paragraph p, WordprocessingDocument doc, string fulltext)
        {
           /* if (fulltext.Replace(" ", "") != "参考文献")
            { Util.printError("参考文献的标题内容有误，应为‘参 考 文 献’"); }*/

            if (!Util.correctJustification(p, doc, TitleJustifi))
                Util.printError("参考文献标题未" + TitleJustifi);
            if (!Util.correctSpacingBetweenLines_Be(p, doc,TitleBef))
                Util.printError("参考文献标题段前间距错误，应为段前0行"/* + TitleBef*/);
            if (!Util.correctSpacingBetweenLines_Af(p, doc, TitleAft))
                Util.printError("参考文献标题段后间距错误，应为段后1行" /*+ TitleAft*/);
            if (!Util.correctSpacingBetweenLines_line(p, doc,TitleSpacing))
                Util.printError("参考文献标题行间距错误，应为1.5倍行距" /*+ TitleSpacing*/);

            if (!Util.correctfonts(p, doc, TitleFonts, TitleFonts))
                Util.printError("参考文献标题字体错误，应为" + TitleFonts);
            if (!Util.correctsize(p, doc, TitleSize))
                Util.printError("参考文献标题字号错误，应为" + TitleSize);

        }

        public void CheckContent(Paragraph p, WordprocessingDocument doc, List<Paragraph> plist, int number)
        {


            string fullText = null;
            if(!thesecond)
            {
                if (plist[number + 1].InnerText != "")
                {
                    Match match1 = Regex.Match(plist[number + 1].InnerText, @"\[[0-9](/OL)?\]");
                    Match match2 = Regex.Match(plist[number + 1].InnerText, @"\[(1|2|3|4)[0-9](/OL)?\]");
                    Match match3 = Regex.Match(plist[number + 1].InnerText, @"\[[A-Z]*(/OL)?\]");
                    Match match4 = Regex.Match(p.InnerText, @"\[[A-Z]*(/OL)?\]");
                    
                    if (!has2p &&( (!match1.Success && !match2.Success && !match3.Success&&plist[number+1].InnerText.IndexOf("致谢")<0&&plist[number+1].InnerText.IndexOf("论文情况")<0)||
                        (!match4.Success&&match3.Success&&!match1.Success&&!match2.Success)))
                    {

                        fullText = p.InnerText + plist[number + 1].InnerText;
                        has2p = true;
                        //Console.WriteLine("..." + fullText);

                    }
                    else
                    {
                        fullText = p.InnerText;

                    }
                }
                else
                {
                    fullText = p.InnerText;
                }

               // if (!Util.correctJustification(p, doc, ContentJustifi))
                //    Util.printError("参考文献未居中  ----" + fullText.Substring(0,10));
                if (!Util.correctfonts(p, doc, ContentFonts, ContentFonts))
                    Util.printError("参考文献字体错误，应为" + ContentFonts + "  ----" + fullText.Substring(0, 10));
                if (!Util.correctsize(p, doc, ContentSize))
                    Util.printError("参考文献字号错误,应为" + ContentSize + "  ----" + fullText.Substring(0, 10));
                if (!Util.correctSpacingBetweenLines_Be(p, doc, ContentBef))
                    Util.printError("参考文献段前间距错误,应为0行  ----" + fullText.Substring(0, 10));
                if (!Util.correctSpacingBetweenLines_Af(p, doc, ContentAft))
                    Util.printError("参考文献段后间距错误,应为0行  ----"+ fullText.Substring(0, 10));
                if (!Util.correctSpacingBetweenLines_line(p, doc, ContentSpacing))
                    Util.printError("参考文献行距错误,应为1.5倍行距  ----" + fullText.Substring(0, 10));
            }

            if (thesecond)
            {

                has2p = false;
                thesecond = false;
                return;
            }
            else
            {
                countRef++;

                //期刊数目统计
                RefTypes refRype = getRefType(fullText);
                if (refRype == RefTypes.J)
                {
                    countRef_J++;
                }
                //中文参考文献统计
                bool isCnRef = hasChinese(fullText);
                if (isCnRef)
                {
                    countCnRef++;
                }

                if (has2p)
                    thesecond = true;
                else
                    thesecond = false;

                CheckSpecific(fullText, isCnRef, refRype, countRef);
            }
        }

        public void CheckSpecific(string fulltext, bool isCnRef, RefTypes reftype, int count)
        {
            Match match1 = Regex.Match(fulltext, @"\[[0-9](/OL)?\]");
            Match match2 = Regex.Match(fulltext, @"\[(1|2|3|4)[0-9](/OL)?\]");

            if (match1.Success)
            {
                if (match1.Value.ToString().Substring(1, match1.Length - 2) != count.ToString())
                {
                    Util.printError("参考文献编号疑似错误  ----" + fulltext.Substring(0, 10));
                }
            }
            else if (match2.Success)
            {
                if (match2.Value.ToString().Substring(1, match2.Length - 2) != count.ToString())
                {
                    Util.printError("参考文献编号疑似错误  ----" + fulltext.Substring(0, 10));
                }
            }
            else { }

            bool hasnum = hasNum(fulltext);
            if (!hasnum)
            {
                Util.printError("参考文献缺少编号  ----" + fulltext.Substring(0, 10));
            }

            //参考文献类型检测，酌情添加对应类型的处理函数

            switch (reftype)
            {

                case RefTypes.M: checkRefType_M(fulltext, isCnRef, hasnum); break;//普通图书,
                case RefTypes.J: checkRefType_J(fulltext, isCnRef, hasnum); break;//期刊
                case RefTypes.C: checkRefType_C(fulltext, isCnRef, hasnum); break; //论文集、会议录
                case RefTypes.D: checkRefType_D(fulltext, isCnRef, hasnum); break;//学位论文
                case RefTypes.P: /*checkRefType_P(para, paranext, doc, isCnRef);*/ break;//专利文献
                case RefTypes.P_OL: break;//专利文献包含链接地址, 标志为"[P/OL]"
                case RefTypes.R: checkRefType_M(fulltext, isCnRef, hasnum); break;//科技报告
                case RefTypes.N: checkRefType_N(fulltext, isCnRef, hasnum); break;;//报纸
                case RefTypes.S: break;//标准
                case RefTypes.G: break;//汇编
                case RefTypes.J_OL: checkRefType_online(fulltext, isCnRef, hasnum); break;//电子文献一种,标志为"[J/OL]"
                case RefTypes.EB_OL: checkRefType_online(fulltext, isCnRef, hasnum); break;//电子文献一种,电子公告，标志为"[EB/OL]"
                case RefTypes.C_OL: checkRefType_online(fulltext, isCnRef, hasnum); break; //电子文献一种,标志为"[C/OL]"
                case RefTypes.M_OL: checkRefType_online(fulltext, isCnRef, hasnum); break;//电子文献一种,标志为"[M/OL]",
                case RefTypes.Error:
                    {
                        Util.printError("参考文献标志代码错误  ----" + fulltext.Substring(0, 10));
                        break;//不是参考文献的类型 
                    }
                case RefTypes.None:
                    {
                        Util.printError("参考文献缺少标志代码  ----" + fulltext.Substring(0, 10));
                        break;//不是参考文献的类型 
                    }
            }
        }



        unsafe public void checkRefType_M(string fulltext, bool isCnRef, bool hasnum)
        {
            bool hascom = false;
            checkpunctuation(fulltext, &hascom);
            
            //  bool hasnum = hasNum(fulltext);



            string[] TextArr = Regex.Split(fulltext, @"\[\w*\]");
            string TextBefore = "";
            string TextAfter = "";
            if (hasnum)
            {
                TextBefore = TextArr[1];
                TextAfter = TextArr[2];
            }

            else
            {
                if (fulltext.Substring(0, 5).IndexOf("[") >= 0 && fulltext.Substring(0, 5).IndexOf("]") >= 0)
                {
                    TextBefore = TextArr[1];
                    TextAfter = TextArr[2];
                }
                else
                {
                    TextBefore = TextArr[0];
                    TextAfter = TextArr[1];
                }
            }

            if (checkcomplete(TextBefore, TextAfter, fulltext))
                return;
            checkwriter(TextBefore, fulltext);
            checkyear(TextAfter,fulltext);
            if (isCnRef)
            {
                City(TextAfter, hascom, fulltext);
                CheckspaceBef(TextBefore, fulltext);
                CheckspaceAft(TextAfter, fulltext);
            }

            else
                CheckEngCity(TextAfter, fulltext);

        }

        unsafe public void checkRefType_N(string fulltext, bool isCnRef, bool hasnum)
        {
            bool hascom = false;
            checkpunctuation(fulltext, &hascom);
            checkyearN(fulltext);
            //  bool hasnum = hasNum(fulltext);



            string[] TextArr = Regex.Split(fulltext, @"\[\w*\]");
            string TextBefore = "";
            string TextAfter = "";
            if (hasnum)
            {
                TextBefore = TextArr[1];
                TextAfter = TextArr[2];
            }

            else
            {
                if (fulltext.Substring(0, 5).IndexOf("[") >= 0 && fulltext.Substring(0, 5).IndexOf("]") >= 0)
                {
                    TextBefore = TextArr[1];
                    TextAfter = TextArr[2];
                }
                else
                {
                    TextBefore = TextArr[0];
                }
                TextAfter = TextArr[1];
            }

            if (checkcomplete(TextBefore, TextAfter, fulltext))
                return;
            checkwriter(TextBefore, fulltext);
            if (isCnRef)
            {
                //City(TextAfter, hascom, fulltext);
                CheckspaceBef(TextBefore, fulltext);
                CheckspaceAft(TextAfter, fulltext);
            }


        }

        /*
 * 期刊类型检测检测方法
 * 标志为"J"
 */
        unsafe public void checkRefType_J(string fulltext, bool isCnRef, bool hasnum)
        {

            bool Hascom = false;
            checkpunctuation(fulltext, &Hascom);
            // bool hasnum = hasNum(fulltext);
            Match match = checkyearJ(fulltext);
            if (!Hascom)
            {
                Util.printError("参考文献缺少起止页  ----" + fulltext.Substring(0,10));
            }

            string[] textArr = Regex.Split(fulltext, @"\[\w*\]");//用中括号分割参考文献条目
            string textBef = "";
            string textAft = "";
            if (hasnum)
            {
                textBef = textArr[1];
                textAft = textArr[2];
            }

            else
            {
                if (fulltext.Substring(0, 5).IndexOf("[") >= 0 && fulltext.Substring(0, 5).IndexOf("]") >= 0)
                {
                    
                    textBef = textArr[1];
                    textAft = textArr[2];

                }
                else
                {
                    textBef = textArr[0];
                    textAft = textArr[1];
                }
            }

         
            if (checkcomplete(textBef, textAft, fulltext))
                return;
            checkwriter(textBef, fulltext);
            int AftLength = textAft.Length;
            string txt = Tool.test();

            checkafterpoint(textAft, fulltext);

           // if (!Hascom)
              //  return;

            int colonP = selectcheckissue(textBef, textAft, fulltext, match);
            if(colonP!=0)
            checkhorizantal(textAft, fulltext, colonP);

            if (isCnRef)
            {
                CheckspaceBef(textBef, fulltext);
                CheckspaceAft(textAft, fulltext);
               

            }

        }

        /*
        * 论文集、会议录类型检测方法
        * 标志为"C"
        */
        unsafe private void checkRefType_C(string fulltext, bool isCnRef, bool hasnum)
        {

            bool Hascom = false;
            checkpunctuation(fulltext, &Hascom);
            // bool hasnum = hasNum(fulltext);



            string[] textArr = Regex.Split(fulltext, @"\[\w*\]");//用中括号分割参考文献条目
            string textBef = "";
            string textAft = "";
            if (hasnum)
            {
                textBef = textArr[1];
                textAft = textArr[2];
            }

            else
            {

                if (fulltext.Substring(0, 5).IndexOf("[") >= 0 && fulltext.Substring(0, 5).IndexOf("]") >= 0)
                {
                    textBef = textArr[1];
                    textAft = textArr[2];
                }
                else
                {
                    textBef = textArr[0];
                    textAft = textArr[1];
                }
            }
            checkcomplete(textBef, textAft, fulltext);

            checkwriter(textBef, fulltext);
            checkyear(textAft,fulltext);
            if (isCnRef)
            {
                City(textAft, Hascom, fulltext);
                CheckspaceBef(textBef, fulltext);
                CheckspaceAft(textAft, fulltext);
            }
            else
                CheckEngCity(textAft, fulltext);

           
        }
        /*
         * 学位论文类型检测方法
         * 标志为"D"
         */
        unsafe private void checkRefType_D(string fulltext, bool isCnRef, bool hasnum)
        {

            bool Hascom = false;
            checkpunctuation(fulltext, &Hascom);
            // bool hasnum = hasNum(fulltext);

            string[] textArr = Regex.Split(fulltext, @"\[\w*\]");//用中括号分割参考文献条目
            string textBef = "";
            string textAft = "";
            if (hasnum)
            {
                textBef = textArr[1];
                textAft = textArr[2];
            }

            else
            {

                if (fulltext.Substring(0, 5).IndexOf("[") >= 0 && fulltext.Substring(0, 5).IndexOf("]") >= 0)
                {
                    textBef = textArr[1];
                    textAft = textArr[2];
                }
                else
                {
                    textBef = textArr[0];
                    textAft = textArr[1];
                }
            }

            checkcomplete(textBef, textAft, fulltext);

            checkwriter(textBef, fulltext);
            checkyear(textAft,fulltext);

            if (isCnRef)
            {

                checkschool(textAft, fulltext);

                City(textAft, Hascom, fulltext);
                CheckspaceBef(textBef, fulltext);
                CheckspaceAft(textAft, fulltext);
            }

            else
                CheckEngCity(textAft, fulltext);

           


        }

        private void checkRefType_P(string fulltext, bool isCnRef, bool hasnum)
        {
            //return false;
        }

        unsafe private void checkRefType_online(string fulltext, bool isCnRef, bool hasnum)
        {


            bool Hascom = false;
            checkpunctuation(fulltext, &Hascom);
            // bool hasnum = hasNum(fulltext);
            checkweb(fulltext);
            string[] textArr = Regex.Split(fulltext, @"\[.*?\]");//用中括号分割参考文献条目
            string s = textArr[0];
            string s1 = textArr[1];

            string textBef = "";
            string textAft = "";
            if (hasnum)
            {
                textBef = textArr[1];
                textAft = textArr[2];
                if (textArr.Length == 4)
                    textAft += textArr[3];
            }

            else
            {

                if (fulltext.Substring(0, 5).IndexOf("[") >= 0 && fulltext.Substring(0, 5).IndexOf("]") >= 0)
                {
                    textBef = textArr[1];
                    textAft = textArr[2];
                }
                else
                {
                    textBef = textArr[0];
                    textAft = textArr[1];
                }
            }

          
            checkcomplete(textBef, textAft, fulltext);
            checkwriter(textBef, fulltext);



            if (!isCnRef)
            {
                //英文作者首字母大写
                checkfirstword(textBef, fulltext);
            }

            else
            {
                CheckspaceBef(textBef,fulltext);
                CheckspaceAft(textAft, fulltext);
                
            }


            /* else
             {
                 Match match = Regex.Match(paraText, @"\[[A-Z]*(/OL)?\]");
                 if (match.Success)
                 {
                     if (hasnum)
                     {
                         paraText = paraText.Substring(0, paraText.IndexOf("]") + 1) + " " + textBef + paraText.Substring(match.Index - 1, match.Length + 1) + textAft;
                     }

                     else
                     {

                         paraText = "[" + count.ToString() + "]" + " " + textBef.Substring(FirstChinese(textBef)) + paraText.Substring(match.Index - 1, match.Length + 1) + textAft;
                     }

                     if (paranext != null)
                     {
                         paranext.Remove();
                     }

                     Tool.changeText(para, paraText);
                     RemoveHy(para);
                 }
             }*/


            /* if (!hasnum)
             {
                 Util.printError("缺少编号,是否应为" + count.ToString() + ":" + fulltext.Substring(0, 10));
             }*/

        }
        public void checkweb(string fulltext)
        {
            if ((fulltext.IndexOf("http://www.") == -1 && fulltext.IndexOf("https://") == -1) && fulltext.IndexOf("www.") == -1
                && fulltext.IndexOf("http://") == -1)
            {
                Util.printError("参考文献缺少网址  ----" + fulltext.Substring(0,10));
            }
        }


        public void CheckEngCity(string textaft, string fulltext)
        {
            if (textaft.IndexOf(".") >= 0 && (textaft.IndexOf(":") >= 0 || textaft.IndexOf("：") >= 0) && (textaft.IndexOf(":") > textaft.IndexOf(".") + 5 || textaft.IndexOf("：") > textaft.IndexOf(".") + 5)
                && (textaft.IndexOf(":") < textaft.IndexOf(".") + 12 || textaft.IndexOf("：") < textaft.IndexOf(".") + 12))
            { }

            else
                Util.printError("参考文献标志代码后缺少英文地名  ----" + fulltext.Substring(0,10));
        }
        public void CheckspaceBef(string textbef,string fulltext)
        {
            int i;
            for ( i = 0; i < textbef.Length - 1; i++)
            {
                if (textbef[i] != ' ')
                    break;
            }
            //排除开头为对齐的空格对判断的影响
            if (textbef.Substring(i,textbef.Length-i).IndexOf(" ") > 0)
            {

                Util.printError("参考文献标志代码前的内容中疑似有多余空格  ----" + fulltext.Substring(0,10));
            }

        }

           public void CheckspaceAft(string textaft,string fulltext)
        {
            if (textaft.IndexOf(" ") >= 0&&textaft.IndexOf(" ")!=textaft.Length-1)
            {
                Util.printError("参考文献标志代码后的内容中疑似有多余空格  ----" + fulltext.Substring(0, 10));
            }

        }
        public void checkfirstword(string textBef, string fulltext)
        {
            int index = textBef.IndexOf('.');
            if (index >= 0)
            {
                string[] autorname = Regex.Split(textBef.Substring(0, index), @",");
                for (int i = 0; i < autorname.Length; i++)
                {
                    //找到第一个字母位置
                    Match match = Regex.Match(autorname[i], @"\w");
                    if (match.Index != -1)
                    {
                        if (autorname[i][match.Index] <= 'A' || autorname[i][match.Index] >= 'Z')
                        {
                            Util.printError("参考文献英文作者首字母应大写  ----" + fulltext.Substring(0,10));
                            break;
                        }
                    }
                }
            }
        }




        public void checkschool(string textAft, string fulltext)
        {
            if (textAft.IndexOf("学院") < 0 && textAft.IndexOf("系") < 0)
            {
                Util.printError("参考文献缺少授学位单位（院或系）  ----" + fulltext.Substring(0,10));
            }
        }
        public int selectcheckissue(string textBef, string textAft, string fulltext, Match match)
        {
            
            int colonP = 0;
            int AftLength = textAft.Length;
            for (int a = AftLength - 1; a > 0; a--)
            {
                if (textAft[a] == ':')
                {
                    colonP = a;
                    break;
                }
            }
            if (colonP == 0)
            {
                
                if (textAft.IndexOf("-")<0 &&textAft.IndexOf("~") < 0)
                {
                    Util.printError("参考文献缺少起止页  ----" + fulltext.Substring(0,10));
                }
                else
                {
                    Util.printError("参考文献期号和起止页间未隔开  ----" + fulltext.Substring(0,10));
                }
                //return 0;
            }
            bool Findissue = false;
            int right = 0;
            int left = 0;

            for (int a = textAft.Length - 1; a > 0; a--)
            {
                if (textAft[a] == ')' || textAft[a] == '）')
                {
                    
                    right = a;

                    for (int b = a; b > 0; b--)
                    {
                        if (textAft[b] == '(' || textAft[b] == '（')
                        {

                            left = b;
                            Findissue = true;
                            break;
                        }
                    }
                    break;
                }
            }


            bool AllNum = true;
            if (Findissue)
            {

                for (int a = left + 1; a < right; a++)
                {
                    if (textAft[a] < 48 || textAft[a] > 57)
                    {
                        AllNum = false;
                        break;
                    }
                }

                checkreel(textBef, textAft, fulltext, left, match);
            }

            if (Findissue && !AllNum)
            {

               // Util.printError("期号应全为数字:" + fulltext);
            }

            else if (!Findissue)
            {

                Util.printError("参考文献缺少出版期号  ----" + fulltext.Substring(0,10));

            }

           /* else
            {

                string issue = textAft.Substring(left + 1, right - left - 1);
                if (Convert.ToInt32(issue) > 20)
                {

                    Util.printError("警告：参考文献出版期号过大  ----" + fulltext.Substring(0,10));

                }
            }*/
            
            return colonP;
        }
        public void checkreel(string textBef, string textAft, string fulltext, int left, Match match)
        {
            
            int reelP = 0;
            bool hasReel = true;
            /*for (int a = left - 1; a > 0; a--)
            {
                if (textAft[a] == ',')
                {
                    reelP = a;
                    break;
                }
                if (textAft[a] == '.'&&match.Success)
                {
                    Util.printError("参考文献出版年份与期卷号之间的标点符号错误  ----" + fulltext.Substring(0,10));
                    return;
                }
            }*/
            bool realReel = true;
            Match matchh = Regex.Match(textAft.Substring(reelP,6), @"[1-2][0-9][0-9][0-9]");
            if (matchh.Success)
                hasReel = false;
                

            else if (reelP == left - 1)
            {
                hasReel = false;
            }
            else if (match.Success && textAft.Substring(reelP + 1, left - reelP - 1) == match.Value)
            {
                hasReel = false;
            }

            

           /* else
            {

                for (int a = reelP + 1; a < left; a++)
                {
                    if (textAft[a] < 48 || textAft[a] > 57)
                    {
                        realReel = false;
                        if (textAft[1] == ' ')
                            Util.printError("卷号当中不应有空格：" + fulltext);
                        else
                        {

                            Util.printError("卷号应全为数字:" + fulltext);
                        }

                        break;

                    }
                }
            }*/

           /* if (!hasReel)
            {
                Util.printError("警告：疑似缺少卷号:" + fulltext);
            }*/

            if (match.Success && hasReel && realReel)
            {

                checkreelAndyear(textBef, textAft, fulltext, match, left, reelP);
            }


        }

        public void checkreelAndyear(string textBef, string textAft, string fulltext, Match match, int left, int reelP)
        {
            string txt = test();
            int year = Convert.ToInt32(match.Value);
            DateTime now = DateTime.Now;
            if (year <= now.Year)
            {
                Match match2 = Regex.Match(fulltext, @"[1-2][0-9][0-9][0-9]");
                string Writtenreel = textAft.Substring(reelP + 1, left - reelP - 1);
                int writtenreel = Convert.ToInt32(Writtenreel);
                string Writtenyear = fulltext.Substring(match2.Index, 4);
                int writtenyear = Convert.ToInt32(Writtenyear);
                if (textAft[0] == '.')
                {
                    string PublishHouse = "";
                    for (int i = 1; i < textAft.Length; i++)
                    {
                        if (textAft[i] == ',')
                        {
                            PublishHouse = textAft.Substring(1, i - 1);

                            break;
                        }
                    }
                    int indexx = txt.IndexOf(PublishHouse.Trim());

                    if (indexx != -1)
                    {
                        int firstyearbeg = 0;
                        for (int a = indexx; a < txt.Length; a++)
                        {
                            if (txt[a] >= 48 && txt[a] <= 57)
                            {
                                firstyearbeg = a;
                                break;
                            }
                        }

                        if (firstyearbeg != 0)
                        {
                            string firstyear = txt.Substring(firstyearbeg, 4);
                            int Firstyear = Convert.ToInt32(firstyear);

                            if ((Firstyear + writtenreel) - writtenyear - 1 == 0)
                            { }


                            else if (((Firstyear + writtenreel - writtenyear - 1 < 10) && (Firstyear + writtenreel - writtenyear - 1 > 0)) ||
                                   ((Firstyear + writtenreel - writtenyear - 1 > -10) && (Firstyear + writtenreel - writtenyear - 1 < 0)))
                            {

                                Util.printError("参考文献卷号与出版社创刊年份不符  ----" + fulltext.Substring(0,10));
                            }

                           /* else
                            {

                                Util.printError("卷号与出版社标注的卷号相差过大:" + fulltext);
                            }*/
                        }
                    }
                }
            }
        }
        public void checkhorizantal(string textAft, string fulltext, int colonP)
        {
            int AftLength = textAft.Length;
            for (int a = colonP; a < AftLength; a++)
            {
                if (textAft[a] == '~')
                {
                    Util.printError("参考文献起止页间符号错误，应为“-”  ----" + fulltext.Substring(0, 10));
                }
            }
        }
        public void checkafterpoint(string textAft, string fulltext)
        {
            if (textAft[0] != '.' && (textAft[0] > 127 || Char.IsNumber(textAft[0])))
            {
                Util.printError("参考文献标志代码后缺少标点符号“.”  ----" + fulltext.Substring(0,10));
            }
            else if (textAft[0] != '.')
            {
                Util.printError("参考文献标志代码后的标点符号错误，应为“.”  ----" + fulltext.Substring(0, 10));
            }
        }

        public void checkwriter(string TextBefore, string fulltext)
        {
            int index = TextBefore.IndexOf('.');
            if (index == -1)
            {
                Util.printError("参考文献作者后缺少标点符号“.”  ----" + fulltext.Substring(0,10));
            }
            else
            {
                string[] autorname = Regex.Split(TextBefore.Substring(0, index), @",");
                if (autorname.Length > 3)
                {
                    if (autorname[autorname.Length - 1].IndexOf('等') == -1 && autorname[autorname.Length - 1].IndexOf("etal") == -1)
                    {

                        Util.printError("参考文献作者超三人时，只列三个后加“等”或“et al”  ----" + fulltext.Substring(0,10));
                    }
                }

                else if (autorname.Length <= 3)
                {
                    if (autorname[autorname.Length - 1].IndexOf('等') != -1 ||
                        autorname[autorname.Length - 1].IndexOf("et al") != -1)
                    {
                        if (autorname.Length == 3)
                        {
                           // Util.printError("参考文献作者后的‘等’或‘et al’应该用‘,’与前面的作者名分隔开  ----" + fulltext);
                        }
                        else
                        {
                            Util.printError("参考文献作者少于三人时应全部写出  ----" + fulltext);
                        }
                    }
                }
            }
        }

        public bool checkcomplete(string textBef, string textAft, string fulltext)
        {
            bool lack = false;
            if (textAft.Trim() == "")
            {
                Util.printError("参考文献不完整，缺少标志代码后的部分  ----" + fulltext.Substring(0,10));
                lack = true;
            

            }
            if (textBef.Trim() == "")
            {
                Util.printError("参考文献不完整，缺少标志代码前的部分  ----" + fulltext.Substring(0,10));
                lack = true;
         

            }
            return lack;
        }

        public Match checkyearJ(string paraText)
        {
            Match match = Regex.Match(paraText, @"[1-2][0-9][0-9][0-9]");
            if (match.Success)
            {
                int year = Convert.ToInt32(match.Value);
                DateTime now = DateTime.Now;
                if (year > now.Year)
                {
                    Util.printError("参考文献出版年份超出当前年，不合法  ----" + paraText.Substring(0,10));
                }


              /*  if (paraText[match.Index - 1] != ',' && ((int)paraText[match.Index - 1] > 127 || Char.IsNumber(paraText[match.Index - 1])))
                {
                    if (paraText[match.Index - 1] == '，')
                        Util.printError("年份前标点符号不应为中文的：" + paraText);

                    else
                        Util.printError("年份前缺少标点符号‘,’：" + paraText);
                }

                else if (paraText[match.Index - 1] == ' ')
                {
                    Util.printError("年份前不应有空格：" + paraText);
                }
                else
                {

                    if (paraText[match.Index - 1] != ',')
                    {
                        Util.printError("年份前标点符号错误：" + paraText);
                      
                    }
                }*/
                if (paraText[match.Index - 1] != ',' && ((int)paraText[match.Index - 1] > 127 || Char.IsNumber(paraText[match.Index - 1]) || Char.IsLetter(paraText[match.Index - 1])))
                {
                    if (paraText[match.Index - 1] == '，')
                        Util.printError("参考文献出版年份前的标点符号不应该用中文的  ----" + paraText.Substring(0,10));
                    else

                        Util.printError("参考文献出版年份前缺少标点符号“,”  ----" + paraText.Substring(0, 10));
                }

              /*  else if (paraText[match.Index - 1] == ' ')
                {
                    Util.printError("年份前不应有空格：" + paraText);
                }*/

                else if (paraText[match.Index - 1] == ',')
                { }
                else
                {
                    Util.printError("参考文献出版年份前标点符号错误，应为“,”  ----" + paraText.Substring(0, 10));
                }
                return match;
            }

            else
            {
                Util.printError("参考文献缺少出版年份  ----" + paraText.Substring(0, 10));
                return null;
            }
        }

        public void City(string TextAfter, bool Hascom, string fulltext)
        {
            
            // Console.WriteLine(para.InnerText);
            string txt = test2();
            if (Hascom)
            {
               
                if (TextAfter.Replace(" ","")[0] == '.')
                {
                    /*if (/*TextAfter.IndexOf(':') - 1 != 2 && TextAfter.IndexOf(':') - 1 != 3 && TextAfter.IndexOf(':') - 1 != 4 &&
                        TextAfter.IndexOf('：') - 1 != 2 && TextAfter.IndexOf('：') - 1 != 3 && TextAfter.IndexOf('：') - 1 != 4*/
                       /* TextAfter.IndexOf(':')<0&&TextAfter.IndexOf('：')<0)
                    {
                        Util.printError("缺少城市名或城市名后的标点符号有误:" + fulltext);
                    }*/
                  
                }
                else
                {
                    //缺标点符号“.”
                    if (TextAfter.Replace(" ", "")[0] > 127 && TextAfter.Replace(" ", "")[0] != '．' && (TextAfter.Replace(" ", "").IndexOf(':') == 2 || TextAfter.Replace(" ", "").IndexOf(':') == 3))
                    {
                        Util.printError("参考文献标志代码后缺少标点符号“.”  ----" + fulltext.Substring(0, 10));
                    }

                    //不缺标点符号  但是是错误的
                    else if (TextAfter.Replace(" ", "").IndexOf(':') == 3 || TextAfter.Replace(" ", "").IndexOf(':') == 4)
                    {
                        Util.printError("参考文献标志代码后标点符号错误，应为“.”  ----" + fulltext.Substring(0, 10));
                    }

                    else
                    {
                        Util.printError("参考文献缺少城市名（出版地或授学位地）  ----" + fulltext.Substring(0,10));
                    }
                }
            }

            else
            {
                if (TextAfter[0] == '.')
                {
                    if (txt.IndexOf(TextAfter.Substring(1).Trim().Substring(0, 2)) < 0 && txt.IndexOf(TextAfter.Substring(1).Trim().Substring(0, 3)) < 0)
                    {
                        Util.printError("参考文献缺少城市名（出版地或授学位地）  ----" + fulltext.Substring(0, 10));
                    }

                    else if (txt.IndexOf(TextAfter.Substring(1).Trim().Substring(0, 2)) >= 0)
                    {
                        if ((int)TextAfter.Substring(1).Trim()[2] > 127 && TextAfter.Substring(1).Trim()[2] != '.')
                        {
                            Util.printError("参考文献城市名（出版地或授学位地）后缺少标点符号“:”  ----" + fulltext.Substring(0, 10));
                        }
                        else
                        {
                            Util.printError("参考文献城市名（出版地或授学位地）后标点符号错误，应为“:”  ----" + fulltext.Substring(0, 10));
                        }
                    }
                    else if (txt.IndexOf(TextAfter.Substring(1).Trim().Substring(0, 3)) >= 0)
                    {
                        if ((int)TextAfter.Substring(1).Trim()[3] > 127 && TextAfter.Substring(1).Trim()[3] != '.')
                        {
                            Util.printError("参考文献城市名（出版地或授学位地）后缺少标点符号“:”  ----" + fulltext.Substring(0, 10));
                        }
                        else
                        {
                            Util.printError("参考文献城市名（出版地或授学位地）后标点符号错误，应为“:”  ----" + fulltext.Substring(0, 10));
                        }
                    }



                }

                else
                {
                    Util.printError("参考文献标志代码后缺少标点符号“.”  ----" + fulltext.Substring(0,10));
                    if (txt.IndexOf(TextAfter.Trim().Substring(0, 2)) < 0 && txt.IndexOf(TextAfter.Trim().Substring(0, 3)) < 0)
                    {
                        Util.printError("参考文献缺少城市名（出版地或授学位地）  ----" + fulltext.Substring(0, 10));
                    }

                }
            }


        }

        unsafe public void checkpunctuation(string paraText, bool* Hascom)
        {
            for (int i = 0; i < paraText.Length - 1; i++)
            {
                if (paraText[i] == '，')
                {
                    Util.printError("参考文献中的标点符号错误，应为英文的“,”   ----" + paraText.Substring(0,10));
                }

                if (paraText[i] == '：')
                {
                    Util.printError("参考文献中的标点符号错误，应为英文的“:”   ----" + paraText.Substring(0,10));
                    *Hascom = true;
                }
                if (paraText[i] == ':')
                {
                    *Hascom = true;
                }

                if (paraText[i] == '。')
                {
                    Util.printError("参考文献中的标点符号错误，应为英文的“.”   ----" + paraText.Substring(0, 10));
                }

                if (paraText[i] == '（')
                {
                    Util.printError("参考文献中的标点符号错误，应为英文的“(”   ----" + paraText.Substring(0, 10));
                }
                if (paraText[i] == '）')
                {
                    Util.printError("参考文献中的标点符号错误，应为英文的“)”   ----" + paraText.Substring(0, 10));
                }


            }

            /*结尾的.*/

            if (paraText.Trim()[paraText.Trim().Length - 1] != '.' && (paraText.Trim()[paraText.Trim().Length - 1] > 127 || Char.IsNumber(paraText.Trim()[paraText.Trim().Length - 1])))
            {
                Util.printError("参考文献未以英文句点结尾  ----" + paraText.Substring(0,10));
                
            }

            else if (paraText.Trim()[paraText.Trim().Length - 1] != '.')
            {
                if (paraText.Trim()[paraText.Trim().Length - 1] == '．')
                    Util.printError("参考文献未以英文句点结尾  ----" + paraText.Substring(0, 10));
                else

                    Util.printError("参考文献未以英文句点结尾  ----" + paraText.Substring(0, 10));
            }


        }

        public void checkyear(string paraText,string fulltext)
        {
            Match match = Regex.Match(paraText.Trim(), @"[1-2][0-9][0-9][0-9]");
            if (match.Success)
            {

                if (match.Index + 4 != paraText.Trim().Length - 1 && match.Index + 3 != paraText.Trim().Length - 1)//paraText.Substring(match.Index).Length < 15)
                {
                   /* if (paraText.Trim().Substring(paraText.Trim().Length - 1, 1) == "." && )
                    { }*/
                    Util.printError("参考文献出版年份位置错误，应放在结尾  ----" + fulltext.Substring(0,10));
                }
                int year = Convert.ToInt32(match.Value);
                DateTime now = DateTime.Now;
                if (year > now.Year)
                {
                    Util.printError("参考文献出版年份超出当前年，不合法  ----" + fulltext.Substring(0, 10));
                }

                //缺","且不是错误符号
            
                if (paraText[match.Index - 1] != ',' && ((int)paraText[match.Index - 1] > 127 || Char.IsNumber(paraText[match.Index - 1]) || Char.IsLetter(paraText[match.Index - 1])))
                {
                    if (paraText[match.Index - 1] == '，')
                        Util.printError("参考文献出版年份前标点符号错误（不应为中文的），应为“,”  ----" + fulltext.Substring(0, 10));
                    else

                        Util.printError("参考文献出版年份前缺少标点符号“,”  ----" + fulltext.Substring(0, 10));
                }

                else if (paraText[match.Index - 1] == ' ')
                {
                   // Util.printError("年份前不应有空格：" + fulltext);
                }
               
                else if (paraText[match.Index - 1] == ',')
                { }
                else
                {
                    Util.printError("参考文献出版年份前标点符号错误，应为“,”  ----" + fulltext.Substring(0, 10));
                }

            }

            else
            {
                Util.printError("参考文献缺少出版年份  ----" + fulltext.Substring(0, 10));
            }
        }

        public void checkyearN(string paraText)
        {
            Match match = Regex.Match(paraText, @"[1-2][0-9][0-9][0-9]");
            if (match.Success)
            {

                int year = Convert.ToInt32(match.Value);
                DateTime now = DateTime.Now;
                if (year > now.Year)
                {
                    Util.printError("参考文献出版年份超出当前年，不合法  ----" + paraText.Substring(0, 10));
                }
                checkmonthN(paraText, match);

                //缺","且不是错误符号
                if (paraText[match.Index - 1] != ',' && ((int)paraText[match.Index - 1] > 127 || Char.IsNumber(paraText[match.Index - 1]) || Char.IsLetter(paraText[match.Index - 1])))
                {
                    if (paraText[match.Index - 1] == '，')
                        Util.printError("参考文献出版年份前标点符号错误（不应为中文的），应为“,”  ----" + paraText.Substring(0, 10));
                    else

                        Util.printError("参考文献出版年份前缺少标点符号“,”  ----" + paraText.Substring(0, 10));
                }

                else if (paraText[match.Index - 1] == ' ')
                {
                   // Util.printError("年份前不应有空格：" + paraText);
                }
                else if (paraText[match.Index - 1] == ',')
                { }
                else
                {
                    Util.printError("参考文献出版年份前标点符号错误，应为“,”  ----" + paraText.Substring(0, 10));
                }

            }

            else
            {
                Util.printError("参考文献缺少出版年份  ----" + paraText.Substring(0,10));
            }
        }

        public void checkmonthN(string paratext, Match match)
        {
            string paratext2 = paratext.Substring(match.Index + 4, paratext.Length - match.Index - 4);
            if (Regex.Matches(paratext2, @",").Count + Regex.Matches(paratext2, @"，").Count < 2)
            {
                Util.printError("参考文献出版信息有误，格式应为：年,月,日(版次).   ----" + paratext.Substring(0,10));
                return;
            }
            Match match2 = Regex.Match(paratext2, @"[1-9]");
            if (match2.Success)
            {
                checkdayN(paratext,paratext2, match2,1);
                if (paratext2[match2.Index - 1] != ',' && paratext2[match2.Index - 1] != ' ')
                    Util.printError("参考文献月份前标点符号有误，应为“,”  ----" + paratext.Substring(0,10));
               // else if(paratext2[match2.Index - 1] != ',')
                   // Util.printError("月份前不应有空格：" + paratext);
            }
            else
            {

                Match match3 = Regex.Match(paratext2, @"[1][0-2]");
                if (match3.Success)
                {
                    checkdayN(paratext,paratext2, match3,2);
                    if (paratext2[match3.Index - 1] != ',' && paratext2[match3.Index - 1] != ' ')
                        Util.printError("参考文献月份前标点符号有误，应为“,”  ----" + paratext.Substring(0,10));
                    //else if (paratext2[match3.Index - 1] != ',')
                       // Util.printError("月份前不应有空格：" + paratext);
                }
                else
                    Util.printError("参考文献缺少月份  ----" + paratext.Substring(0,10));
            }

        }

        public void checkdayN(string paratext, string paratext2, Match match,int i)
        {
            string paratext3 ;
            if(i==1)
            paratext3= paratext2.Substring(match.Index +1, paratext2.Length - match.Index - 1);
            else
                paratext3 = paratext2.Substring(match.Index + 2, paratext2.Length - match.Index - 2);

            Match match4 = Regex.Match(paratext3, @"[1-9]");
            if (match4.Success && paratext3[match4.Index - 1] != '(' && paratext3[match4.Index - 1] != '（')
            {
                
                if (paratext3[match4.Index - 1] != ',' && paratext3[match4.Index - 1] != ' ')
                    Util.printError("参考文献日期前标点符号有误，应为“,”  ----" + paratext.Substring(0,10));
               /* else if (paratext3[match4.Index - 1] != ',')
                    Util.printError("日期前不应有空格：" + paratext);*/
                else { }

                if (paratext3.IndexOf("(") >= 0 && paratext3.IndexOf(")") >= 0)
                { }
                else if (paratext3.IndexOf("（") >= 0 && paratext3.IndexOf("）") >= 0)
                { }
                else
                    Util.printError("参考文献缺少版次，格式应为：年,月,日(版次).   ----" + paratext.Substring(0,10));

            }
            else
            {

                Match match5 = Regex.Match(paratext3, @"[1-3][0-9]");
                if (match5.Success && paratext3[match5.Index - 1] != '(' && paratext3[match5.Index - 1] != '（')
                {
                   
                    if (paratext3[match5.Index - 1] != ',' && paratext3[match5.Index - 1] != ' ')
                        Util.printError("参考文献日期前标点符号有误，应为“,”  ----" + paratext.Substring(0,10));
                   /* else if (paratext3[match5.Index - 1] != ',')
                        Util.printError("日期前不应有空格：" + paratext);*/
                    else { }

                    if (paratext3.IndexOf("(") >= 0 && paratext3.IndexOf(")") >= 0)
                    { }
                    else if (paratext3.IndexOf("（") >= 0 && paratext3.IndexOf("）") >= 0)
                    { }
                    else
                        Util.printError("参考文献缺少版次，格式应为：年,月,日(版次).   ----" + paratext.Substring(0,10));


                }
                else
                    Util.printError("参考文献缺少日期  ----" + paratext.Substring(0,10));
            }


        }

        private bool hasChinese(string str)
        {
            return Regex.IsMatch(str, @"[\u4e00-\u9fa5]");
        }

        public bool hasNum(string paraText)
        {
            Match match1 = Regex.Match(paraText.Substring(0, 5), @"\[[0-9](/OL)?\]");
            Match match2 = Regex.Match(paraText.Substring(0, 5), @"\[(1|2|3|4)[0-9](/OL)?\]");
            if (match1.Success || match2.Success)
            {
              
                return true;
            }

            return false;
        }

        static byte[] byData = new byte[2000];
        static char[] charData = new char[2000];
        static string str = null;
        public static int Read()
        {
            try
            {
                string str = Util.environmentDir + "/常用期刊合集.txt";
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
        public static string test()//读取期刊名和创刊年份
        {
            int a = Read();
            str = new string(charData);
            return str;
        }

        static byte[] byData2 = new byte[2000];
        static char[] charData2 = new char[2000];
        static string str2 = null;
        public static int Read2()
        {
            try
            {
                string str2 = Util.environmentDir + "/城市合集.txt";
                FileStream file = new FileStream(str2, FileMode.Open);
                file.Seek(0, SeekOrigin.Begin);
                file.Read(byData2, 0, 2000); //byData传进来的字节数组,用以接受FileStream对象中的数据,第2个参数是字节数组中开始写入数据的位置,它通常是0,表示从数组的开端文件中向数组写数据,最后一个参数规定从文件读多少字符.
                Decoder d = Encoding.Default.GetDecoder();
                d.GetChars(byData2, 0, byData2.Length, charData2, 0);
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
        public static string test2()//读取城市名
        {
            int a = Read2();
            str2 = new string(charData2);
            return str2;
        }

        public void isNumberingCorrectinContents(int RefCount, WordprocessingDocument doc)
        {
            IEnumerable<Paragraph> paras = doc.MainDocumentPart.Document.Body.Elements<Paragraph>();
            List<int> list = new List<int>();
            int maxnumber = 0;
            int location = -1;

            bool flagover = false;
            Paragraph p = new Paragraph();
            foreach (Paragraph para in paras)
            {

                location++;
                string runText = null;
                IEnumerable<Run> runs = para.Elements<Run>();
                List<Run> Psrunlist = runs.ToList<Run>();
                int runn = -1;

                foreach (Run run in runs)
                {
                    runn++;
                    if (run.RunProperties != null)
                    {
                        if (run.RunProperties.VerticalTextAlignment != null)
                        {
                            runText = run.InnerText;
                            Match match = Regex.Match(runText, @"\[\d+\-*\d*\]");
                            if (match.Success)
                            {
                                int index = match.Value.IndexOf('-');
                                if (index == -1)
                                {
                                    if (flagover)
                                    {
                                        maxnumber = Convert.ToInt16(runText.Substring(match.Index + 1, match.Length - 2)) - 1;
                                        flagover = false;

                                    }
                                    if (Convert.ToInt16(runText.Substring(match.Index + 1, match.Length - 2)) != maxnumber + 1)
                                    {
                                        if (maxnumber == 0)
                                        {
                                            Util.printError("参考文献角标数字起始值有误  ----" + para.InnerText.Substring(0,10));
                                            return;

                                        }
                                        else
                                        {
                                            if (Convert.ToInt16(runText.Substring(match.Index + 1, match.Length - 2)) > RefCount)
                                                Util.printError("参考文献角标数字超出了参考文献总条目数  ----" + para.InnerText.Substring(0,10));
                                            else
                                                Util.printError("参考文献角标数字与前面的不连续  ----" + para.InnerText.Substring(0,10));
                                        }

                                        maxnumber++;
                                    }
                                    else
                                    {
                                        maxnumber++;
                                    }

                                }
                                else
                                {
                                    //[m-n]
                                    //m
                                    if (flagover)
                                    {
                                        maxnumber = Convert.ToInt16(match.Value.Substring(1, index - 1)) - 1;
                                        flagover = false;
                                    }
                                    string number1 = match.Value.Substring(1, index - 1);
                                    if (Convert.ToInt16(number1) != maxnumber + 1)
                                    {
                                        maxnumber++;
                                        Util.printError("参考文献角标数值[m-n]中的m有误（不连续）  ----" + para.InnerText.Substring(0, 10));
                                    }

                                    //n
                                    maxnumber = Convert.ToInt16(match.Value.Substring(index + 1, match.Length - (index + 2)));
                                    if (maxnumber > RefCount)
                                    {
                                        flagover = true;

                                        Util.printError("参考文献角标[m-n]中的n超过总参考文献数目  ----" + para.InnerText.Substring(0, 10));
                                        continue;
                                    }
                                }

                                runText = null;
                            }

                            else if (runText.IndexOf("[") != -1)
                            {
                                int num = runn + 1;
                                while (Psrunlist[num].RunProperties != null && Psrunlist[num].RunProperties.VerticalTextAlignment != null)
                                {
                                    if (Psrunlist[num].RunProperties.VerticalTextAlignment.Val != null)
                                    {
                                        if (Psrunlist[num].RunProperties.VerticalTextAlignment.Val == VerticalPositionValues.Superscript)
                                        {
                                            runText += Psrunlist[num].InnerText;
                                            num++;
                                        }
                                    }
                                }

                                Match match2 = Regex.Match(runText, @"\[\d+\-*\d*\]");
                                if (match2.Success)
                                {

                                    int index = match2.Value.IndexOf('-');
                                    if (index == -1)
                                    {

                                        if (flagover)
                                        {
                                            maxnumber = Convert.ToInt16(runText.Substring(match2.Index + 1, match2.Length - 2)) - 1;
                                            flagover = false;

                                        }
                                        if (Convert.ToInt16(runText.Substring(match2.Index + 1, match2.Length - 2)) != maxnumber + 1)
                                        {
                                            if (Convert.ToInt16(runText.Substring(match2.Index + 1, match2.Length - 2)) > RefCount)
                                                Util.printError("参考文献角标数字超出了参考文献总条目数  ----" + para.InnerText.Substring(0, 10));

                                          //  num = runn + 1;
                                            else
                                                Util.printError("参考文献角标数字与前面的不连续  ----" + para.InnerText.Substring(0, 10));
                                            /* while (Psrunlist[num].RunProperties.VerticalTextAlignment != null)
                                             {
                                                 if (Psrunlist[num].RunProperties.VerticalTextAlignment.Val != null)
                                                 {
                                                     if (Psrunlist[num].RunProperties.VerticalTextAlignment.Val == VerticalPositionValues.Superscript)
                                                     {
                                                         int loc = -1;
                                                         foreach (Run r in runs)
                                                         {
                                                             loc++;
                                                             if (loc == num)
                                                             {
                                                                 r.GetFirstChild<Text>().Text = null;
                                                                 break;
                                                             }
                                                         }
                                                     }
                                                 }
                                                 num++;
                                             }*/

                                            //run.GetFirstChild<Text>().Text = "[" + maxnumber + "]";
                                            maxnumber++;
                                        }
                                        else
                                        {
                                            maxnumber++;
                                        }

                                    }
                                    else
                                    {
                                        //[m-n]
                                        //m
                                        if (flagover)
                                        {
                                            maxnumber = Convert.ToInt16(match2.Value.Substring(1, index - 1)) - 1;
                                            flagover = false;
                                        }
                                        string number1 = match2.Value.Substring(1, index - 1);
                                        if (Convert.ToInt16(number1) != maxnumber + 1)
                                        {
                                            maxnumber++;
                                            /* num = runn + 1;
                                             while (Psrunlist[num].RunProperties.VerticalTextAlignment != null)
                                             {
                                                 if (Psrunlist[num].RunProperties.VerticalTextAlignment.Val != null)
                                                 {
                                                     if (Psrunlist[num].RunProperties.VerticalTextAlignment.Val == VerticalPositionValues.Superscript)
                                                     {
                                                         int loc = -1;
                                                         foreach (Run r in runs)
                                                         {
                                                             loc++;
                                                             if (loc == num)
                                                             {
                                                                 r.GetFirstChild<Text>().Text = null;
                                                                 break;
                                                             }
                                                         }
                                                     }
                                                 }
                                                 num++;
                                             }

                                             run.GetFirstChild<Text>().Text = "[" + maxnumber + runText.Substring(index);*/
                                            Util.printError("参考文献角标数值[m-n]中的m有误（不连续）  ----" + para.InnerText.Substring(0, 10));
                                        }

                                        //n
                                        maxnumber = Convert.ToInt16(match2.Value.Substring(index + 1, match2.Length - (index + 2)));
                                        if (maxnumber > RefCount)
                                        {
                                            flagover = true;

                                            // Console.WriteLine(match2);
                                            // addComment(doc, para, "此段落参考文献角标超过总参考文献数目");
                                            Util.printError("参考文献角标[m-n]中的n超过总参考文献数目  ----" + para.InnerText.Substring(0, 10));
                                            continue;
                                        }
                                    }

                                    runText = null;
                                }



                            }
                            else
                            {
                                continue;
                            }
                        }
                    }

                }

                p = para;

            }
            if (maxnumber < RefCount - 1)
            {

                Util.printError("正文中缺少参考文献角标，请补全");
            }
        }
    }


}
