using System;
using System.Collections.Generic;
using LoiTasks.Models;
using LoiTasks.Models.Tokens;

namespace LoiTasks.Algorithms.LLAnalysis
{
    public class SelectSet
    {
        private readonly FirstSet firstSet;
        private readonly FollowSet followSet;

        public SelectSet(Grammar grammar)
        {
            firstSet = new FirstSet(grammar);
            followSet = new FollowSet(grammar);
        }

        public HashSet<Terminal> GetSelectSet(Rule item)
        {
            var first = firstSet.GetFirstSet(item.RuleTokens);
            if (!first.Contains(Terminal.Lambda))
            {
                return first;
            }
            var follow = followSet.GetFollowSet(item.SourceToken).ToHashSet();
            follow.UnionWith(first.Where(t => !t.Equals(Terminal.Lambda)));
            return follow;
        }
    }
}
