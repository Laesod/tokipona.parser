﻿using System;
using BasicTypes.Collections;
using BasicTypes.NormalizerCode;
using NUnit.Framework;

namespace BasicTypes.Parser
{
    [TestFixture]
    public class ParserUtilsChallengePhrases
    {


        [Test]
        public void ShouldBeGoodKunpapa()
        {
            const string s = "jan Kunpapa";
            Dialect dialect = Dialect.LooseyGoosey;
            Normalizer norm = new Normalizer(dialect);

            Console.WriteLine(norm.NormalizeText(s));
            TokenParserUtils pu = new TokenParserUtils();

            Word[] words = pu.ValidWords(s);


            foreach (Word word in words)
            {
                Console.WriteLine(word);
            }
        }




        //jan Oliwa 
        [Test]
        public void ShouldBeGoodProperModifier()
        {
            const string s = "jan Oliwa";
            
            Dialect dialect = Dialect.LooseyGoosey;
            Normalizer norm = new Normalizer(dialect);

            Console.WriteLine(norm.NormalizeText(s));
            TokenParserUtils pu = new TokenParserUtils();

            Word[] words = pu.ValidWords(s);


            foreach (Word word in words)
            {
                Console.WriteLine(word);
            }
        }


        //jan MaliyA
        [Test]
        public void DoubleBadProperModifer()
        {
            const string s = "jan MaliyA";

            Dialect dialect = Dialect.LooseyGoosey;
            Normalizer norm = new Normalizer(dialect);

            Console.WriteLine(norm.NormalizeText(s));
            TokenParserUtils pu = new TokenParserUtils();
            Word[] words;
            try
            {
                words = pu.ValidWords(s);
            }
            catch (Exception)
            {
                return;
            }

            foreach (Word word in words)
            {
                Console.WriteLine(word);
            }

            Assert.Fail();
        }





        [Test]
        public void CreateTpPredicateAfterSplitingEChain()
        {
            Dialect c = Dialect.LooseyGoosey;
            ParserUtils pu = new ParserUtils(c);

            const string ePhrase = "li moku e soweli suli mute";
            TpPredicate predicate = pu.ProcessPredicates(ePhrase);
            Console.WriteLine(predicate.ToString("b"));
            Assert.IsTrue(predicate.Directs != null);
        }
    }
}
