﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using BasicTypes.Exceptions;
using BasicTypes.Extensions;
using NUnit.Framework;

namespace BasicTypes.Collections
{
    /// <summary>
    /// Anything between li and e.
    /// </summary>
    /// <remarks>
    /// This class might should be implemented as two classes, since the noun-phrase and verb phrase properties are disjoint.
    /// </remarks>
    [Serializable]
    [DataContract]
    public class VerbPhrase : IContainsWord, IFormattable, IToString
    {
        //Modals, pre-verbs, serial verbs
        //jan li wile tawa
        [DataMember]
        private WordSet modals;

        //jan li tawa
        [DataMember]
        private Word headVerb;

        //jan li tawa mute.
        [DataMember]
        private WordSet adverbs; //TODO: extend to include pi chains? li moku pi mute suli.

        //jan li tawa ma Wasinton
        [DataMember]
        private ComplexChain nounComplement;

        public WordSet Modals { get { return modals; } }
        public Word HeadVerb { get { return headVerb; } }
        public WordSet Adverbs { get { return adverbs; } }
        public ComplexChain NounComplement { get { return nounComplement; } }

        public VerbPhrase(Word headVerb, WordSet modals , ComplexChain nounComplement)
        {
            if (headVerb == null)
            {
                throw new ArgumentNullException("headVerb");
            }
            if (!(headVerb.Text=="tawa" || headVerb.Text=="kama"))
            {
                throw new ArgumentException("Don't know what these are called but this sort of phrase can only have tawa or kama as the head verb.");
            }
            if (Particle.NonContentParticles.Contains(headVerb.Text))
            {
                throw new TpSyntaxException("Head verb cannot be a particle.");
            }
            if (modals != null)
            {
                foreach (Word modal in modals)
                {
                    if (modal.Text != "pi" && Particle.NonContentParticles.Contains(modal.Text))
                    {
                        throw new TpSyntaxException("Modals cannot be a particle.");
                    }
                    if (!Token.IsModal(modal))
                    {
                        throw new TpSyntaxException("Modals must be one of these: " + string.Join(",", Token.Modals) + " but got " + modal);
                    }
                }
            }

            //jan li (ken, wile) kama jan pali pi tomo pi telo nasa.
            //How to split?  jan li kama sona. => ? jan li kama wawa sona.
            //How to split?  jan li kama sona. => jan li ken kama sona. (modals are fine)
            //if (adverbs != null)
            //{
            //    foreach (Word adverb in adverbs)
            //    {
            //        if (adverb.Text != "pi" && Particle.NonContentParticles.Contains(adverb.Text))
            //        {
            //            throw new TpSyntaxException("Adverbs cannot be a particle. (maybe we have a nominal predicate here?)");
            //        }
            //    }
            //}

            this.headVerb = headVerb; //Any content word.
            this.modals = modals;
            //this.adverbs = adverbs;
            this.nounComplement = nounComplement;
        }

        //Doesn't deal with adj pi adj adj
        public VerbPhrase(Word headVerb, WordSet modals = null, WordSet adverbs = null)
        {
            if (headVerb == null)
            {
                throw new ArgumentNullException("headVerb");
            }
            if (Particle.NonContentParticles.Contains(headVerb.Text))
            {
                throw new TpSyntaxException("Head verb cannot be a particle.");
            }
            if (modals != null)
            {
                foreach (Word modal in modals)
                {
                    if (modal.Text != "pi" && Particle.NonContentParticles.Contains(modal.Text))
                    {
                        throw new TpSyntaxException("Modals cannot be a particle.");
                    }
                    if (!Token.IsModal(modal))
                    {
                        throw new TpSyntaxException("Modals must be one of these: " + string.Join(",", Token.Modals) + " but got " + modal);
                    }
                }
            }

            if (adverbs != null)
            {
                foreach (Word adverb in adverbs)
                {
                    if (adverb.Text != "pi" && Particle.NonContentParticles.Contains(adverb.Text))
                    {
                        throw new TpSyntaxException("Adverbs cannot be a particle. (maybe we have a nominal predicate here?)");
                    }
                }
            }

            this.headVerb = headVerb; //Any content word.
            this.modals = modals;
            this.adverbs = adverbs;
        }

        public VerbPhrase(ComplexChain nounComplement = null)
        {
            this.nounComplement = nounComplement;
        }


        /// <summary>
        /// Useful for glossing, evening if tp doesn't have morphological Perfective
        /// </summary>
        /// <returns></returns>
        public bool IsPerfect()
        {
            if (Modals.Contains(Words.pini))
            {
                return true;
            }
            return false;
        }

        public bool IsNegative()
        {
            if (Adverbs.Contains(Words.ala))
            {
                return true;
            }
            return false;
        }

        public bool Contains(Word word)
        {
            if (headVerb != null)
            {
                if (WordByValue.Instance.Equals(word, headVerb)) return true;
            }
            if (modals != null)
            {
                if (modals.Contains(word)) return true;
            }
            if (adverbs != null)
            {
                if (adverbs.Contains(word)) return true;
            }
            if (nounComplement != null)
            {
                if (nounComplement.Contains(word)) return true;
            }
            return false;
        }

        public string ToString(string format)
        {
            if (format == null)
            {
                format = "g";
            }
            return this.ToString(format, Config.CurrentDialect);
        }

        public override string ToString()
        {
            return this.ToString("g", Config.CurrentDialect);
        }

        public string ToString(string format, IFormatProvider formatProvider)
        {
            if (format == null)
            {
                format = "g";
            }
            List<string> sb = ToTokenList(format, formatProvider);

            return sb.SpaceJoin(format);
        }

        public List<string> ToTokenList(string format, IFormatProvider formatProvider, bool isVerbTransitive = false)
        {
            List<string> sb = new List<string>();

            if (nounComplement != null)
            {
                List<string> list = nounComplement.ToTokenList(format, formatProvider);
                if (list.Count > 0)
                {
                    sb.AddIfNeeded("\\", format);
                    sb.AddRange(list);
                    sb.AddIfNeeded("/", format);
                }
            }
            else
            {
                if (modals != null)
                {
                    List<string> list = modals.ToTokenList(format, formatProvider);
                    if (list.Count > 0)
                    {
                        if (format.StartCheck("b"))
                        {
                            sb.Add("\\");
                        }
                        sb.AddRange(list);
                        if (format.StartCheck("b"))
                        {
                            sb.Add("/");
                        }
                    }
                }

                string verbMarker = string.Empty;
                if (format.StartCheck("b"))
                {
                    verbMarker = isVerbTransitive ? "%%" : "%";
                }

                sb.Add(verbMarker + headVerb.ToString(format, formatProvider));

                if (adverbs != null)
                {
                    List<string> list = adverbs.ToTokenList(format, formatProvider);
                    if (list.Count > 0)
                    {
                        if (format.StartCheck("b"))
                        {
                            sb.Add("\\");
                        }
                        sb.AddRange(list);
                        if (format.StartCheck("b"))
                        {
                            sb.Add("/");
                        }
                    }

                }
            }

            return sb;
        }

        public string[] SupportedsStringFormats { get; private set; }

    }
}
