using System;
using LoiTasks.Models;
using LoiTasks.Models.PushdownAutomaton;
using LoiTasks.Models.Tokens;

namespace LoiTasks.Algorithms.LLAnalysis
{
    // ReSharper disable once InconsistentNaming
    public class LL1Analyzer
    {
        private readonly Grammar grammar;
        private readonly SelectSet selectSet;
        private static readonly Token StackEndToken = new StackEndToken();

        // ReSharper disable once InconsistentNaming
        public static bool IsGrammarLL1(Grammar grammar)
        {
            var selectSet = new SelectSet(grammar);
            foreach (var source in grammar.GetAllSources())
            {
                var union = new HashSet<Terminal>();
                foreach (var alternative in grammar.GetRulesBySource(source))
                {
                    var select = selectSet.GetSelectSet(alternative);
                    if (union.Intersect(select).Count() != 0)
                    {
                        return false;
                    }
                    union.UnionWith(select);
                }
            }
            return true;
        }

        public LL1Analyzer(Grammar grammar)
        {
            this.grammar = grammar;
            selectSet = new SelectSet(grammar);
        }

        public bool IsAccepted(string word)
        {
            return GetLL1Analyzer()
                .Run(word, new List<Token>{grammar.Axiom, StackEndToken})
                .IsAcceptableInput();
        }

        // ReSharper disable once InconsistentNaming
        public PushdownAutomaton<Token> GetLL1Analyzer()
        {
            var automaton = new PushdownAutomaton<Token>();
            var terminalsInAutomaton = new HashSet<Terminal>();

            foreach (var rule in grammar.GetAllRules())
            {
                var select = selectSet.GetSelectSet(rule);

                FinishState<Token> finishState;
                if (!rule.RuleTokens[0].Equals(Terminal.Lambda) && rule.RuleTokens[0].Type == "Terminal")
                {
                    var tokens = rule.RuleTokens.Skip(1).ToList();
                    if (tokens.Count == 0)
                    {
                        tokens.Add(Terminal.Lambda);
                    }
                    finishState = FinishState<Token>.StateWithShift(tokens);
                }
                else
                {
                    finishState = FinishState<Token>.StateWithNoShift(rule.RuleTokens);
                }

                terminalsInAutomaton.UnionWith(finishState.TopStackSymbols
                        .Where(t => t.Type == "Terminal" && !t.Equals(Terminal.Lambda))
                        .Select(t => (Terminal)t));

                foreach (var terminal in select)
                {
                    var startState = new StartState<Token>(terminal, rule.SourceToken);
                    automaton.TryAddTransitions(startState, finishState);
                }
            }

            var lambdaWithShift = FinishState<Token>.StateWithShift(new List<Token>{Terminal.Lambda});
            foreach (var terminal in terminalsInAutomaton) {
                var startState = new StartState<Token>(terminal, terminal);
                automaton.TryAddTransitions(startState, lambdaWithShift);
            }

            automaton.TryAddAcceptableState(new StartState<Token>(Terminal.End, StackEndToken));

            return automaton;
        }
    }
}
