using System;
using System.Collections.Generic;
using LoiTasks.Models;
using LoiTasks.Models.Tokens;

namespace LoiTasks.Algorithms.GrammarTransformations
{
    public static class LeftFactorization
    {
        public static Grammar Factorize(Grammar grammar)
        {
            var oldGrammar = grammar.Copy();
            while (true)
            {
                var newGrammar = oldGrammar.Copy();
                FactorizeForSomePrefix(newGrammar);

                if (newGrammar.GetAllRules().SetEquals(oldGrammar.GetAllRules()))
                {
                    return newGrammar;
                }
                oldGrammar = newGrammar;
            }
        }

        private static void FactorizeForSomePrefix(Grammar grammar)
        {
            foreach (var groupBySource in grammar.GetAllRules().GroupBy(r => r.SourceToken))
            {
                var source = groupBySource.Key;
                foreach (var groupByFirstToken in groupBySource.GroupBy(r => r.RuleTokens[0]))
                {
                    var firstToken = groupByFirstToken.Key;
                    if (firstToken.Equals(Terminal.Lambda))
                    {
                        continue;
                    }
                    var sequences = groupByFirstToken.Select(r => r.RuleTokens).ToList();
                    if (sequences.Count == 1)
                    {
                        continue;
                    }
                    var prefix = GetLongestPrefix(sequences);
                    var sequencesWithPrefix = sequences.Where(x => StartsWith(x, prefix)).ToList();
                    foreach (var sequenceWithSomeLongestPrefix in sequencesWithPrefix)
                    {
                        grammar.RemoveRule(new Rule(source, sequenceWithSomeLongestPrefix));
                    }

                    var trimmedSequences = sequencesWithPrefix
                        .Select(s => s.Skip(prefix.Count).ToList())
                        .Select(s => s.Count == 0 ? new List<Token> { Terminal.Lambda } : s)
                        .ToList();
                    var newNonTerminal = source.GetNewNonTerminal();

                    prefix.Add(newNonTerminal);
                    grammar.AddRule(new Rule(source, prefix));
                    grammar.AddRules(trimmedSequences.Select(s => new Rule(newNonTerminal, s)).ToHashSet());
                }
            }
        }

        private static bool StartsWith(List<Token> sequence, List<Token> prefix)
        {
            if (prefix.Count > sequence.Count)
            {
                return false;
            }
            for (var i = 0; i < prefix.Count; i++)
            {
                if (prefix[i] != sequence[i])
                {
                    return false;
                }
            }
            return true;
        }

        private static List<Token> GetLongestPrefix(List<List<Token>> tokens)
        {
            var maxLongestPrefix = new List<Token>();
            foreach (var group in tokens.Where(x => x.Count != 0 && x[0] != Terminal.Lambda).GroupBy(x => x[0]))
            {
                var tokensWithSameFirstLetter = group.ToList();
                if (tokensWithSameFirstLetter.Count < 2) {
                    continue;
                }
                var longestPrefix = GetLongestPrefix(tokensWithSameFirstLetter.Select(x => x.Skip(1).ToList()).ToList());
                longestPrefix.Insert(0, group.Key);
                if (longestPrefix.Count > maxLongestPrefix.Count)
                {
                    maxLongestPrefix = longestPrefix;
                }
            }
            return maxLongestPrefix;
        }
    }
}
