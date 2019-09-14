/*  ---------------------------------------------------------------------------------------------------------------------------------------
 *  (C) 2019, Dr Warren Creemers.
 *  This file is subject to the terms and conditions defined in the included file 'LICENSE.txt'
 *  ---------------------------------------------------------------------------------------------------------------------------------------
 */
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Resolvers;
using WD_toolbox.Files.XML;

namespace WDLang.Words.TextMining.CorpusParsers
{
    public class BioMedXMLParser : GenericXMLParser
    {
        
        protected static LocalCacheXmlResolver bioMedXMLResolver = null;

        public BioMedXMLParser() : base()
        {
        }

        protected override XmlResolver GetXMLResolver()
        {
            if(bioMedXMLResolver == null)
            {
                bioMedXMLResolver = new LocalCacheXmlResolver(@"C:\DATA\Text Mining DataBase\medical\BioMed Central's Corpus\[NOPARSE] journal-publishing-dtd-3.0");
            }
            return bioMedXMLResolver;  

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
        }
    }
}
