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
using WDLang.Words;
using WDLang.Words.Import;

namespace BuildDataFiles_Util
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Loading dictionary (will take a while)");
            LanguageDictionary dic = LanguageDictionaryFactory.fromGCIDE_XML();
            Console.WriteLine("Done.");
            Console.WriteLine(dic.Count() + " words imported.");
            Console.WriteLine();

            string fileName = "English.dic";
            Console.WriteLine("Saving file: " + fileName);
            dic.Save(fileName);
            Console.WriteLine();

            Console.WriteLine("Loading file: " + fileName);
            LanguageDictionary engLoad = LanguageDictionary.Load(fileName);
            Console.WriteLine(engLoad.Count() + " words loaded.");
            Console.WriteLine();

            string checkFileName = "English2.dic";
            Console.WriteLine("Comparing saved loaded file against original.");
            engLoad.Save(checkFileName);
            if (File.ReadAllText(fileName) == File.ReadAllText(checkFileName))
            {
                Console.WriteLine("\tSucsess");
            }
            else
            {
                Console.WriteLine("\tFailed");
            }
            Console.WriteLine();
            dic.Save(@"X:\Programming\Resources\dictionaries\English.dic");

            Console.WriteLine("Press any key to continue.");
            Console.ReadKey();
        }
    }
}
