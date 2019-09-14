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
    public class PlainTextParser : CorpusParser
    {
        public override IList<TextData> LoadData(Stream stream, string path, TextDataCatagory catagoryHint)
        {
            IList<TextData> data = new List<TextData>();
            using (StreamReader reader = new StreamReader(stream))
            {
                string text = reader.ReadToEnd();
                TextData td = TextData.fromBook(Path.GetFileNameWithoutExtension(path), text, catagoryHint);

                data.Add(td);
            }

            return data;
        }
    }
}
