using System.Collections.Generic;
using LoiTasks.Algorithms.LLAnalysis;
using LoiTasks.Models;
using LoiTasks.Models.Tokens;
using NUnit.Framework;

// ReSharper disable InconsistentNaming

namespace LoiTasks.Tests
{
    [TestFixture]
    public class SelectSetTests
    {
        private static readonly NonTerminal S = new NonTerminal("S");
        private static readonly NonTerminal A = new NonTerminal("A");
        private static readonly NonTerminal B = new NonTerminal("B");
        private static readonly Terminal a = new Terminal("a");
        private static readonly Terminal b = new Terminal("b");
        private static readonly Terminal c = new Terminal("c");


        [Test]
        public void BasicTest1()
        {
            NonTerminal.NewNonTerminalsCount = new Dictionary<string, int>();

            var setByRule = new Dictionary<Rule, HashSet<Terminal>>();
            var grammar = new Grammar(S);
            
            var tempRule = new Rule(S, new List<Token> {A, B, B});
            grammar.AddRule(tempRule.Copy());
            setByRule.Add(tempRule.Copy(), new HashSet<Terminal>{a,b});
            
            tempRule = new Rule(S, new List<Token> {c});
            grammar.AddRule(tempRule.Copy());
            setByRule.Add(tempRule.Copy(), new HashSet<Terminal>{c});
            
            tempRule = new Rule(A, new List<Token> {Terminal.Lambda});
            grammar.AddRule(tempRule.Copy());
            setByRule.Add(tempRule.Copy(), new HashSet<Terminal>{a,b});
            
            tempRule = new Rule(B, new List<Token> {a});
            grammar.AddRule(tempRule.Copy());
            setByRule.Add(tempRule.Copy(), new HashSet<Terminal>{a});
            
            tempRule = new Rule(B, new List<Token> {b});
            grammar.AddRule(tempRule.Copy());
            setByRule.Add(tempRule.Copy(), new HashSet<Terminal>{b});
            
            var rules = grammar.GetAllRules();

            var selectSet = new SelectSet(grammar);

            foreach (var rule in rules)
            {
                var actualSet = selectSet.GetSelectSet(rule);
                var expectedSet = setByRule[rule];
                Assert.True(expectedSet.SetEquals(actualSet));
            }
        }


        [Test]
        public void BasicTest2()
        {
            NonTerminal.NewNonTerminalsCount = new Dictionary<string, int>();

            var setByRule = new Dictionary<Rule, HashSet<Terminal>>();
            var grammar = new Grammar(S);
            
            var tempRule = new Rule(S, new List<Token> {A, B, B});
            grammar.AddRule(tempRule.Copy());
            setByRule.Add(tempRule.Copy(), new HashSet<Terminal>{Terminal.End, b});
            
            tempRule = new Rule(S, new List<Token> {c});
            grammar.AddRule(tempRule.Copy());
            setByRule.Add(tempRule.Copy(), new HashSet<Terminal>{c});
            
            tempRule = new Rule(A, new List<Token> {Terminal.Lambda});
            grammar.AddRule(tempRule.Copy());
            setByRule.Add(tempRule.Copy(), new HashSet<Terminal>{Terminal.End,b});
            
            tempRule = new Rule(B, new List<Token> {Terminal.Lambda});
            grammar.AddRule(tempRule.Copy());
            setByRule.Add(tempRule.Copy(), new HashSet<Terminal>{Terminal.End,b});
            
            tempRule = new Rule(B, new List<Token> {b});
            grammar.AddRule(tempRule.Copy());
            setByRule.Add(tempRule.Copy(), new HashSet<Terminal>{b});
            
            var rules = grammar.GetAllRules();

            var selectSet = new SelectSet(grammar);

            foreach (var rule in rules)
            {
                var actualSet = selectSet.GetSelectSet(rule);
                var expectedSet = setByRule[rule];
                Assert.True(expectedSet.SetEquals(actualSet));
            }
        }
    }
}