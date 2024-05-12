using System.Collections.Generic;
using LoiTasks.Algorithms.LLAnalysis;
using LoiTasks.Models;
using LoiTasks.Models.Tokens;
using NUnit.Framework;

// ReSharper disable InconsistentNaming

namespace LoiTasks.Tests
{
    [TestFixture]
    public class LL1AnalyzerTests
    {
        private static readonly NonTerminal S = new NonTerminal("S");
        private static readonly NonTerminal A = new NonTerminal("A");
        private static readonly NonTerminal B = new NonTerminal("B");
        private static readonly Terminal a = new Terminal("a");
        private static readonly Terminal b = new Terminal("b");
        private static readonly Terminal c = new Terminal("c");
        
        
        [TestCase("a", false)]
        [TestCase("aa", true)]
        [TestCase("ab", true)]
        [TestCase("aab", false)]
        [TestCase("b", false)]
        [TestCase("ba", true)]
        [TestCase("bb", true)]
        [TestCase("bab", false)]
        [TestCase("c", true)]
        [TestCase("ca", false)]
        [TestCase("", false)]
        public void BasicTest1(string word, bool expectedResult)
        {
            NonTerminal.NewNonTerminalsCount = new Dictionary<string, int>();

            var grammar = new Grammar(S);
            grammar.AddRule(new Rule(S, new List<Token> {A,B,B}));
            grammar.AddRule(new Rule(S, new List<Token> {c}));
            grammar.AddRule(new Rule(A, new List<Token> {Terminal.Lambda}));
            grammar.AddRule(new Rule(B, new List<Token> {a}));
            grammar.AddRule(new Rule(B, new List<Token> {b}));
            
            Assert.IsTrue(LL1Analyzer.IsGrammarLL1(grammar));
            var analyzer = new LL1Analyzer(grammar);
            Assert.AreEqual(expectedResult, analyzer.IsAccepted(word));
        }

        
        [Test]
        public void BasicTest2()
        {
            NonTerminal.NewNonTerminalsCount = new Dictionary<string, int>();

            var grammar = new Grammar(S);
            grammar.AddRule(new Rule(S, new List<Token> {A,B,B}));
            grammar.AddRule(new Rule(S, new List<Token> {c}));
            grammar.AddRule(new Rule(A, new List<Token> {Terminal.Lambda}));
            grammar.AddRule(new Rule(B, new List<Token> {Terminal.Lambda}));
            grammar.AddRule(new Rule(B, new List<Token> {b}));
            
            Assert.IsFalse(LL1Analyzer.IsGrammarLL1(grammar));
        }
    }
}