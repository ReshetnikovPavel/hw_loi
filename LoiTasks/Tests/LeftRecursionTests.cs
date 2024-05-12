using System.Collections.Generic;
using LoiTasks.Algorithms.GrammarTransformations;
using LoiTasks.Models;
using LoiTasks.Models.Tokens;
using NUnit.Framework;

// ReSharper disable InconsistentNaming

namespace LoiTasks.Tests
{
    [TestFixture]
    public class LeftRecursionTests
    {
        private static readonly NonTerminal S = new NonTerminal("S");
        private static readonly NonTerminal S1 = new NonTerminal("S", 1);
        private static readonly NonTerminal A = new NonTerminal("A");
        private static readonly NonTerminal B = new NonTerminal("B");
        private static readonly NonTerminal B1 = new NonTerminal("B", 1);
        private static readonly Terminal a = new Terminal("a");
        private static readonly Terminal b = new Terminal("b");
        private static readonly Terminal c = new Terminal("c");


        [Test]
        public void BasicTest1()
        {
            NonTerminal.NewNonTerminalsCount = new Dictionary<string, int>();

            var grammar = new Grammar(S);
            grammar.AddRule(new Rule(S, new List<Token> {S, a}));
            grammar.AddRule(new Rule(S, new List<Token> {S, b}));
            grammar.AddRule(new Rule(S, new List<Token> {c}));
            
            var expectedGrammar = new Grammar(S);
            expectedGrammar.AddRule(new Rule(S, new List<Token> {c, S1}));
            expectedGrammar.AddRule(new Rule(S1, new List<Token> {a, S1}));
            expectedGrammar.AddRule(new Rule(S1, new List<Token> {b, S1}));
            expectedGrammar.AddRule(new Rule(S1, new List<Token> {Terminal.Lambda}));

            var actualGrammar = RemoveLeftRecursion.RemoveRecursion(grammar);
            Assert.AreEqual(expectedGrammar, actualGrammar);
        }


        [Test]
        public void BasicTest2()
        {
            Console.WriteLine("TEST2");
            NonTerminal.NewNonTerminalsCount = new Dictionary<string, int>();

            var grammar = new Grammar(S);
            grammar.AddRule(new Rule(S, new List<Token> {S, a}));
            grammar.AddRule(new Rule(S, new List<Token> {S, b}));
            grammar.AddRule(new Rule(S, new List<Token> {a, c}));
            grammar.AddRule(new Rule(S, new List<Token> {b}));
            grammar.AddRule(new Rule(A, new List<Token> {S, A}));
            grammar.AddRule(new Rule(A, new List<Token> {a}));
            
            var expectedGrammar = new Grammar(S);
            expectedGrammar.AddRule(new Rule(S, new List<Token> {b, S1}));
            expectedGrammar.AddRule(new Rule(S, new List<Token> {a, c, S1}));
            expectedGrammar.AddRule(new Rule(S1, new List<Token> {a, S1}));
            expectedGrammar.AddRule(new Rule(S1, new List<Token> {b, S1}));
            expectedGrammar.AddRule(new Rule(S1, new List<Token> {Terminal.Lambda}));
            expectedGrammar.AddRule(new Rule(A, new List<Token> {b, S1, A}));
            expectedGrammar.AddRule(new Rule(A, new List<Token> {a, c, S1, A}));
            expectedGrammar.AddRule(new Rule(A, new List<Token> {a}));

            var actualGrammar = RemoveLeftRecursion.RemoveRecursion(grammar);
            Assert.AreEqual(expectedGrammar, actualGrammar);
        }

        [Test]
        public void BasicTest3()
        {
            NonTerminal.NewNonTerminalsCount = new Dictionary<string, int>();

            var grammar = new Grammar(S);
            grammar.AddRule(new Rule(S, new List<Token> {S, a}));
            grammar.AddRule(new Rule(S, new List<Token> {S, b}));
            grammar.AddRule(new Rule(S, new List<Token> {c}));
            
            var expectedGrammar = new Grammar(S);
            expectedGrammar.AddRule(new Rule(S, new List<Token> {c, S1}));
            expectedGrammar.AddRule(new Rule(S1, new List<Token> {a, S1}));
            expectedGrammar.AddRule(new Rule(S1, new List<Token> {b, S1}));
            expectedGrammar.AddRule(new Rule(S1, new List<Token> {Terminal.Lambda}));

            var actualGrammar = RemoveLeftRecursion.RemoveRecursion(grammar);
            Assert.AreEqual(expectedGrammar, actualGrammar);
        }

        [Test]
        public void BasicTest4()
        {
            NonTerminal.NewNonTerminalsCount = new Dictionary<string, int>();

            var grammar = new Grammar(S);
            grammar.AddRule(new Rule(S, new List<Token> {A, a}));
            grammar.AddRule(new Rule(S, new List<Token> {A}));
            grammar.AddRule(new Rule(A, new List<Token> {B}));
            grammar.AddRule(new Rule(A, new List<Token> {a, c}));
            grammar.AddRule(new Rule(B, new List<Token> {S, c}));
            grammar.AddRule(new Rule(B, new List<Token> {b}));
            
            var expectedGrammar = new Grammar(S);
            expectedGrammar.AddRule(new Rule(S, new List<Token> {A, a}));
            expectedGrammar.AddRule(new Rule(S, new List<Token> {A}));
            expectedGrammar.AddRule(new Rule(A, new List<Token> {B}));
            expectedGrammar.AddRule(new Rule(A, new List<Token> {a, c}));
            expectedGrammar.AddRule(new Rule(B, new List<Token> {b, B1}));
            expectedGrammar.AddRule(new Rule(B, new List<Token> {a, c, a, c, B1}));
            expectedGrammar.AddRule(new Rule(B, new List<Token> {a, c, c, B1}));
            expectedGrammar.AddRule(new Rule(B1, new List<Token> {c, B1}));
            expectedGrammar.AddRule(new Rule(B1, new List<Token> {a, c, B1}));
            expectedGrammar.AddRule(new Rule(B1, new List<Token> {Terminal.Lambda}));


            var actualGrammar = RemoveLeftRecursion.RemoveRecursion(grammar);
            Assert.AreEqual(expectedGrammar, actualGrammar);
        }
    }
}
