# Overview
WDLang (WDData namespace) is a library for working with dictionaries and parsing text.

It uses the [GNU Collaborative International Dictionary of English](http://gcide.gnu.org.ua/) as the source for it's english dictionary.

The code is Copyright (C) 2019, Dr Warren Creemers.
The code and project is licenced under [GPLv3](http://www.gnu.org/licenses/gpl-3.0.html)

# Example usage

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

# Projects

## WDData
A library for handeling dictionaries and words. This is the project pushed to nuget.

## BuildDataFiles_util
Some batch file like code for updating the english dictionary file.

## EaxmpleWDLang
Examples of usage.

 