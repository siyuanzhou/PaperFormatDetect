using System;
using System.Collections.Generic;
using System.Xml;
using System.Linq;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using PaperFormatDetection.Tools;
using PaperFormatDetection.Frame;
using System.Text.RegularExpressions;

namespace PaperFormatDetection.Undergraduate
{
    class Punctuation : Paperbase.Punctuation
    {
        public Punctuation(WordprocessingDocument doc)
        {
            detectPunctuation(doc);
        }
    }

}
