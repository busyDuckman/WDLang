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
using System.Xml.Resolvers;
using WD_toolbox;
using WD_toolbox.Data.Text;

namespace WDLang.Words.TextMining.CorpusParsers
{
    public class GenericXMLParser : CorpusParser
    {
        public bool SplitParagraphs { get; set; }
        public int MinParagraphLen { get; set; }
        public int MinWordsPerParagraph { get; set; }

        HashSet<LowerCaseString> unWantedNodes;
        HashSet<LowerCaseString> wantedNodes;

        HashSet<LowerCaseString> newLineNodes;
        
        public GenericXMLParser()
        {
            SplitParagraphs = false;
            MinParagraphLen = 0;       //disable
            MinWordsPerParagraph = 0;  //disable

            unWantedNodes = new HashSet<LowerCaseString>() { 
                "fm".GetLowerCaseString(),
                "ui".GetLowerCaseString(),
                "ji".GetLowerCaseString(),
                "dochead".GetLowerCaseString(),
                "bibl".GetLowerCaseString(),
                "history".GetLowerCaseString(),
                "cpyrt".GetLowerCaseString(),
                "abs".GetLowerCaseString(),
                "tbl".GetLowerCaseString(),
                "bm".GetLowerCaseString(),
                "refgrp".GetLowerCaseString(),
                "bibl".GetLowerCaseString(),
                "abbrgrp".GetLowerCaseString(),
                "abbr".GetLowerCaseString(),
                "tblr".GetLowerCaseString()
            };

            wantedNodes = new HashSet<LowerCaseString>() { 
                "art".GetLowerCaseString(),
                "bdy".GetLowerCaseString(),
                "sec".GetLowerCaseString(),
                "st".GetLowerCaseString(),
                "p".GetLowerCaseString(),
                "title".GetLowerCaseString(),
            };

            newLineNodes = new HashSet<LowerCaseString>() { 
                "p".GetLowerCaseString(),
                "st".GetLowerCaseString()
            };
        }

        public override IList<TextData> LoadData(System.IO.Stream stream, string path, TextDataCatagory catagoryHint)
        {
            //setup a compitent xml reader
            XmlReaderSettings settings = new XmlReaderSettings();
            settings.DtdProcessing = DtdProcessing.Parse;
            settings.ValidationType = ValidationType.DTD;
            settings.MaxCharactersFromEntities = 64 * 1024 * 1024;

            XmlResolver resolver = GetXMLResolver();
            if (resolver != null)
            {
                settings.XmlResolver = resolver;
            }

            XmlReader reader = XmlReader.Create(stream, settings);

            XmlDocument xml = new XmlDocument();
            xml.Load(reader);

            List<TextData> items = new List<TextData>();
            
            //do the xml parsing
            StringBuilder sb = new StringBuilder();

            Stack<XmlNode> nodes = new Stack<XmlNode>();
            nodes.Push(xml);

            while (nodes.Count > 0)
            {
                XmlNode currentNode = nodes.Pop();
                if (useNode(currentNode))
                {
                    if (currentNode.Name == "#text")
                    {
                        //Console.Write(indent + node.InnerText);
                        sb.Insert(0, currentNode.InnerText);
                    }
                    else
                    {
                        if (SplitParagraphs && isParagraphNode(currentNode))
                        {
                            items.Add(TextData.fromParagraph("", sb.ToString(), catagoryHint));
                            sb.Clear();
                        }
                        else
                        {
                            string s = nodeToString(currentNode);
                            if (s != null)
                            {
                                sb.Insert(0, s);
                            }
                        }
                    }

                    foreach (XmlNode subNode in currentNode)
                    {
                        nodes.Push(subNode);
                    }
                }
            }

            if (sb.Length > 3)
            {
                items.Add(TextData.fromParagraph("", sb.ToString(), catagoryHint));
            }
            //return results
            return items;
        }

        protected virtual bool isParagraphNode(XmlNode node)
        {
            return (node.Name.ToLower() == "p");
        }


        /// <summary>
        /// Some nodes oter than #text nodes may imply text. This 
        /// method will return text in those cases.
        /// </summary>
        protected virtual string nodeToString(XmlNode node)
        {
            if (newLineNodes.Contains(node.Name.GetLowerCaseString()))
            {
                return "\r\n"; 
            }

            return null; //normal case, no error implied
        }

        /// <summary>
        /// True for nodes that we will use
        /// </summary>
        protected virtual bool useNode(XmlNode node)
        {
            LowerCaseString key = node.Name.GetLowerCaseString();
            if (unWantedNodes.Contains(key))
            {
                return false;
            }
            else if (wantedNodes.Contains(key))
            {
                return true;
            }
            else
            {
                Console.WriteLine("new node: " + key);
                return true;
            }
        }

        protected virtual XmlResolver GetXMLResolver()
        {
            /*XmlPreloadedResolver resolver = new XmlPreloadedResolver(new XmlKnownDtds());
            string resourceDir = @"C:\DATA\Text Mining DataBase\medical\BioMed Central's Corpus\";
            foreach (string file in Directory.GetFiles(resourceDir, "*.dtd", SearchOption.TopDirectoryOnly))
            {
                string name = Path.GetFileName(file);
                resolver.Add(resolver.ResolveUri(null, name), File.ReadAllText(file));

                if (name == "mathml2.dtd")
                {
                    resolver.Add(resolver.ResolveUri(null, @"http://www.biomedcentral.com/xml/MathML2/" + name), File.ReadAllText(file));
                }
            }
            return resolver;*/
            return null;
        }

      
        /*public override IList<TextData> LoadData2(System.IO.Stream stream, string path, TextDataCatagory catagoryHint)
        {
            XmlDocument xml = new XmlDocument();
            //xml.Load("fixedxml.xml");
            Console.WriteLine("xml loading");

            xml.Load(stream);
            XmlNodeList firstNodes = xml.ChildNodes;

            foreach (XmlNode node in firstNodes)
            {

            }
        }*/
    }
}
