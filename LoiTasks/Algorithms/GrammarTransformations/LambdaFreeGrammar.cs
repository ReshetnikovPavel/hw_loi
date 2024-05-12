using System;
using System.Collections.Generic;
using System.Linq;
using LoiTasks.Models;
using LoiTasks.Models.Tokens;

namespace LoiTasks.Algorithms.GrammarTransformations
{
    public static class LambdaFreeGrammar
    {
        public static Grammar Build(Grammar grammar)
        {
            var ann = BuildAnn(grammar);

            foreach (var rule in grammar.GetAllRules())
            {
                grammar.AddRules(GetNewRules(rule, ann));
            }

            var superAnn = BuildSuperAnn(grammar, ann);
            RemoveSuperAnnRules(grammar, superAnn);

            AddNewAxiomIfNeeded(grammar, ann);
            return grammar;
        }

        private static HashSet<NonTerminal> BuildAnn(Grammar grammar)
        {
            var ann = new HashSet<NonTerminal>();

            while (true)
            {
                var oldAnn = ann.ToHashSet();
                foreach (var rule in grammar.GetAllRules())
                {
                    if (rule.RuleTokens.All(x => x.Equals(Terminal.Lambda) || ann.Contains(x)))
                    {
                        ann.Add(rule.SourceToken);
                    }
                }
                if (oldAnn.SequenceEqual(ann))
                {
                    break;
                }
            }
            return ann;
        }

        private static HashSet<NonTerminal> BuildSuperAnn(Grammar grammar, HashSet<NonTerminal> ann)
        {
            var superAnn = new HashSet<NonTerminal>();
            while (true)
            {
                var oldSuperAnn = superAnn.ToHashSet();
                foreach (var annSource in ann)
                {
                    var annRules = grammar.GetRulesBySource(annSource);
                    var superAnnRules = annRules
                        .Where(r => r.RuleTokens.All(x => x.Equals(Terminal.Lambda) || superAnn.Contains(x)))
                        .ToHashSet();
                    if (annRules.Count == superAnnRules.Count)
                    {
                        superAnn.Add(annSource);
                    }
                }
                if (oldSuperAnn.SequenceEqual(superAnn))
                {
                    break;
                }
            }
            return superAnn;
        }

        private static void AddNewAxiomIfNeeded(Grammar grammar, HashSet<NonTerminal> ann)
        {
            if (ann.Contains(grammar.Axiom))
            {
                var oldAxiom = grammar.Axiom;
                grammar.SetAxiom(grammar.Axiom.GetNewNonTerminal());
                grammar.AddRule(new Rule(grammar.Axiom, new List<Token> { oldAxiom }));
                grammar.AddRule(new Rule(grammar.Axiom, new List<Token> { Terminal.Lambda }));
            }
        }

        private static void RemoveSuperAnnRules(Grammar grammar, HashSet<NonTerminal> superAnn)
        {
            foreach (var rule in grammar.GetAllRules().ToHashSet())
            {
                if (rule.RuleTokens.Any(x => superAnn.Contains(x))
                        || rule.RuleTokens.All(x => x.Equals(Terminal.Lambda)))
                {
                    grammar.RemoveRule(rule);
                }
            }
        }

        public static HashSet<Rule> GetNewRules(Rule rule, HashSet<NonTerminal> ann)
        {
            var newTokensList = new List<List<Token>>();
            foreach (var token in rule.RuleTokens)
            {
                if (newTokensList.Count == 0)
                {
                    if (ann.Contains(token))
                    {
                        newTokensList.Add(new List<Token>());
                    }
                    newTokensList.Add(new List<Token> { token });
                }
                else if (ann.Contains(token))
                {
                    var count = newTokensList.Count;
                    for (int i = 0; i < count; i++)
                    {
                        newTokensList.Add(newTokensList[i].ToList());
                        newTokensList[i].Add(token);
                    }
                }
                else
                {
                    foreach (var newTokens in newTokensList)
                    {
                        newTokens.Add(token);
                    }
                }
            }
            return newTokensList
                .Where(l => l.Count != 0)
                .Select(t => new Rule(rule.SourceToken, t))
                .ToHashSet();
        }
    }
}

