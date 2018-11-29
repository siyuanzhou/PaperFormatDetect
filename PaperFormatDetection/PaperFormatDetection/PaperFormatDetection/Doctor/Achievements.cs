using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using PaperFormatDetection.Tools;
using System.Xml;
using System.Text.RegularExpressions;

namespace PaperFormatDetection.Doctor
{
    class Achievements : Paperbase.Achievements
    {
        WordprocessingDocument doc;
        public Achievements(WordprocessingDocument doc1)
        {
            this.doc = doc1;
            Init();
            detectAchievement(Util.sectionLoction(doc, "攻读博士学位期间科研项目及科研成果", 2), doc);
        }
        public void Init()
        {
            this.section = "攻读博士学位期间科研项目及科研成果";
            this.beginList = "发表论文";
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(@"./Template/Doctor/Achievements.xml");
            int m = 0;
            //标题
            XmlNodeList conTitleNode = xmlDoc.SelectSingleNode("Root").SelectSingleNode("Achievements").SelectSingleNode("Title").ChildNodes;
            m = 0;
            foreach (XmlNode node in conTitleNode)
            {
                this.achievementTitle[m] = node.InnerText; m++;
            }
            //正文
            XmlNodeList conTextNode = xmlDoc.SelectSingleNode("Root").SelectSingleNode("Achievements").SelectSingleNode("Text").ChildNodes;
            m = 0;
            foreach (XmlNode node in conTextNode)
            {
                this.achievementText[m] = node.InnerText; m++;
            }
        }
        public override void detectAlist(List<Paragraph> list)
        {
            bool paperFlag=true;
            bool projectFlag = false;
            bool patentFlag = false;
            bool awardFlag = false;
            int INDEX = 1;
            foreach (Paragraph p in list)
            {
                string temp = Util.getFullText(p).Trim();
                string tips = "     " + (temp.Length > 10 ? temp.Substring(0, 10) : temp) + "......";
                switch(temp)
                {
                    case "发表论文": paperFlag = true; projectFlag = false; patentFlag = false; awardFlag = false; INDEX = 1; break;
                    case "参与科研项目": paperFlag = false; projectFlag = true; patentFlag = false; awardFlag = false; INDEX = 1; break;
                    case "发明专利": paperFlag = false; projectFlag = false; patentFlag = true; awardFlag = false; INDEX = 1; break;
                    case "获得奖励": paperFlag = false; projectFlag = false; patentFlag = false; awardFlag = true; INDEX = 1; break;
                    default: break;
                }
                MatchCollection mc = Regex.Matches(temp, "^[[1-9][0-9]*]");
                string str;
                if(mc.Count>0)
                {
                    str = mc[0].ToString();
                    if (!(temp.Substring(str.Length).Substring(0, 1) == " " && temp.Substring(str.Length).Substring(1, 1) != " "))
                        Util.printError("序号与内容间应空一格"+tips);
                    if (str.Replace("[", "").Replace("]", "") != INDEX.ToString())
                    {
                        Util.printError("序号错误,应为[" + INDEX.ToString()+"]" + tips);
                    }
                    if (paperFlag)
                    {
                        if (!containBold(p, doc))
                            Util.printError("论文作者姓名应加粗" + tips);
                        if (temp.IndexOf("（本学位论文") < 0)
                            Util.printError("与学位论文内容（章节）无关的论文不得列出或未注明与学位论文相关章节" + tips);
                    }
                    if (projectFlag)
                    {

                    }
                    if (patentFlag)
                    {
                        if (!containBold(p, doc))
                            Util.printError("发明人姓名应加粗" + tips);
                    }
                    if (awardFlag)
                    {

                    }
                    INDEX++;
                }
            }
        }
    }
}
