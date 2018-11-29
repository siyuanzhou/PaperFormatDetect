using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DocumentFormat.OpenXml.Packaging;

namespace PaperFormatDetection.Doctor
{
    class PaperDetection
    {
        private string paperPath;
        WordprocessingDocument wd;
        public PaperDetection(string paperpath)
        {
            paperPath = paperpath;
            wd = WordprocessingDocument.Open(paperPath, true);
            detectProcess();
        }
        public void detectProcess()
        {
            //new CoverStyle(wd);
            //new Catalog(wd);
            new ChartCatalog(wd);
            //new ConclusionAndThanks(wd);
            //new Achievements(wd);
            //new CopyrightAndOriginstmt(wd);
            //new AuthorIntroduction(wd);
            //new Punctuation(wd);
            //new Figure(wd);
            //new TableStyle(wd);
            //new HeaderFooter(wd);
            new Abstract(wd);
            //new Appendix(wd);
        }
    }
}
