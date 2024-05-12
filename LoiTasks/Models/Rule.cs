using System.Collections.Generic;
using System.Linq;
using System.Text;
using LoiTasks.Models.Tokens;

namespace LoiTasks.Models
{
    public class Rule
    {
        public NonTerminal SourceToken { get; }

        public List<Token> RuleTokens { get; }

        public int Point { get; private set; }

        public HashSet<Terminal> LookaheadTerminals;

        public Rule(NonTerminal sourceToken, List<Token> tokens = null, int pointPosition = 0, HashSet<Terminal> lookaheadTerminals = null)
        {
            SourceToken = sourceToken;
            RuleTokens = tokens ?? new List<Token>();
            Point = pointPosition;
            LookaheadTerminals = lookaheadTerminals ?? new HashSet<Terminal>();
        }   

        public void AddToken(Token token) => RuleTokens.Add(token);

        public void AddTokens(IEnumerable<Token> tokens) => RuleTokens.AddRange(tokens);
        
        public bool RemoveToken(Token token) => RuleTokens.Remove(token);
        
        public void RemoveTokenAt(int index) => RuleTokens.RemoveAt(index);

        public void RemoveTokens(HashSet<Token> tokens)
        {
            foreach(var token in tokens) 
                RuleTokens.Remove(token);
        }

        public void ShiftPoint()
        {
            if (Point < RuleTokens.Count) 
                Point += 1;
        }

        public void AddLookaheadTerminal(Terminal terminal) => LookaheadTerminals.Add(terminal);

        public void AddLookaheadTerminals(IEnumerable<Terminal> terminals) => LookaheadTerminals.UnionWith(terminals);

        public Token GetCurrentToken()
        {
            return RuleTokens.Count == Point 
                ? Terminal.End 
                : RuleTokens[Point];
        }

        public Token GetNextToken()
        {
            return RuleTokens.Count <= Point + 1 
                ? Terminal.End 
                : RuleTokens[Point + 1];
        }

        public Rule Copy()
        {
            return new Rule(SourceToken, RuleTokens.ToList(), Point, LookaheadTerminals.ToHashSet());
        }

        public bool ShouldMakeClosure() => GetCurrentToken().Type == "NonTerminal";

        public override string ToString() => $"{SourceToken} -> {string.Join("", RuleTokens)}";

        public string ToStringExtended()
        {
            var leftPart = new StringBuilder();
            for (var i = 0; i < RuleTokens.Count; i++)
            {
                if (Point == i)
                    leftPart.Append("*");
                leftPart.Append(RuleTokens[i].ToStringExtended());
            }
            
            return $"{SourceToken} -> {leftPart} | Lookahead terminals: {string.Join("", LookaheadTerminals)}";
        }

        public override bool Equals(object obj)
        {
            if (!(obj is Rule rule))
                return false;

            if (!RuleTokens.Count.Equals(rule.RuleTokens.Count)) 
                return false;

            for (var i = 0; i < rule.RuleTokens.Count; i++)
            {
                if (!RuleTokens[i].Equals(rule.RuleTokens[i])) 
                    return false;
            }

            return SourceToken.Equals(rule.SourceToken) &&
                Point.Equals(rule.Point) &&
                LookaheadTerminals.SetEquals(rule.LookaheadTerminals);
        }

        public override int GetHashCode() => ToStringExtended().GetHashCode();
    }
}
