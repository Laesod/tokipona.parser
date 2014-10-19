﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using BasicTypes.Collections;
using BasicTypes.Exceptions;
using BasicTypes.Extensions;
using System.Collections.ObjectModel;

namespace BasicTypes
{
    [DataContract]
    [Serializable]
    public class Sentence : IContainsWord, IFormattable
    {
        [DataMember]
        private readonly Sentence conclusion;

        [DataMember]
        private readonly Sentence[] preconditions;

        //Breaks immutability :-(
        [DataMember]
        public List<Chain> LaFragment { get; set; }

        [DataMember]
        private readonly Chain fragments;

        [DataMember]
        private readonly Chain[] subjects;
        [DataMember]
        private readonly PredicateList predicates;
        [DataMember]
        private readonly Punctuation punctuation;

        [DataMember]
        private readonly Particle conjunction;

        public Sentence(Sentence[] preconditions = null, Sentence conclusion = null)
        {
            if (preconditions != null && preconditions.Length > 0 && conclusion == null)
            {
                throw new TpSyntaxException("There must be a head sentence (conclusions) if there are preconditions.");
            }
            if (conclusion != null && (preconditions != null))
            {
                throw new TpSyntaxException("Only a head sentence can have preconditions.");
            }
            this.conclusion = conclusion;
            this.preconditions = preconditions;//Entire sentences.       

            if (preconditions != null)
            {
                foreach (Sentence precondition in preconditions)
                {
                    precondition.HeadSentence = conclusion;
                }

            }
        }

        public Sentence HeadSentence { get; private set; }

        public Sentence(Chain fragments, Chain subjects, PredicateList predicates, Punctuation punctuation = null, Particle conjuction = null)
        {
            this.subjects = new Chain[] { subjects }; //only (*), o, en
            this.predicates = predicates; //only li, pi, en
            this.punctuation = punctuation ?? new Punctuation(".");
            this.conjunction = conjuction;
            this.fragments = fragments;
        }

        public Sentence(Chain subjects, PredicateList predicates, Punctuation punctuation = null, Particle conjuction = null)
        {
            //TODO: Validate. 
            this.subjects = new Chain[] { subjects }; //only (*), o, en
            this.predicates = predicates; //only li, pi, en
            this.punctuation = punctuation ?? new Punctuation(".");
            this.conjunction = conjuction;
        }

        public Sentence(Chain[] subjects, PredicateList predicates, Punctuation punctuation = null)
        {
            //TODO: Validate. 
            this.subjects = subjects; //only (*), o, en
            this.predicates = predicates; //only li, pi, en
            this.punctuation = punctuation ?? new Punctuation(".");
        }

        public Sentence BindSeme(Sentence question)
        {
            //Strong bind, all words match except seme
            //Weak bind, all head words match except seme.
            if (SentenceByValue.Instance.Equals(this, question))
            {
                //unless this is a question too!
                return this; //Echo! Better to return yes!
            }

            if (question.Contains(Words.seme))
            {
                //if (SentenceSemeEqual.Instance.Equals(this, question))
                //{
                //    //We have a match!
                //    //Compress & return.
                //}
            }

            //Null bind, replace seme phrase with jan ala
            //Or don't know.
            return null;
        }

        public bool Contains(Word word)
        {
            List<IContainsWord> parts = subjects.Cast<IContainsWord>().ToList();
            parts.AddRange(predicates);
            return parts.Any(x => x.Contains(word));
        }

        public bool IsTrue()
        {
            return false;
        }

        public Chain[] Subjects { get { return subjects; } }
        public PredicateList Predicates { get { return predicates; } }
        public Punctuation Punctuation { get { return punctuation; } }
        public Particle Conjunction { get { return conjunction; } }

        public Sentence EquivallencyGenerator()
        {
            return (Sentence)this.MemberwiseClone();
        }

        public IContainsWord[] Segments()
        {
            List<IContainsWord> w = new List<IContainsWord>();
            w.AddRange(Predicates);
            w.AddRange(Subjects);
            return w.ToArray();
        }

        public string ToString(string format)
        {
            return this.ToString(format, Config.CurrentDialect);
        }

        public override string ToString()
        {
            return this.ToString(null, Config.CurrentDialect);
        }

        public string ToString(string format, IFormatProvider formatProvider)
        {
            List<string> sb = new List<string>();
            if (preconditions != null)
            {
                foreach (Sentence precondition in preconditions)
                {
                    sb.AddRange(precondition.ToTokenList(format, formatProvider));
                }
                sb.AddRange(conclusion.ToTokenList(format, formatProvider));

            }
            else
            {
                //Simple sentence
                sb = ToTokenList(format, formatProvider);   
            }

            string spaceJoined = sb.SpaceJoin(format);
            spaceJoined = spaceJoined + this.punctuation.ToString();
            
            if (format != "bs")
            {
                string result = Denormalize(spaceJoined);
                return result;
            }
            else
            {
                return spaceJoined;
            }
        }

        public List<string> ToTokenList(string format, IFormatProvider formatProvider)
        {
            List<string> sb = new List<string>();

            if (LaFragment != null)
            {
                foreach (Chain chain in LaFragment)
                {
                    sb.Add("{");
                    sb.AddRange(chain.ToTokenList(format, formatProvider));
                    sb.Add(Particles.la.ToString(format, formatProvider));
                    sb.Add("}");
                }
            }
            
            //Unless it is an array, delegate to member ToString();
            sb.Add("[");
            sb.AddRange(Particles.en, subjects.Select(x => x.ToString(format, formatProvider)));
            sb.Add("]");

            sb.Add("<");
            sb.AddRange(Predicates.ToTokenList(format, formatProvider));
            sb.Add(">");

            return sb;
        }

        private string Denormalize(string value)
        {
            if (value.Contains("mi li"))
            {
                Regex r = new Regex(@"\bmi li\b");
                value = r.Replace(value, "mi");
            }

            if (value.Contains("sina li"))
            {
                Regex r = new Regex(@"\bsina li li\b");
                value = r.Replace(value, "sina");
            }

            if (value.Contains("~"))
            {
                value = value.Replace("~", ", ");
            }
            return value;
        }


        public static Sentence Parse(string value, IFormatProvider formatProvider)
        {
            if (string.IsNullOrEmpty(value))
            {
                throw new ArgumentException("value is null or zero length string");
            }
            return SentenceTypeConverter.Parse(value, formatProvider);
        }

        public static bool TryParse(string value,  IFormatProvider formatProvider, out Sentence result)
        {
            try
            {
                result = SentenceTypeConverter.Parse(value, formatProvider);
                return true;
            }
            catch (Exception)
            {
                result = null;
                return false;
            }
        }

        public bool HasPunctuation()
        {
            return Punctuation != null;
        }
        public bool HasConjunction()
        {
            return Conjunction != null;
        }
    }



}
