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
using WD_toolbox.Persistance;
using WD_toolbox;

namespace WDLang.Words
{
    public enum WordTypes {Unspecified=0, Noun, Adjective, Verb, Adverb, Preposition, ProperNoun, Misc};

    /// <summary>
    /// Stores information about a word.
    /// </summary>
    public class Word : IComparable<Word>, IStreamReadWriteCoupled<LanguageDictionary>
    {        

        protected static string[] typeAbrviations = new string[] {"?", "n", "adj", "v", "adv", "prep", "pn", "misc" };
        //--------------------------------------------------------------------------------------------
        // Instance data
        //--------------------------------------------------------------------------------------------
        List<string> _spellings;
        List<Pronunciation> _pronunciations;
        List<Definition> _definitions;

        //--------------------------------------------------------------------------------------------
        // Accessors
        //--------------------------------------------------------------------------------------------

        public IReadOnlyCollection<string> Spellings { get { return _spellings.AsReadOnly(); } }
        public IReadOnlyCollection<Pronunciation> Pronunciations { get { return _pronunciations.AsReadOnly(); } }
        public IReadOnlyCollection<Definition> Definitions { get { return _definitions.AsReadOnly(); } }

        public string PrimarySpelling { get { return _spellings[0]; } }

        public virtual WordTypes WordType { get { return Words.WordTypes.Unspecified; } }
        public virtual string TypeAbreviation { get { return typeAbrviations[(int)WordType]; } }

        private static char[][] ShortDefinitionSplitOptions = { ";.".ToCharArray(), ".".ToCharArray() };

        public string ShortDefinition
        {
            get 
            {
                foreach(Definition def in Definitions.Where(D => D.TheDefinition != null)) 
                {
                    foreach(char[] splitOption in ShortDefinitionSplitOptions)
                    {
                        string txt = def.TheDefinition.Split(splitOption)[0].Trim();
                        if (txt.Length > 5)
                        {
                            return txt + ".";
                        }
                    }
                }
                return Definitions.First().TheDefinition;
            }
        }

        //--------------------------------------------------------------------------------------------
        // Constructors and factory methods
        //--------------------------------------------------------------------------------------------
        public Word(string word, Pronunciation pronunciation, List<Definition> definitions)
        {
            List<Pronunciation> pronunciations = new List<Pronunciation>();
            pronunciations.Add(pronunciation);
            init(word, pronunciations, definitions);
        }

        public Word(string word, Pronunciation pronunciation, Definition definition)
        {
            List<Pronunciation> pronunciations = new List<Pronunciation>();
            pronunciations.Add(pronunciation);
            List<Definition> definitions = new List<Definition>();
            definitions.Add(definition);
            init(word, pronunciations, definitions);
        }


        internal Word()
        {
            _spellings = new List<string>();
            _definitions = new List<Definition>();
            _pronunciations = new List<Pronunciation>();
        }

        protected void init(string word, List<Pronunciation> pronunciations, List<Definition> definition)
        {
            _spellings = new List<string>();
            _spellings.Add(word);
            _definitions = new List<Definition>(definition);
            _pronunciations = new List<Pronunciation>(pronunciations);
        }

        //--------------------------------------------------------------------------------------------
        // Public methods
        //--------------------------------------------------------------------------------------------

        public static string normaliseWordForComparison(string word)
        {
            return word.ToLower().Trim();
        }

        public bool IsSameWord(Word other)
        {
            return normaliseWordForComparison(other.PrimarySpelling) == normaliseWordForComparison(this.PrimarySpelling);
        }

        public int CompareTo(Word other)
        {
            return PrimarySpelling.CompareTo(other.PrimarySpelling);
        }

        public override string ToString()
        {
            var def = this.ShortDefinition??"(no definition found)";
            return $"{this.PrimarySpelling}: {this.WordType} - {def}";
        }

        //--------------------------------------------------------------------------------------------
        // Other methods
        //--------------------------------------------------------------------------------------------
        internal void addInfoFrom(Word word)
        {
            var newSpellings = from spl in word._spellings where !_spellings.Contains(spl) select spl;
            _spellings.AddRange(newSpellings);

            var newDefs = from d in word._definitions where !_definitions.Contains(d) select d;
            _definitions.AddRange(newDefs);

            var newProns = from p in word._pronunciations where !_pronunciations.Contains(p) select p;
            _pronunciations.AddRange(newProns);
        }
    
        public bool Write(System.IO.StreamWriter sw, LanguageDictionary coupledObject)
        {
 	        _spellings.WriteAsSingleLines(sw);
            _definitions.WriteList(sw, coupledObject);
            _pronunciations.WriteList(sw);
            return true;
        }

        public bool Read(System.IO.StreamReader sr, LanguageDictionary coupledObject)
        {
 	      //_spellings = new List<string>();
            _spellings.ReadFromSingleLines(sr);
            _definitions.ReadList(sr, coupledObject, delegate(string s, LanguageDictionary d)
                {return Definition.blankForReading(d);});
            _pronunciations.ReadList(sr, Pronunciation.newObject);
            return true;
        }

        internal static Word newObject(string type, LanguageDictionary coupledObject)
        {
            if(type == "Word") 
            {
                return new Word();
            }
            else if(type == "Noun")
            {
                return new Noun();
            }
            else if(type ==  "Adjective") 
            {
                return new Adjective();
            }
            else if(type == "Verb") 
            {
                return new Verb();
            }
            else if(type == "Adverb") 
            {
                return new Adverb();
            }
            else if(type == "Preposition") 
            {
                return new Preposition();
            }
            else if(type == "ProperNoun") 
            {
                return new ProperNoun();
            }
            else if(type ==  "MiscWord")
            {
                return new MiscWord();
            }
            else
            {
                throw new InvalidOperationException("No word type: " + type);
            }
            
        }

        public string FormatedCompleteDefinition 
        {
            get
            {
                StringBuilder sb = new StringBuilder();
                foreach (Definition def in Definitions)
                {
                    string txt = def.TheDefinition.Trim().EnsureEndsWith(".") + " ";
                    sb.Append(txt);
                }

                return sb.ToString().Trim();
            }
        }
    }

    public class Noun : Word
    {
        
        public Noun(string word, Pronunciation pronunciation, List<Definition> definitions)
            : base (word, pronunciation, definitions)
        {
        }

        public Noun(string word, Pronunciation pronunciation, Definition definition)
            : base(word, pronunciation, definition)
        {
        }

        internal Noun() : base()
        {
        }

        public override WordTypes WordType { get { return Words.WordTypes.Noun;} }
    }

    public class Adjective : Word
    {
        public Adjective(string word, Pronunciation pronunciation, List<Definition> definitions)
            : base (word, pronunciation, definitions)
        {
        }

        public Adjective(string word, Pronunciation pronunciation, Definition definition)
            : base(word, pronunciation, definition)
        {
        }

        internal Adjective() : base()
        {
        }

        public override WordTypes WordType { get { return Words.WordTypes.Adjective; } }
    }

    public class Verb : Word
    {
        public Verb(string word, Pronunciation pronunciation, List<Definition> definitions)
            : base(word, pronunciation, definitions)
        {
        }

        public Verb(string word, Pronunciation pronunciation, Definition definition)
            : base(word, pronunciation, definition)
        {
        }

        internal Verb() : base()
        {
        }

        public override WordTypes WordType { get { return Words.WordTypes.Verb; } }
    }

    public class Adverb : Word
    {
        public Adverb(string word, Pronunciation pronunciation, List<Definition> definitions)
            : base(word, pronunciation, definitions)
        {
        }

        public Adverb(string word, Pronunciation pronunciation, Definition definition)
            : base(word, pronunciation, definition)
        {
        }

        internal Adverb() : base()
        {
        }

        public override WordTypes WordType { get { return Words.WordTypes.Adverb; } }
    }

    public class Preposition : Word
    {
        public Preposition(string word, Pronunciation pronunciation, List<Definition> definitions)
            : base(word, pronunciation, definitions)
        {
        }

        public Preposition(string word, Pronunciation pronunciation, Definition definition)
            : base(word, pronunciation, definition)
        {
        }

        internal Preposition() : base()
        {
        }


        public override WordTypes WordType { get { return Words.WordTypes.Preposition; } }
    }

    public class ProperNoun : Word
    {
        public ProperNoun(string word, Pronunciation pronunciation, List<Definition> definitions)
            : base(word, pronunciation, definitions)
        {
        }

        public ProperNoun(string word, Pronunciation pronunciation, Definition definition)
            : base(word, pronunciation, definition)
        {
        }

        internal ProperNoun() : base()
        {
        }

        public override WordTypes WordType { get { return Words.WordTypes.ProperNoun; } }
    }

    public class MiscWord : Word
    {
        public MiscWord(string word, Pronunciation pronunciation, List<Definition> definitions)
            : base(word, pronunciation, definitions)
        {
        }

        public MiscWord(string word, Pronunciation pronunciation, Definition definition)
            : base(word, pronunciation, definition)
        {
        }

        internal MiscWord() : base()
        {
        }

        public override WordTypes WordType { get { return Words.WordTypes.Misc; } }
    }
}
