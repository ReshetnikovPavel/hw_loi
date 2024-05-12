using System.Collections.Generic;
using LoiTasks.Algorithms.GrammarTransformations;
using LoiTasks.Models;
using LoiTasks.Models.Tokens;
using NUnit.Framework;

// ReSharper disable InconsistentNaming

namespace LoiTasks.Tests
{
    [TestFixture]
    public class LeftFactorizationTests
    {
        private static readonly NonTerminal S = new NonTerminal("S");
        private static readonly NonTerminal S1 = new NonTerminal("S", 1);
        private static readonly NonTerminal S2 = new NonTerminal("S", 2);
        private static readonly NonTerminal A = new NonTerminal("A");
        private static readonly NonTerminal B = new NonTerminal("B");
        private static readonly Terminal a = new Terminal("a");
        private static readonly Terminal b = new Terminal("b");
        private static readonly Terminal c = new Terminal("c");


        [Test]
        public void BasicTest()
        {
            NonTerminal.NewNonTerminalsCount = new Dictionary<string, int>();

            var grammar = new Grammar(S);
            grammar.AddRule(new Rule(S, new List<Token> {A, B, c}));
            grammar.AddRule(new Rule(S, new List<Token> {A, B, a}));
            grammar.AddRule(new Rule(S, new List<Token> {A, b}));
            grammar.AddRule(new Rule(A, new List<Token> {a}));
            grammar.AddRule(new Rule(B, new List<Token> {b}));

            var expectedGrammar = new Grammar(S);
            expectedGrammar.AddRule(new Rule(S, new List<Token> {A, S2}));
            expectedGrammar.AddRule(new Rule(S2, new List<Token> {b}));
            expectedGrammar.AddRule(new Rule(S2, new List<Token> {B, S1}));
            expectedGrammar.AddRule(new Rule(S1, new List<Token> {c}));
            expectedGrammar.AddRule(new Rule(S1, new List<Token> {a}));
            expectedGrammar.AddRule(new Rule(A, new List<Token> {a}));
            expectedGrammar.AddRule(new Rule(B, new List<Token> {b}));

            var actualGrammar = LeftFactorization.Factorize(grammar);
            Assert.AreEqual(expectedGrammar, actualGrammar);
        }
    }
}