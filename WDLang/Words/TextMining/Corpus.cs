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
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.IO.Compression;
using WDLang.Words.TextMining.CorpusParsers;

namespace WDLang.Words.TextMining
{
    public class Corpus
    {
        public delegate void CorpusProcessDelegate(TextData data);

        public List<TextData> Items { get; set; }

        Corpus(List<TextData> _items)
        {
            Items = new List<TextData>(_items);
        }
        
        public static Corpus Merge(Corpus a, Corpus b)
        {
            List<TextData> newItems = new List<TextData>();
            newItems.AddRange(a.Items);
            foreach (TextData item in b.Items)
            {
                if (!newItems.Contains(item))
                {
                    newItems.Add(item);
                }
            }

            return new Corpus(newItems);
        }

        public static void ProcessZip(string path, Regex filePattern, TextDataCatagory catagoryHint, CorpusProcessDelegate process)
        {
            using (ZipArchive archive = ZipFile.OpenRead(path))
            {
                foreach (ZipArchiveEntry entry in archive.Entries)
                {
                    if (filePattern.Matches(entry.Name).Count > 0)
                    {
                        using (Stream s = entry.Open())
                        {
                            ProcessFile(s, path + "@" + entry.FullName, catagoryHint, process);
                        }
                    }
                }
            } 
        }

        public static void ProcessDir(string path, Regex filePattern, TextDataCatagory catagoryHint, CorpusProcessDelegate process)
        {
            if (Directory.Exists(path))
            {
                string[] files = Directory.GetFiles(path, "*", SearchOption.AllDirectories);
                foreach(string file in files)
                {
                    if(file.Contains("[NOPARSE]"))
                    {
                        continue;
                    }

                    string fileName = Path.GetFileName(file);
                    if (filePattern.Matches(fileName).Count > 0)
                    {
                        if (Path.GetExtension(file).ToLower() == ".zip")
                        {
                            ProcessZip(file, filePattern, catagoryHint, process);
                        }
                        else
                        {
                            using (Stream s = File.OpenRead(file))
                            {
                                ProcessFile(s, file, catagoryHint, process);
                            }
                        }
                    }
                }
            }
        }

        public static void ProcessFile(Stream stream, string path, TextDataCatagory catagoryHint, CorpusProcessDelegate process)
        {
            CorpusParser parser = GetCorpusParserFromPath(path);
            IList<TextData> data = parser.LoadData(stream, path, catagoryHint);

            foreach (TextData item in data)
            {
                process(item);
            }
        }

        public static CorpusParser GetCorpusParserFromPath(string path)
        {
            string extension = Path.GetExtension(path).ToLower().Trim(" .".ToCharArray());

            string dir = Path.GetDirectoryName(path).ToLower();

            if(dir.Contains("biomed"))
            {
                return new BioMedXMLParser();
            }

            //finally
            if (extension == "txt")
            {
                return new PlainTextParser();
            }
            if (extension == "xml")
            {
                return new GenericXMLParser();
            }

            return new PlainTextParser();
        }

        public static void ProcessCorpus(string corpusPath, CorpusProcessDelegate process)
        {
            ProcessCorpus(corpusPath, new Regex(".*"), process);
        }

        public static void ProcessCorpus(string corpusPath, CorpusProcessor processor)
        {
            ProcessCorpus(corpusPath, new Regex(".*"), processor);
        }

        public static void ProcessCorpus(string corpusPath, IList<CorpusProcessor> processors)
        {
            ProcessCorpus(corpusPath, new Regex(".*"), processors);
        }

        public static void ProcessCorpus(string corpusPath, Regex filePattern, CorpusProcessor processor)
        {
            List<CorpusProcessor> processors = new List<CorpusProcessor>();
            processors.Add(processor);
            ProcessCorpus(corpusPath, filePattern, processors);
        }

        public static void ProcessCorpus(string corpusPath, Regex filePattern, IList<CorpusProcessor> processors)
        {
            CorpusProcessDelegate d = delegate(TextData data) 
            {
                foreach (CorpusProcessor processor in processors)
                {
                    processor.Process(data);
                }
            };

            ProcessCorpus(corpusPath, filePattern, d);
        }



        public static void ProcessCorpus(string corpusPath, Regex filePattern, CorpusProcessDelegate process)
        {
            string[] dirs = Directory.GetDirectories(corpusPath, "*", SearchOption.TopDirectoryOnly);
            foreach (string dir in dirs)
            {
                TextDataCatagory catagory = GetCatagoryFromDir(dir);
                ProcessDir(dir, filePattern, catagory, process);
            }
        }

        protected static TextDataCatagory GetCatagoryFromDir(string dir)
        {
            string dir2 = dir.Trim("\\/".ToCharArray());
            dir2 = Path.GetFileName(dir2).ToLower().Replace(" ", "");

            foreach (var value in Enum.GetValues(typeof(TextDataCatagory)))
            {
                string name = Enum.GetName(typeof(TextDataCatagory), ((TextDataCatagory)value)).ToLower();
                if (dir2 == name)
                {
                    return (TextDataCatagory)value;
                }
            }

            return TextDataCatagory.Unknown;
        }
    }
}
