using System.Collections.Generic;
using LoiTasks.Algorithms.GrammarTransformations;
using LoiTasks.Models;
using LoiTasks.Models.Tokens;
using NUnit.Framework;

// ReSharper disable InconsistentNaming

namespace LoiTasks.Tests
{
    [TestFixture]
    public class LambdaFreeGrammarTests
    {
        private static readonly NonTerminal S = new NonTerminal("S");
        private static readonly NonTerminal S1 = new NonTerminal("S", 1);
        private static readonly NonTerminal A = new NonTerminal("A");
        private static readonly NonTerminal B = new NonTerminal("B");
        private static readonly Terminal a = new Terminal("a");


        [Test]
        public void BasicTest1()
        {
            NonTerminal.NewNonTerminalsCount = new Dictionary<string, int>();

            var grammar = new Grammar(S);
            grammar.AddRule(new Rule(S, new List<Token> {A, B}));
            grammar.AddRule(new Rule(A, new List<Token> {Terminal.Lambda}));
            grammar.AddRule(new Rule(A, new List<Token> {a}));
            grammar.AddRule(new Rule(B, new List<Token> {Terminal.Lambda}));

            var expectedGrammar = new Grammar(S1);
            expectedGrammar.AddRule(new Rule(S1, new List<Token> {Terminal.Lambda}));
            expectedGrammar.AddRule(new Rule(S1, new List<Token> {S}));
            expectedGrammar.AddRule(new Rule(S, new List<Token> {A}));
            expectedGrammar.AddRule(new Rule(A, new List<Token> {a}));

            var actualGrammar = LambdaFreeGrammar.Build(grammar);
            Assert.AreEqual(expectedGrammar, actualGrammar);
        }


        [Test]
        public void BasicTest2()
        {
            NonTerminal.NewNonTerminalsCount = new Dictionary<string, int>();

            var grammar = new Grammar(S);
            grammar.AddRule(new Rule(S, new List<Token> {a, A, A}));
            grammar.AddRule(new Rule(A, new List<Token> {Terminal.Lambda}));
            grammar.AddRule(new Rule(A, new List<Token> {a}));

            var expectedGrammar = new Grammar(S);
            expectedGrammar.AddRule(new Rule(S, new List<Token> {a}));
            expectedGrammar.AddRule(new Rule(S, new List<Token> {a, A}));
            expectedGrammar.AddRule(new Rule(S, new List<Token> {a, A, A}));
            expectedGrammar.AddRule(new Rule(A, new List<Token> {a}));

            var actualGrammar = LambdaFreeGrammar.Build(grammar);
            Assert.AreEqual(expectedGrammar, actualGrammar);
        }
    }
}
