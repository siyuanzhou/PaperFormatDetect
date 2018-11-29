using System;
using System.IO;
using PaperFormatDetection.Tools;
using System.Text.RegularExpressions;

namespace PaperFormatDetection.Frame
{
    public class Program
    {
        public static int Main(string[] args)
        {
            string paperType = "本科";
            paperType = "硕士";
            //paperType = "博士";
            DateTime start = DateTime.Now;
            Util.paperPath = Util.environmentDir + "\\Papers\\王晨晨.docx";
            if (args.Length > 0)
                Util.paperPath = args[0];
            if (args.Length > 1)
                Util.masterType = (args[1] == "0") ? "学术型硕士" : "专业学位硕士";
            //获取页码
            Console.WriteLine("正在获取页码...");
            MSWord msword = new MSWord();
            if (Util.paperPath.EndsWith(".doc"))
            {
                Console.WriteLine("正在将doc文件转为docx...");
                Util.paperPath = msword.DocToDocx(Util.paperPath);
                Console.WriteLine("文件转换成功！");
            }
            Util.pageDic = msword.getPage(Util.paperPath);
            foreach (var item in Util.pageDic)
            {
                //Util.printError(item.Key + "  " + item.Value);
                Console.WriteLine(item.Key + "  " + item.Value);
            }
            Console.WriteLine("成功获取页码信息！");

            Undergraduate.PaperDetection UndergraduatePD = null;
            Master.PaperDetection MasterPD = null;
            Doctor.PaperDetection DoctorPD = null;
            if (paperType.Equals("本科"))
                UndergraduatePD = new Undergraduate.PaperDetection(Util.paperPath);
            else if (paperType.Equals("硕士"))
                MasterPD = new Master.PaperDetection(Util.paperPath);
            else if (paperType.Equals("博士"))
                DoctorPD = new Doctor.PaperDetection(Util.paperPath);

            DateTime end = DateTime.Now;
            TimeSpan ts = end - start;
            Console.WriteLine("");
            Console.WriteLine(" <= 检测用时： " + ts.TotalSeconds + " =>");
            //Console.ReadKey();
            return 0;
        }
    }
}