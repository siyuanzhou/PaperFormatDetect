using System;
using System.Collections.Generic;
using System.Xml;
using System.Linq;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using PaperFormatDetection.Tools;
using PaperFormatDetection.Frame;
using System.Text.RegularExpressions;

namespace PaperFormatDetection.Master
{
    class Punctuation : Paperbase.Punctuation
    {
        public Punctuation(WordprocessingDocument doc)
        {
            Util.printError("标点符号，量和单位，数字检测");
            Util.printError("----------------------------------------------");
            try
            {
                detectPunctuation(doc);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            Util.printError("----------------------------------------------");
        }
    }
}
