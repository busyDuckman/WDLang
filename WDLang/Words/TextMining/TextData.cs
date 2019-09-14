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

namespace WDLang.Words.TextMining
{
    public enum TextDataType 
    {
        Fragment,    //A sentace or some such.
        Paragraph,    //A paragraph or some such.
        EntireWork,  //A shole book etc.
        Chapter  // A chapter, page, paragraph or similar.
    }

    public enum TextDataCatagory
    {
        Unknown, Humanities, Religious, Philosophy, Historical, Scientific, Medical, Technical,
        Mathamatical, Political, Economics, Sport, Music,
        Nautical, Military, Marketing, Informal, Formal, Legal,
        ChildrensStories, FolkTales, Fantasy, Journalisim, Erotica, Horror,
        Perjorative, Satire
    }


    public class TextData
    {
        public string Text { get; set; }
        public string Title { get; set; }
        TextDataType Type { get; set; }
        TextDataCatagory Catagory { get; set; }

        public static TextData fromBook(string title, string text, TextDataCatagory catagory)
        {
            TextData td = new TextData();
            td.Text = text;
            td.Title = title;
            td.Type = TextDataType.EntireWork;
            td.Catagory = catagory;

            return td;
        }

        public static TextData fromFragment(string title, string text, TextDataCatagory catagory)
        {
            TextData td = new TextData();
            td.Text = text;
            td.Title = title;
            td.Type = TextDataType.Fragment;
            td.Catagory = catagory;

            return td;
        }

        public static TextData fromParagraph(string title, string text, TextDataCatagory catagory)
        {
            TextData td = new TextData();
            td.Text = text;
            td.Title = title;
            td.Type = TextDataType.Paragraph;
            td.Catagory = catagory;

            return td;
        }
    }
}
