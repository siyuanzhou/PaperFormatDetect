using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;

namespace PaperFormatDetection.Doctor
{
    class Punctuation : Paperbase.Punctuation
    {
        public Punctuation(WordprocessingDocument doc)
        {
            this.CNtemplate = new char[]{'.', ',', '?', '!', ';', ':', '"', '（', '\'' };
            detectPunctuation(doc);
        }
    }
}
