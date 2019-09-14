/*  ---------------------------------------------------------------------------------------------------------------------------------------
 *  (C) 2019, Dr Warren Creemers.
 *  This file is subject to the terms and conditions defined in the included file 'LICENSE.txt'
 *  ---------------------------------------------------------------------------------------------------------------------------------------
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WD_toolbox.Persistance;
using WD_toolbox;

namespace WDLang.Words
{
    public enum PronuinciationType {IPA, SylabilFragments, DictionaryGeneric};
    public class Pronunciation : IStreamReadWrite
    {
        public PronuinciationType Type {get; protected set;}
        public String PronunciationText {get; protected set; }
        public string Notes { get; protected set; }

        public Pronunciation(string pronunciationText)
        {
            PronunciationText = pronunciationText;
            Notes = null;
            Type = PronuinciationType.DictionaryGeneric;
        }

        public bool Write(System.IO.StreamWriter sw)
        {
            sw.WriteLineEnumAsString(Type);
            sw.WriteLine(PronunciationText.EncodeOneLineSerilisable());
            sw.WriteLine(Notes.EncodeOneLineSerilisable());
            return true;
        }

        public bool Read(System.IO.StreamReader sr)
        {
            Type = sr.ReadLineEnumAsString<PronuinciationType>();
            PronunciationText = sr.ReadLine().DecodeLineSerilisable();
            Notes = sr.ReadLine().DecodeLineSerilisable();
            return true;
        }

        internal static Pronunciation newObject(string type)
        {
            return new Pronunciation("?");
        }
    }
}
