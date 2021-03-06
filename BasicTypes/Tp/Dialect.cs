﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace BasicTypes
{
    //public sealed class Singleton
    //{
    //    private static readonly Singleton instance = new Singleton();
    //    public static Singleton Instance { get { return instance; } }
    //
    //    static Singleton() { }
    //    private Singleton() { }
    //}

    //For testing.
    [Serializable]
    public class ConfigProvider : IFormatProvider
    {
        private readonly Config dialect;

        public ConfigProvider(Config dialect)
        {
            this.dialect = dialect;
        }
        public object GetFormat(Type formatType)
        {
            //Regardless of format type
            return dialect;
        }

    }

    //Every decision that has never by completely made
    //For deployed apps.

    //This is for a machine wide config when none has been specified. 
    //Works best when you want behavior to match the Users OS on an installed windows app.
    //Otherwise, it is all pain. Have to specify dialect all the time.
    [Serializable]
    public sealed class Config 
    {
        private static readonly Dialect currentDialect = new Dialect();

        //Should only be 1 of this one. //app domain wide, default if none other provided.
        public static Dialect CurrentDialect { get { return currentDialect; } }

        //Can be others.
        public Config()
        {
        }
        static Config()
        {
            //Check AppSettings... if none...
            currentDialect = Dialect.LooseyGoosey;
            Regex.CacheSize = 500;
        }
        
    }

    [Serializable]
    public class Dialect : IFormatProvider
    {
        //Versions of toki pona
        public int UpToVersion { get; set; } //oldest (1)| mani, pan, esun... (2)| kipisi,monsuta ...(3)| ... pu (4)|
        public bool IncludeApocrypha { get; set; } //apeja, maljuno
        
        //Controversies
        public bool LiPiIsValid { get; set; } //jan li pi ma Tosi
        public bool PreferAle { get; set; } // (otherwise ali)
        public bool TemporalLon { get; set; } // lon tenpo suno ni vs tenpo suno ni la
        public bool StrictPos { get; set; } //e.g. vt must have e phrase, adj must follow head word, etc.
        public bool ColorsAreOnlyAdjectives { get; set; } //ni li laso vs ni li kule laso (black and white are explicitly nouns)
        public bool EmbeddedPrepositionalPhrasesRequirePi { get; set; } //e.g. kule pi lon luka palisa li ...
        public bool AllowMiOVerbPhrase { get; set; } //E.g. mi o moku. vs o mi moku.
        
        //Deprecated. This was a bad idea.
        //public bool ThrowOnSyntaxError { get; set; }//With human users, don't throw!

        public string CalendarType { get; set; }
        public string NumberType { get; set; } //poman, stupid, half-stupid, body
        public string WritingSystem { get; set; } //ToString to a prestige or utility script (e.g. pretty or compressed)
 
        //Glossing
        public string WriteProperNounsInThisLanguage { get; set; }
        public string TargetGloss { get; set; } //Language letter codes, defaults to tp, thread is special & means culture of current computer.
        public bool GlossWithFallBacks { get; set; } //Fallback to other POS if not found.
 
        //Strict Mode (user explicity tells parser things)
        public bool InferNumbers { get; set; } //Stupid numbers
        public bool AllowUnpunctuated { get; set; } // # numbers, ", " prep, "X" foreign, etc. Destroys one's ability to parse.
        public bool InferCompoundsPrepositionsForeignText { get; set; } //Stabilizes unit tests, as the particular dictionary can change the # of words hyphenated.

        //Grammaticalization, the community isn't this strict, but you can if you want to
        public bool ObligatoryPlural { get; set; } //e.g. jan tu li jan mute.
        public bool ObligatoryGender { get; set; } //e.g. meli li lukin e sama meli. (ona meli, etc.)

        //set to tp/en/eo/etc, e.g. ma tomo "New York" vs ma tomo Nujoku


        /// <summary>
        /// For interactive use of parser, for someone who is trying to write good toki pona.
        /// </summary>
        public static Dialect WordProcessorRules
        {
            get
            {
                return new Dialect()
                {
                    UpToVersion = 999,
                    StrictPos = false,
                    ObligatoryPlural = false,
                    ObligatoryGender = false,
                    IncludeApocrypha = true,
                    PreferAle = true,
                    TemporalLon = false,
                    AllowUnpunctuated = true,
                    EmbeddedPrepositionalPhrasesRequirePi = false,
                    ColorsAreOnlyAdjectives = false,
                    LiPiIsValid = false,
                    CalendarType = "Compact",
                    NumberType = "Body",
                    WritingSystem = "Roman",
                    TargetGloss = "tp",
                    InferCompoundsPrepositionsForeignText = false, //Make user enter their own!
                    InferNumbers =false
                };
            }
        }

        /// <summary>
        /// For reading toki pona, which potentially is a mess.
        /// </summary>
        public static Dialect LooseyGoosey
        {
            get
            {
                return new Dialect()
                {
                    UpToVersion = 999,
                    StrictPos = false,
                    ObligatoryPlural = false,
                    ObligatoryGender = false,
                    IncludeApocrypha = false,
                    PreferAle = true,
                    TemporalLon = false,
                    AllowUnpunctuated = true,
                    EmbeddedPrepositionalPhrasesRequirePi = false,
                    ColorsAreOnlyAdjectives = false,
                    LiPiIsValid = true,
                    CalendarType = "Compact",
                    NumberType = "Body",
                    WritingSystem = "Roman",
                    TargetGloss = "tp",
                    InferCompoundsPrepositionsForeignText = true,
                    InferNumbers =true
                };
            }
        }

        

        public object GetFormat(Type formatType)
        {
            //Dialect can vary by type, e.g. Word,  Particle, Chain.
            //I don't know why except maybe to return different objects 
            //more specialized to a given type. (e.g. number or date)
            return this;
        }
    }
}
