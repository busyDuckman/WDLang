/*  ---------------------------------------------------------------------------------------------------------------------------------------
 *  (C) 2019, Dr Warren Creemers.
 *  This file is subject to the terms and conditions defined in the included file 'LICENSE.txt'
 *  ---------------------------------------------------------------------------------------------------------------------------------------
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using WD_toolbox.Data.DataStructures;
using WD_toolbox;
using System.IO;
using WD_toolbox.Persistance;
using WD_toolbox.Files;
using System.Reflection;

namespace WDLang.Words
{
    public class LanguageDictionary : IReadOnlyList<Word>, IStreamReadWrite
    {
        //--------------------------------------------------------------------------------------------
        // Instance data
        //--------------------------------------------------------------------------------------------
        /// <summary>
        /// The Language in use.
        /// </summary>
        public string Language {get; protected set;}

        /// <summary>
        /// TODO: remove this, ValueList is obsolete.
        /// </summary>
        internal ValueList<string> Sources = new ValueList<string>(delegate(string s) { return s.Trim(); });

        protected List<Word> words = new List<Word>();

        // List of words in the dictionary
        public List<Word> Words { get { return words; } }

        //--------------------------------------------------------------------------------------------
        // Transient data
        //--------------------------------------------------------------------------------------------
        [NonSerialized]
        protected object _lock = new object();

        //--------------------------------------------------------------------------------------------
        // Constructors and factory methods
        //--------------------------------------------------------------------------------------------
        public static LanguageDictionary fromFile(string fileName)
        {
            LanguageDictionary dic = new LanguageDictionary();
            using(StreamReader sr = File.OpenText(fileName))
            {
                dic.Read(sr);
            }

            return dic;
        }

        //--------------------------------------------------------------------------------------------
        // Methods
        //--------------------------------------------------------------------------------------------
        public void MergeWordIntoSortedDictionary(Word word)
        {
            lock(_lock) 
            {
                int pos = words.BinarySearch(word);
                if(pos >= 0)
                {
                    //found, so add a definition
                    words[pos].addInfoFrom(word);
                }
                else
                {
                    //not found (bitwise compliment of the next largest)
                    pos = ~pos;
                    words.Insert(pos, word);
                }
            }
        }

        public Word this[string spelling]
        {
            get
            {
                int pos = BinarySearchWordsPrimarySpelling(spelling);
                if (pos >= 0)
                {
                    return this[pos];
                }
                else
                {
                    return words.FirstOrDefault(W => W.Spellings.Contains(spelling));
                }
            }
        }

        private int BinarySearchWordsPrimarySpelling(string word)
        {
            int mid, lowBound = 0, highBound = words.Count-1;
            while (lowBound <= highBound)
            {
                mid = (lowBound + highBound) / 2;
                int c = words[mid].PrimarySpelling.CompareTo(word);
                if (c > 0) {
                    lowBound = mid + 1;
                    continue;
                }
                else if (c < 0) {
                    highBound = mid - 1;
                    continue;
                }
                else
                {
                    return mid;
                }
            }
            return -1; //the word was not found
        }

        public Word this[int index]
        {
            get { return words[index]; }
        }

        public int Count
        {
            get { return words.Count; }
        }

        public IEnumerator<Word> GetEnumerator()
        {
            return words.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return ((System.Collections.IEnumerable)words).GetEnumerator();
        }

        //--------------------------------------------------------------------------------------------
        // IStreamReadWrite
        //--------------------------------------------------------------------------------------------
        public bool Write(StreamWriter sw)
        {
            sw.WriteLine(Language.EncodeOneLineSerilisable());
            Sources.WriteAsSingleLines(sw);
            words.WriteList(sw, this);
            return true;
        }

        public bool Read(StreamReader sr)
        {
            Language = sr.ReadLine().DecodeLineSerilisable();
            Sources.ReadFromSingleLines(sr);
            words.ReadList(sr, this,  Word.newObject);
            return true;
        }

        //--------------------------------------------------------------------------------------------
        // File load / save
        //--------------------------------------------------------------------------------------------
        public bool Save(string path)
        {
            using (StreamWriter sw = File.CreateText(path))
            {
                return Write(sw);
            }
        }

        public static LanguageDictionary Load(string path)
        {
            LanguageDictionary ld = new LanguageDictionary();
            using (StreamReader sr = File.OpenText(path))
            {
                if (ld.Read(sr))
                {
                    return ld;
                }
            }

            return null;
        }

        public static LanguageDictionary loadInternalEnglish()
        {
            string internal_dictionary = "WDLang.Dictionaries.english_gcide.dic";
            var assembly = Assembly.GetExecutingAssembly();
            using (var s = assembly.GetManifestResourceStream(internal_dictionary))
            {
                if (s != null)
                {
                    using (var sr = new StreamReader(s))
                    {
                        LanguageDictionary ld = new LanguageDictionary();
                        if (ld.Read(sr))
                        {
                            return ld;
                        }
                    }

                }
            }
            throw new Exception($"Unable to load internal dictionary {internal_dictionary}");
        }

        //--------------------------------------------------------------------------------------------
        // Lookup
        //--------------------------------------------------------------------------------------------
        public List<Word> GetdWords(Predicate<Word> test)
        {

            var res = from w in this.words where test(w) select w;
            return res.ToList();
        }

        public List<Word> GetdWordsWildCard(string pattern)
        {
            return GetdWords(W => Wildcards.isMatch(W.PrimarySpelling, pattern, false));
        }
    }
}
