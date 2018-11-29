using System.Xml;
using System.Collections.Generic;
using System;
using PaperFormatDetection.Frame;
using DocumentFormat.OpenXml.Packaging;

namespace PaperFormatDetection.Undergraduate
{
    /**
     * 论文检测类，整合论文检测的流程 
     */
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
            new CoverStyle(wd);
            new Abstract(wd);
            new Catalog(wd);
            new ConclusionAndThanks(wd);
            new Formula(wd);
            new Reference(wd);
            new Figure(wd);
            new Tabledect(wd);
            new HeaderFooter(wd);
            new Appendix(wd);
            new Punctuation(wd);
        }
    }
}
