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
    class Abstract
    { /// 标题9个参数 顺序分别是 0该部分是否必要 1标题之间空格数目 2对齐方式 3行间距 4段前间距 5段后间距 6字体 7字号 8是否加粗
      /// 正文7个参数 顺序分别是 0缩进  1行间距 2段前间距 3段后间距 4字体 5字号
      /// 关键词的参数 顺序是 0字体  1 字号 3 最小 4最大

        protected string[] abstitle = { "ture", "4", "center", "0", "0", "0", "0", "0" ,"0"};
        protected string[] abstext = { "2", "0", "0", "0", "黑体", "0", "0" };
        protected string[] abskeyword = { "黑体", "0", "2", "5", "0" ,""};
        protected string[] abstitleE = { "ture", "8", "center", "0", "0", "0", "0", "0" ,"false", "justify" };
        protected string[] abstextE = { "2", "0", "0", "0", "Times New Roman", "0", "0" , "Cambria","Calibri" };
        protected string[] abskeywordE = { "Times New Roman", "0", "2", "5", "0" };
        protected int change = 0;
        public Abstract()
        {

        }
        public void detectabstitle(List<Paragraph> list, WordprocessingDocument doc)
        {
            Body body = doc.MainDocumentPart.Document.Body;
            IEnumerable<Paragraph> paras = body.Elements<Paragraph>();
            int exist = 0;
            if (list.Count <= 0)
            {
                if (bool.Parse(abstitle[0]))
                    Util.printError("论文缺少摘要部分");
            }
            else
            {/// 标题9个参数 顺序分别是 0该部分是否必要 1标题之间空格数目 2对齐方式 3行间距 4段前间距 5段后间距 6字体 7字号 8是否加粗
                string temp = Tool.getFullText(list[0]).Trim(); //list[0].InnerText.Trim()
                if (temp.Length - 2 != int.Parse(abstitle[1])) Util.printError("“摘要”标题两字之间需要4个空格");
                if (!Util.correctJustification(list[0], doc, abstitle[2])) Util.printError("“摘要”标题对齐方式错误，应为居中");
                if (!Util.correctSpacingBetweenLines_line(list[0], doc, abstitle[3])) Util.printError("“摘要”标题行间距错误，应为1.5倍行距");
                if (!Util.correctSpacingBetweenLines_Be(list[0], doc, abstitle[4])) Util.printError("“摘要”标题段前间距错误，应为0行");
                if (!Util.correctSpacingBetweenLines_Af(list[0], doc, abstitle[5])) Util.printError("“摘要”标题段后间距错误，应为1行");
                if (!Util.correctfonts(list[0], doc, abstitle[6], "Times New Roman")) Util.printError("“摘要”标题字体错误，应为黑体");
                if (!Util.correctsize(list[0], doc, abstitle[7])) Util.printError("“摘要”标题字号错误，应为小三");
                if (!Util.correctBold(list[0], doc, bool.Parse(abstitle[8])))
                {
                    if (!bool.Parse(abstitle[8]))
                        Util.printError("“摘要”标题不需加粗");

                }
            }
            for (int i = 1; i < list.Count; i++)
            { /// 正文7个参数 顺序分别是 0缩进  1行间距 2段前间距 3段后间距 4字体 5字号
                string temp = Tool.getFullText(list[i]).Trim();
                temp = (temp.Length > 5 ? temp.Substring(0, 5) : temp);
                if (temp.Length > 4 && (temp.Replace(" ", "").Substring(0, 4) == "关键词：" || temp.Replace(" ", "").Substring(0, 4) == "关键词:")) break;
                if (temp.Length == 0) continue;
                if (!Util.correctIndentation(list[i], doc, abstext[0])) Util.printError("摘要正文段落缩进错误，该段落首行缩进2字符" + "   " + temp);
                if (!Util.correctSpacingBetweenLines_line(list[i], doc, abstext[1])) Util.printError("摘要正文行距错误，该段落应为多倍行距1.25" + "   " + temp);
                if (!Util.correctSpacingBetweenLines_Be(list[i], doc, abstext[2])) Util.printError("摘要正文段前间距错误，该段落应为段前0行" + "   " + temp);
                if (!Util.correctSpacingBetweenLines_Af(list[i], doc, abstext[3])) Util.printError("摘要正文段后间距错误，该段落应为段后0行" + "   " + temp);
                if (!Util.correctfonts(list[i], doc, abstext[4], "Times New Roman")) Util.printError("摘要正文字体错误，该段落应为宋体" + "   " + temp);
                if (!Util.correctsize(list[i], doc, abstext[5])) Util.printError("摘要正文字号错误，该段落应为小四" + "   " + temp);
            }
            for (int i = 1; i < list.Count; i++)
            {/// 关键词的参数 顺序是 0字体  1 字号 3 最小 4最大

                string temp1 = Tool.getFullText(list[i - 1]);
                string temp = Tool.getFullText(list[i]);
                string fulltext = Tool.getFullText(list[i]);
                if (temp1.Replace(" ", "").Length != 0 && fulltext.Length > 4 && (fulltext.Replace(" ", "").Substring(0, 4) == "关键词：" || fulltext.Replace(" ", "").Substring(0, 4) == "关键词:")) { exist = 1; Util.printError("摘要正文应与关键词空一行"); }
                if (temp.Replace(" ", "").Length > 4 && (temp.Replace(" ", "").Substring(0, 4) == "关键词：" || temp.Replace(" ", "").Substring(0, 4) == "关键词:") && (temp.Substring(0, 4).Trim() != "关键词：" && temp.Substring(0, 4).Trim() != "关键词:")) { exist = 1; Util.printError("“关键词：”这四个字中间不能有空格"); }
                if (fulltext.Replace(" ", "").Length > 4 && (fulltext.Trim().Substring(0, 4) == "关键词：" || fulltext.Trim().Substring(0, 4) == "关键词:") && (fulltext.Substring(0, 4) != "关键词：" && fulltext.Substring(0, 4) != "关键词:")) { exist = 1; Util.printError("“关键词：”这四个字前不能有空格"); }
                if (fulltext.Replace(" ", "").Length > 4 && fulltext.Substring(0, 4).Trim() == "关键词:") { exist = 1; Util.printError("“关键词：”这四个字中的冒号应为中文的："); }
                if (temp.Replace(" ", "").Length > 4 && (temp.Substring(0, 4) == "关键词：" || temp.Substring(0, 4) == "关键词:"))
                {
                    exist = 1;
                    IEnumerable<Run> runlist = list[i].Elements<Run>();
                    foreach (Run run in runlist)
                    {
                        string ky = run.InnerText;
                        if (ky.Replace(" ", "") == "关键词：" || ky.Replace(" ", "") == "关" || ky.Replace(" ", "") == "键" || ky.Replace(" ", "") == "词" || ky.Replace(" ", "") == "关键" || ky.Replace(" ", "") == "键词" || ky.Replace(" ", "") == "词：" || ky.Replace(" ", "") == "关键词" || ky.Replace(" ", "") == "键词：")
                        {
                         if (!Util.correctRunFonts(run, list[i], doc, "黑体", "")) Util.printError("“关键词：”这四个字字体错误，应为黑体");              
                        }
                        else { if (ky.Replace(" ", "") != "；" && ky.Replace(" ", "") != "：" && !Util.correctRunFonts(run, list[i], doc, "仿宋_GB2312", "")) { Util.printError("中文关键词的内容字体错误，应为仿宋_GB2312"); } }
                    }
                    if (!Util.correctsize(list[i], doc, abskeyword[1])) Util.printError("中文摘要关键词字号错误，应为小四");
                    string[] sArray = Regex.Split(temp.Replace(";", "；"), "；", RegexOptions.IgnoreCase);
                    int keyWordCount = sArray.Length;
                    if (sArray[sArray.Length - 1] == "")
                    {
                        Util.printError("中文摘要关键词末尾不应加标点");
                        keyWordCount--;
                    }
                    if (fulltext.IndexOf("；") <1 && fulltext.IndexOf(";") <1) Util.printError("中文摘要关键词之间应该以中文分号间隔");
                    else
                    {
                        if (fulltext.IndexOf(";") > -1)
                            Util.printError("中文摘要关键词的分号应为中文的；");
                        else
                        {
                            if (keyWordCount > int.Parse(abskeyword[3]))
                                Util.printError("中文摘要关键词不能超过" + abskeyword[3] + "个");
                            if (keyWordCount < int.Parse(abskeyword[2]))
                                Util.printError("中文摘要关键词不能少于" + abskeyword[2] + "个");
                        }

                    }

                    if (!Util.correctBold(list[i], doc, bool.Parse(abstitle[8])))
                    {
                        if (!bool.Parse(abstitle[8]))
                            Util.printError("中文关键词不需加粗");

                    }
                }
                if (i == list.Count - 1 && exist != 1) Util.printError("未检测到关键词部分，请确定是否有“关键词：”");
            }
        }
        public virtual void detectabstitleE1(List<Paragraph> list, WordprocessingDocument doc)
        {

        }
        public void detectabstitleE(List<Paragraph> list, WordprocessingDocument doc)
        {
            int exist = 0;
            if (list.Count <= 0)
            {/// 标题9个参数 顺序分别是 0该部分是否必要 1标题之间空格数目 2对齐方式 3行间距 4段前间距 5段后间距 6字体 7字号 8是否加粗
                if (Boolean.Parse(abstitleE[0])) Util.printError("论文缺少“Abstract”部分");
            }
            else
            {
                if (list[0].InnerText.Trim().Length != int.Parse(abstitleE[1])) Util.printError("“Abstract”标题之间不需要空格");
                if (!Util.correctJustification(list[0], doc, abstitleE[2])) Util.printError("“Abstract”标题对齐方式错误，应为居中");
                if (!Util.correctSpacingBetweenLines_line(list[0], doc, abstitleE[3])) Util.printError("“Abstract”标题行间距错误，应为1.5倍行距");
                if (!Util.correctSpacingBetweenLines_Be(list[0], doc, abstitleE[4])) Util.printError("“Abstract”标题段前间距错误，应为0行");
                if (!(Util.correctSpacingBetweenLines_Af(list[0], doc, abstitleE[5])|| !Util.correctSpacingBetweenLines_Af(list[0], doc, abstitle[5]))) Util.printError("“Abstract”标题段后间距错误，应为11磅");
              if (change == 1) { if (!Util.correctfonts(list[0], doc, "黑体", abstextE[7])) Util.printError("“Abstract”标题字体错误，应为Cambria"); }
                //   if (!Util.correctfonts(list[0], doc, abstitleE[6], "Times New Roman")) Util.printError("“Abstract”标题字体错误，应为Times New Roman");
                if (!Util.correctsize(list[0], doc, abstitleE[7])) Util.printError("“Abstract”标题字号错误，应为小三");
                if (!Util.correctBold(list[0], doc, bool.Parse(abstitleE[8]))) Util.printError("“Abstract”标题不需要加粗");


            }
            int j = 0;int num = 0;
            for (int i = 1; i < list.Count; i++)
            {/// 正文7个参数 顺序分别是 0缩进  1行间距 2段前间距 3段后间距 4字体 5字号
                string temp = Tool.getFullText(list[i]);
                string temp1 = Tool.getFullText(list[i]).Replace(" ", "");
                string fulltext = Tool.getFullText(list[i]);
                temp = (temp.Length > 10 ? temp.Substring(0, 10) : temp);
                if (temp.Length == 0) continue;
                if ((fulltext.Length > 10 && fulltext.Replace(" ", "").Substring(0, 9) == "KeyWords：") || (fulltext.Length > 10 && fulltext.Replace(" ", "").Substring(0, 9) == "keywords:") || (fulltext.Length > 10 && fulltext.Replace(" ", "").Substring(0, 9) == "keywords：") || (fulltext.Length > 10 && fulltext.Replace(" ", "").Substring(0, 9) == "Keywords:") || (fulltext.Length > 10 && fulltext.Replace(" ", "").Substring(0, 9) == "Keywords：") || (fulltext.Length > 10 && fulltext.Replace(" ", "").Substring(0, 9) == "keyWords:") || (fulltext.Length > 10 && fulltext.Replace(" ", "").Substring(0, 9) == "keyWords：")) break;          
                if (!Util.correctIndentation(list[i], doc, abstext[0])) Util.printError("abstract正文段落缩进错误，应为每段落首行缩进2字符" + "  "+temp);
                if (!Util.correctSpacingBetweenLines_line(list[i], doc, abstextE[1])) Util.printError("abstract正文行距错误，应为多倍行距1.25" + "  " + temp);
                if (!Util.correctSpacingBetweenLines_Be(list[i], doc, abstextE[2])) Util.printError("abstract正文段前间距错误，应为段前0行" + "  " + temp);
                if (!Util.correctSpacingBetweenLines_Af(list[i], doc, abstextE[3])) Util.printError("abstract正文段后间距错误，应为段后0行" + "  " + temp);
                if (!Util.correctfonts(list[i], doc,"黑体", abstextE[4])) Util.printError("abstract正文字体错误，应为Times New Roman" + "  " + temp);
                if (!Util.correctsize(list[i], doc, abstextE[5])) Util.printError("abstract正文字号错误，应为小四" + "  " + temp);
                if (!Util.correctJustification(list[i], doc, abstitleE[9])) Util.printError("abstract正文对齐方式错误，应为两端对齐");
                for (int u = 2; u < fulltext.Length; u++)
                {
                   char yi1 = fulltext[u-2]; char yi0 = fulltext[u-1]; char yi2 = fulltext[u];
                    if ((yi2 >= 'a' && yi2 <= 'z') && !(yi1 >= 'A' && yi1 <= 'Z')&& (yi0 >= 'A' && yi0 <= 'Z')){ j++; }
                    if (yi2 == '.') {num++;}
                   
                }
                for (int u = 2; u < temp1.Length; u++)
                {
                    char yi0 = temp1[u - 1]; char yi2 = temp1[u];
                    if (yi0 == '.' && (yi2 >= 'a' && yi2 <= 'z')) Util.printError("abstract正文的句首单词首字母应该大写" + "  " + yi0+yi2);

                }

                if (!Util.correctBold(list[i], doc, bool.Parse(abstitle[8])))
                {
                    if (!bool.Parse(abstitle[8]))
                        Util.printError("abstract正文不需加粗");
            
                }
            }
            //if ((j-num)>0) { Util.printError("警告：请注意英文摘要正文中的大写是否使用正确，除句首字母和专有名词外不能出现大写" + "  "); }
            for (int n = 1; n < list.Count; n++)
            {
                string fulltext1 = Tool.getFullText(list[n]).Trim();
                string fulltext = Tool.getFullText(list[n]);
                if (fulltext.Length > 10 && fulltext1.Replace(" ", "").Length == 0 && (fulltext.Replace(" ", "").Substring(0, 9) == "KeyWords：" || fulltext.Replace(" ", "").Substring(0, 9) == "KeyWords:")) { exist = 1; Util.printError("abstract正文应与keywords:空一行"); }
                if ((fulltext.Length>10&&fulltext.Replace(" ", "").Substring(0,9) == "KeyWords：")||(fulltext.Length > 10 && fulltext.Replace(" ", "").Substring(0, 9) =="keywords:") || (fulltext.Length > 10 && fulltext.Replace(" ", "").Substring(0, 9) == "keywords：") || (fulltext.Length > 10 && fulltext.Replace(" ", "").Substring(0, 9) == "Keywords:") || (fulltext.Length > 10 && fulltext.Replace(" ", "").Substring(0, 9) == "Keywords：") || (fulltext.Length > 10 && fulltext.Replace(" ", "").Substring(0, 9) == "keyWords:") || (fulltext.Length > 10 && fulltext.Replace(" ", "").Substring(0, 9) == "keyWords："))
{
                    exist = 1;
                    int rule = 0;
                    int iu = 1;int ko = 0;
                   
                    if(!(fulltext.Length > 10 && fulltext.Replace(" ", "").Substring(0, 9) == "KeyWords：")) { exist = 1; Util.printError("“Key Words：”格式错误"); }
                    if (fulltext.Length > 10 && fulltext.Replace(" ", "").Substring(0, 8) != "KeyWords") { exist = 1; Util.printError("请注意“Key Words：”中的大小写"); }
                    if (fulltext.Length > 10&&(fulltext.Substring(0, 9)!= "KeyWords："&& fulltext.Substring(0, 9) != "KeyWords:") && (fulltext.Replace(" ", "").Substring(0, 9) == "KeyWords：" || fulltext.Replace(" ", "").Substring(0, 9) == "KeyWords:") && (fulltext.Trim().Substring(0, 10)!= "Key Words：" &&fulltext.Trim().Substring(0, 10) != "Key Words:")) { exist = 1; Util.printError("“Key Words：”中间不能有多余的空格"); }
                    if (fulltext.Length > 10 && (fulltext.Trim().Substring(0, 9) == "KeyWords：" || fulltext.Trim().Substring(0, 9) == "KeyWords:")) { exist = 1; Util.printError("“Key Words：”中间缺少必要的空格"); }
                    if (fulltext.Length > 10 && fulltext.Substring(0, 10).Trim() == "Key Words：" && fulltext.Substring(0, 10) != "Key Words：") { exist = 1; Util.printError("“Key Words：”前不能有空格"); }
                    if (fulltext.Length > 10 && fulltext.Substring(0, 10).Trim() == "Key Words:") { exist = 1; Util.printError("“Key Words：”前应为中文的："); }
                     string[] sArray = Regex.Split(fulltext.Replace(";", "；"), "；", RegexOptions.IgnoreCase);
                        int keyWordCount = sArray.Length;
                    int i1 = fulltext.Replace(" ", "").Trim().Length;
                    int i2 = fulltext.Length;
                    int i3 = i2 - i1;
                    if (fulltext.Replace(" ", "").Substring(0, 9) == "KeyWords：")
                    {
                        IEnumerable<Run> runlist = list[n].Elements<Run>();

                        foreach (Run run in runlist)
                        {
                            string ky = run.InnerText;
                            Console.Write(ky);
                            if (ky.Replace(" ", "") == "KeyWords" || ky.Replace(" ", "") == "KeyWords：" || ky.Replace(" ", "") == "Key" || ky.Replace(" ", "") == "Words：" || ky.Replace(" ", "") == "Words"/* || ky.Replace(" ", "") == "：" */|| ky.Replace(" ", "") == "K" || ky.Replace(" ", "") == "ey" || ky.Replace(" ", "") == "W" || ky.Replace(" ", "") == "ords：" || ky.Replace(" ", "") == "ords")
                            {

                                if (ko == 0) { if (!Util.correctRunFonts(run, list[n], doc, "黑体", abstextE[4])) { ko = 1; Util.printError("“Key Words：”字体错误，应为Times New Roman"); } }
                            /*    if (Util.correctBold(list[n], doc, bool.Parse(abstitle[8])))
                                {
                                    if (!bool.Parse(abstitle[8]))
                                        Util.printError("Key Words：需加粗");

                                }*/
                            }
                            else { if (ky.Replace(" ", "") != "；" && ky.Replace(" ", "") != "：" && !Util.correctRunFonts(run, list[n], doc, "黑体", abstextE[8])) { Util.printError("英文摘要关键词的内容字体错误，应为Calibri"); }
                                if (ky.Trim() != ky) { rule++; };                                
                            }
                        }
                    }
                   
                    if (i3<(keyWordCount)) {  Util.printError("每个英文关键词前应该有且只有1个空格"); };
                    if (!Util.correctsize(list[n], doc, abskeywordE[1])) Util.printError("英文摘要关键词字号错误，应为小四");
              //      string[] sArray = Regex.Split(fulltext.Replace(";", "；"), "；", RegexOptions.IgnoreCase);
              //      int keyWordCount = sArray.Length;
                   if(change==1){
                        if (fulltext.IndexOf("；") < 1 && fulltext.IndexOf(";") < 1) Util.printError("英文摘要关键词之间应该以英文分号间隔");
                        else
                        {
                            if (fulltext.IndexOf("；") > -1)
                                Util.printError("英文摘要关键词的分号应为英文的;");
                            else
                            {
                                if (keyWordCount > int.Parse(abskeywordE[3]))
                                    Util.printError("英文摘要关键词的数目不能超过" + abskeywordE[3] + "个");
                                if (keyWordCount < int.Parse(abskeywordE[2]))
                                    Util.printError("英文摘要关键词的数目不能少于" + abskeywordE[2] + "个");
                            }

                        }
                    }; 
                    if (sArray[sArray.Length - 1] == "")
                    {
                        Util.printError("英文摘要关键词末尾不应加标点");
                        keyWordCount--;
                    }
                           
                }
                if (n == list.Count - 1&& exist!=1)
                {
                 Util.printError("未检测到Key Words：部分，请确定是否有Key Words：部分");
                }
            }
        }
    }
}