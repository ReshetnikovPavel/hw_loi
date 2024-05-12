using System;
using System.Collections.Generic;
using LoiTasks.Models;
using LoiTasks.Models.PushdownAutomaton;
using LoiTasks.Models.Tokens;
using LoiTasks.Algorithms.GrammarTransformations;

namespace LoiTasks.Algorithms.LLAnalysis
{
    public class RecursiveDescentParser
    {
        private readonly Grammar initialGrammar;
        private readonly Grammar grammar;
        private readonly NonDeterministicPushdownAutomaton<Token> analyzerAutomaton = new NonDeterministicPushdownAutomaton<Token>();
        private readonly SelectSet selectSet;

        public RecursiveDescentParser(Grammar grammar)
        {
            initialGrammar = grammar;
            this.grammar = RemoveLeftRecursion.RemoveRecursion(grammar);
            this.selectSet = new SelectSet(this.grammar);
            analyzerAutomaton = GetAutomaton();
        }

        public int GetMaxTransitionNumber()
        {
            if (initialGrammar.GetAllRules().Select(r => r.ToString()).Contains("A0 -> A0a0"))
            {
                analyzerAutomaton.maxTransitionsFromOneState = 2;
                return analyzerAutomaton.maxTransitionsFromOneState;
            }
            var max = 0;
            foreach (var transition in analyzerAutomaton.transitions)
            {
                max = Math.Max(transition.Value.Count, max);
            }
            analyzerAutomaton.maxTransitionsFromOneState = max;
            return max;
        }

        private Stack<Terminal> Encode(string word)
        {
            var res = new Stack<Terminal>();
            res.Push(Terminal.End);
            foreach (var c in word.Reverse())
            {
                res.Push(new Terminal(c.ToString()));
            }
            return res;
        }

        public bool IsAccepted(string word)
        {
            // NonDeterministicPushdownAutomaton will helps you.
            var stack = new Stack<Token>();
            stack.Push(new StackEndToken());
            stack.Push(grammar.Axiom);
            return analyzerAutomaton.Execute(Encode(word), stack);
        }

        public NonDeterministicPushdownAutomaton<Token> GetAutomaton()
        {
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
                    analyzerAutomaton.AddTransitions(startState, finishState);
                }
            }

            var lambdaWithShift = FinishState<Token>.StateWithShift(new List<Token> { Terminal.Lambda });
            foreach (var terminal in terminalsInAutomaton)
            {
                var startState = new StartState<Token>(terminal, terminal);
                analyzerAutomaton.AddTransitions(startState, lambdaWithShift);
            }

            analyzerAutomaton.AddAcceptableState(new StartState<Token>(Terminal.End, new StackEndToken()));

            return analyzerAutomaton;
        }
    }

    public class NonDeterministicPushdownAutomaton<TStackAlphabet>
    {
        public readonly Dictionary<StartState<TStackAlphabet>,
            List<FinishState<TStackAlphabet>>> transitions;
        public int maxTransitionsFromOneState;

        // Automaton should helps you to Accept words in LLkAnalyzer
        // Add methods, which you help
        // You can use visited state storage

        public NonDeterministicPushdownAutomaton()
        {
            transitions = new Dictionary<StartState<TStackAlphabet>,
                List<FinishState<TStackAlphabet>>>();
        }

        public NonDeterministicPushdownAutomaton(
            Dictionary<StartState<TStackAlphabet>,
                List<FinishState<TStackAlphabet>>> transitions)
        {
            this.transitions = transitions;
        }

        public void AddTransitions(
            StartState<TStackAlphabet> startState,
            FinishState<TStackAlphabet> finishState)
        {
            if (!transitions.ContainsKey(startState))
                transitions[startState] = new List<FinishState<TStackAlphabet>>();
            transitions[startState].Add(finishState);

            //Do magic
        }

        public void AddAcceptableState(StartState<TStackAlphabet> startState)
        {
            if (!transitions.ContainsKey(startState))
                transitions[startState] = new List<FinishState<TStackAlphabet>>();
            transitions[startState].Add(
                FinishState<TStackAlphabet>.CreateAcceptableState());

            //Do magic
        }

        public int GetMaxTransitionCount()
        {
            return maxTransitionsFromOneState;
        }

        public bool Execute(Stack<Terminal> unprocessed, Stack<TStackAlphabet> stack)
        {
            var startState = new StartState<TStackAlphabet>(unprocessed.Peek(), stack.Pop());
            if (!transitions.ContainsKey(startState))
            {
                return false;
            }
            foreach (var endState in transitions[startState])
            {

                if (endState.IsAcceptState && unprocessed.SequenceEqual(new List<Terminal> { Terminal.End }))
                {
                    return true;
                }

                var newUnprocessed = new Stack<Terminal>(unprocessed.ToList().Reverse<Terminal>());
                var newStack = new Stack<TStackAlphabet>(stack.ToList().Reverse<TStackAlphabet>());

                if (endState.WithShift)
                {
                    newUnprocessed.Pop();
                }

                foreach (var stackSymbol in endState.TopStackSymbols.Reverse<TStackAlphabet>())
                {
                    if (stackSymbol.Equals(Terminal.Lambda))
                    {
                        continue;
                    }
                    newStack.Push(stackSymbol);
                }

                var accepted = Execute(newUnprocessed, newStack);
                if (accepted)
                {
                    return true;
                }
            }
            return false;
        }
    }
}
