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
using WD_toolbox;

namespace WDLang.Words.Import.GCIDE
{
    class GCIDEWord
    {
        public string Text { get; set; }
        public string Definition { get; set; }
        public int Freq { get; set; }
        public string Type { get; set; }
        public string Source { get; set; }
        
        public string BasicType
        {
            get
            {
                if (isNounAndVerb(Type))
                {
                    //Console.WriteLine("---- " + Text);
                    return "n_and_v";
                }
                if (Type.Contains("misc"))
                {
                    return "misc";
                }
                else if (Type.Contains("prep"))
                {
                    return "prep";
                }
                else
                {
                    return Type.Split(" ".ToCharArray())[0];
                }
            }
        }

        static bool isNounAndVerb(string type)
        {
            if (type == "n v") {
                return true;
            }
            string clean = type.Replace("adj", "").Replace("adv", "").Replace("misc", "").Replace("prep", "").Replace("pn", "");
            clean = clean.Replace(" ", "");
            return (clean == "nv") || (clean == "vn");
        }

        static string[] validTokens = new string[] { "n", "adj", "v", "adv", "prep", "pn", "misc" };

        private static readonly string allLetters = "-ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz";

        public GCIDEWord(string word, string def, string type)
        {
            Type = parseType(type);
            this.Text = cleanWordText(word).ToLower();
            //this.Text = (Type == "pn") ? capitiliseName(this.Text) : this.Text.ToLower();
            this.Definition = cleanDefinition(def);
            
        }

        private string capitiliseName(string p)
        {
            if(p.Length == 1)
            {
                return p.ToUpper();
            }
            string res= p.Substring(0, 1).ToUpper() +  p.Substring(1).ToLower();
            return res;
        }

        

        private static string simplifyType(string type)
        {
            string clean = type.ToLower().Replace("vb. n.", "v.").Replace("v. i.", "v.").Replace("v. t.", "v.")
                           .Replace("vb.n.", "v.").Replace("v.i.", "v.").Replace("v.t.", "v.");

            //one participle = verb
            clean = clean.Replace("p.p.", "v.").Replace("p. p.", "v.").Replace("p.pr.", "v.")
                           .Replace("p.pr.", "v.");//.Replace("p.", "v.");

            //participial adjective = adjective
            clean = clean.Replace("prop adj", "adj");
            clean = clean.Replace("prop. adj", "adj");
            clean = clean.Replace("prop a", "adj");
            clean = clean.Replace("prop. a.", "adj");
            clean = clean.Replace("prop. a", "adj");
            clean = clean.Replace("p. a.", "adj.").Replace("p.a.", "adj.");
            clean = clean.Replace("a.", "adj.");
            clean = clean.Replace("pred adj", "adj");
            clean = clean.Replace("pred. adj.", "adj");

            //dont care about plurals
            clean = clean.Replace("pl.", "");

            clean = clean.Replace("prop n", "pn");
            clean = clean.Replace("prop. n.", "pn");
            clean = clean.Replace("prop. n", "pn");
            clean = clean.Replace("prenom.", "");
            clean = clean.Replace("inf.", "");
            clean = clean.Replace("superl", "");
            clean = clean.Replace("3d sing.", "");
            clean = clean.Replace("sing", "");
            clean = clean.Replace("pres", "");
            clean = clean.Replace("3d", "");

            clean = clean.Replace("interj", "misc");
            clean = clean.Replace("conj", "misc");
            clean = clean.Replace("prefix", "misc");
            clean = clean.Replace("pref", "misc");
            clean = clean.Replace("pr.", "misc");  //pc conficls with prep

            clean = clean.Replace("compar", "misc");
            clean = clean.Replace("ads", "misc");


            clean = clean.Replace("imperative", "misc");
            //clean = clean.Replace("", "misc");
            //clean = clean.Replace("", "misc");
            //clean = clean.Replace("", "misc");

            clean = clean.Replace("prop.", "");  //NOTE: DONE AFTER "prop n" => "pn"
            clean = clean.Replace("fem.", "");
            clean = clean.Replace("indic", "");

            //because its bound to happen
            clean = clean.Replace("\r", " ");
            clean = clean.Replace("\n", " ");
            clean = clean.Replace("\t", " ");

            //clean = clean.Replace("", "");
            //clean = clean.Replace("", "");

            clean = stripEnding(clean, ". pl");
            

             
            

            clean = clean.Replace(".","").Trim().ToLower();
            if (clean == "pron"){
                return "n";
            }
            if (clean == "p")
            {
                return "misc";
            }

            return (clean == "a") ? "adj" : clean;
        }

        private static string stripEnding(string text, string ending)
        {
            if(text.EndsWith(ending))
            {
                return text.Substring(0, text.Length-ending.Length);
            }
            return text;
        }

        private static string parseType(string type)
        {
            //allverb types = verb
            string clean = simplifyType(type);

            string[] tokens = type.Split("&,".ToCharArray());
            List<string> res = new List<string>();
            foreach (string _token in tokens)
            {
                //prep fpr tokenise
                string token = simplifyType(_token.Trim());

                if ((token == "") || (token == " ") //ok
                    || (token == "i") || (token == "t") || token.StartsWith("but ")) //known issues
                {
                    continue;
                }

                if ((token == "p") || (token == "pret") || (token == "imp"))
                {
                    //yet another type of verb
                    if(!res.Contains("v")) {
                        res.Add("v");
                    }
                }
                else
                {
                    //sometimes an explination follows
                    if (token.Contains(' '))
                    {
                        token = simplifyType(token.Split(" ".ToCharArray())[0]);
                    }

                    if (!validTokens.Contains(token))
                    {
                        //Console.WriteLine("BAD TYPE TOKEN: " + token);
                    }
                    else
                    {
                        if (!res.Contains(token)) {
                            res.Add(token);
                        }
                    }
                }
            }

            //unparsible jibberish
            if (res.Count == 0)
            {
                return "misc";
            }

            //return the list
            string typeString = "";
            res.Sort();
            foreach (string item in res)
            {
                typeString += item + " ";
            }
            return typeString.Trim();
            
        }

        public static bool isNormalWord(GCIDEWord wrd)
        {
            return isNormalWord(wrd.Text);
        }

        public static bool isNormalWord(string text)
        {
            foreach (char c in text)
            {
                if ("&1234567890".Contains(c))
                {
                    return false;
                }
            }

            return true;
        }

        public bool isNormalWord() { return isNormalWord(Text); }

        public bool hasOddChars()
        {
            foreach (char c in Text)
            {
                if (!allLetters.Contains(c))
                {
                    //System.Windows.Forms.Clipboard.SetData(System.Windows.Forms.DataFormats.Text, ".Replace(\"" + c + "\", \"\")");
                    return true;
                }
            }
            return false;
        }

        private static string toEnglishLetters(string word)
        {
            return word.Replace("ù", "u")
                .Replace("Œ", "CE")
                .Replace("á", "a")
                .Replace("û", "u")
                .Replace("ü", "u")
                .Replace("â", "a")
                .Replace("î", "i")
                .Replace("É", "E")
                .Replace(",", "")
                .Replace("à", "a")
                .Replace("ó", "o")
                .Replace("é", "e")
                .Replace("æ", "ae")
                .Replace("ö", "o")
                .Replace("/", "")
                .Replace("Æ", "AE")
                .Replace("'ë", "e")
                .Replace("ë", "e")
                .Replace("ç'", "c")
                .Replace("œ", "oe")
                .Replace("è", "e")
                .Replace("ç", "c")
                .Replace("‖", "")
                .Replace("ê", "e")
                .Replace("ä", "a")
                .Replace("ã", "a")
                .Replace("ï", "i")
                .Replace("ñ", "n")
                .Replace("Ç", "c")
                .Replace("ô", "o");
        }

        public static string cleanWordText(string word)
        {
            string text = word.Trim().Replace("'", "").Replace("*", "").Replace("\"", "").Replace("`", "")
                                     .Replace(".", "").Replace(" ", "-")
                                     .Replace("&", "");
            return toEnglishLetters(text);
        }

        public static string cleanDefinition(string def)
        {
            string oneLine = def.Replace("[PIC]","").Replace("\r\n", " ").Replace("\r", " ").Replace("\n", " ").Replace("\t", " ").Replace("   ", " ").Replace("  ", " ").Replace("  ", " ").Trim();
            if (!oneLine.EndsWith("."))
            {
                oneLine = oneLine + ".";
            }
            return toEnglishLetters(oneLine);
        }


        public void appendDefinition(string def)
        {
            this.Definition = this.Definition + "; " + cleanDefinition(def);
            this.Definition = this.Definition.Replace(".;", "; ");
        }

        public static Dictionary<string, GCIDEWord> formulateSimepleDictionary(IList<GCIDEWord> allWords)
        {
            Dictionary<string, GCIDEWord> dic = new Dictionary<string, GCIDEWord>();
            foreach (GCIDEWord word in allWords)
            {
                if (dic.ContainsKey(word.Text))
                {
                    dic[word.Text].appendDefinition(word.Definition);
                }
                else
                {
                    dic.Add(word.Text, word);
                }

            }

            ResolveSeeOtherWordReferences(dic);

            return dic;
        }

        public static List<GCIDEWord> formulateDictionary(IList<GCIDEWord> allWords)
        {
            Dictionary<string, GCIDEWord> dic = new Dictionary<string, GCIDEWord>();
            foreach (GCIDEWord word in allWords)
            {
                if (dic.ContainsKey(word.UniqueWordID))
                {
                    dic[word.UniqueWordID].appendDefinition(word.Definition);
                }
                else
                {
                    dic.Add(word.UniqueWordID, word);
                }

            }

            //ResolveSeeOtherWordReferences(dic);

            return dic.Values.ToList();
        }

        private static void ResolveSeeOtherWordReferences(Dictionary<string, GCIDEWord> dic)
        {
            //resolve annoying see .... definitions
            foreach (var entry in dic)
            {
                string other = getCrossReferenceWord(entry.Value.Definition);
                if (other != null)
                {
                    if ((other != entry.Key) && dic.ContainsKey(other))
                    {
                        entry.Value.Definition = entry.Value.Definition + " - " + dic[other].Definition;
                    }
                }
            }
        }

        public static string getCrossReferenceWord(string def)
        {
            //pick out a cross reference defninition
            if (def.ToLower().StartsWith("see ") || def.ToLower().StartsWith("same as "))
            {
                //short definition
                if (def.Count(C => C == ' ') <= 4)
                {
                    List<string> words = def.Trim().Trim(".;',()".ToCharArray()).Split(" ".ToCharArray()).ToList();
                    return cleanWordText(words.Last());
                }
            }

            return null; //no cross reference (detected)
        }

        public override string ToString()
        {
            return string.Format("{0}: ({1}) {2}.", Text, Type, Definition);
        }

        public string UniqueWordID { get { return this.Text + ": " + this.Type; } }

        internal Word toWord(LanguageDictionary dic)
        {
            var prn = new Pronunciation("?");
            string text = this.Text;
            var def = new Definition(dic, Definition, "?");

            // "n", "adj", "v", "adv", "prep", "pn", "misc" };
            if (BasicType == "n")
            {
                return new Noun(text, prn, def);
            }
            else if (BasicType == "v")
            {
                return new Verb(text, prn, def);
            }
            else if (BasicType == "adj")
            {
                return new Adjective(text, prn, def);
            }
            else if (BasicType == "adv")
            {
                return new Adverb(text, prn, def);
            }
            else if (BasicType == "prep")
            {
                return new Preposition(text, prn, def);
            }
            else if (BasicType == "pn")
            {
                return new ProperNoun(text, prn, def);
            }
            else if (BasicType == "misc")
            {
                return new MiscWord(text, prn, def);
            }
            else
            {
                Word w = new Word(text, prn, def);
                return w;
            }
        }

        public class GCIDEDef
        {
            public string Definition {get; set;}
            public string Source{ get; set; }

        }

        public List<string> Definitions
        {
            get;
            set;
            /*get 
            {
                List<string> defs = Definition.Split(";".ToCharArray()).ToList();
                defs.ForEach(R => R = R.Trim());
                defs = defs.Where(R => R.Length > 0).ToList();
                return defs;
            }*/
        }
    }
}
