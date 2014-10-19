﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.Serialization;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using BasicTypes.Collections;
using BasicTypes.Extensions;
using System.Collections.ObjectModel;

namespace BasicTypes
{
    [DataContract]
    [Serializable]
    public class Sentence : IContainsWord, IFormattable
    {
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

        public Sentence(Chain fragments, Chain subjects, PredicateList predicates, Punctuation punctuation = null, Particle conjuction = null)
        {
            //TODO: Validate. 
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
            return this.ToString(format, System.Globalization.CultureInfo.CurrentCulture);
        }

        public override string ToString()
        {
            return this.ToString(null, System.Globalization.CultureInfo.CurrentCulture);
        }

        public string ToString(string format, IFormatProvider formatProvider)
        {
            List<string> sb = new List<string>();

            //Unless it is an array, delegate to member ToString();
            sb.Add("[");
            sb.AddRange(Particles.en, subjects.Select(x => x.ToString(format)));
            sb.Add("]");

            sb.Add("<");
            sb.Add(Predicates.ToString(format));
            sb.Add(">");

            string spaceJoined = sb.SpaceJoin(format);
            if (this.Punctuation == null)
            {
                if (Contains(Words.seme))
                {
                    spaceJoined = spaceJoined + "?";
                }
            }
            else
            {
                spaceJoined = spaceJoined + this.punctuation.ToString();
            }
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


        public static Sentence Parse(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                throw new ArgumentException("value is null or zero length string");
            }
            return SentenceTypeConverter.Parse(value);
        }

        public static bool TryParse(string value, out Sentence result)
        {
            try
            {
                result = SentenceTypeConverter.Parse(value);
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
