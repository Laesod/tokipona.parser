﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using BasicTypes.Exceptions;
using BasicTypes.MoreTypes;

namespace BasicTypes.Parts
{
    //To do realistic parsing, we must break the illusion about how many lexemes we have
    // jan pona != jan-pona.
    // BUT
    // tomo pi telo nasa ==> tomo mute pi telo nasa   (Dear me, it's an infix)
    // 
    public class CompoundWord
    {
        internal CompoundWord()
        {
            //For XML serialization only.
        }

        [DataMember(IsRequired = true)]
        private readonly string word;

        [DataMember(IsRequired = false,EmitDefaultValue = false)]
        private readonly Dictionary<string, Dictionary<string, string[]>> glossMap;

        public CompoundWord(string word)
        {
            if (word == null)
            {
                throw new ArgumentNullException("word", "Can't construct words with null");
            }
            if (!word.Contains("-") || word.Contains(" "))
            {
                throw new ArgumentNullException("word", "Compound words must have dashes, but no spaces");
            }
            if (word.IndexOfAny(new char[] { '.', ' ', '?', '!' }) != -1)
            {
                throw new InvalidLetterSetException("Words must not have spaces or punctuation, (other than the preposition marker ~)");
            }

            //Add semantic info
            if (Words.Dictionary.ContainsKey(word))
            {
                glossMap = Words.Dictionary[word].GlossMap;
            }
            //Validate

            this.word = word;
        }

        public CompoundWord(string word, Dictionary<string, Dictionary<string, string[]>> glossMap)
        {
            if (word == null)
            {
                throw new ArgumentNullException("word", "Can't construct words with null");
            }
            //Validate
            this.word = word;
            this.glossMap = glossMap;
        }

    }
}