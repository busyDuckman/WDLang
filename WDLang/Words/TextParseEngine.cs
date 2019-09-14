/*  ---------------------------------------------------------------------------------------------------------------------------------------
 *  (C) 2019, Dr Warren Creemers.
 *  This file is subject to the terms and conditions defined in the included file 'LICENSE.txt'
 *  ---------------------------------------------------------------------------------------------------------------------------------------
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace WDLang.Words
{
    /// <summary>
    /// A class to parse english language text.
    /// </summary>
    public class TextParseEngine
    {
        public enum ParticleType {Word, Punctuation, Numerical, Symbol, Other, WhiteSpace}
        //public enum TokenT { Number, Text, Whitespace }
        
        public string Text { get; protected set; }
        int readPos;
        //ParseStates parseState;

        public class Token 
        {
            public string Text { get; protected set; }
            public ParticleType Type { get; protected set; }

            public Token(string token, ParticleType type)
            {
                this.Text = token;
                this.Type = type;
            }

            public override string ToString()
            {
                string typeName = Enum.GetName(typeof(ParticleType), Type);
                typeName = typeName.PadRight(14);
                return String.Format("{0} --->{1}<---", typeName, Text);
            }         
        }

        public TextParseEngine(string text)
        {
            this.Text = text;
            readPos = 0;
            //parseState = ParseStates.NewSentance;
        }

        //---------------------------------------------------------------------------------------------------------------
        // Structure parsing
        //---------------------------------------------------------------------------------------------------------------

        public static bool willTokenEndSentance(Token token)
        {
            if (token.Type == ParticleType.Punctuation)
            {
                return (token.Text.IndexOfAny(".?!".ToCharArray()) > 0);
            }
            return false;
        }

        public Token[] readTokensTill(Func<Token, bool> stopReading)
        {    
            List<Token> tokens = new List<Token>();
            Token t = readNextToken();
            while (t != null)
            {
                tokens.Add(t);
                if (stopReading(t))
                {
                    break;
                }
                t = readNextToken();
            }
            return tokens.ToArray();            
        }

        //---------------------------------------------------------------------------------------------------------------
        // tokens
        //---------------------------------------------------------------------------------------------------------------
        public Token readNextToken()
        {
            ParticleType tokenType = ParticleType.Other;
            string token = "";
                       
            while (readPos < Text.Length)
            {
                char c = Text[readPos];
                ParticleType type = getType(c);
                if (token.Length == 0)
                {
                    tokenType = type; //set up the parse type for this fragment
                }

                //first non-whitespace token?
                if ((token.Length == 0) && (type != ParticleType.WhiteSpace))
                {
                    //quick check for a difficult problem
                    string numerical = extractValidNumericalFromHereToNextWhiteSpcae();
                    if (numerical != null)
                    {
                        readPos += numerical.Length;
                        //readPos++; //char after last read item is the read pos
                        tokenType = ParticleType.Numerical;
                        return new Token(numerical, tokenType);
                    }

                    //set the token type
                    tokenType = type;
                }

                //finish when the token goes sideways
                if (type != tokenType)
                {
                    bool keepParsing = false;
                    //look for a reason not to finish parsing
                    if ((token.Length == 1) && (type == ParticleType.Word))
                    {
                        if(isAllowedToStartWord(token[0])) {
                            //odd start wors eg: 'cept
                            tokenType = ParticleType.Word;
                            keepParsing = true;
                        }
                    }

                    if ((!keepParsing) && ((token.Length > 1) && (tokenType == ParticleType.Word)))
                    {
                        if(readPos < (Text.Length-1))
                        {
                            if(getType(Text[readPos+1]) == ParticleType.Word)
                            {
                                //eg bumble-bee
                                keepParsing = isAllowedInsideWord(c);
                            }
                        }
                    }

                    if ((!keepParsing) && ((token.Length > 1) && (tokenType == ParticleType.Word)))
                    {
                         if(readPos < (Text.Length-1))
                         {
                             if(getType(Text[readPos+1]) != ParticleType.Word)
                             {
                                //eg: bah`
                                keepParsing = isAllowedToFinishWord(c);
                             }
                         }
                    }

                    if ((!keepParsing) && (isLetterDigitOrSymbol(tokenType) && isLetterDigitOrSymbol(type)))
                    {
                        tokenType = ParticleType.Other;
                        keepParsing = true;
                    }

                    if(!keepParsing)
                    {
                        return new Token(token, tokenType);
                    }
                   
                }

                //ready for next
                token += c;
                readPos++;
            }

            //final token
            if (token.Length > 0)
            {
                return new Token(token, tokenType);
            }

            //nothing more to parse
            return null;
        }

        private bool isLetterDigitOrSymbol(ParticleType tokenType)
        {
            switch (tokenType)
            {
                case ParticleType.Word:
                    return true;
                case ParticleType.Numerical:
                    return true;
                case ParticleType.Symbol:
                    return true;
                case ParticleType.Other:
                    return true;
                default:
                    return false;
            }
        }

        public bool isAllowedToStartWord(char c)
        {
            return (char.IsLetter(c) || "'".Contains(c));
        }

        public bool isAllowedInsideWord(char c)
        {
            return (char.IsLetter(c) || "-'".Contains(c));
        }

        public bool isAllowedToFinishWord(char c)
        {
            return (char.IsLetter(c) || "'".Contains(c));
        }

        /// <summary>
        /// A lot of things may be numerical eg -5,000.0Hz
        /// This method parses forward to see if we have such an event.
        /// This keeps this complicated logic away from the main parser, which is then free
        /// to treat punctuation etc in the simple case.
        /// </summary>
        /// <returns></returns>
        private string extractValidNumericalFromHereToNextWhiteSpcae(int start=-1)
        {
            if (start < 0)
            {
                start = readPos;
            }
            int nextWhiteSpacePos = getNextWhiteSpacePos(start);
            if (nextWhiteSpacePos > 0)
            {
                string text = Text.Substring(start, nextWhiteSpacePos - start);

                //must have one number to look numeric
                bool vaguelyNumeric = false;
                foreach (char c in text)
                {
                    if (char.IsNumber(c))
                    {
                        vaguelyNumeric = true;
                    }
                }

                //has a number, so running a regex won't nesesarily be a waste of time
                if (vaguelyNumeric)
                {
                    if (looksNumerical(text))
                    {
                        //the ending . may be a full stop
                        if (text.EndsWith("."))
                        {
                            int  scan=nextWhiteSpacePos;
                            while (scan < Text.Length)
                            {
                                if(!Char.IsWhiteSpace(Text[scan]))
                                {
                                    if (Char.IsUpper(Text[scan]))
                                    {
                                        // length > 1 is impled by code path (vaguelyNumeric & EndsWith("."))
                                        text = text.Substring(0, text.Length - 1);
                                        return text;
                                    }
                                    return text;
                                }
                                scan++;
                            }
                        }
                        return text;
                    }
                    return null;
                }
            }
            return null;
        }

        private int getNextWhiteSpacePos(int start = -1)
        {
            if (start < 0)
            {
                start = readPos;
            }
            int pos = readPos;
            while (pos < Text.Length)
            {
                if(char.IsWhiteSpace(Text[pos]))
                {
                    return pos;
                }
                pos++;
            }
            return -1;
        }

        public static ParticleType getType(char c)
        {
            if (char.IsLetter(c))
            {
                return ParticleType.Word;
            }
            if (char.IsWhiteSpace(c))
            {
                return ParticleType.WhiteSpace;
            }
            else if (char.IsPunctuation(c))
            {
                return ParticleType.Punctuation;
            }
            else if(char.IsNumber(c))
            {
                return ParticleType.Numerical;
            }
            else if (char.IsSymbol(c))
            {
                return ParticleType.Symbol;
            }
            else
            {
                return ParticleType.Other;
            }
        }

        private static Regex looksNumericalRegex = new Regex(@"^[\S]{0,3}[0-9][,.\-\(\)0-9]*[\S]{0,3}$");
        public static bool looksNumerical(string token)
        {
            return looksNumericalRegex.IsMatch(token);
        }

        public class Chapter : List<Paragraph>
        {
        }

        public class Paragraph : List<Sentance>
        {

        }

        public class Sentance : List<Particle>
        {

        }

        public class Particle
        {
            string token;
        }
    }
}
