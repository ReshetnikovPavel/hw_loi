using System.Collections.Generic;
using LoiTasks.Models.Tokens;

namespace LoiTasks.Models.PushdownAutomaton
{
    public class PushdownAutomaton<TStackAlphabet>
    {
        private readonly Dictionary<StartState<TStackAlphabet>, 
            FinishState<TStackAlphabet>> transitions;

        public Dictionary<StartState<TStackAlphabet>, 
            FinishState<TStackAlphabet>> Transitions => 
            new Dictionary<StartState<TStackAlphabet>, 
                FinishState<TStackAlphabet>>(transitions);
        
        public PushdownAutomaton()
        {
            transitions = new Dictionary<StartState<TStackAlphabet>, FinishState<TStackAlphabet>>();
        }

        public PushdownAutomaton(
            Dictionary<StartState<TStackAlphabet>, FinishState<TStackAlphabet>> transitions)
        {
            this.transitions = transitions;
        }
        
        public bool TryAddTransitions(
            StartState<TStackAlphabet> startState, FinishState<TStackAlphabet> finishState)
        {
            if (transitions.ContainsKey(startState))
            {
                return false;
            }
            transitions.Add(startState, finishState);
            return true;
        }

        public bool TryAddAcceptableState(StartState<TStackAlphabet> state)
        {
            if (transitions.ContainsKey(state))
            {
                return false;
            }
            transitions.Add(state, FinishState<TStackAlphabet>.CreateAcceptableState());
            return true;
        }
        
        public RunningPushdownAutomaton<TStackAlphabet> Run(
            string input, List<TStackAlphabet> startStackData)
        {
            var inputQueue = new Queue<Terminal>();
            foreach (var character in input)
            {
                inputQueue.Enqueue(new Terminal(character.ToString()));
            }
            inputQueue.Enqueue(Terminal.End);
            
            var stack = new Stack<TStackAlphabet>();
            for (var i = startStackData.Count - 1; i >= 0; --i)
            {
                stack.Push(startStackData[i]);
            }

            return new RunningPushdownAutomaton<TStackAlphabet>(transitions, inputQueue, stack);
        }
    }
}