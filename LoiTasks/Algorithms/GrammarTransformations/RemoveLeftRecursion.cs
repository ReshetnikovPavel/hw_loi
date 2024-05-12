using System;
using System.Collections.Generic;
using LoiTasks.Models;
using LoiTasks.Models.Tokens;

namespace LoiTasks.Algorithms.GrammarTransformations
{
    public static class RemoveLeftRecursion
    {
        public static Grammar RemoveRecursion(Grammar grammar)
        {
            var tokenToNumber = Numerate(grammar.GetAllSources().ToList());
            var numbers = tokenToNumber.OrderBy(x => x.Value).ToList();

            while (true)
            {
                var oldGrammar = grammar.Copy();

                foreach (var (source, number) in numbers)
                {
                    var rulesWithWrongNumbers = grammar.GetRulesBySource(source)
                        .Where(r => r.RuleTokens[0].Type == "NonTerminal")
                        .Where(r => number > tokenToNumber[(NonTerminal)r.RuleTokens[0]])
                        .ToList();
                    foreach (var ruleWithWrongNumbers in rulesWithWrongNumbers)
                    {
                        grammar.RemoveRule(ruleWithWrongNumbers);

                        var first = (NonTerminal)ruleWithWrongNumbers.RuleTokens[0];
                        foreach (var rule in grammar.GetRulesBySource(first).ToHashSet())
                        {
                            var tokens = rule.RuleTokens.ToList();
                            tokens.AddRange(ruleWithWrongNumbers.RuleTokens.Skip(1));
                            grammar.AddRule(new Rule(ruleWithWrongNumbers.SourceToken, tokens));
                        }
                    }
                    RemoveDirectRecursion(grammar, source);
                }

                if (grammar.GetAllRules().SetEquals(oldGrammar.GetAllRules()))
                {
                    break;
                }
            }
            return grammar;
        }

        private static Dictionary<NonTerminal, int> Numerate(List<NonTerminal> nonTerminals)
        {
            var indices = new Dictionary<NonTerminal, int>();
            for (var i = 0; i < nonTerminals.Count; i++)
            {
                indices[nonTerminals[i]] = i;
            }
            return indices;
        }

        private static void RemoveDirectRecursion(Grammar grammar, NonTerminal source)
        {
            var rulesWithDirectRecursion = grammar.GetRulesBySource(source)
                .Where(r => r.SourceToken.Equals(r.RuleTokens[0]))
                .ToList();

            if (rulesWithDirectRecursion.Count == 0)
            {
                return;
            }

            var rulesWithoutDirectRecursion = grammar.GetRulesBySource(source)
                .Where(r => !r.SourceToken.Equals(r.RuleTokens[0]))
                .ToList();

            var newNonTerminal = source.GetNewNonTerminal();

            foreach (var rule in rulesWithoutDirectRecursion)
            {
                grammar.RemoveRule(rule);
                var tokens = rule.RuleTokens.ToList();
                tokens.Add(newNonTerminal);
                grammar.AddRule(new Rule(source, tokens));
            }

            foreach (var rule in rulesWithDirectRecursion)
            {
                grammar.RemoveRule(rule);
                var tokens = rule.RuleTokens.Skip(1).ToList();
                tokens.Add(newNonTerminal);
                grammar.AddRule(new Rule(newNonTerminal, tokens));
            }

            grammar.AddRule(new Rule(newNonTerminal, new List<Token> { Terminal.Lambda }));
        }
    }
}
