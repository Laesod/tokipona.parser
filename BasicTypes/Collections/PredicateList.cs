﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using BasicTypes.Extensions;

namespace BasicTypes.Collections
{
    public class PredicateList:List<TpPredicate>,IContainsWord,IFormattable
    {
        public bool Contains(Word word)
        {
            return this.Any(tpPredicate => tpPredicate.Contains(word));
        }

        public string ToString(string format, IFormatProvider formatProvider)
        {
            if (this.Count == 0) 
                return "";

            var sb = ToTokenList(format, formatProvider);
            return sb.SpaceJoin(format);
        }

        public List<string> ToTokenList(string format, IFormatProvider formatProvider)
        {
            List<string> sb = new List<string>();
            foreach (TpPredicate tpPredicate in this)
            {
                sb.Add(tpPredicate.Particle.ToString(format, formatProvider));
                if (tpPredicate.VerbPhrases != null)
                {
                    sb.AddRange(tpPredicate.VerbPhrases.ToTokenList(format, formatProvider));
                }
                if (tpPredicate.Directs != null)
                {
                    sb.Add(Particles.e.ToString());
                    sb.AddRange(tpPredicate.Directs.ToTokenList(format, formatProvider));
                }
                if (tpPredicate.Prepositionals != null)
                {
                    sb.AddRange(tpPredicate.Prepositionals.ToTokenList(format, formatProvider));
                }
            }
            return sb;
        }

        public string ToString(string format)
        {
            return this.ToString(format, Config.CurrentDialect);
        }

        public override string ToString()
        {
            return this.ToString(null, Config.CurrentDialect);
        }
    }
}
