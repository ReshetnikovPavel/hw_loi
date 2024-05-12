using System;
using System.Collections.Generic;
using LoiTasks.Models;
using LoiTasks.Models.Tokens;

namespace LoiTasks.Algorithms.LLAnalysis
{
    public class FirstSet
    {
        private readonly Grammar grammar;
        private readonly Dictionary<Token, HashSet<Terminal>> first = new Dictionary<Token, HashSet<Terminal>>();

        public FirstSet(Grammar grammar)
        {
            this.grammar = grammar;
            Build();
        }

        private void Build()
        {
            CreateHashSets();
            foreach (var terminal in grammar.GetAllTerminals())
            {
                Build(terminal);
            }


            Dictionary<Token, HashSet<Terminal>> clone;
            do
            {
                clone = CloneFirst();
                foreach (var nonTerminal in grammar.GetAllSources())
                {
                    Build(nonTerminal);
                }
            } while (AddedSomething(clone));
        }

        private void CreateHashSets()
        {
            foreach (var nonTerminal in grammar.GetAllSources())
            {
                first[nonTerminal] = new HashSet<Terminal>();
            }
        }

        private HashSet<Terminal> Build(Token token)
        {
            if (token.Type == "Terminal")
            {
                first[token] = new HashSet<Terminal> { (Terminal)token };
            }
            else
            {
                foreach (var rule in grammar.GetRulesBySource((NonTerminal)token))
                {
                    first[token].UnionWith(GetFirstSet(rule.RuleTokens));
                }
            }
            return first[token];
        }

        private Dictionary<Token, HashSet<Terminal>> CloneFirst()
        {
            var newFirst = new Dictionary<Token, HashSet<Terminal>>();
            foreach (var kv in first)
            {
                newFirst[kv.Key] = kv.Value.ToHashSet();
            }
            return newFirst;
        }

        private bool AddedSomething(Dictionary<Token, HashSet<Terminal>> clone)
        {
            foreach (var kv in clone)
            {
                if (!first[kv.Key].IsSubsetOf(kv.Value))
                {
                    return true;
                }
            }
            return false;
        }

        public HashSet<Terminal> GetFirstSet(List<Token> tokens)
        {
            var res = new HashSet<Terminal>();
            for (var i = 0; i < tokens.Count; i++)
            {
                res.UnionWith(first[tokens[i]].Where(t => !t.Equals(Terminal.Lambda)));
                if (!first[tokens[i]].Contains(Terminal.Lambda))
                {
                    break;
                }
                else if (i == tokens.Count - 1)
                {
                    res.Add(Terminal.Lambda);
                }
            }
            return res;
        }


        public HashSet<Terminal> GetFirstSet(Token token)
        {
            return first[token];
        }
    }
}
