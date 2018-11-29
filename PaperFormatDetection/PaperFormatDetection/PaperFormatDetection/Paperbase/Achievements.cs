using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using PaperFormatDetection.Tools;
using System.Text.RegularExpressions;

namespace PaperFormatDetection.Paperbase
{
    class Achievements
    {
        /// <summary>
        /// 未避免变量过多，将同一部分变量放在一个数组里面 注意变量在数组中的顺序 
        /// 这里包含结论标题，结论正文， 致谢标题和致谢正文四个数组
        /// 标题9个参数 顺序分别是 0该部分是否必要 1标题之间空格数目 2对齐方式 3行间距 4段前间距 5段后间距 6字体 7字号 8是否加粗
        /// 正文7个参数 顺序分别是 0缩进 1对齐方式 2行间距 3段前间距 4段后间距 5字体 6字号
        /// 若以后需要新增属性一定注意文件中各属性的顺序 因为为了方便 后面初始化赋值就按照这个顺序赋值 这相当于一个约定
        /// </summary>
        protected string[] achievementTitle = new string[8];
        protected string[] achievementText = new string[7];
        protected string section = "";
        protected string beginList = "";

        public Achievements()
        {

        }
        //博士研究成果  硕士论文发表
        public void detectAchievement(List<Paragraph> list, WordprocessingDocument doc)
        {
            Util.printError(section + "检测");
            Util.printError("----------------------------------------------");
            if (list.Count > 0)
            {
                //标题
                if (!Util.correctJustification(list[0], doc, achievementTitle[1])) 
                    Util.printError(section + " 标题未" + achievementTitle[1]);
                if (!Util.correctSpacingBetweenLines_line(list[0], doc, achievementTitle[2]))
                    Util.printError(section + " 标题行间距错误，应为" + Util.DSmap[achievementTitle[2]]);
                if (!Util.correctSpacingBetweenLines_Be(list[0], doc, achievementTitle[3]))
                    Util.printError(section + " 标题段前距错误，应为段前0行");
                if (!Util.correctSpacingBetweenLines_Af(list[0], doc, achievementTitle[4]))
                    Util.printError(section + " 标题段后距错误，应为段后1行");
                if (!Util.correctfonts(list[0], doc, achievementTitle[5], "Times New Roman")) 
                    Util.printError(section+" 标题字体错误，应为"+achievementTitle[5]);
                if (!Util.correctsize(list[0], doc, achievementTitle[6])) 
                    Util.printError(section+" 标题字号错误，应为"+achievementTitle[6]);
                if (!Util.correctBold(list[0], doc, bool.Parse(achievementTitle[7])))
                {
                    if (bool.Parse(achievementTitle[7]))
                        Util.printError(section+" 标题未加粗");
                    else
                        Util.printError(section+" 标题不需加粗");
                }
                //正文
                bool isAchivement = false;
                List<Paragraph> achieList = new List<Paragraph>();
                int number = 1;//正文段落的序号
                string Rnumber = "^[0-9]";
                for (int i = 1; i < list.Count; i++)
                {
                    string temp = Tool.getFullText(list[i]).Trim();
                    string t = temp.Replace(" ", "");
                    string s = temp;
                    isAchivement = false;
                    if (temp.Length == 0) continue;
                    //if (temp.StartsWith(beginList)) isAchivement = true;
                    if (Regex.IsMatch(t, Rnumber)) isAchivement = true;
                    temp = "  ----" + (temp.Length > 10 ? temp.Substring(0, 10) : temp) + "......";
                    if (!Util.correctSpacingBetweenLines_line(list[i], doc, achievementText[1]))
                        Util.printError(section + " 行间距错误，应为" + Util.DSmap[achievementText[1]] + temp);
                    if (!Util.correctSpacingBetweenLines_Be(list[i], doc, achievementText[2]))
                        Util.printError(section + " 段前距错误，应为段前0行" + temp);
                    if (!Util.correctSpacingBetweenLines_Af(list[i], doc, achievementText[3]))
                        Util.printError(section + " 段后距错误，应为段后0行" + temp);
                    if (!Util.correctfonts(list[i], doc, achievementText[4], "Times New Roman")) 
                        Util.printError(section + " 字体错误，应为"+achievementText[4] + temp);
                    if (!Util.correctsize(list[i], doc, achievementText[5])) 
                        Util.printError(section + " 字号错误，应为"+achievementText[5] + temp);
                    if(!isAchivement)
                    {
                        if (!Util.correctIndentation(list[i], doc, achievementText[0]))
                            Util.printError(section + " 缩进错误，应为首行缩进" + achievementText[0] + "字符" + temp);
                    }
                    else
                    {
                        //序号判断
                        int tnumber = Convert.ToInt32(t[0]) - 48;
                        if (tnumber != number)
                        {
                            //Util.printError(t[0].ToString());
                            Util.printError("版权使用授权书段落序号错误，应为" + number + " " + temp);

                        }
                        //序号后的空格判断
                        if (s.Trim().Length > 3 && (s[1] != ' ' || s[2] != ' '))
                            Util.printError("版权使用授权书段落序号与内容之间应有两个空格" + " " + temp);
                        if (!Util.correctIndentation(list[i], doc, achievementText[6]))
                            Util.printError(section + " 缩进错误，应为首行缩进" + achievementText[6] + "字符" + temp);
                        achieList.Add(list[i]);
                        number++;
                    }
                }
                detectAlist(achieList);
            }
            Util.printError("----------------------------------------------");
        }
        public virtual void detectAlist(List<Paragraph> list)
        {

        }
        public bool containBold(Paragraph p, WordprocessingDocument doc)
        {
            bool containbold = false;
            string each = null;
            if (p != null)
            {
                IEnumerable<Run> rlist = p.Elements<Run>();
                foreach (Run run in rlist)
                {
                    each = Util.getFromRunPpr(run, 5); //从Run属性中查找
                    if (each == null)
                    {
                        if (run.RunProperties != null)
                        {
                            RunStyle rs = run.RunProperties.RunStyle;
                            if (rs != null)
                            {
                                each = Util.getFromStyle(doc, rs.Val, 5);//从Runstyle中查找
                            }
                        }
                    }
                    if (each == null)
                    {
                        each = Util.getFromPpr(p, 5);//从段落属性中找
                        if (each == null && p.ParagraphProperties!=null)
                        {
                            ParagraphStyleId style_id = p.ParagraphProperties.ParagraphStyleId;
                            if (style_id != null)//从paragraphstyle中获取
                            {
                                each = Util.getFromStyle(doc, style_id.Val, 5);
                            }
                            if (each == null)//style没找到
                            {
                                each = Util.getFromDefault(doc, 5);//从段落默认style中找
                                if (each == null)//default没找到
                                {
                                    each = "false";
                                }
                            }
                        }
                    }
                    if (each == "true" && run.InnerText.ToString().Trim()!="")
                    {
                        containbold = true;
                    }
                }
            }
            return containbold;
        }
    }
}