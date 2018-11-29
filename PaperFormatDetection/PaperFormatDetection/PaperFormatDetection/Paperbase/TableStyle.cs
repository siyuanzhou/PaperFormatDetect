using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using PaperFormatDetection.Tools;
using System.Text.RegularExpressions;
//using Microsoft.Office.Interop.Word;

//表上下空行，空行表与表名，N，表居中
namespace PaperFormatDetection.Paperbase
{
    class Tabledect
    {
        protected string tableFont;         //中文表名字体
        protected string tableJustification;//中文表名对齐
        protected string tableFontsize;       //中文表名字体大小
        protected int spacelnTotableUp;     //中文表名向上空行数



        protected string englishtableFont;//英文表名字体
        protected string englishtableJustification;//英文表名对齐
        protected string englishtableFontsize; //英文表名字体大小
        protected int englishspacelnTotableUp;     //英文表名向上空行数



        protected string intableFont;//表内中文字体
        protected string inEntableFont;//表内英文字体   
        protected string intableJustification;//表内对齐
        protected string intableFontsize; //表内字体大小
        


        protected int MNtoName;       //M.N 到表名空格数
        protected string tbsJustification="center";//表格对齐


        //判断表格居中 居中为true
        public bool detecttablecenter(DocumentFormat.OpenXml.Wordprocessing.Table table,string tbsJustification, WordprocessingDocument docx)
        {
            //居中检测
            TableProperties tpr = table.GetFirstChild<TableProperties>();
            if (tpr != null)
            {
                if (tpr.GetFirstChild<TableJustification>() != null)
                {
                    TableJustification tj = tpr.GetFirstChild<TableJustification>();
                    if (tj.Val.ToString() != tbsJustification && tj.Val.ToString() != null)
                    {
                        return false;
                    }
                }
                else
                {                    
                    if (tpr.TableStyle != null)
                    {
                        TableStyle style_id = tpr.TableStyle;
                        if (style_id != null)//从style中获取
                        {
                            string jc = Util.getFromStyle(docx, style_id.Val, 1);
                            if (jc!=null&&jc.ToString() != tbsJustification)
                            {
                                return false;
                            }
                        }
                    }
                }
            }
            return true;
        }



        public void detectTable(WordprocessingDocument doc,string type)
        {
            Body body = doc.MainDocumentPart.Document.Body;
            List<int> list = new List<int>();
            list = TableLocation(body); //获得表格位置用list保存
            IEnumerable<DocumentFormat.OpenXml.Wordprocessing.Table> tbl = body.Elements<DocumentFormat.OpenXml.Wordprocessing.Table>();
            int table = -1; //表数目,用来求表的位置 


            foreach (Table tbls in tbl)
            {
                table++;
                int location = 0;
                if (table >= 0 && table < list.LongCount())
                {
                    location = list[table];
                }

                //获得章节名以及第几个表
                string chapter = "";
                List<int> listchapter = Util.getTitlePosition(doc);//标题位置
                int numbertable = Util.NumTblCha(listchapter, list, location);//第几个表

                List<int> biglistchapter = Util.getchaptertitleposition(doc);//大章位置
                int numInBigChapter = Util.NumTblCha(biglistchapter, list, location);//大章第几个表,N

                chapter = Util.Chapter(listchapter, location, body);//获得章节号名称
                SectionProperties sectpr = sectPr(location, body);  //获取节属性
                string message = getPicMassage(location, body);
                //表名位置
                /*
                location[Chinese title,English title,blank line before table,blank line after table]
                */

                int[] index = locationOfTitleAndBlankLine(doc, location);


                //中文表名检测
                string CNtitle = null;
                string ENtitle = null;
                string num = null;
                #region
                if (index[0] == -1)
                {
                    //Util.printError("缺少中文表名，----" + message);
                }
                else if (index[0] != -1)
                {
                    Paragraph p = (Paragraph)body.ChildElements.GetItem(index[0]);
                    CNtitle = p.InnerText.Trim();
                    num = Util.number(CNtitle);

                    //中文表名位置
                    if (index[0] > location)
                    {
                        PaperFormatDetection.Tools.Util.printError("中文表名位置错误，应在表的上方，----" + CNtitle);
                    }
                    if (index[0] != location - spacelnTotableUp)
                    {
                        PaperFormatDetection.Tools.Util.printError("表名与表格之间不应空行，----" + CNtitle);
                    }


                    #region 表名字体字号居中
                    if (!Util.correctfonts(p, doc, tableFont, "Times New Roman"))
                    {
                        PaperFormatDetection.Tools.Util.printError("中文表名字体错误，应为" + tableFont + "----" + CNtitle);
                    }
                    if (!Util.correctsize(p, doc, tableFontsize))
                    {
                        PaperFormatDetection.Tools.Util.printError("中文表名字号错误，应为" + tableFontsize + "----" + CNtitle);
                    }
                    if (!Util.correctJustification(p, doc, tableJustification))
                    {
                        PaperFormatDetection.Tools.Util.printError("中文表名未居中，----" + CNtitle);
                    }
                    #endregion


                    #region 表名中间空格 m.n格式连续
                    //中文
                    //*******5.24新增 表格序号检测a[i]=0表示不成立
                    //检测项  1.序号前无空格  
                    //       2.序号后g空格
                    //       3.是否满足m.n格式
                    int[] e = Util.numberstyle(CNtitle, num.Length, MNtoName);
                    if (e[0] == 0)
                    {
                        Util.printError("中文表名序号M.N与“表”之间不应空格，----" + CNtitle);
                    }
                    if (e[1] == 0)
                    {
                        Util.printError("中文表名序号M.N与表名内容之间应空两格，----" + CNtitle);
                    }
                    if (e[2] == 0)
                    {
                        Util.printError("中文表名序号错误，应为表M.N的形式，----" + CNtitle);
                    }
                    if (!Util.correctm(num, chapter))
                    {
                        Util.printError("中文表名序号与章节号不一致，----" + CNtitle);
                    }
                    if (!Util.correctn(num, numInBigChapter, 0))
                    {
                        //Util.printError("中文表名序号M.N中的N未连续，----"  + CNtitle);
                    }
                    #endregion



                }
                #endregion

                if(type!="undergraduate")
                {
                    //英文表名检测
                    string ENnum = null;
                    #region
                    if (index[1] == -1)
                    {
                        //Util.printError("缺少英文表名,----" + message);
                    }
                    else
                    {
                        Paragraph p = (Paragraph)body.ChildElements.GetItem(index[1]);
                        ENtitle = p.InnerText.Trim();
                        ENnum = Util.number(ENtitle);

                        //英文表名位置
                        if (index[1] > location)
                        {
                            PaperFormatDetection.Tools.Util.printError("英文表名位置错误，应在表的上方，----" + ENtitle);
                        }
                        if (index[1] != location - englishspacelnTotableUp)
                        {
                            PaperFormatDetection.Tools.Util.printError("表名与表格之间不应空行，----" + ENtitle);
                        }


                        #region 表名字体字号居中
                        if (!Util.correctfonts(p, doc, englishtableFont, "Times New Roman"))
                        {
                            PaperFormatDetection.Tools.Util.printError("英文表名字体错误，应为" + englishtableFont + "----" + ENtitle);
                        }
                        if (!Util.correctsize(p, doc, englishtableFontsize))
                        {
                            PaperFormatDetection.Tools.Util.printError("英文表名字号错误，应为" + englishtableFontsize + "----" + ENtitle);
                        }
                        if (!Util.correctJustification(p, doc, englishtableJustification))
                        {
                            PaperFormatDetection.Tools.Util.printError("英文表名未居中，----" + ENtitle);
                        }
                        #endregion


                        //英文表名两格M.N Tab
                        #region
                        //*******5.24新增 表格序号检测
                        //检测项  1.Tab.正确否
                        //        2.Tab.后有空格 
                        //         3.序号后空格
                        //       4.是否满足m.n格式
                        int[] e = EnNumberStyle(ENtitle, ENnum.Length, MNtoName);
                        if (e[0] == 0)
                        {
                            PaperFormatDetection.Tools.Util.printError("英文表名序号错误，应为“Tab. M.N”的形式，----" + ENtitle);
                        }
                        if (e[1] == 0)
                        {
                            PaperFormatDetection.Tools.Util.printError("英文表名序号M.N与“Tab.”之间有且仅有一空格，----" + ENtitle);
                        }
                        if (e[2] == 0)
                        {
                            PaperFormatDetection.Tools.Util.printError("英文表名序号M.N与表名内容之间应空两格，----" + ENtitle);
                        }
                        if (e[3] == 0)
                        {
                            PaperFormatDetection.Tools.Util.printError("英文表名序号错误，应为M.N的形式，----" + ENtitle);
                        }
                        if (!Util.correctm(ENnum, chapter))
                        {
                            PaperFormatDetection.Tools.Util.printError("英文表名序号与章节号不一致，----" + ENtitle);
                        }
                        if (!Util.correctn(ENnum, numInBigChapter, 0))
                        {
                            //PaperFormatDetection.Tools.Util.printError("英文表名序号M.N中的N未连续，----"+ ENtitle);
                        }

                        #endregion
                        string SubtitleEnText = null;
                        if (getnumloc(ENtitle + 1)!=-1)
                            SubtitleEnText = ENtitle.Substring(getnumloc(ENtitle + 1)).Trim();
                        //英文表名首字母大写
                        if (SubtitleEnText!=null&&(SubtitleEnText[0] >= 97 && SubtitleEnText[0] <= 122))
                        {
                            PaperFormatDetection.Tools.Util.printError("英文表名首字母应大写，----" + ENtitle);
                        }

                    }
                    #endregion
                }

                //表跨页 上下空行
                #region
                if (CNtitle!=null&&isTableCrossPage(table+1,Util.pageDic, CNtitle))
                {
                    PaperFormatDetection.Tools.Util.printError("表格不能跨页" +  "----" + CNtitle);
                }

                if (CNtitle != null && isTableInMiddleBefore(Util.pageDic, CNtitle))
                {
                    if (index[2] == -1 || index[2] != index[0] - 1)
                    {
                        PaperFormatDetection.Tools.Util.printError("表格标题之前应空一行，----" + CNtitle);
                    }
                }

                //if (ENtitle != null && isTableInMiddleAfter(Util.pageDic, ENtitle))
                //{
                //    if (index[3] == -1|| index[3]!=location + 1)
                //    {
                //        PaperFormatDetection.Tools.Util.printError("表格之后应空一行，----" + ENtitle);
                //    }
                //}
                //if (ENtitle == null && index[3] == -1)
                //{
                //    PaperFormatDetection.Tools.Util.printError("表格若不在页尾，表格之后应空一行，----" + message);
                //}
                #endregion

                //表内检测和三线表,表过宽,居中
                #region 表内检测和三线表,表过宽,居中
                //表内检测和三线表,表过宽
                //字体正确返回1错误返回0
                //字号正确返回1错误返回0
                //Center正确返回1错误返
                int[] b = TableText(tbls, doc, intableFont, inEntableFont, intableFontsize, intableJustification);
                if (CNtitle == null || CNtitle == "")
                {
                    CNtitle = message;
                }
                if (b[0] == 0)
                {
                    PaperFormatDetection.Tools.Util.printError("表格内文字字体错误，应为" + intableFont + ",----" + CNtitle);
                }
                if (b[1] == 0)
                {
                    PaperFormatDetection.Tools.Util.printError("表格内文字字号错误，应为" + intableFontsize + "，----" + CNtitle);
                }
                if (b[2] == 0)
                {
                    PaperFormatDetection.Tools.Util.printError("表格内文字应" + intableJustification + "，----" + CNtitle);
                }
                if (!correctTable(tbls))
                {
                    PaperFormatDetection.Tools.Util.printError("表格的形式应为三线表，----" + CNtitle);
                }
                //表过宽
                if (!width(tbls, sectpr))
                {
                    PaperFormatDetection.Tools.Util.printError("表格的宽度超过页边距，----" + CNtitle);
                }
                if (detecttablecenter(tbls, tableJustification, doc))
                {
                    Util.printError("表格应居中，----" + CNtitle);
                }
                #endregion

            }
        }



        //中文表名检测
        public void detectTableCNName(WordprocessingDocument doc)
        {
            Body body = doc.MainDocumentPart.Document.Body;
            List<int> list = new List<int>();
            list = TableLocation(body); //获得表格位置用list保存
            IEnumerable<DocumentFormat.OpenXml.Wordprocessing.Table> tbl = body.Elements<DocumentFormat.OpenXml.Wordprocessing.Table>();
            int table = -1; //表数目,用来求表的位置   



            foreach (Table tbls in tbl)
            {
                table++;
                int location = 0;
                if (table >= 0 && table < list.LongCount())
                {
                    location = list[table];
                }

                //获得章节名以及第几个表
                string chapter = "";
                List<int> listchapter = Util.getTitlePosition(doc);//标题位置
                int numbertable = Util.NumTblCha(listchapter, list, location);//第几个表

                List<int> biglistchapter = Util.getchaptertitleposition(doc);//大章位置
                int numInBigChapter = Util.NumTblCha(biglistchapter, list, location);//大章第几个表,N

                chapter = Util.Chapter(listchapter, location, body);//获得章节号名称
                string message = getPicMassage(location, body);

                //表名位置
                /*
                location[Chinese title,English title,blank line before table,blank line after table]
                */
                int[] index = locationOfTitleAndBlankLine(doc, location);


                //中文表名
                string CNtitle = null;
                string num = null;
                //中文表名检测
                #region
                if (index[0] == -1)
                {
                   // Util.printError("缺少中文表名，----" + message);
                }
                else if(index[0]!=-1)
                {
                    Paragraph p = (Paragraph)body.ChildElements.GetItem(index[0]);
                    CNtitle = p.InnerText.Trim();
                    num = Util.number(CNtitle);

                    //中文表名位置
                    if (index[0] > location)
                    {
                        PaperFormatDetection.Tools.Util.printError("中文表名位置错误，应在表的上方，----" + CNtitle);
                    }
                    if (index[0] != location - spacelnTotableUp)
                    {
                        PaperFormatDetection.Tools.Util.printError("表名与表格之间不应空行，----" + CNtitle);
                    }


                    #region 表名字体字号居中
                    if (!Util.correctfonts(p, doc, tableFont, "Times New Roman"))
                    {
                        PaperFormatDetection.Tools.Util.printError("中文表名字体错误，应为" + tableFont+"----" + CNtitle);
                    }
                    if (!Util.correctsize(p, doc, tableFontsize))
                    {
                        PaperFormatDetection.Tools.Util.printError("中文表名字号错误，应为" + tableFontsize + "----" + CNtitle);
                    }
                    if (!Util.correctJustification(p, doc, tableJustification))
                    {
                        PaperFormatDetection.Tools.Util.printError("中文表名未居中，----"+ CNtitle );
                    }
                    #endregion


                    #region 表名中间空格 m.n格式连续
                    //中文
                    //*******5.24新增 表格序号检测a[i]=0表示不成立
                    //检测项  1.序号前无空格  
                    //       2.序号后g空格
                    //       3.是否满足m.n格式
                    int[] e = Util.numberstyle(CNtitle, num.Length, MNtoName);
                    if (e[0] == 0)
                    {
                        Util.printError("中文表名序号M.N与“表”之间不应空格，----"+ CNtitle);
                    }
                    if (e[1] == 0)
                    {
                        Util.printError("中文表名序号M.N与表名内容之间应空两格，----" + CNtitle);
                    }
                    if (e[2] == 0)
                    {
                        Util.printError("中文表名序号错误，应为表M.N的形式，----" + CNtitle );
                    }
                    if (!Util.correctm(num, chapter))
                    {
                        Util.printError("中文表名序号与章节号不一致，----" + CNtitle);
                    }
                    if (!Util.correctn(num, numInBigChapter, 0))
                    {
                        //Util.printError("中文表名序号M.N中的N未连续，----"  + CNtitle);
                    }
                    #endregion



                }
                #endregion

            }
        }




        //英文表名检测
        public void detectTableENName(WordprocessingDocument doc)
        {
            Body body = doc.MainDocumentPart.Document.Body;
            List<int> list = new List<int>();
            list = TableLocation(body); //获得表格位置用list保存
            IEnumerable<DocumentFormat.OpenXml.Wordprocessing.Table> tbl = body.Elements<DocumentFormat.OpenXml.Wordprocessing.Table>();
            int table = -1; //表数目,用来求表的位置   



            foreach (Table tbls in tbl)
            {
                table++;
                int location = 0;
                if (table >= 0 && table < list.LongCount())
                {
                    location = list[table];
                }

                //获得章节号以及第几个表
                string chapter = "";
                List<int> listchapter = Util.getTitlePosition(doc);
                int numbertable = Util.NumTblCha(listchapter, list, location);//第几个表
                List<int> biglistchapter = Util.getchaptertitleposition(doc);//大章位置
                int numInBigChapter = Util.NumTblCha(biglistchapter, list, location);//大章第几个表,N
                chapter = Util.Chapter(listchapter, location, body);//获得章节号
                string message = getPicMassage(location, body);
                //表名位置
                /*
                location[Chinese title,English title,blank line before table,blank line after table]
                */
                int[] index = locationOfTitleAndBlankLine(doc, location);
                //英文表名
                string ENtitle = null;
                string ENnum = null;


                //英文表名检测
                #region
                if (index[1] == -1)
                {
                    //Util.printError("缺少英文表名,----" +message);
                }
                else
                {
                    Paragraph p = (Paragraph)body.ChildElements.GetItem(index[1]);
                    ENtitle = p.InnerText.Trim();
                    ENnum = Util.number(ENtitle);

                    //英文表名位置
                    if (index[1] > location)
                    {
                        PaperFormatDetection.Tools.Util.printError("英文表名位置错误，应在表的上方，----" + ENtitle);
                    }
                    if (index[1] != location - englishspacelnTotableUp)
                    {
                        PaperFormatDetection.Tools.Util.printError("表名与表格之间不应空行，----" + ENtitle);
                    }


                    #region 表名字体字号居中
                    if (!Util.correctfonts(p, doc, englishtableFont, "Times New Roman"))
                    {
                        PaperFormatDetection.Tools.Util.printError("英文表名字体错误，应为" + englishtableFont + "----" + ENtitle );
                    }
                    if (!Util.correctsize(p, doc, englishtableFontsize))
                    {
                        PaperFormatDetection.Tools.Util.printError("英文表名字号错误，应为" + englishtableFontsize + "----" + ENtitle );
                    }
                    if (!Util.correctJustification(p, doc, englishtableJustification))
                    {
                        PaperFormatDetection.Tools.Util.printError("英文表名未居中，----" + ENtitle);
                    }
                    #endregion


                    //英文表名两格M.N Tab
                    #region
                    //*******5.24新增 表格序号检测
                    //检测项  1.Tab.正确否
                    //        2.Tab.后有空格 
                    //         3.序号后空格
                    //       4.是否满足m.n格式
                    int[] e = EnNumberStyle(ENtitle, ENnum.Length, MNtoName);
                    if (e[0] == 0)
                    {
                        PaperFormatDetection.Tools.Util.printError("英文表名序号错误，应为“Tab. M.N”的形式，----" + ENtitle);
                    }
                    if (e[1] == 0)
                    {
                        PaperFormatDetection.Tools.Util.printError("英文表名序号M.N与“Tab.”之间有且仅有一空格，----"+ ENtitle);
                    }
                    if (e[2] == 0)
                    {
                        PaperFormatDetection.Tools.Util.printError("英文表名序号M.N与表名内容之间应空两格，----" + ENtitle);
                    }
                    if (e[3] == 0)
                    {
                        PaperFormatDetection.Tools.Util.printError("英文表名序号错误，应为M.N的形式，----" + ENtitle);
                    }
                    if (!Util.correctm(ENnum, chapter))
                    {
                        PaperFormatDetection.Tools.Util.printError("英文表名序号与章节号不一致，----"  + ENtitle);
                    }
                    if (!Util.correctn(ENnum, numInBigChapter, 0))
                    {
                        //PaperFormatDetection.Tools.Util.printError("英文表名序号M.N中的N未连续，----"+ ENtitle);
                    }

                    #endregion
                    string SubtitleEnText=ENtitle.Substring(getnumloc(ENtitle+1)).Trim();
                    //英文表名首字母大写
                    if ((SubtitleEnText[0] >= 97  && SubtitleEnText[0] <= 122))
                    {
                        PaperFormatDetection.Tools.Util.printError("英文表名首字母应大写，----" + ENtitle);
                    }
                    
                }
                #endregion






            }
        }

       


        //表内检测和三线表,表过宽，表居中
        //字体正确返回1错误返回0
        //字号正确返回1错误返回0
        //Center正确返回1错误返回0
        public void detectTableInText(WordprocessingDocument doc)
        {
            Body body = doc.MainDocumentPart.Document.Body;
            List<int> list = new List<int>();
            list = TableLocation(body); //获得表格位置用list保存
            IEnumerable<DocumentFormat.OpenXml.Wordprocessing.Table> tbl = body.Elements<DocumentFormat.OpenXml.Wordprocessing.Table>();
            int table = -1; //表数目,用来求表的位置   




            foreach (Table tbls in tbl)
            {
                table++;
                int location = 0;
                if (table >= 0 && table < list.LongCount())
                {
                    location = list[table];
                }
                //获得章节号以及第几个表
                string chapter = "";
                List<int> listchapter = Util.getTitlePosition(doc);
                int numbertable = Util.NumTblCha(listchapter, list, location);//第几个表
                chapter = Util.Chapter(listchapter, location, body);//获得章节号
                SectionProperties sectpr = sectPr(location, body);  //获取节属性

                //获取表名
                string[] title = { null,null};
                title = getTableName(doc, location);
                string CNtitle = title[0];
                string message = getPicMassage(location, body);
                if(CNtitle ==null|| CNtitle=="")
                {
                    CNtitle = message;
                }
                #region 表内检测和三线表,表过宽,居中
                //表内检测和三线表,表过宽
                //字体正确返回1错误返回0
                //字号正确返回1错误返回0
                //Center正确返回1错误返
                int[] b = TableText(tbls, doc, intableFont, inEntableFont, intableFontsize, intableJustification);
                if (b[0] == 0)
                {
                    PaperFormatDetection.Tools.Util.printError("表格内文字字体错误，应为"+ intableFont+ ",----"+ CNtitle );
                }
                if (b[1] == 0)
                {
                    PaperFormatDetection.Tools.Util.printError("表格内文字字号错误，应为"+ intableFontsize +"，----"  + CNtitle);
                }
                if (b[2] == 0)
                {
                    PaperFormatDetection.Tools.Util.printError("表格内文字应"+intableJustification+"，----"+ CNtitle);
                }
                if (!correctTable(tbls))
                {
                    PaperFormatDetection.Tools.Util.printError("表格的形式应为三线表，----" + CNtitle);
                }
                //表过宽
                if (!width(tbls, sectpr))
                {
                    PaperFormatDetection.Tools.Util.printError("表格的宽度超过页边距，----"+ CNtitle);
                }
                if (!detecttablecenter(tbls, tableJustification, doc))
                {
                    Util.printError("表格未居中，----" + CNtitle);
                }

                #endregion

            }


        }



        //表上下空行
        public void detectTableLocation(WordprocessingDocument doc)
        {
            Body body = doc.MainDocumentPart.Document.Body;
            List<int> list = new List<int>();
            list = TableLocation(body); //获得表格位置用list保存
            IEnumerable<DocumentFormat.OpenXml.Wordprocessing.Table> tbl = body.Elements<DocumentFormat.OpenXml.Wordprocessing.Table>();
            int table = -1; //表数目,用来求表的位置   

            foreach (Table tbls in tbl)
            {
                table++;
                int location = 0;
                if (table >= 0 && table < list.LongCount())
                {
                    location = list[table];
                }
                //获得章节号以及第几个表
                string chapter = "";
                List<int> listchapter = Util.getTitlePosition(doc);
                int numbertable = Util.NumTblCha(listchapter, list, location);//第几个表
                chapter = Util.Chapter(listchapter, location, body);//获得章节号

                string message = getPicMassage(location, body);

                //表名位置
                /*
                location[Chinese title,English title,blank line before table,blank line after table]
                */
                int[] index = locationOfTitleAndBlankLine(doc, location);
                string[] title = { null, null };
                title = getTableName(doc, location);
                string CNtitle = title[0];
                string ENtitle = title[1];

                //表前后空行
                #region
                if (index[2] == -1)
                {
                    if (CNtitle == null)
                    {
                        PaperFormatDetection.Tools.Util.printError("表格若不在页首，表格标题之前应空一行，----" + message);
                    }
                    else
                    {
                        PaperFormatDetection.Tools.Util.printError("表格若不在页首，表格标题之前应空一行，----" + CNtitle);
                    }
                }
                else
                {
                    if (index[0]!=-1 && index[2] != index[0] - 1)
                    {
                        PaperFormatDetection.Tools.Util.printError("表格若不在页首，表格标题之前应空一行，----" + CNtitle );
                    }
                }
                //后
                if (index[3] == -1)
                {
                    if (CNtitle == null)
                    {
                        PaperFormatDetection.Tools.Util.printError("表格若不在页尾，表格之后应空一行，----" +message);
                    }
                    else
                    {
                        PaperFormatDetection.Tools.Util.printError("表格若不在页尾，表格之后应空一行，----" + CNtitle);
                    }
                }
                else
                {
                    if (index[3] != location + 1)
                    {
                        PaperFormatDetection.Tools.Util.printError("表格若不在页尾，表格之后应空一行，----" + CNtitle);
                    }
                }
                #endregion


            }
        }



































        //表格(上面）是否在每页的中间 在中间返回true
        protected bool isTableInMiddleBefore(Dictionary<string, string> pageDic, string tableName)
        {
            //通过键的集合取
            foreach (string keys in pageDic.Keys)
            {
                if (tableName == keys)
                {
                    string[] sArray = pageDic[keys].Split('_');// 一定是单引                    
                    int a = int.Parse(sArray[0]);
                    int b= int.Parse(sArray[1]);
                    if(a==b)
                    {
                        return true;
                    }
                }
            }
            return false;
        }
        //表格(下面）是否在每页的中间 在中间返回true
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


        //跨页返回true
        protected bool isTableCrossPage(int i,Dictionary<string, string> pageDic,string tableName)
        {
            string key = "第" + i + "张表";
            int tablePageNum =-1;
            int tableNamePageNum = -1;
            //通过键的集合取
            foreach (string keys in pageDic.Keys)
            {
                if(key==keys)
                {
                    tablePageNum = int.Parse(pageDic[keys]);
                }
                if(tableName==keys)
                {
                    string[] sArray = pageDic[keys].Split('_');// 一定是单引                    
                    tableNamePageNum = int.Parse(sArray[1]);
                }

            }
            if(tablePageNum != tableNamePageNum)
            {
                return true;
            }
            else
            {
                return false;
            }
        }



        protected int getnumloc(string ENtitletitle)
        {
            int i = 0;
            foreach (char c in ENtitletitle)
            {
                i++;
                if (c <= '9' && c >= '0'&&i+1<ENtitletitle.Length)
                {
                    if (ENtitletitle[i + 1] == ' ')
                    {
                        return i;
                    }
                }
            }
            return -1;
        }
        //           if(m[0]==1)
        //           {
        //               PaperFormatDetection.Tools.Util.printError("英文表名首字母应大写，----" + ENtitle);
        //           }
        //           if(m[1]==1)
        //           {
        //               PaperFormatDetection.Tools.Util.printError("英文表名内虚词首字母应小写，----" + ENtitle);
        //           }
        //           if (m[2] == 1)
        //           {
        //               PaperFormatDetection.Tools.Util.printError("英文表名内实词首字母应大写，----" + ENtitle);
        //           }
        //           if (m[3] == 1)
        //           {
        //               PaperFormatDetection.Tools.Util.printError("英文表名内单词间含有多余的空格，----" + ENtitle);
        //           }
        //public int[] firstLetterBigger(string SubtitleEnText)
        //{
        //    int []flag = new int[4]{ 0, 0, 0, 0 };
        //    //英文标题字母大小写的检测
        //    string[] CapitalEmptyWord = { "A", "Above", "An", "As", "Behind", "But", "Before", "If", "The", "At", "For", "From", "Of", "Off", "On", "To", "In", "With", "And", "As", "While", "So" };
        //    string[] SmallEmptyWord = { "a", "above", "an", "as", "behind", "but", "before", "if", "the", "at", "for", "from", "of", "off", "on", "to", "in", "with", "and", "as", "while", "so" };
        //    int SubtitleEnTextLength = SubtitleEnText.Length;
        //    if (!(SubtitleEnText[0] >= 65 && SubtitleEnText[0] <= 90))
        //    {
        //        flag[0] = 1;  //首字母应大写
        //    }
        //    int LastSpaceInText = SubtitleEnText.LastIndexOf(' ');
        //    int NumInText = 0;
        //    bool EmptyWordFlag = true;
        //    bool NotionalWordFlag = true;
        //    bool IsHaveEmptyWord = false;
        //    for (; NumInText < SubtitleEnTextLength; NumInText++)
        //    {
        //        if (SubtitleEnText[NumInText] == ' ' && NumInText < LastSpaceInText)
        //        {
        //            string Word = "";
        //            bool IsWordEmpty = true;
        //            bool WordFlag = true;
        //            int a = SubtitleEnText.IndexOf(' ', NumInText + 1);
        //            Word = SubtitleEnText.Substring(NumInText + 1, a - NumInText - 1);
        //            if (a - NumInText - 1 > 0)
        //                IsWordEmpty = false;
        //            else
        //                IsHaveEmptyWord = true;
        //            foreach (string S in CapitalEmptyWord)
        //            {
        //                if (Word == S) EmptyWordFlag = false;
        //            }
        //            foreach (string C in SmallEmptyWord)
        //            {
        //                if (Word == C) WordFlag = false;
        //            }
        //            if (WordFlag && !IsWordEmpty)
        //            {
        //                if (!(Word[0] >= 65 && Word[0] <= 90) && Word.IndexOf("(") == -1) NotionalWordFlag = false;
        //            }
        //        }
        //        if (SubtitleEnText[NumInText] == ' ' && NumInText == LastSpaceInText)
        //        {
        //            bool WordFlag = true;
        //            string Word = SubtitleEnText.Substring(NumInText + 1);
        //            foreach (string S in CapitalEmptyWord)
        //            {
        //                if (Word == S) EmptyWordFlag = false;
        //            }
        //            foreach (string C in SmallEmptyWord)
        //            {
        //                if (Word == C) WordFlag = false;
        //            }
        //            if (WordFlag)
        //            {
        //                if (Word[0] != '\0')
        //                    if (!(Word[0] >= 65 && Word[0] <= 90) && Word.IndexOf("(") == -1) NotionalWordFlag = false;
        //            }
        //        }
        //    }

        //    if (!EmptyWordFlag) flag[1] = 1;//Util.printError("论文英文题目里非句首的虚词有首字母大写的错误");
        //    if (!NotionalWordFlag) flag[2] = 1; //Util.printError("论文英文题目里的实词有首字母小写的错误");
        //    if (IsHaveEmptyWord) flag[3] = 1;//Util.printError("论文英文题目里单词间含有多余的空格");
        //    return flag;
        //}



        //title = { CNtitle, ENtitle };
        protected string[] getTableName(WordprocessingDocument doc, int location)
        {
            Body body = doc.MainDocumentPart.Document.Body;



            string[] title =new string[2];



            //表名位置
            /*
            location[Chinese title,English title,blank line before table,blank line after table]
            */
            int[] index = locationOfTitleAndBlankLine(doc, location);


            if (index[0] != -1)
            {
                Paragraph p = (Paragraph)body.ChildElements.GetItem(index[0]);
                title[0]=p.InnerText.Trim();
            }
            if (index[1] != -1)
            {
                Paragraph p = (Paragraph)body.ChildElements.GetItem(index[1]);
                title[1] = p.InnerText.Trim();
            }

            return title;
        }






        //判断是否为三线表 是为true
        protected bool correctTable(DocumentFormat.OpenXml.Wordprocessing.Table t)
        {
            int tcCount = 0;
            IEnumerable<TableRow> trList = t.Elements<TableRow>();
            int rowCount = trList.Count<TableRow>();
            TableProperties tpr = t.GetFirstChild<TableProperties>();
            TableBorders tb = tpr.GetFirstChild<TableBorders>();
            if (tpr != null)
            {

                if (tb != null)
                {
                    if (rowCount <= 2)
                    {
                        return true;
                    }
                    foreach (TableRow tr in trList)
                    {
                        tcCount++;
                        IEnumerable<TableCell> tcList = tr.Elements<TableCell>();
                        foreach (TableCell tc in tcList)
                        {
                            TableCellProperties tcp = tc.GetFirstChild<TableCellProperties>();
                            int bottom = 1;
                            if (tcp != null)
                            {
                                TableCellBorders tcb = tcp.GetFirstChild<TableCellBorders>();
                                if (tcb != null)
                                {
                                    if (tcb.GetFirstChild<LeftBorder>() != null)
                                    {
                                        if (tcb.GetFirstChild<LeftBorder>().Val != "nil")
                                        {
                                            return false;
                                        }
                                    }
                                    if (tcb.GetFirstChild<RightBorder>() != null)
                                    {
                                        if (tcb.GetFirstChild<RightBorder>().Val != "nil")
                                        {
                                            return false;
                                        }
                                    }
                                    //第一行
                                    if (tcCount == 1)
                                    {
                                        if (tcb.GetFirstChild<BottomBorder>() != null)
                                        {
                                            if (tcb.GetFirstChild<BottomBorder>().Val == "nil")
                                            {
                                                bottom = 0;
                                            }
                                        }
                                        else
                                        {
                                            if (tb.GetFirstChild<InsideHorizontalBorder>() != null)
                                            {
                                                if (tb.GetFirstChild<InsideHorizontalBorder>().Val == "none")
                                                {
                                                    return false;
                                                }
                                            }

                                        }
                                        if (tcb.GetFirstChild<TopBorder>() != null)
                                        {
                                            if (tcb.GetFirstChild<TopBorder>().Val == "nil")
                                            {
                                                return false;
                                            }
                                        }
                                        else
                                        {
                                            if (tb.GetFirstChild<TopBorder>() != null)
                                            {
                                                if (tb.GetFirstChild<TopBorder>().Val == "none")
                                                {
                                                    return false;
                                                }
                                            }
                                        }
                                    }
                                    //第二行的top
                                    if (tcCount == 2)
                                    {
                                        if (tcb.GetFirstChild<TopBorder>() != null)
                                        {
                                            if (tcb.GetFirstChild<TopBorder>().Val == "nil" && bottom == 0)
                                            {
                                                return false;
                                            }
                                        }
                                    }
                                    //除去第一行和最后一行的其他所有行
                                    if (tcCount != 1 && tcCount != rowCount)
                                    {
                                        if (tcb.GetFirstChild<BottomBorder>() != null)
                                        {
                                            if (tcb.GetFirstChild<BottomBorder>().Val == "single")
                                            {
                                                return false;
                                            }
                                        }
                                        else
                                        {
                                            if (tcCount != 2 && tb.GetFirstChild<InsideHorizontalBorder>() != null && tb.GetFirstChild<InsideHorizontalBorder>().Val == "single")
                                            {
                                                return false;
                                            }
                                        }
                                    }
                                    //最后一行并且不是第二行
                                    if (tcCount == rowCount && tcCount != 2)
                                    {
                                        if (tcb.GetFirstChild<TopBorder>() != null)
                                        {
                                            if (tcb.GetFirstChild<TopBorder>().Val == "single")
                                            {
                                                return false;
                                            }
                                        }
                                        else
                                        {
                                            if (tb.GetFirstChild<InsideHorizontalBorder>() != null && tb.GetFirstChild<InsideHorizontalBorder>().Val == "single")
                                            {
                                                return false;
                                            }
                                        }
                                        if (tcb.GetFirstChild<BottomBorder>() != null)
                                        {
                                            if (tcb.GetFirstChild<BottomBorder>().Val == "nil")
                                            {
                                                return false;
                                            }
                                        }
                                        else
                                        {
                                            if (tb.GetFirstChild<BottomBorder>() != null)
                                            {
                                                if (tb.GetFirstChild<BottomBorder>().Val == "none")
                                                {
                                                    return false;
                                                }
                                            }
                                        }
                                    }
                                }
                                //没有tcb的情况
                                else
                                {
                                    //第一行
                                    if (tcCount == 1)
                                    {
                                        if (tb.GetFirstChild<TopBorder>() != null)
                                        {
                                            if (tb.GetFirstChild<TopBorder>().Val == "none")
                                            {
                                                return false;
                                            }
                                        }
                                        if (tb.GetFirstChild<InsideHorizontalBorder>() != null)
                                        {
                                            if (tb.GetFirstChild<InsideHorizontalBorder>().Val == "none")
                                            {
                                                return false;
                                            }
                                        }
                                    }
                                    //中间行
                                    if (tcCount != 1 && tcCount != rowCount)
                                    {
                                        if (tcCount != 2 && tb.GetFirstChild<InsideHorizontalBorder>() != null && tb.GetFirstChild<InsideHorizontalBorder>().Val == "single")
                                        {
                                            return false;
                                        }
                                    }
                                    //最后一行
                                    if (tcCount == rowCount && tcCount - 1 != rowCount)
                                    {
                                        if (tb.GetFirstChild<InsideHorizontalBorder>() != null && tb.GetFirstChild<InsideHorizontalBorder>().Val == "single")
                                        {
                                            return false;
                                        }
                                        if (tb.GetFirstChild<BottomBorder>() != null)
                                        {
                                            if (tb.GetFirstChild<BottomBorder>().Val == "none")
                                            {
                                                return false;
                                            }
                                        }
                                    }

                                }
                            }
                        }

                    }

                }

            }
            return true;
        }





        //字体正确返回1错误返回0
        //字号正确返回1错误返回0
        //Center正确返回1错误返回0
        protected int[] TableText(DocumentFormat.OpenXml.Wordprocessing.Table table, WordprocessingDocument doc, string font, string enFont, string size, string justification)
        {
            int[] a = new int[3] { 1, 1, 1 };
            IEnumerable<TableRow> tr = table.Elements<TableRow>();
            foreach (TableRow trs in tr)
            {
                IEnumerable<TableCell> tc = trs.Elements<TableCell>();
                foreach (TableCell tcs in tc)
                {
                    IEnumerable<DocumentFormat.OpenXml.Wordprocessing.Paragraph> paras = tcs.Elements<DocumentFormat.OpenXml.Wordprocessing.Paragraph>();
                    foreach (DocumentFormat.OpenXml.Wordprocessing.Paragraph p in paras)
                    {
                        if (Util.correctfonts(p, doc, font, enFont) == false)
                        {
                            a[0] = 0;
                        }
                        if (Util.correctsize(p, doc, size) == false)
                        {
                            a[1] = 0;
                        }
                        if (Util.correctJustification(p, doc, justification) == false)
                        {
                            a[2] = 0;
                        }
                    }
                }
            }
            return a;
        }
























        //英文
        //*******5.24新增 表格序号检测
        //检测项  1.Tab.正确否
        //        2.Tab.后有空格 
        //         3.序号后g空格
        //       4.是否满足m.n格式
         protected  static int[] EnNumberStyle(string title, int numlen,int g)
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
            if (title.IndexOf("Tab.") < 0)
            {
                a[0] = 0;
                if (title.IndexOf("Tab") >= 0)
                {
                    if (title.IndexOf("Tab ") == -1)
                    {
                        a[1] = 0;
                    }
                }
            }
            else
            {
                if (title.IndexOf("Tab. ") < 0)
                {
                    a[1] = 0;//若没有空格报错
                }
                else
                {
                    //多空格报错
                    if (title.IndexOf("Tab.  ") >= 0)
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
                else if (title[l + 1] != '.'||title[l + 2] != '.')
                {
                    a[3] = 0;
                }
            }
            return a;
        }









        /*
         * 初始值-1，false
        location[Chinese title,English title,blank line before table,blank line after table]
        find[i]=true   表示找到
        */
        protected  int[] locationOfTitleAndBlankLine(WordprocessingDocument wordPro, int tablelocation)
        {
            int[] location = { -1, -1, -1, -1 };
            bool[] find = { false, false, false, false };
            Regex[] reg;
            reg = new Regex[9];
            reg[0] = new Regex(@"^表[1-9][0-9]*\.[1-9][0-9]*  ");//中文表匹配  关键字段：表m.n空格空格
            reg[1] = new Regex(@"^表[1-9][0-9]*\.[1-9][0-9]*");//中文表匹配  关键字段：表m.n
            reg[2] = new Regex(@"^表\ *[1-9][0-9]*");//中文表匹配  关键字段：表m
            reg[3] = new Regex(@"^Tab.\ *[1-9][0-9]*\.[1-9][0-9]*  ");//英文表匹配  关键字段Tab.空格m.n空格空格
            reg[4] = new Regex(@"^Tab. [1-9][0-9]*\.[1-9][0-9]*");//英文表匹配  关键字段Tab.空格m.n
            reg[5] = new Regex(@"^Tab.[1-9][0-9]*\.[1-9][0-9]*");//英文表匹配  关键字段Tab.m.n
            reg[6] = new Regex(@"^Tab(. [1-9][0-9]*)*");//英文表匹配  关键字段Tab.空格m
            reg[7] = new Regex(@"^Tab.[1-9][0-9]*");//英文表匹配  关键字段Tab.m
            reg[8] = new Regex(@"^Tab\ *[1-9][0-9]*");//英文表匹配  关键字段Tab  m


            Body body = wordPro.MainDocumentPart.Document.Body;
            //从table往前找
            for (int index = tablelocation - 1; index > tablelocation - 4 && index >= 0; index--)
            {
                if (body.ChildElements.GetItem(index).GetType().ToString() == "DocumentFormat.OpenXml.Wordprocessing.Paragraph")
                {
                    Paragraph p = (Paragraph)body.ChildElements.GetItem(index);
                    string s = p.InnerText.Trim();
                    if (s.Length == 0 && find[2] == false)
                    {
                        if (find[2] == false)//表前空行匹配
                        {
                            location[2] = index;
                            find[2] = true;
                        }
                    }
                    else if (s.Length > 0 && s.Length < 100)//长度过滤
                    {
                        //中文表名匹配
                        for (int i = 0; i <= 2; i++)
                        {
                            Match m = reg[i].Match(s);
                            if (m.Success)
                            {
                                if (find[0] == false && s.Length <= 40)
                                {
                                    location[0] = index;
                                    find[0] = true;
                                    break;
                                }
                            }
                        }
                        //英文表名匹配
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
            //从table往后找
            for (int index = tablelocation + 1; index < tablelocation + 3 && index < body.Count(); index++)
            {
                if (body.ChildElements.GetItem(index).GetType().ToString() == "DocumentFormat.OpenXml.Wordprocessing.Paragraph")
                {
                    Paragraph p = (Paragraph)body.ChildElements.GetItem(index);
                    string s = p.InnerText.Trim();
                    if (s.Length == 0 && find[3] == false)
                    {
                        location[3] = index;
                        find[3] = true;
                        break;
                    }
                }
            }
            return location;
        }



        //获得表格位置用list保存
        protected List<int> TableLocation(Body body)
        {
            List<int> list = new List<int>(30);
            int l = body.ChildElements.Count();
            for (int i = 0; i < l; i++)
            {
                if (body.ChildElements.GetItem(i).GetType().ToString() == "DocumentFormat.OpenXml.Wordprocessing.Table")
                {
                    list.Add(i);
                }
            }
            return list;
        }











        //判断是否超宽
        public bool width(Table table, SectionProperties sectPr)
        {

            uint width = 0;
            uint pagewidth = 0;
            uint leftmargin = 0;
            uint rightmargin = 0;
            //获得表宽
            if (table.GetFirstChild<TableProperties>() != null)
            {
                if (table.GetFirstChild<TableProperties>().GetFirstChild<TableWidth>() != null)
                {
                    width = Convert.ToUInt32(table.GetFirstChild<TableProperties>().GetFirstChild<TableWidth>().Width.Value);
                }
            }
            if (width == 0)
            {
                if (table.GetFirstChild<TableGrid>() != null)
                {
                    IEnumerable<GridColumn> gridCols = table.GetFirstChild<TableGrid>().Elements<GridColumn>();
                    foreach (GridColumn gridCol in gridCols)
                    {
                        width += Convert.ToUInt32(gridCol.Width.Value);
                    }
                }
            }
            //获得左、右间距、页宽
            if (sectPr != null)
            {
                if (sectPr.GetFirstChild<PageMargin>() != null)
                {
                    if (sectPr.GetFirstChild<PageMargin>().Left != null)
                    {
                        leftmargin = sectPr.GetFirstChild<PageMargin>().Left.Value;
                    }
                    if (sectPr.GetFirstChild<PageMargin>().Right != null)
                    {
                        rightmargin = sectPr.GetFirstChild<PageMargin>().Right;
                    }
                }
                if (sectPr.GetFirstChild<PageSize>() != null)
                {
                    pagewidth = sectPr.GetFirstChild<PageSize>().Width.Value;
                }
            }
            //1.若是浮动型
            if (table.GetFirstChild<TableProperties>() != null)
            {
                if (table.GetFirstChild<TableProperties>().GetFirstChild<TablePositionProperties>() != null)
                {
                    TablePositionProperties tblpPr = table.GetFirstChild<TableProperties>().GetFirstChild<TablePositionProperties>();
                    
                    if (tblpPr.HorizontalAnchor!=null&&tblpPr.HorizontalAnchor.Value.ToString() == "Margin")
                    {
                        string s = tblpPr.HorizontalAnchor.Value.ToString();
                        if (tblpPr.TablePositionX != null && tblpPr.TablePositionXAlignment == null)
                        {
                            if (tblpPr.TablePositionX.Value >= 0 && tblpPr.TablePositionX.Value + width + leftmargin < pagewidth - rightmargin)
                            {
                                return true;
                            }
                            else
                            {
                                return false;
                            }
                        }
                        if (tblpPr.TablePositionX == null && tblpPr.TablePositionXAlignment == null)
                        {
                            return true;
                        }
                        if (tblpPr.TablePositionXAlignment != null)
                        {
                            if (pagewidth - leftmargin - rightmargin >= width)
                            {
                                return true;
                            }
                            else
                            {
                                return false;
                            }
                        }
                    }
                    if (tblpPr.HorizontalAnchor.Value.ToString() == "Page")
                    {
                        if (tblpPr.TablePositionX != null && tblpPr.TablePositionXAlignment == null)
                        {
                            if (tblpPr.TablePositionX.Value >= leftmargin && tblpPr.TablePositionX.Value + width < pagewidth - rightmargin)
                            {
                                return true;
                            }
                            else
                            {
                                return false;
                            }
                        }
                        if (tblpPr.TablePositionX == null && tblpPr.TablePositionXAlignment == null)
                        {
                            return true;
                        }
                        if (tblpPr.TablePositionXAlignment != null)
                        {
                            if (pagewidth - leftmargin - rightmargin >= width)
                            {
                                return true;
                            }
                            else
                            {
                                return false;
                            }
                        }
                    }
                }
                //若是无环绕型
                else if (table.GetFirstChild<TableProperties>().GetFirstChild<TableIndentation>() != null)
                {
                    int indentation = table.GetFirstChild<TableProperties>().GetFirstChild<TableIndentation>().Width.Value;
                    if (indentation < 0)
                    {
                        return false;
                    }
                    else
                    {
                        if (width - indentation + leftmargin > pagewidth - rightmargin)
                        {
                            return false;
                        }
                        else
                        {
                            return true;
                        }
                    }
                }
                else
                {
                    if (table.GetFirstChild<TableProperties>().GetFirstChild<TableJustification>() != null)
                    {
                        if (width > pagewidth - leftmargin - rightmargin)
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



        /**
         * 获取节属性
         */
        public SectionProperties sectPr(int location, Body body)
        {
            SectionProperties sectPr = new SectionProperties();
            int flag = 0;
            for (int i = location; i < body.ChildElements.Count(); i++)
            {
                if (body.ChildElements.GetItem(i).GetFirstChild<ParagraphProperties>() != null)
                {
                    if (body.ChildElements.GetItem(i).GetFirstChild<ParagraphProperties>().GetFirstChild<SectionProperties>() != null)
                    {
                        flag = 1;
                        sectPr = body.ChildElements.GetItem(i).GetFirstChild<ParagraphProperties>().GetFirstChild<SectionProperties>();
                        return sectPr;
                    }
                }
            }
            if (flag == 0)
            {
                if (body.ChildElements.GetItem(body.ChildElements.Count() - 1).GetType().ToString() == "DocumentFormat.OpenXml.Wordprocessing.SectionProperties")
                {
                    sectPr = (SectionProperties)body.ChildElements.GetItem(body.ChildElements.Count() - 1);
                }
            }
            return sectPr;
        }





        //判断是否是续表
        public bool continuedtable(string[] title)
        {
            if (title[0] != null && title[1] != null)
            {
                if (title[0].Length <= 4 || title[1].Length <= 2)
                {
                    return false;
                }
                else
                {
                    if (title[0].IndexOf("Cont") >= 0 || (title[1].IndexOf("续") >= 0))
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            else
            {
                if (title[0] != null && title[1] == null)
                {
                    if (title[0].Length <= 4)
                    {
                        return false;
                    }
                    if (title[0].IndexOf("Cont") >= 0)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                else if (title[0] == null && title[1] != null)
                {
                    if (title[1].Length <= 2)
                    {
                        return false;
                    }
                    if (title[1].IndexOf("续") >= 0)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                return false;
            }
        }



        //每章续表个数
        //????????????????????????????????????????????
        public int contNumEveryChapter(WordprocessingDocument doc, List<int> list, int location, string[] title)
        {
            int continued = 0;
            List<int> listchapter2 = Util.getchaptertitleposition(doc);//获得章标题位置
            int numbertablechapter = Util.NumTblCha(listchapter2, list, location);//判断是从某个章开始的第几个表格
                                                                                  //续表统计
                                                                                  //新节开始，将continued置为0
            if (numbertablechapter == 1)
            {
                continued = 0;
            }
            if (continuedtable(title) == true)
            {
                continued++;
                //   Console.WriteLine("continued"+continued);
            }
            return continued;
        }

        //未命名表的信息
        protected string getPicMassage(int i, Body body)
        {
            string message = "";
            for (int j = i + 1; j < i + 6&&j<body.ChildElements.Count(); j++)
            {
                if (j < body.ChildElements.Count())
                {
                    if (body.ChildElements.GetItem(j).GetType().ToString() == "DocumentFormat.OpenXml.Wordprocessing.Paragraph")
                    {
                        message = Util.getFullText((Paragraph)body.ChildElements.GetItem(j)).Trim();
                        if (message != "")
                        {
                            if (message.Length >= 11)
                            {
                                message = message.Substring(0, 10);
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
