using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using PaperFormatDetection.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Wp = DocumentFormat.OpenXml.Drawing.Wordprocessing;

namespace PaperFormatDetection.Paperbase
{
    class Figure
    {
        protected string PicCNFont;
        protected string PicCNFontSize;
        protected string PicCNFontJustification;
        protected int CNFontToPicLns;

        protected string PicENFont;
        protected string PicENFontSize;
        protected string PicENFontJustification;
        protected int ENFontToCNFont;

        protected int UpBlankToPicLns;    //向上空格
        protected int DownBlankToPicLns;      //向下空格

        protected int MNtoName;       //M.N 到图名空格数

        protected string PicJustification;






        protected void detectCNPicName(WordprocessingDocument docx, int i)
        {
            Body body = docx.MainDocumentPart.Document.Body;
            int countBodyChild = body.ChildElements.Count();
            int pic = 0;
            string pict = null;
            string temp = null;

            Paragraph p = (Paragraph)body.ChildElements.GetItem(i);
            string num = null;  //M.N
            List<int> iList = Util.getTitlePosition(docx);//标题位置
            string chapter = Util.Chapter(iList, i, body);            //图所在章
            List<int> lists = Tool.getchaptertitleposition(docx);
            string chapteR = Util.Chapter(lists, i, body);
            if (pict != null)
                temp = pict;
            pict = chapteR;
            if (temp != null)
                if (pict != temp)
                    pic = 1;
            
            /*图名位置
            location[ Chinese title, English title, blank line before table, blank line after table]
            */
            int[] index = locationOfTitleAndBlankLine(docx, i);

            #region 有图名
            if (index[0] != -1)
            {
                if (body.ChildElements.GetItem(index[0]).GetType().ToString() == "DocumentFormat.OpenXml.Wordprocessing.Paragraph")
                {
                    Paragraph pCNtitle = (Paragraph)body.ChildElements.GetItem(index[0]);
                    string CNtitle = Tool.getFullText(pCNtitle).Trim();
                    num = Util.number(CNtitle);//M.N
                    string s = CNtitle;
                    string StrNum = Util.number(s);
                    //中文图名位置
                    if (index[0] > i + CNFontToPicLns)//&& index[0] != i + CNFontToPicLns + HaveBlankText(i + 1,docx))
                    {
                        Util.printError("图与中文图名之间不应有空行" + "  ----" + s);
                    }
                    if (CNtitle != null && isTableInMiddleBefore(Util.pageDic, CNtitle))
                    {
                        if (index[2] == -1 || index[2] != i - 1)
                        {
                            Util.printError("图之前应空一行，----" + CNtitle);
                        }
                    }
                    #region 图名字体字号居中
                    if (!Util.correctfonts(pCNtitle, docx, "宋体", "Times New Roman"))
                    {
                        PaperFormatDetection.Tools.Util.printError("中文图名字体错误，应为" + PicCNFont + "  ----" + s);
                    }
                    if (!Util.correctsize(pCNtitle, docx, PicCNFontSize))
                    {
                        PaperFormatDetection.Tools.Util.printError("中文图名字号错误，应为" + PicCNFontSize + "  ----" + s);
                    }
                    if (!Util.correctJustification(pCNtitle, docx, PicCNFontJustification))
                    {
                        PaperFormatDetection.Tools.Util.printError("中文图名未" + PicCNFontJustification + "  ----" + s);
                    }
                    #endregion

                    #region 图名中间空格 m.n格式连续
                    //中文
                    //*******5.24新增 图序号检测a[i]=0表示不成立
                    //检测项  1.序号前无空格  
                    //       2.序号后g空格
                    //       3.是否满足m.n格式
                    int[] e = Util.numberstyle(CNtitle, num.Length, MNtoName);
                    if (e[0] == 0)
                    {
                        Util.printError("中文图名序号与“图”之间不应有空格" + "  ----" + s);
                    }
                    if (e[1] == 0)
                    {
                        Util.printError("中文图名序号与图名内容之间应有" + MNtoName + "个空格" + "  ----" + s);
                    }
                    if (e[2] == 0)
                    {
                        Util.printError("中文图名序号应为M.N形式" + "  ----" + s);
                    }
                    else
                    {
                        if (!Util.correctm(num, chapter))
                        {
                            Util.printError("中文图名序号与章节号不一致" + "  ----" + s);
                        }
                        //if (!Util.correctn(StrNum, pic, 0))
                        //{
                        //    Util.printError("中文图名序号不连续" + "  ----" + s);
                        //}
                    }
                    #endregion
                }
                #endregion
            }
        }

        //英文图名检测
        protected void detectENPicName(WordprocessingDocument docx, int i)
        {
            Body body = docx.MainDocumentPart.Document.Body;
            int countBodyChild = body.ChildElements.Count();
            Paragraph p = (Paragraph)body.ChildElements.GetItem(i);
            string num = null;  //M.N
            List<int> iList = Util.getTitlePosition(docx);//标题位置
            string chapter = Util.Chapter(iList, i, body);            //图所在章

            /*图名位置
            location[ Chinese title, English title, blank line before table, blank line after table]
            */
            int[] index = locationOfTitleAndBlankLine(docx, i);

            #region 有图名
            if(index[1]!=-1)
            {
                if (body.ChildElements.GetItem(index[1]).GetType().ToString() == "DocumentFormat.OpenXml.Wordprocessing.Paragraph")
                {
                    Paragraph pENtitle = (Paragraph)body.ChildElements.GetItem(index[1]);
                    string ENtitle = pENtitle.InnerText.Trim();
                    num = Util.number(ENtitle);//M.N
                    string s = ENtitle;

                    if (ENtitle != null && isFigCrossPage(Util.pageDic, ENtitle))
                    {
                        Util.printError("图与图名不应跨页"+ENtitle);
                    }
                    //英文图名位置
                    if (index[0] != -1 && index[1] != index[0] + ENFontToCNFont)
                    {
                        Util.printError("英文图名应在中文图名后" + ENFontToCNFont + "行");
                    }


                    #region 图名字体字号居中
                    if (!Util.correctfonts(pENtitle, docx, PicENFont, PicENFont))
                    {
                        PaperFormatDetection.Tools.Util.printError("英文图名字体错误，应为" + PicENFont + "  ----" + s);
                    }
                    if (!Util.correctsize(pENtitle, docx, PicENFontSize))
                    {
                        PaperFormatDetection.Tools.Util.printError("英文图名字号错误，应为" + PicENFontSize + "  ----" + s);
                    }
                    if (!Util.correctJustification(pENtitle, docx, PicENFontJustification))
                    {
                        PaperFormatDetection.Tools.Util.printError("英文图名未" + PicENFontJustification + "  ----" + s);
                    }
                    #endregion


                    #region 图名中间空格 m.n格式连续
                    //*******5.24新增 表格序号检测
                    //检测项  1.Fig.正确否
                    //        2.Fig.后有空格 
                    //        3.序号后空格
                    //        4.是否满足m.n格式
                    int[] e = EnNumberStyle(ENtitle, num.Length, MNtoName);
                    if (e[0] == 0)
                    {
                        PaperFormatDetection.Tools.Util.printError("英文图名开头应为“Fig.”" + "  ----" + s);
                    }
                    if (e[1] == 0)
                    {
                        PaperFormatDetection.Tools.Util.printError("英文图名序号与“Fig.”之间应有一个空格" + "  ----" + s);
                    }
                    if (e[2] == 0)
                    {
                        PaperFormatDetection.Tools.Util.printError("英文图名序号与图名内容之间应有" + MNtoName + "个空格" + "  ----" + s);
                    }
                    if (e[3] == 0)
                    {
                        PaperFormatDetection.Tools.Util.printError("英文图名序号应为M.N的形式" + "  ----" + s);
                    }
                    else
                    {
                        if (!Util.correctm(num, chapter))
                        {
                            PaperFormatDetection.Tools.Util.printError("英文图名序号与章节号不一致" + "  ----" + s);
                        }

                    }
                    #endregion

                    //英文图名后空行
                    if (ENtitle != null && isTableInMiddleAfter(Util.pageDic, ENtitle))
                    {
                        if (index[3] == -1)
                        {
                            PaperFormatDetection.Tools.Util.printError("英文图名之后应空一行，----" + ENtitle);
                        }
                    }

                }
            }
            #endregion
        }


        protected void detectPicJusAndBlank(WordprocessingDocument docx, int i)
        {

            Body body = docx.MainDocumentPart.Document.Body;
            int countBodyChild = body.ChildElements.Count();

            Paragraph p = (Paragraph)body.ChildElements.GetItem(i);

            /*图名位置
            location[ Chinese title, English title, blank line before table, blank line after table]
            */

            int[] index = locationOfTitleAndBlankLine(docx, i);
            #region 缺少中文图名
            //if (index[0] == -1)
            //{
            //    string message = getPicMassage(i, body);
            //    Util.printError("缺少中文图名  " + message + "前面的图");
            //}
            //#endregion
            //#region 缺少英文图名
            //if (index[1] == -1)
            //{
            //    string message = getPicMassage(i, body);
            //    Util.printError("缺少英文图名  " + message + "前面的图");
            //}
            #endregion

            if (index[0] == -1)
            {
                //if (!Util.correctJustification(p, docx, PicJustification))
                //{
                //    string message = getPicMassage(i, body);
                //    Util.printError("某未命名图未" + PicJustification + "  " + message + "前的图");
                //}
            }
            else
            {
                Paragraph pCNtitle = (Paragraph)body.ChildElements.GetItem(index[0]);

                string CNtitle = pCNtitle.InnerText.Trim();
                string s = CNtitle;


                if (!Util.correctJustification(p, docx, PicJustification))
                {
                    Util.printError("图未" + PicJustification + "  ----" + s);
                }

                #region
                //上下空行
                //if (index[3] == -1)
                //{

                //    if (CNtitle == null)
                //    {
                //        PaperFormatDetection.Tools.Util.printError("若图不在页首，未命名图前应空一行" + "  " + getPicMassage(i, body));
                //    }
                //    else
                //    {
                //        PaperFormatDetection.Tools.Util.printError("图若不在页首，图与上文间应该空一行" + "  ----" + s);
                //    }
                //}
                //else
                //{

                //    if (index[3] != i - UpBlankToPicLns)
                //    {
                //        PaperFormatDetection.Tools.Util.printError("图若不在页首，图与上文间应该空一行" + "  ----" + s);
                //    }
                //}
                //后
                //if (index[2] == -1)
                //{
                //    if (NotEnd(i, docx))
                //    {
                //        if (CNtitle == null)
                //        {
                //            PaperFormatDetection.Tools.Util.printError("图若不在页尾，未命名图与下文间应该空一行" + "  " + getPicMassage(i, body));
                //        }
                //        else
                //        {
                //            PaperFormatDetection.Tools.Util.printError("图若不在页尾，图与下文间应该空一行" + "  ----" + s);
                //        }
                //    }
                //}
                //else
                //{
                //    if (NotEnd(i, docx))
                //    {
                //        if (index[2] != i + DownBlankToPicLns)
                //        {
                //            PaperFormatDetection.Tools.Util.printError("图若不在页尾，图与下文间应该空一行" + "  ----" + s);
                //        }
                //    }
                //}
                #endregion
                //图宽度超页边距
                ParagraphProperties pPr_position = null;
                Run r_position = p.GetFirstChild<Run>();
                Drawing d_position = r_position.GetFirstChild<Drawing>();
                Picture p_position = r_position.GetFirstChild<Picture>();
                if (d_position != null || p_position != null)
                {
                    pPr_position = p.GetFirstChild<ParagraphProperties>();

                    if (d_position != null)
                    {
                        if (d_position.GetFirstChild<Wp.Inline>() != null)
                        {
                            Wp.Inline wp_inline = d_position.GetFirstChild<Wp.Inline>();
                            if (wp_inline.GetFirstChild<Wp.Extent>() != null)
                            {
                                string a = wp_inline.GetFirstChild<Wp.Extent>().Cx;
                                int p_size = int.Parse(a);
                                if (a != null && p_size > 5727941)
                                {
                                    PaperFormatDetection.Tools.Util.printError("图的宽度超过页边距" + "  ----" + s);
                                }
                            }
                        }
                    }

                }
            }
        }


        //跨页返回true
        protected bool isFigCrossPage(Dictionary<string, string> pageDic, string picName)
        {
            int picNamePageNum = -1;
            int picPageNum = -1;
            //通过键的集合取
            for (int i=0;i< pageDic.Keys.Count();i++)              
            {
                string a=pageDic.ElementAt(i).Key;
                if (picName == a)
                {
                    string b = "";
                    if(i-2>0)
                    {
                        b = pageDic.ElementAt(i - 2).Key;
                    }
                    if (Regex.IsMatch(b, @"^第[1-9][0-9]*张图"))
                    {
                        picPageNum = int.Parse(pageDic.ElementAt(i - 2).Value);
                        string[] sArray = pageDic.ElementAt(i).Value.Split('_');// 一定是单引                    
                        picNamePageNum = int.Parse(sArray[1]);
                        if (picNamePageNum != picPageNum)
                        {
                            return true;
                        }
                    }
                }

            }
                return false;
        }
        //图(上面）是否在每页的中间 在中间返回true tableName为中文图名
        protected bool isTableInMiddleBefore(Dictionary<string, string> pageDic, string tableName)
        {
            //通过键的集合取
            foreach (string keys in pageDic.Keys)
            {
                if (tableName == keys)
                {
                    string[] sArray = pageDic[keys].Split('_');// 一定是单引                    
                    int a = int.Parse(sArray[0]);
                    int b = int.Parse(sArray[1]);
                    if (a == b)
                    {
                        return true;
                    }
                }
            }
            return false;
        }
        //表格(下面）是否在每页的中间 在中间返回true tableName为英文图名
        protected bool isTableInMiddleAfter(Dictionary<string, string> pageDic, string tableName)
        {
            //通过键的集合取
            foreach (string keys in pageDic.Keys)
            {
                if (tableName == keys)
                {
                    string[] sArray = pageDic[keys].Split('_');// 一定是单引                    
                    int a = int.Parse(sArray[2]);
                    int b = int.Parse(sArray[1]);
                    if (a == b)
                    {
                        return true;
                    }
                }
            }
            return false;
        }








        protected int getPicN(WordprocessingDocument docx, string title)
        {
            Body body = docx.MainDocumentPart.Document.Body;
            string tTemp = Util.number(title);
            return 2;
        }

        protected bool NotEnd(int i, WordprocessingDocument docx)//判断图是不是在章的末尾
        {
            Body body = docx.MainDocumentPart.Document.Body;
            int countBodyChild = body.ChildElements.Count();
            List<int> iList = Tool.getchaptertitleposition(docx);

            for (int j = i + 3; j < i + 7; j++)//从英文图名开始检测
            {
                if (j < countBodyChild)
                {
                    string chapter = Util.Chapter(iList, j + 1, body);
                    if (body.ChildElements.GetItem(j).GetType().ToString() == "DocumentFormat.OpenXml.Wordprocessing.Paragraph")
                    {
                        Paragraph p = (Paragraph)body.ChildElements.GetItem(j);
                        string s = p.InnerText.Trim();
                        if (s != "")
                        {
                            if (s[0] != 'F')//防止在图名前面有空行，去掉Fig这一项
                            {
                                if (s[0] == chapter[0] + 1)
                                    return false;
                                else
                                    return true;

                            }
                            else
                                continue;
                        }
                        else
                            continue;
                    }
                }
            }
            return true;
        }

        protected int HaveBlankText(int i, WordprocessingDocument docx)
        {
            int bools = 0;
            Body body = docx.MainDocumentPart.Document.Body;
            if (body.ChildElements.GetItem(i).GetType().ToString() == "DocumentFormat.OpenXml.Wordprocessing.Paragraph")
            {
                Paragraph p = (Paragraph)body.ChildElements.GetItem(i);
                var list = p.ChildElements;
                if (p == null)
                {
                    bools = 1;
                }
                if (p != null)
                {
                    bools = 0;

                }
            }
            return bools;
        }




        /*
        返回中英文图名位置，前后空行位置
        初始值-1，false
        picloc[Chinese title,English title,blank line before picture,blank line after picture]
        find[i]=true   表示找到
        */
        protected int[] locationOfTitleAndBlankLine(WordprocessingDocument wordPro, int picloc)
        {
            int[] location = { -1, -1, -1, -1 };
            bool[] find = { false, false, false, false };
            Regex[] reg;
            reg = new Regex[9];
            reg[0] = new Regex(@"^图[1-9][0-9]*\.[1-9][0-9]*  ");//中文图匹配  关键字段：图m.n空格空格
            reg[1] = new Regex(@"^图[1-9][0-9]*\.[1-9][0-9]*");//中文图匹配  关键字段：图m.n
            reg[2] = new Regex(@"^图\ *[1-9][0-9]*");//中文图匹配  关键字段：图m
            reg[3] = new Regex(@"^Fig.\ *[1-9][0-9]*\.[1-9][0-9]*  ");//英文图匹配  关键字段Fig.空格m.n空格空格
            reg[4] = new Regex(@"^Fig. [1-9][0-9]*\.[1-9][0-9]*");//英文图匹配  关键字段Fig.空格m.n
            reg[5] = new Regex(@"^Fig.[1-9][0-9]*\.[1-9][0-9]*");//英文图匹配  关键字段Fig.m.n
            reg[6] = new Regex(@"^Fig(. [1-9][0-9]*)*");//英文图匹配  关键字段Fig.空格m
            reg[7] = new Regex(@"^Fig.[1-9][0-9]*");//英文图匹配  关键字段Fig.m
            reg[8] = new Regex(@"^Fig\ *[1-9][0-9]*");//英文图匹配  关键字段Figm
            Body body = wordPro.MainDocumentPart.Document.Body;
            //从pic往前找空格
            for (int index = picloc - 1; index > picloc - 2 && index >= 0; index--)
            {
                if (body.ChildElements.GetItem(index).GetType().ToString() == "DocumentFormat.OpenXml.Wordprocessing.Paragraph")
                {
                    Paragraph p = (Paragraph)body.ChildElements.GetItem(index);
                    string s = p.InnerText.Trim();
                    if (s.Length == 0 && find[2] == false)
                    {
                        location[2] = index;
                        find[2] = true;
                        break;
                    }
                }
            }

            //从pic往后找 图后空行匹配
            for (int index = picloc + 1; index < picloc + 4 && index < body.Count(); index++)
            {
                if (body.ChildElements.GetItem(index).GetType().ToString() == "DocumentFormat.OpenXml.Wordprocessing.Paragraph")
                {
                    Paragraph p = (Paragraph)body.ChildElements.GetItem(index);
                    string ss = p.InnerText.Trim();
                    if (ss.Length == 0 && find[3] == false)
                    {
                        if (find[3] == false)//图后空行匹配
                        {
                            location[3] = index;
                            find[3] = true;
                        }
                    }
                }
            }


            for (int index = picloc; index < picloc + 4 && index < body.Count(); index++)
            {
                if (body.ChildElements.GetItem(index).GetType().ToString() == "DocumentFormat.OpenXml.Wordprocessing.Paragraph")
                {
                    Paragraph p = (Paragraph)body.ChildElements.GetItem(index);
                    string s = Tool.getFullText(p).Trim();
                    if (s.IndexOf(',')==-1&&s.Length > 0 && s.Length < 100)//长度过滤
                    {
                        //中文图名匹配
                        for (int i = 0; i <= 2; i++)
                        {
                            Match m = reg[i].Match(s);
                            if (m.Success)
                            {
                                if (find[0] == false && s.Length <= 100)
                                {
                                    location[0] = index;
                                    find[0] = true;
                                    break;
                                }
                            }
                        }
                        //英文图名匹配
                        for (int j = 3; j <= 8; j++)
                        {
                            Match m = reg[j].Match(s);
                            if (m.Success)
                            {
                                if (find[1] == false)
                                {
                                    location[1] = index;
                                    find[1] = true;
                                    break;
                                }
                            }
                        }
                    }
                }
            }
            return location;
        }


        //是否匹配到了图名 picloc为图的位置
        protected bool matchPicName(int picloc, Body body)
        {
            Regex[] reg;
            reg = new Regex[3];
            reg[0] = new Regex(@"^图[1-9][0-9]*\.[1-9][0-9]*  ");//中文图匹配  关键字段：图m.n空格空格
            reg[1] = new Regex(@"^图[1-9][0-9]*\.[1-9][0-9]*");//中文图匹配  关键字段：图m.n
            reg[2] = new Regex(@"^图\ *[1-9][0-9]*");//中文图匹配  关键字段：图m
            for (int index = picloc + 1; index < picloc + 4 && index < body.ChildElements.Count(); index++)
            {
                if (body.ChildElements.GetItem(index).GetType().ToString() == "DocumentFormat.OpenXml.Wordprocessing.Paragraph")
                {
                    Paragraph p1 = (Paragraph)body.ChildElements.GetItem(index);
                    string s = p1.InnerText.Trim();
                    if (s.Length > 0 && s.Length < 100)//长度过滤
                    {
                        if(s!="")
                        {
                            //中文图名匹配
                            for (int i = 0; i <= 2; i++)
                            {
                                Match m = reg[i].Match(s);
                                if (m.Success)
                                {
                                    return true;
                                }
                            }
                            break;
                        }
                    }
                }
            }
            return false;
        }


        //判断是否是图 参数picloc为图在第几段，所有可能的图都加进来了
        protected bool isPic(int picloc, Body body)
        {
            bool flag = false;
            if (matchPicName(picloc, body))
            {
                flag = true;
            }
            Paragraph p = null;
            if (body.ChildElements.GetItem(picloc).GetType().ToString() == "DocumentFormat.OpenXml.Wordprocessing.Paragraph")
            {
                p = (Paragraph)body.ChildElements.GetItem(picloc);
            }
            if (p != null)
            {
                IEnumerable<Run> runlist = p.Elements<Run>();
                foreach (Run r in runlist)
                {
                    if (r != null)
                    {
                        Drawing d = r.GetFirstChild<Drawing>();
                        Picture pic = r.GetFirstChild<Picture>();
                        EmbeddedObject objects = r.GetFirstChild<EmbeddedObject>();
                        AlternateContent Alt = r.GetFirstChild<AlternateContent>();
                        if (((d != null || pic != null ))|| (flag == true && objects != null))
                        {
                            return true;
                        }
                        else if (Alt != null)
                        {
                            AlternateContentChoice AltChoice = Alt.GetFirstChild<AlternateContentChoice>();
                            AlternateContentFallback AltFallback = Alt.GetFirstChild<AlternateContentFallback>();
                            if (AltChoice != null && AltFallback != null)
                            {
                                if (flag == true && (AltChoice.GetFirstChild<Drawing>() != null || AltFallback.GetFirstChild<Picture>() != null))
                                {
                                    return true;
                                }
                            }
                        }
                    }

                }
            }
            return false;
        }


        //英文
        //*******5.24新增 表格序号检测
        //检测项  1.Fig.正确否
        //        2.Fig.后有空格 
        //         3.序号后g空格
        //       4.是否满足m.n格式
        protected static int[] EnNumberStyle(string title, int numlen, int g)
        {
            int l = -1;
            int i = -1;
            int[] a = new int[4] { 1, 1, 1, 1 };
            foreach (char c in title)//寻找第一个数字位置
            {
                i++;
                if (c <= '9' && c >= '0')
                {
                    l = i;
                    break;
                }
            }
            //没标号，找不到数字
            if (l == -1)
            {
                a[2] = 0;
                return a;
            }
            if (title.IndexOf("Fig.") < 0)
            {
                a[0] = 0;
                if (title.IndexOf("Fig") >= 0)
                {
                    if (title.IndexOf("Fig ") == -1)
                    {
                        a[1] = 0;
                    }
                }
            }
            else
            {
                if (title.IndexOf("Fig. ") < 0)
                {
                    a[1] = 0;//若没有空格报错
                }
                else
                {
                    //多空格报错
                    if (title.IndexOf("Fig.  ") >= 0)
                    {
                        a[1] = 0;
                    }
                }
            }
            //序号后g空格
            if (l + numlen + 1 < title.Length)
            {
                for (int j = l + numlen; j < numlen + l + g; j++)
                {
                    if (title[j] != ' ')
                    {
                        a[2] = 0;
                        break;
                    }
                }
                if (title[l + numlen + g] == ' ')
                {
                    a[2] = 0;
                }
            }
            //m.n格式
            if (l + 2 < title.Length && l >= 0)
            {
                if (title[l + 1] == '.')//m为一位数
                {
                    for (int j = 2; j < numlen; j++)
                    {
                        if (title[l + j] < '0' || title[l + j] > '9')
                        {
                            a[3] = 0;
                        }
                    }
                }
                else if (title[l + 2] == '.')//m为两位数
                {
                    for (int j = 3; j < numlen; j++)
                    {
                        if (title[l + j] <= '0' || title[l + j] >= '9')
                        {
                            a[3] = 0;
                        }
                    }
                }
            }
            return a;
        }



        //未命名图的信息
        protected string getPicMassage(int i, Body body)
        {
            string message = "";
            for (int j = i + 1; j < i + 6&&j<body.ChildElements.Count; j++)
            {
                message = Util.getFullText((Paragraph)body.ChildElements.GetItem(j)).Trim();
                if (message != "")
                {
                    if (message.Length >= 16)
                    {
                        message = message.Substring(0, 15);
                    }
                    break;
                }
            }
            return message;
        }



        //获得图片位置用list保存
        protected List<int> figLocation(Body body)
        {
            List<int> list = new List<int>(50);
            int l = body.ChildElements.Count();
            for (int i = 0; i < l; i++)
            {
                if (body.ChildElements.GetItem(i).GetType().ToString() == "DocumentFormat.OpenXml.Wordprocessing.Paragraph")
                {
                    if (isPic(i, body))
                    {
                        list.Add(i);
                    }

                }
            }
            return list;
        }
        protected void PicDetection(WordprocessingDocument docx)
        {
            Body body = docx.MainDocumentPart.Document.Body;
            int countBodyChild = body.ChildElements.Count();

            int i = 0;

            for (i = 0; i < countBodyChild; i++)
            {
                if (body.ChildElements.GetItem(i).GetType().ToString() == "DocumentFormat.OpenXml.Wordprocessing.Paragraph")
                {
                    if (isPic(i, body))
                    {
                        detectCNPicName(docx, i);
                        detectENPicName(docx, i);
                        detectPicJusAndBlank(docx, i);
                    }
                }
            }
        }
        protected void PicDetectionUnder(WordprocessingDocument docx)
        {
            Body body = docx.MainDocumentPart.Document.Body;
            int countBodyChild = body.ChildElements.Count();

            int i = 0;

            for (i = 0; i < countBodyChild; i++)
            {
                if (body.ChildElements.GetItem(i).GetType().ToString() == "DocumentFormat.OpenXml.Wordprocessing.Paragraph")
                {
                    if (isPic(i, body))
                    {
                        detectCNPicName(docx, i);
                        detectPicJusAndBlank(docx, i);
                    }
                }
            }
        }



    }
}



