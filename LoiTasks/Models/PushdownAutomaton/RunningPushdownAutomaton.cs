using System.Collections.Generic;
using LoiTasks.Models.Tokens;

namespace LoiTasks.Models.PushdownAutomaton
{
    public class RunningPushdownAutomaton<TStackAlphabet>
    {
        public RunningPushdownAutomaton(
            Dictionary<StartState<TStackAlphabet>, FinishState<TStackAlphabet>> transitions,
            Queue<Terminal> input, Stack<TStackAlphabet> stack)
        {
            this.transitions = transitions;
            CurrentStack = stack;
            CurrentInput = input;
            CurrentAutomatonState = AutomatonState.Running;
        }

        public bool IsAcceptableInput()
        {
            while (CurrentAutomatonState == AutomatonState.Running)
            {
                TryNext();
            }
            return CurrentAutomatonState == AutomatonState.Accept;
        }

        public void TryNext()
        {
            if (CurrentStack.Count == 0)
            {
                CurrentAutomatonState = AutomatonState.Error;
                return;
            }
            var stackTop = CurrentStack.Pop();
            var inputTop = CurrentInput.Peek();
            var startState = new StartState<TStackAlphabet>(inputTop, stackTop);

            if (transitions.TryGetValue(startState, out var goTo))
            {
                if (goTo.IsAcceptState)
                {
                    CurrentAutomatonState = AutomatonState.Accept;
                    return;
                }
                ApplyTransition(goTo);
            }
            else
            {
                CurrentAutomatonState = AutomatonState.Error;
            }
            
        }

        private void ApplyTransition(FinishState<TStackAlphabet> goTo)
        {
            if (goTo.WithShift)
            {
                CurrentInput.Dequeue();
            }

            for (var i = goTo.TopStackSymbols.Count - 1; i >= 0; --i)
            {
                if (!Equals(goTo.TopStackSymbols[i], Terminal.Lambda))
                {
                    CurrentStack.Push(goTo.TopStackSymbols[i]);
                }
            }
        }

        public enum AutomatonState
        {
            Error, Accept, Running
        }

        private readonly Dictionary<StartState<TStackAlphabet>, 
            FinishState<TStackAlphabet>> transitions;

        public AutomatonState CurrentAutomatonState { get; private set; }

        public Stack<TStackAlphabet> CurrentStack { get; }

        public Queue<Terminal> CurrentInput { get; }
    }
}
