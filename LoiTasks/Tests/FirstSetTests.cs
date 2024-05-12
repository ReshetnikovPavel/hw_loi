using System.Collections.Generic;
using System.Linq;
using LoiTasks.Algorithms.LLAnalysis;
using LoiTasks.Models;
using LoiTasks.Models.Tokens;
using NUnit.Framework;

// ReSharper disable InconsistentNaming

namespace LoiTasks.Tests
{
    [TestFixture]
    public class FirstSetTests
    {
        private static readonly NonTerminal S = new NonTerminal("S");
        private static readonly NonTerminal A = new NonTerminal("A");
        private static readonly NonTerminal B = new NonTerminal("B");
        private static readonly Terminal a = new Terminal("a");
        private static readonly Terminal b = new Terminal("b");
        private static readonly Terminal c = new Terminal("c");


        [TestCase("S", "a", "b", "c")]
        [TestCase("SA", "a", "b", "c")]
        [TestCase("A", "Lambda")]
        [TestCase("AB", "a", "b")]
        [TestCase("B", "a", "b")]
        [TestCase("BS", "a", "b")]
        public void BasicTest1(string stringTokens, params string[] expectedFirstTerminals)
        {
            NonTerminal.NewNonTerminalsCount = new Dictionary<string, int>();

            var grammar = new Grammar(S);
            grammar.AddRule(new Rule(S, new List<Token> {A, B, B}));
            grammar.AddRule(new Rule(S, new List<Token> {c}));
            grammar.AddRule(new Rule(A, new List<Token> {Terminal.Lambda}));
            grammar.AddRule(new Rule(B, new List<Token> {a}));
            grammar.AddRule(new Rule(B, new List<Token> {b}));

            var tokens = stringTokens
                .Select(token => new NonTerminal(token.ToString())).Cast<Token>().ToList();

            var firstSet = new FirstSet(grammar);
            var actualSet = firstSet.GetFirstSet(tokens).ToList();

            Assert.AreEqual(expectedFirstTerminals.Length, actualSet.Count);
            foreach (var terminal in expectedFirstTerminals)
            {
                Assert.Contains(new Terminal(terminal), actualSet);
            }
        }


        [TestCase("S", "b", "c", "Lambda")]
        [TestCase("SA", "b", "c", "Lambda")]
        [TestCase("A", "Lambda")]
        [TestCase("AB", "Lambda", "b")]
        [TestCase("B", "Lambda", "b")]
        [TestCase("BS", "b", "c", "Lambda")]
        public void BasicTest2(string stringTokens, params string[] expectedFirstTerminals)
        {
            NonTerminal.NewNonTerminalsCount = new Dictionary<string, int>();

            var grammar = new Grammar(S);
            grammar.AddRule(new Rule(S, new List<Token> {A, B, B}));
            grammar.AddRule(new Rule(S, new List<Token> {c}));
            grammar.AddRule(new Rule(A, new List<Token> {Terminal.Lambda}));
            grammar.AddRule(new Rule(B, new List<Token> {Terminal.Lambda}));
            grammar.AddRule(new Rule(B, new List<Token> {b}));

            var tokens = stringTokens
                .Select(token => new NonTerminal(token.ToString())).Cast<Token>().ToList();

            var firstSet = new FirstSet(grammar);
            var actualSet = firstSet.GetFirstSet(tokens).ToList();

            Assert.AreEqual(expectedFirstTerminals.Length, actualSet.Count);
            foreach (var terminal in expectedFirstTerminals)
            {
                Assert.Contains(new Terminal(terminal), actualSet);
            }
        }


        [TestCase("S", "a", "Lambda")]
        public void BasicTest3(string stringTokens, params string[] expectedFirstTerminals) {
            NonTerminal.NewNonTerminalsCount = new Dictionary<string, int>();

            var grammar = new Grammar(S);
            grammar.AddRule(new Rule(S, new List<Token> {S, a}));
            grammar.AddRule(new Rule(S, new List<Token> {Terminal.Lambda}));

            var tokens = stringTokens
                .Select(token => new NonTerminal(token.ToString())).Cast<Token>().ToList();

            var firstSet = new FirstSet(grammar);
            var actualSet = firstSet.GetFirstSet(tokens).ToList();

            Assert.AreEqual(expectedFirstTerminals.Length, actualSet.Count);
            foreach (var terminal in expectedFirstTerminals)
            {
                Assert.Contains(new Terminal(terminal), actualSet);
            }
        }
    }
}
