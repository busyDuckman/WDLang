/*  ---------------------------------------------------------------------------------------------------------------------------------------
 *  (C) 2019, Dr Warren Creemers.
 *  This file is subject to the terms and conditions defined in the included file 'LICENSE.txt'
 *  ---------------------------------------------------------------------------------------------------------------------------------------
 */
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WDLang.Words.TextMining.CorpusParsers
{
    public abstract class CorpusParser
    {
        public abstract IList<TextData> LoadData(Stream stream, string path, TextDataCatagory catagoryHint);
    }
}
