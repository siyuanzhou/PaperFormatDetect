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
    class Copyright : Paperbase.CopyrightAndOriginstmt
    {
        public Copyright(WordprocessingDocument doc)
        {
            Tools.Util.printError("学位论文版权使用授权书检测检测");
            Util.printError("----------------------------------------------");
            try
            {
                Init(Util.environmentDir + "/Template/Master/CopyrightAndOringinstmt.xml");
                detectCopyright(Util.sectionLoction(doc, "大连理工大学学位论文版权使用授权书", 1), doc);
                //detectOriginstmt(Util.sectionLoction(doc, "大连理工大学学位论文独创性声明", 1), doc);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            Util.printError("----------------------------------------------");
        }
    }
}


