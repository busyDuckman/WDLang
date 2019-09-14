/*  ---------------------------------------------------------------------------------------------------------------------------------------
 *  (C) 2019, Dr Warren Creemers.
 *  This file is subject to the terms and conditions defined in the included file 'LICENSE.txt'
 *  ---------------------------------------------------------------------------------------------------------------------------------------
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WD_toolbox.Data.DataStructures;
using WD_toolbox.Persistance;
using WD_toolbox;

namespace WDLang.Words
{
    public class Definition : IStreamReadWriteCoupled<LanguageDictionary>
    {
        public string TheDefinition { get; protected set; }
        public ListValue<string> source;
        public string Source {get {return source.Value;}}

        internal Definition(LanguageDictionary dic, string def, string source)
        {
            TheDefinition = def;
            this.source = new ListValue<string>(source, dic.Sources);
        }

        private Definition()
        {
        }


        public bool Write(System.IO.StreamWriter sw, LanguageDictionary coupledObject)
        {
            sw.WriteLine(TheDefinition.EncodeOneLineSerilisable());
            sw.WriteLine(source.Index);
            return true;
        }

        public bool Read(System.IO.StreamReader sr, LanguageDictionary coupledObject)
        {
            TheDefinition = sr.ReadLine().DecodeLineSerilisable();
            int index = int.Parse(sr.ReadLine());
            source = ListValue<string>.FromIndex(coupledObject.Sources, index);
            return true;
        }

        internal static Definition blankForReading(LanguageDictionary d)
        {
            //Definition def = new Definition(d);
            return new Definition();
        }
    }
}
