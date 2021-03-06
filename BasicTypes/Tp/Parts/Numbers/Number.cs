﻿using System;
using System.Collections.Generic;
using System.Linq;
//using System.Runtime.DesignerServices;
using System.Text;
using System.Threading.Tasks;
using BasicTypes.Extensions;
using NUnit.Framework;

namespace BasicTypes.Parts
{
    /// <summary>
    /// Words can be converted to .NET types such as integer, decimal, etc.
    /// </summary>
    /// <remarks>
    /// Necessarily, this entails some degree of "community proposal"
    /// </remarks>
    [Serializable]
    public class Number : Token
    {
        public Number(string word)
            : base(word)
        {
            if (string.IsNullOrWhiteSpace(word))
            {
                throw new ArgumentException("Null or blank number");
            }
            if (word.Length == 1 && !"1234567890".Contains(word[0]))
            {
                throw new ArgumentException("Too short to be a word. " + word);
            }
            if (word.StartCheck("#"))
            {
                this.word = word.Substring(1);
            }
            else
            {
                this.word = word;
            }
        }

        //0 ala (nothing)
        //* 1 nena (nose) (or wan)
        //* 2 oko (eyes) (or tu)
        //3 kute (ears) (sum of everything inbetween)
        //4 uta (mouth)
        //* 5 luka (hand)
        //6 insa (belly)
        //7 monsi (butt)
        //8 palisa/lupa (netherregions, obviously depends on who's counting)
        //9 noka (leg)
        //* 10 noka pini (toes) (nena ala would be proper decimal)

        public static string ConvertToBodyNumber(decimal i, string gender = "M")
        {
            StringBuilder sb = new StringBuilder();
            ConvertDigits(i.ToString(), gender, sb);
            return ProcessNegatives(i.ToString(), sb);
        }
        

        public static string ConvertToBodyNumber(double i, string gender = "M")
        {
            StringBuilder sb = new StringBuilder();
            Console.WriteLine(i.ToString("E2"));
            ConvertDigits(i.ToString("E2"), gender, sb);
            return ProcessNegatives(i.ToString(), sb);
        }
        public static string ConvertToBodyNumber(int i, string gender = "M")
        {
            StringBuilder sb = new StringBuilder();
            ConvertDigits(i.ToString(), gender, sb);
            return ProcessNegatives(i.ToString(), sb);
        }

        private static string ProcessNegatives(string i, StringBuilder sb)
        {

            string temp = "#" + sb.ToString().Trim(new[] { '-' });
            if (i.ContainsCheck("-"))
            {
                temp = "nanpa-anpa " + temp;
            }
            while (temp.ContainsCheck("--"))
            {
                temp = temp.Replace("--", "-");
            }
            while (temp.ContainsCheck("wawa-ala"))
            {
                temp = temp.Replace("wawa-ala", "wawa-");
            }
            while (temp.ContainsCheck("wawa-ala"))
            {
                temp = temp.Replace("wawa-ala", "wawa-");
            }
            while (temp.ContainsCheck("--"))
            {
                temp = temp.Replace("--", "-");
            }
            while (temp.ContainsCheck("wawa-lili-ala"))
            {
                temp = temp.Replace("wawa-lili-ala", "wawa-lili-");
            }
            while (temp.ContainsCheck("wawa-lili-ala"))
            {
                temp = temp.Replace("wawa-lili-ala", "wawa-lili-");
            }
            while (temp.ContainsCheck("--"))
            {
                temp = temp.Replace("--", "-");
            }
            return temp;
        }

        private static void ConvertDigits(string representation, string gender, StringBuilder sb)
        {
            foreach (char c in representation.Replace("E-", "E~"))
            {
                switch (c)
                {
                    case '1':
                        sb.Append("wan");
                        break;
                    case '2':
                        sb.Append("tu");
                        break;
                    case '3':
                        sb.Append("kute");
                        break;
                    case '4':
                        sb.Append("uta");
                        break;
                    case '5':
                        sb.Append("luka");
                        break;
                    case '6':
                        sb.Append("insa");
                        break;
                    case '7':
                        sb.Append("tu");
                        break;
                    case '8':
                        sb.Append("monsi");
                        break;
                    case '9':
                        sb.Append(gender == "M" ? "palisa" : "lupa");
                        break;
                    case '0':
                        sb.Append("ala");
                        break;
                    case '.':
                        sb.Append("sike");
                        break;
                    case 'E':
                        sb.Append("wawa");
                        break;
                    case '-':
                    //skip deal with elsewhere.
                    case '+':
                        //default to positive
                        break;
                    case '~':
                        sb.Append("lili");
                        break;
                    default:
                        sb.Append("[" + c + "]");
                        break;
                }
                sb.Append("-");
            }
        }

        public static bool IsPomanNumber(string word)
        {
            string testWord = word.Replace("#", "");
            testWord = testWord.Replace("\"", "");
            return testWord.All(c => Token.PomanNumbers.Contains(c.ToString()));
        }

        public static int ConvertFromPoman(string value)
        {
            int pomanSum = 0;
            foreach (char c in value.ToUpperInvariant())
            {
                if (c == 'A')
                {
                    pomanSum += 100;
                }
                else if(c =='M')
                {
                    pomanSum += 100;
                }
                else if (c == 'L')
                {
                    pomanSum += 5;
                }
                else if (c == 'T')
                {
                    pomanSum += 2;
                }
                else if (c == 'W')
                {
                    pomanSum += 1;
                }
                else
                {
                    throw new ArgumentOutOfRangeException("Unexpected letter " + c);
                }
            }
            return pomanSum;
        }
        

        //The ordinal/cardinal split in TP is fundamentally borked
        public static bool IsNumber(HeadedPhrase phrase)
        {
            if (WordByValue.Instance.Equals(phrase.Head, Words.nanpa))
            {
                //We have an ordinal...
                return true;
            }
            //# numbers.
            if (BodyNumbers.Contains(phrase.Head.Text) && phrase.Modifiers.All((x => Token.BodyNumbers.Contains(x.Text))))
            {
                return true;
            }
            if (StupidNumbers.Contains(phrase.Head.Text) && phrase.Modifiers.All(x => Token.StupidNumbers.Contains(x.Text)))
            {
                return true;
            }
            if (HalfStupidNumbers.Contains(phrase.Head.Text) && phrase.Modifiers.All(x => Token.HalfStupidNumbers.Contains(x.Text)))
            {
                return true;
            }
            return false;
        }

        public int ToInteger()
        {
            throw new NotImplementedException();
        }

        public string TryGloss(string language, string pos, Dialect dialect)
        {
            //Arabic numbers. Just return them.
            int n;
            bool isNumeric = int.TryParse(word, out n);
            if (isNumeric) return word;

            if (Number.IsPomanNumber(word))
            {
                return Number.ConvertFromPoman(word).ToString();
            }
            //Toki pona numbers of any sort (body, stupid, half stupid, but must be explicit! (or via normalizer, marked with #))
            if (dialect.NumberType == "Half Stupid" || dialect.NumberType == "Stupid")
            {
                int sum = 0;
                foreach (string part in word.Split(new char[] { '-' }))
                {
                    switch (part)
                    {
                        case "ala":
                            continue;
                        case "wan":
                            sum += 1;
                            break;
                        case "tu":
                            sum += 2;
                            break;
                        case "luka":
                            sum += 5;
                            break;
                        case "mute":
                            sum += 20;
                            break;
                        case "ali":
                        case "ale":
                            sum += 5;
                            break;
                        default:
                            throw new ArgumentOutOfRangeException("Unexpected token " + part + " in " + word + " maybe we have the wrong numbertype set in the dialect.");
                    }
                }
                return sum.ToString();
            }
            else
            {
                Dictionary<string, int> bodyNumbers = new Dictionary<string, int>(14)
                {
                    {"ala",0 },
                    {"nena",1},{ "wan",1},
                    {"oko",2}, {"tu",2},
                    {"kute",3},
                    {"uta",4}, 
                    {"luka",5},
                    {"insa",6},
                    {"monsi",7},
                    {"palisa",8},{ "lupa",8},
                    {"noka",9}
                };


                string[] digits = word.Split(new char[] { '-' });
                int bodySum = 0;
                for (int i = digits.Length-1; i >= 0; i--)
                {
                    bodySum += bodyNumbers[digits[i]] * 10 ^ i;
                }
                return bodySum.ToString();
            }
        }
    }
}
