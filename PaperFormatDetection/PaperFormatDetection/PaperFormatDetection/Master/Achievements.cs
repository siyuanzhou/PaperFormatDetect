using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using PaperFormatDetection.Tools;
using System.Xml;

namespace PaperFormatDetection.Master
{
    class Achievements:Paperbase.Achievements
    {
        WordprocessingDocument doc;
        public Achievements(WordprocessingDocument doc1)
        {
            try
            {
                this.doc = doc1;
                Init();
                detectAchievement(Util.sectionLoction(doc, "攻读硕士学位期间发表学术论文情况", 1), doc);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
        public void Init()
        {
            this.section = "攻读硕士学位期间发表学术论文情况";
            this.beginList = "1";
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(Util.environmentDir + "/Template/Master/Achievements.xml");
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
           // int INDEX = 0;
            foreach(Paragraph p in list)
            {
                string temp = Util.getFullText(p).Trim(); 
                if (temp.Length == 0) continue;
                string tips = "     " + (temp.Length > 10 ? temp.Substring(0, 10) : temp) + "......";
                if (Util.IsInt(temp.Substring(0, 1)))
                {
                   // INDEX++;
                    if (!containBold(p,doc))
                        Util.printError("学位论文作者姓名应加粗" + tips);
                    //if (temp.IndexOf("（本硕士学位论文") < 0)
                       // Util.printError("与学位论文内容（章节）无关的论文不得列出或未注明与学位论文相关章节" + tips);;
                    //if (temp.Substring(0, 1) != INDEX.ToString())
                     //   Util.printError("学位论文序号错误,应为" + INDEX.ToString() + tips);

                }
            }
        }
    }
}
