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
    public class FollowSetTests
    {
        private static readonly NonTerminal S = new NonTerminal("S");
        private static readonly NonTerminal A = new NonTerminal("A");
        private static readonly NonTerminal B = new NonTerminal("B");
        private static readonly Terminal a = new Terminal("a");
        private static readonly Terminal b = new Terminal("b");
        private static readonly Terminal c = new Terminal("c");


        [TestCase("S", "End")]
        [TestCase("A", "a", "b")]
        [TestCase("B", "End", "a", "b")]
        public void BasicTest1(string stringToken, params string[] expectedFollowTerminals)
        {
            NonTerminal.NewNonTerminalsCount = new Dictionary<string, int>();

            var grammar = new Grammar(S);
            grammar.AddRule(new Rule(S, new List<Token> {A, B, B}));
            grammar.AddRule(new Rule(S, new List<Token> {c}));
            grammar.AddRule(new Rule(A, new List<Token> {Terminal.Lambda}));
            grammar.AddRule(new Rule(B, new List<Token> {a}));
            grammar.AddRule(new Rule(B, new List<Token> {b}));

            var token = new NonTerminal(stringToken);

            var followSet = new FollowSet(grammar);
            var actualSet = followSet.GetFollowSet(token).ToList();

            Assert.AreEqual(expectedFollowTerminals.Length, actualSet.Count);
            foreach (var terminal in expectedFollowTerminals)
            {
                Assert.Contains(new Terminal(terminal), actualSet);
            }
        }


        [TestCase("S", "End")]
        [TestCase("A", "End", "b")]
        [TestCase("B", "End", "b")]
        public void BasicTest2(string stringToken, params string[] expectedFollowTerminals)
        {
            NonTerminal.NewNonTerminalsCount = new Dictionary<string, int>();

            var grammar = new Grammar(S);
            grammar.AddRule(new Rule(S, new List<Token> {A, B, B}));
            grammar.AddRule(new Rule(S, new List<Token> {c}));
            grammar.AddRule(new Rule(A, new List<Token> {Terminal.Lambda}));
            grammar.AddRule(new Rule(B, new List<Token> {Terminal.Lambda}));
            grammar.AddRule(new Rule(B, new List<Token> {b}));

            var token = new NonTerminal(stringToken);

            var followSet = new FollowSet(grammar);
            var actualSet = followSet.GetFollowSet(token).ToList();

            Assert.AreEqual(expectedFollowTerminals.Length, actualSet.Count);
            foreach (var terminal in expectedFollowTerminals)
            {
                Assert.Contains(new Terminal(terminal), actualSet);
            }
        }
    }
}