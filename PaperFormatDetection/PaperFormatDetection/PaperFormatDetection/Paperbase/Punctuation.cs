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
    class Punctuation
    {
        protected char[] CNtemplate = { '.', ',', '?', '!', ';', ':', '"', '(', ')', '\'' };
        public Punctuation()
        {

        }
        public void detectPunctuation(WordprocessingDocument doc)
        {
            Body body = doc.MainDocumentPart.Document.Body;
            MainDocumentPart mainPart = doc.MainDocumentPart;
            IEnumerable<Paragraph> paras = body.Elements<Paragraph>();
            List<Paragraph> list = new List<Paragraph>();
            List<Paragraph> Ref = Util.sectionLoction(doc, "参考文献", 1);
            List<Paragraph> Ach = Util.sectionLoction(doc, "攻读硕士学位期间发表学术论文情况", 1);
            foreach (Paragraph p in paras)
            {
                //参考文献不检测
                string str = Util.getFullText(p);
                if (Ref.Contains(p))
                    continue;
                //攻读硕士学位期间发表学术论文情况不检测
                if (Ach.Contains(p))
                    continue;
                if (str.Replace(" ", "").StartsWith("["))
                    continue;
                //英文关键词分割分号是中文的 在摘要检测时检测，这里跳过
                if (str.Length > 10 && str.Replace(" ", "").ToLower().Contains("keyword"))
                    continue;
                //目录不检测
                if (p.GetFirstChild<Hyperlink>() != null)
                    continue;
                detectPunctuation(str);
                detectUnits(str);
                detectNumber(str);
            }
        }
        public void detectPunctuation(string str)
        {
            string t=IsCode(str);
            if (t=="EN")
                ENdetect(str);
            else if(t=="CN")
                CNdetect(str);
        }
        public void CNdetect(string str)
        {
            if (str.Replace(" ", "").Length == 0) return;
            str = str.Trim();
            //char[] CNtemplate = {'.', ',', '?', '!', ';', ':', '"', '(', '\''};
            string temp1 = (str.Length < 8 ? str : str.Substring(0, 8)) + "...";

            Dictionary<int, int> match = new Dictionary<int, int>();
            //去掉中文中这样的代码post(req r1,rep r2)
            MatchCollection mc = Regex.Matches(str, @"[0-9a-zA-Z]+[(]([0-9a-zA-Z ]*([,][0-9a-zA-Z ]*)*)?[)]");
            if (mc.Count > 0)
            {
                foreach (Match m in mc)
                {
                    match.Add(m.Index,m.ToString().Length);
                }
            }
            //去掉[1,2]这样的
            mc = Regex.Matches(str, @"\[[0-9]+([,][0-9]+)+\]");
            if (mc.Count > 0)
            {
                foreach (Match m in mc)
                {
                    match.Add(m.Index, m.ToString().Length);
                }
            }
            //不能直接替换 不然旁边有错提示不准确
            //str = Regex.Replace(str, @"[a-zA-Z]+[(]([a-zA-Z ]*([,][a-zA-Z ]*)*)?[)]", "");
            char[] array = str.ToCharArray();
            int length = str.Length;
            for (int i = 0; i < length; i++)
            {
                if(match.ContainsKey(i))
                {
                    i = i + match[i];
                    continue;
                }
                if (Array.IndexOf(CNtemplate, array[i]) > -1)
                {
                    //去掉右边的引号
                    if (array[i] == '\'' || array[i] == '"')
                    {
                        for(int m=i+1;m<array.Length;m++)
                        {
                            if(array[m]==array[i])
                            {
                                array[m]=' ';
                                break;
                            }
                        }
                    }
                    if ((array[i] == ':') && leftIsLetter(str, i) && rightIsLetter(str, i))
                        continue;
                    if (array[i] == '.' && (leftIsLetter(str, i) || rightIsLetter(str, i)))
                        continue;
                    /*if ((array[i] == ',' || array[i] == '?' || array[i] == '!' ) && leftCharisNotCN(str, i))
                        continue;
                    if ((array[i] == '\'' || array[i] == '"' || array[i] == '=') && (rightIsLetter(str, i) || leftCharisNotCN(str, i)))
                        continue;*/
                    //(123)  (1.2)  (B)格式的是英文括号
                    string temp = i < 5 ? str : str.Substring(i - 4);
                    temp = "..." + (str.Length - i > 4 ? temp.Substring(0, temp.IndexOf(array[i]) + 5) : temp) + "...";

                    if (Regex.IsMatch(temp, @"[\u4e00-\u9fa5_a-zA-Z0-9]"))
                    {
                        Util.printError("此处不应包含英文标点符号 " + array[i].ToString() + "  " + temp + " 所在段落：" + temp1); 
                    }       
                }
            }
        }
        public bool leftIsLetter(string str,int index)
        {
            if (index <= 0)
                return false;
            else if (Regex.IsMatch(str.Substring(index - 1, 1), @"[0-9a-zA-Z]"))
                return true;
            else
                return false;
        }
        public bool rightIsLetter(string str, int index)
        {
            if (index >= str.Length-1)
                return false;
            else if (Regex.IsMatch(str.Substring(index + 1, 1), @"[0-9a-zA-Z]"))
                return true;
            else
                return false;
        }
        public bool leftCharisNotCN(string str, int index)
        {
            string temp = "";
            bool isLetter = false;
            while(index>0){
                index--;
                temp = str.Substring(index, 1);
                if (Regex.IsMatch(temp, @"[\u4e00-\u9fa5]"))
                {
                    break;
                }
                else if (Regex.IsMatch(temp, @"[0-9a-zA-Z]"))
                {
                    isLetter = true;
                    break;
                }
                else
                {

                }
            }
            return isLetter;
        }
        public void ENdetect(string str)
        {
            if (str.Replace(" ", "").Length == 0) return;
            str = str.Trim();
            char[] template = { '。', '，', '？', '！', '：', '；', '“', '”', '（', '）',/* '’',*/ '‘', '、' };
            //排除（1）AAA 格式
            MatchCollection mc = Regex.Matches(str, "[（][0-9]+[）]");
            foreach (Match m in mc)
            {
                str = str.Replace(m.ToString(), "");
                //Console.WriteLine(m.ToString()+"   kkkkkkk");
            }
            char[] array = str.ToCharArray();
            int length = str.Length;
            for (int i = 0; i < length; i++)
            {
                if (Array.IndexOf(template, array[i]) > -1)
                {
                    string temp1 = (str.Length < 12 ? str : str.Substring(0, 12)) + "...";
                    string temp = i < 10 ? str : str.Substring(i - 9);
                    temp = "..." + (str.Length - i > 9 ? temp.Substring(0, temp.IndexOf(array[i]) + 10) : temp) + "...";
                    if (Regex.IsMatch(temp, @"[a-zA-Z]"))
                    {
                        Util.printError("此处不应包含中文标点符号 " + array[i].ToString() + "  " + temp + "  所在段落：" + temp1);
                    }           
                }
            }
        }
        public string IsCode(string str)
        {
            string t = str;
            int CNcount = 0; 
            int ENcount = 0;
            string temp = noteHandle(str);
            bool hasNote = false;
            if (temp != str)
                hasNote = true;
            str = temp;

            temp = Regex.Replace(temp, "[\"].*?[\"]", "");
            temp = Regex.Replace(temp, "['].*?[']", "");
            temp = Regex.Replace(temp, "[“].*?[”]", "");
            temp = Regex.Replace(temp, "[‘].*?[’]", "");

            MatchCollection mc1 = Regex.Matches(temp, "[\u4E00-\u9FA5]");
            CNcount = mc1.Count;
            MatchCollection mc = Regex.Matches(temp, "[a-zA-Z]+");
            ENcount = mc.Count;
            string iscode = CNcount == 0 && ENcount > 0 ? "EN" : "CN";
            if(hasNote)
            {
                detectPunctuation(str);
                iscode = "Note"; ;
            }
            return iscode;
        }
        //注释处理
        public string noteHandle(string str)
        {
            //  /*xxxxxx*/的注释
            MatchCollection mc = Regex.Matches(str, "[/][*].*?[*][/]");
            if(mc.Count>0)
            {
                foreach (Match m in mc)
                {
                    str = str.Replace(m.ToString(), "");
                    if (Regex.IsMatch(m.ToString(), @"[\u4e00-\u9fa5_a-zA-Z]"))
                        detectPunctuation(m.ToString().Replace("/*", "").Replace("*/", ""));
                }
            }
            // //xxxxx的注释
            mc = Regex.Matches(str, "[/][/].*");
            if (mc.Count > 0)
            {
                foreach (Match m in mc)
                {
                    str = str.Replace(m.ToString(), "");
                    if (Regex.IsMatch(m.ToString(), @"[\u4e00-\u9fa5_a-zA-Z]"))
                        detectPunctuation(m.ToString().Replace("//",""));
                }
            }
            // #xxxxx的注释
            mc = Regex.Matches(str, "[#].*");
            if (mc.Count > 0)
            {
                foreach (Match m in mc)
                {
                    str = str.Replace(m.ToString(), "");
                    if (Regex.IsMatch(m.ToString(), @"[\u4e00-\u9fa5_a-zA-Z]"))
                        detectPunctuation(m.ToString().Replace("#", ""));
                }
            }
            // <!-- -->的注释
            mc = Regex.Matches(str, "<!--.*?-->");
            if (mc.Count > 0)
            {
                foreach (Match m in mc)
                {
                    str = str.Replace(m.ToString(), "");
                        if (Regex.IsMatch(m.ToString(), @"[\u4e00-\u9fa5_a-zA-Z]"))
                    detectPunctuation(m.ToString().Replace("-->", "").Replace("<!--", ""));
                }
            }
            // --的注释
            mc = Regex.Matches(str, "--.*");
            if (mc.Count > 0)
            {
                foreach (Match m in mc)
                {
                    str = str.Replace(m.ToString(), "");
                    if (Regex.IsMatch(m.ToString(), @"[\u4e00-\u9fa5_a-zA-Z]"))
                        detectPunctuation(m.ToString().Replace("--", ""));
                }
            }
            //像str="你，好"这样的
            /*mc = Regex.Matches(str, "[\"].+?[\"]");
            if (mc.Count > 0)
            {
                foreach (Match m in mc)
                {
                    str = str.Replace(m.ToString(), m.ToString().Substring(0, 1) + m.ToString().Substring(m.ToString().Length - 1, 1));
                    string temp = m.ToString().Replace("\"", "").Replace("\"", "");
                    if (Regex.IsMatch(temp, @"[\u4e00-\u9fa5_a-zA-Z]"))
                        detectPunctuation(temp);
                }
            }
            //像str=‘你，好’这样的
            mc = Regex.Matches(str, "[‘].+?[’]");
            if (mc.Count > 0)
            {
                foreach (Match m in mc)
                {
                    str = str.Replace(m.ToString(), m.ToString().Substring(0, 1) + m.ToString().Substring(m.ToString().Length - 1, 1));
                    string temp = m.ToString().Replace("‘", "").Replace("’", "");
                    if (Regex.IsMatch(temp, @"[\u4e00-\u9fa5_a-zA-Z]"))
                        detectPunctuation(temp);
                }
            }*/
            //像str=“你，好”这样的
            /*mc = Regex.Matches(str, "[“].+?[”]");
            if (mc.Count > 0)
            {
                foreach (Match m in mc)
                {
                    str = str.Replace(m.ToString(), m.ToString().Substring(0, 1) + m.ToString().Substring(m.ToString().Length - 1, 1));
                    string temp = m.ToString().Replace("“", "").Replace("”", "");
                    detectPunctuation(temp);
                }
            }*/
            return str;
        }
        //单位检测
        public void detectUnits(string str)
        {
            string errorPrompt = "     " + (str.Length > 10 ? str.Substring(0, 10) : str) + "...";
            string[] forbidenUnit = { "高斯", "亩", "公尺", "公寸", "公分", "尺", "寸", "码", "厘" ,"斤","两","钱","公顷","公升","毫"};
            //string[] forbidenU    = { "高斯", "亩", "公尺", "公寸", "公分", "尺", "寸", "码", "厘", "斤", "两", "钱", "公顷", "公升", "毫" };
            for(int i=0;i<forbidenUnit.Length;i++)
            {
                string pat = "[1-9][0-9]*" + forbidenUnit[i];
                MatchCollection mc = Regex.Matches(str, pat);
                foreach (Match m in mc)
                {
                    Util.printError("不应使用已废弃或者市制单位 " + m.ToString() + errorPrompt);
                }
            }
            string[,] unitName = {   { "弧度", "弧度", "rad","平面角" }, { "球面度", "球面度", "sr" ,"立体角"}, { "赫", "赫兹", "Hz","频率"}, { "帕", "帕斯卡", "Pa" ,"压强"}, 
                                     { "焦", "焦耳", "J","能量,功,热" },{ "瓦", "瓦特", "W","功率" }, { "库", "库仑", "C","电荷量" },{ "伏", "伏特", "V","电压" }, 
                                     { "法", "法拉", "F" ,"电容"},{ "欧", "欧姆", "Ω","电阻" }, { "摄氏度", "摄氏度", "℃","摄氏温度" }, { "流明", "流明", "lm" ,"光通量"},
                                     { "勒", "勒克斯", "lx","光照度" }, { "贝可", "贝可勒尔", "Bq" ,"放射性活度"},{ "戈", "戈瑞", "Gy","吸收剂量" },{ "希", "希沃特", "Sv","剂量当量" },
                                     { "厘米", "厘米", "cm","长度" },{ "千克", "公斤", "kg" ,"质量"}, { "安", "安培", "A" ,"电流"},{ "摩", "摩尔", "mol" ,"物质的量"},
                                     { "坎", "坎德拉", "cd","发光强度" },{ "转每分", "转每分", "r/min" ,"旋转速度"},{ "韦", "韦伯", "Wb","磁通量" }, { "升", "升", "L","体积" },
                                     { "分贝", "分贝", "dB","级差" }, { "特", "特克斯", "tex" ,"级密度"},{ "吨", "吨", "t" ,"质量"},{ "特", "特斯拉", "T","磁感应强度" }};
            for (int i = 0; i < unitName.Length / 4; i++)
            {
                //将论文中出现的物理量放在unit_list中，表示本文涉及到这些物理量
                /*if (str.IndexOf(unitName[i, 3]) >= 0 && !unit_list.Contains(unitName[i, 3]))
                    unit_list.Add(unitName[i, 3]);*/
                //如果这个物理量出现过 则进行检测 否则不检测
                if (str.Contains(unitName[i, 3]))
                {
                    string pattern = "[1-9][0-9]*" + unitName[i, 0];
                    string pattern1 = "[1-9][0-9]*" + unitName[i, 0];
                    //中文单位
                    bool CNunit = Regex.IsMatch(str, pattern);
                    bool CNunit1 = Regex.IsMatch(str, pattern);
                    if (CNunit || CNunit1)
                        Util.printError(unitName[i, 3] + "单位不用中文名称" + unitName[i, 0] + "或" + unitName[i, 1] + ",而用符号" + unitName[i, 2] + "表示" + errorPrompt);
                    //格式大小写错误
                    pattern = "[1-9][0-9]*" + unitName[i, 2];
                    MatchCollection mc = Regex.Matches(str, pattern, RegexOptions.IgnoreCase);
                    foreach (Match m in mc)
                    {
                        bool flag = false;
                        int index = m.Index;
                        string temp = Regex.Replace(m.ToString(), "[0-9]+", "");
                        string temp1="";
                        if(index+3 > str.Length-1)
                            temp1 = str.Substring(index + 1, 1)+"。";
                        else
                            temp1= str.Substring(index+1, 2);
                        for (int j = 0; j < unitName.Length / 4;j++ )
                        {
                            if(unitName[j, 2].ToLower().Contains(temp1.ToLower()))
                            {
                                flag = true;
                                break;
                            }
                        }
                        if (flag) break;
                        if (temp != unitName[i, 2])
                            Util.printError(unitName[i, 3] + "单位" + m.ToString() + " 格式书写错误，应为" + unitName[i, 2] + errorPrompt);
                    }
                    //中文数值
                    pattern = "[零一二三四五六七八九十百千万亿]+" + unitName[i, 2];
                    mc = Regex.Matches(str, pattern);
                    foreach (Match m in mc)
                    {
                        bool flag = false;
                        int index = m.Index;
                        int len = (m.ToString().Length + 1 <= str.Length - 1) ? (m.ToString().Length + 1) : (m.ToString().Length);
                        string temp = str.Substring(index, len);
                        temp = Regex.Replace(temp, "[零一二三四五六七八九十百千万亿]+", "");
                        for (int j = 0; j < unitName.Length / 4; j++)
                        {
                            if (unitName[j, 2].ToLower().Contains(temp.ToLower()))
                            {
                                flag = true;
                                break;
                            }
                        }
                        if (flag) break;
                        if (m.ToString() != unitName[i, 2])
                            Util.printError("物理量量值应使用阿拉伯数字表示"+m.ToString() + errorPrompt);
                    }
                }
            }
        }
        //数字检测
        public void detectNumber(string str)
        {
            string errorPrompt = "     " + (str.Length > 10 ? str.Substring(0, 10) : str) + "...";
            //引文标注中版次、卷次、页码应使用阿拉伯数字
            MatchCollection mc = Regex.Matches(str, "[零一二三四五六七八九十百千万亿]+[版卷页]");
            foreach (Match m in mc)
            {
                if (str.IndexOf("《") > -1)
                    Util.printError("引文标注中版次、卷次、页码应使用阿拉伯数字" + "       " + m.ToString() + errorPrompt);
            }
            //阿拉伯数字书写的纯小数必须写出小数点前定位的“0”  如0.45不能写成.45 问题Figure.6
            /*mc = Regex.Matches(str, "[^…0123456789.][.][0-9]+");
            foreach (Match m in mc)
            {
                Util.printError("阿拉伯数字书写的纯小数必须写出小数点前定位的 0" + "       " + m.ToString().Substring(1) + errorPrompt);
            }*/
            //小数点是齐底线的黑圆点“.”示例：0.46不得写成.46和0·46
            mc = Regex.Matches(str, "[0-9]+[·][0-9]+");
            foreach (Match m in mc)
            {
                Util.printError("小数的小数点应该是齐底线的黑圆点“.”" + "       " + m.ToString() + errorPrompt);
            }
            //数字分节
            /*mc = Regex.Matches(str, "[0123456789 ]*[0-9]([.][0-9])?[0123456789 ]*");
            foreach (Match m in mc)
            {
                string s = m.ToString().Trim();
                int index = s.IndexOf('.');
                if (index < 0)
                {
                    if(!Util.isRightSection(s))
                        Util.printError("数字书写分节错误，应为从小数点起，向左和向右每三位数字一组" + "       " + s);
                }
                else
                {
                    string s1 = s.Substring(0,index);
                    string s2 = s.Substring(index + 1, s.Length - index - 1);
                    char[] s2array = s2.ToCharArray();
                    Array.Reverse(s2array);
                    string s3 = new string(s2array);
                    if (!Util.isRightSection(s1) || !Util.isRightSection(s3))
                        Util.printError("数字书写分节错误，应为从小数点起，向左和向右每三位数字一组" + "       " + s);
                }
            }*/
        }
    }
}
