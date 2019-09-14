/*  ---------------------------------------------------------------------------------------------------------------------------------------
 *  (C) 2019, Dr Warren Creemers.
 *  This file is subject to the terms and conditions defined in the included file 'LICENSE.txt'
 *  ---------------------------------------------------------------------------------------------------------------------------------------
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.IO.Compression;
namespace WDLang.Words.TextMining.CorpusParsers
{
    public class GrAFCorpusParser : CorpusParser
    {
        public override IList<TextData> LoadData(System.IO.Stream stream, string path, TextDataCatagory catagoryHint)
        {
            return null;
        }
    }
}
