using System;
using System.Collections.Generic;
using LoiTasks.Models;
using LoiTasks.Models.Tokens;

namespace LoiTasks.Algorithms.LLAnalysis
{
    public class FollowSet
    {
        private readonly Dictionary<NonTerminal, HashSet<Terminal>> follow = new Dictionary<NonTerminal, HashSet<Terminal>>();
        private readonly Grammar grammar;
        private readonly FirstSet firstSet;

        public FollowSet(Grammar grammar)
        {
            this.grammar = grammar;
            this.firstSet = new FirstSet(grammar);
            Build();
        }

        private void CreateHashSets()
        {
            foreach (var nonTerminal in grammar.GetAllSources())
            {
                follow[nonTerminal] = new HashSet<Terminal>();
            }
            follow[grammar.Axiom].Add(Terminal.End);
        }

        private Dictionary<NonTerminal, HashSet<Terminal>> CloneFollow()
        {
            var newFollow = new Dictionary<NonTerminal, HashSet<Terminal>>();
            foreach (var kv in follow)
            {
                newFollow[kv.Key] = kv.Value.ToHashSet();
            }
            return newFollow;
        }

        private bool AddedSomething(Dictionary<NonTerminal, HashSet<Terminal>> clone)
        {
            foreach (var kv in clone)
            {
                if (!follow[kv.Key].IsSubsetOf(kv.Value))
                {
                    return true;
                }
            }
            return false;
        }

        private void Build()
        {
            CreateHashSets();

            Dictionary<NonTerminal, HashSet<Terminal>> clone;
            do
            {
                clone = CloneFollow();
                foreach (var rule in grammar.GetAllRules())
                {
                    var last = rule.RuleTokens[rule.RuleTokens.Count - 1];
                    if (last.Type == "NonTerminal")
                    {
                        follow[(NonTerminal)last].UnionWith(follow[rule.SourceToken]);
                    }


                    for (var i = rule.RuleTokens.Count - 1; i > 0; i--)
                    {
                        if (rule.RuleTokens[i - 1].Type == "Terminal")
                        {
                            continue;
                        }
                        if (rule.RuleTokens[i].Type == "Terminal")
                        {
                            follow[(NonTerminal)rule.RuleTokens[i - 1]].Add((Terminal)rule.RuleTokens[i]);
                            continue;
                        }
                        var first = firstSet.GetFirstSet(rule.RuleTokens[i]);
                        follow[(NonTerminal)rule.RuleTokens[i - 1]].UnionWith(first.Where(t => !t.Equals(Terminal.Lambda)));
                        if (first.Contains(Terminal.Lambda))
                        {
                            follow[(NonTerminal)rule.RuleTokens[i - 1]].UnionWith(follow[(NonTerminal)rule.RuleTokens[i]]);
                        }
                    }
                }
            } while (AddedSomething(clone));
        }

        public HashSet<Terminal> GetFollowSet(NonTerminal token)
        {
            return follow[token];
        }
    }
}
