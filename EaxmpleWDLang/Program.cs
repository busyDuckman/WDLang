using System;
using WDLang.Words;

namespace EaxmpleWDLang
{
    class Program
    {
        static void Main(string[] args)
        {
            int min_len = 20;

            Console.WriteLine("Loading default English dictionary.");
            var dictionary = LanguageDictionary.loadInternalEnglish();
            Console.WriteLine($"Finding words longer than {min_len} characters:");
            Console.WriteLine();
            foreach (Word word in dictionary)
            {
                if ((word.PrimarySpelling.Length > 20) && (!word.PrimarySpelling.Contains('-')))
                {
                    Console.WriteLine(word);
                    Console.WriteLine();
                }
            }
        }
    }
}
