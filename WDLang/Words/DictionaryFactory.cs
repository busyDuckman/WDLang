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
using System.Xml;
using WDLang.Words.Import.GCIDE;

namespace WDLang.Words
{
    public class LanguageDictionaryFactory
    {
        public static LanguageDictionary fromGCIDE_XML(string xmlPath = @"X:\Programming\Resources\dictionaries\gcide_xml")
        {
            List<GCIDEWord> wordList = LoadXML();

            //Dictionary<string, GCIDEWord> words = GCIDEWord.formulateSimepleDictionary(wordList);
            List<GCIDEWord> words = GCIDEWord.formulateDictionary(wordList);
            //words.Add("razzamatazz", new GCIDEWord("razzamatazz", "A flamboyant (gaudy) display intended to awe, impress, bewilder, confuse or deceive. 2. razzle-dazzle", "n."));

            List<GCIDEWord> badWords = words.Where(W => !GCIDEWord.isNormalWord(W)).ToList();
            foreach (var badWord in badWords)
            {
                Console.WriteLine("Pruning: " + badWord);
                words.Remove(badWord);
            }

            //ExportDictionary(words, out maxWord, out maxDef);

            //frequency stuff
            /*GC.Collect();
            string refText = File.ReadAllText(@"X:\uni\c++ assignment\all - ansi - clean.txt");
            string[] allWords = getAllWords(refText);
            foreach (string str in allWords)
            {
                //proper noun match?
                List<GCIDEWord> sameWords = words.Where(W => W.Text == str).ToList();
                if (sameWords.Count == 0)
                {
                    string strClean = GCIDEWord.cleanWordText(str);
                    sameWords = words.Where(W => W.Text == strClean).ToList();
                }

                foreach (GCIDEWord sameWord in sameWords)
                {
                    sameWord.Freq = sameWord.Freq + 1;
                }
            }

            allWords = null; refText = null;*/
            GC.Collect();


            //build the dictionary
            LanguageDictionary dic = new LanguageDictionary();
            foreach (GCIDEWord word in words)
            {
                dic.MergeWordIntoSortedDictionary(word.toWord(dic));
            }
            return dic;
        }

        private static string[] getAllWords(string refText)
        {
            string[] allWords = refText.Split(" ,.<>/?:;'[]{}\\|\"\r\n\t!@#$%^&*()_+=1234567890`~".ToCharArray());
            return allWords;
        }

        private static void ExportDictionary(Dictionary<string, GCIDEWord> words, out int maxWord, out int maxDef)
        {

            StringBuilder outText = new StringBuilder();
            maxWord = 0;
            maxDef = 0;

            Dictionary<string, int> uniqueTypeStrings = new Dictionary<string, int>();

            foreach (var entry in words.OrderBy(KVP => KVP.Key))
            {
                GCIDEWord word = entry.Value;

                outText.AppendLine(word.Text);
                outText.AppendLine(word.Definition);
                outText.AppendLine(word.BasicType);
                outText.AppendLine();

                maxWord = Math.Max(maxWord, word.Text.Length);
                maxDef = Math.Max(maxDef, word.Definition.Length);

                string type = word.BasicType;
                if (!uniqueTypeStrings.ContainsKey(type))
                {
                    uniqueTypeStrings.Add(type, 1);
                }
                else
                {
                    uniqueTypeStrings[type] = uniqueTypeStrings[type] + 1;
                }

                if (word.Text.ToLower().Contains("zz"))//Count(C => C == 'z') >= 2)
                {
                    Console.Out.WriteLine("ZZZZ: " + word.Text);
                }
            }

            foreach (var entry in uniqueTypeStrings)
            {
                string count = "" + entry.Value;
                string what = entry.Key;
                while (what.Length < 10)
                {
                    what += " ";
                }

                Console.WriteLine("UTS: " + what + " " + count);
            }

            File.WriteAllText("dictionary.txt", outText.ToString());
        }

        private static List<GCIDEWord> LoadXML()
        {
            List<GCIDEWord> words = new List<GCIDEWord>();
            try
            {
                Console.WriteLine("xml fix up");
                string dir = @"X:\Programming\Resources\dictionaries\gcide_xml\xml_files";
                string file = Path.Combine(dir, string.Format("gcide_{0}.xml", 'a'));
                string rawXML = File.ReadAllText(file);
                string fixedXml = string.Format("<dictionary>\r\n{0}\r\n</dictionary>", rawXML);
                File.WriteAllText("fixedxml.xml", fixedXml);


                XmlReaderSettings settings = new XmlReaderSettings();
                settings.DtdProcessing = DtdProcessing.Parse;
                settings.ValidationType = ValidationType.DTD;
                settings.MaxCharactersFromEntities = 64 * 1024 * 1024;
                XmlReader reader = XmlReader.Create(Path.Combine(dir, "gcide.xml"), settings);

                XmlDocument xml = new XmlDocument();
                //xml.Load("fixedxml.xml");
                Console.WriteLine("xml loading");
                xml.Load(reader);
                XmlNodeList firstNodes = xml.ChildNodes;


                Queue<XmlNode> nodes = new Queue<XmlNode>();
                foreach (XmlNode node in firstNodes)
                {
                    nodes.Enqueue(node);
                }

                bool multipleDefs = false;

                Console.WriteLine("parsing xml");
                char lastLetter = (char)('a'-1);
                while (nodes.Count > 0) //where is the isEmpty method?
                {
                    XmlNode node = nodes.Dequeue();
                    bool synonim = false;

                    if (node.Name == "p")
                    {
                        //parse the xml
                        XmlNode defNode = node.SelectSingleNode("def");
                        XmlNode wordNode = node.SelectSingleNode("hw");
                        if (wordNode == null)
                        {
                            wordNode = getChild(node, "mhw", "hw");
                        }
                        if (wordNode == null)
                        {
                            wordNode = node.SelectSingleNode("sn");
                            synonim = true;
                        }

                        //multidef?
                        if (wordNode == null)
                        {
                            wordNode = node.SelectSingleNode("h1");
                            if (wordNode != null)
                            {
                                multipleDefs = true;
                            }
                        }
                        else
                        {
                            multipleDefs = false;
                        }

                        //add the word to the dictionary
                        if (defNode != null)
                        {
                            /*if (wordNode != null) if (wordNode.InnerText.Trim() == "2.")
                            {
                                Console.WriteLine("debug");
                            }*/

                            if (synonim)
                            {
                                words.Last().appendDefinition(defNode.InnerText);
                            }
                            else if (wordNode != null)
                            {

                                //Console.WriteLine(wordNode.InnerText);
                                //Console.WriteLine(defNode.InnerText);
                                //Console.WriteLine();

                                

                                //get type
                                string type = "";
                                XmlNode typeNode = node.SelectSingleNode("pos");
                                typeNode = (typeNode == null) ? defNode.SelectSingleNode("pos") : typeNode;
                                if (typeNode != null)
                                {
                                    type = typeNode.InnerText;
                                }
                                else
                                {
                                    //Console.WriteLine("word without type");
                                }

                                //save word
                                GCIDEWord word = new GCIDEWord(wordNode.InnerText, defNode.InnerText, type);
                                words.Add(word);

                                //status
                                if (word.Text.ToLower()[0] > lastLetter)
                                {
                                    lastLetter = word.Text.ToLower()[0];
                                    Console.Write(lastLetter + " ");
                                }

                                if (word.isNormalWord())
                                {
                                    if (word.hasOddChars())
                                    {
                                        Console.WriteLine("ERROR");
                                    }
                                }
                            }
                            else if (multipleDefs)
                            {
                                words.Last().appendDefinition(defNode.InnerText);
                            }
                            else
                            {
                                Console.WriteLine("#ERROR: def without a word");
                            }
                        }
                    }
                    else
                    {
                        foreach (XmlNode childNode in node.ChildNodes)
                        {
                            nodes.Enqueue(childNode);
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }

            return words;
        }

        private static XmlNode getChild(XmlNode node, params string[] args)
        {
            foreach (string nodeName in args)
            {
                if (node == null)
                {
                    return null;
                }
                node = node.SelectSingleNode(nodeName);
            }

            return node;
        }

    }

}
