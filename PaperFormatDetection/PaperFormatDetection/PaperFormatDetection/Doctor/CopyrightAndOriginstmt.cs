using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using PaperFormatDetection.Tools;
using System.Xml;

namespace PaperFormatDetection.Doctor
{
    class CopyrightAndOriginstmt : Paperbase.CopyrightAndOriginstmt
    {
        public CopyrightAndOriginstmt(WordprocessingDocument doc)
        {
            Init(@"./Template/Doctor/CopyrightAndOringinstmt.xml");
            detectCopyright(Util.sectionLoction(doc, "大连理工大学学位论文版权使用授权书", 2), doc);
            detectOriginstmt(Util.sectionLoction(doc, "大连理工大学学位论文独创性声明", 2), doc);
        }
    }
}
