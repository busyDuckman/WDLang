/*  ---------------------------------------------------------------------------------------------------------------------------------------
 *  (C) 2019, Dr Warren Creemers.
 *  This file is subject to the terms and conditions defined in the included file 'LICENSE.txt'
 *  ---------------------------------------------------------------------------------------------------------------------------------------
 */
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WD_toolbox;
using WD_toolbox.Data;
using WD_toolbox.Data.DataStructures;

namespace WDLang.Words.Controls
{
    public partial class WordSelectionBox : UserControl
    {
        protected bool userInput = true;
        public LanguageDictionary Dictionary { get; set; }
        public int MaxRecentWords { get; set; }

        private Queue<Word> recentWords = new Queue<Word>();

        public WordSelectionBox() : this (null)
        {
        }

        public WordSelectionBox(LanguageDictionary dic)
        {
            this.Dictionary = dic;
            InitializeComponent();
            MaxRecentWords = 10;
        }

        private void WordSelectionBox_Load(object sender, EventArgs e)
        {

        }

        private void txtWordInput_TextChanged(object sender, EventArgs e)
        {
            if (userInput)
            {
                updateListOfMatchs(txtWordInput.Text);
            }
        }
        
        /*public void selectAndShowWord(string text)
        {
            if (Dictionary != null)
            {
                Word word = Dictionary[text];
                selectAndShowWord(word);
            }

        }*/

        public void selectAndShowWord(Word word)
        {
            userInput = false;
            txtWordInput.Text = word.PrimarySpelling;
            userInput = true;

            if (word != null)
            {
                tcMain.SelectedTab = tpDefinition;
                recentWords.Enqueue(word);
                txtDefinition.Text = word.FormatedCompleteDefinition;
            }

        }

        public void updateListOfMatchs(string text)
        {
            if (Dictionary == null)
            {
                return;
            }

            if (string.IsNullOrWhiteSpace(text))
            {
                displayWords(recentWords);
            }
            else if (text.Contains("*") || text.Contains("?"))
            {
                //wildcard
                List<Word> wrds = Dictionary.GetdWordsWildCard(text);
                displayWords(wrds);
            }
            else
            {
                List<Word> wrds = Dictionary.GetdWordsWildCard(text + "*");
                displayWords(wrds);
            }
        }

        private void displayWords(IEnumerable<Word> wrds)
        {
            lstMatches.Items.Clear();
            int max = 200;
            foreach (Word wrd in wrds)
            {
                max--;
                if (max < 0)
                {
                    break;
                }

                lstMatches.Items.Add(new Tag<Word>(wrd, wrd.PrimarySpelling));
            }
        }

        private void lstMatches_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lstMatches.SelectedIndex >= 0)
            {
                Tag<Word> tagedWord = lstMatches.SelectedItem as Tag<Word>;
                if (tagedWord != null)
                {
                    selectAndShowWord(tagedWord);
                }
            }
        }
    }
}
