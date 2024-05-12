using System.Collections.Generic;

namespace LoiTasks.Models.PushdownAutomaton
{
    public class FinishState<TStackAlphabet>
    {
        private FinishState()
        {
        }
        
        public static FinishState<TStackAlphabet> CreateAcceptableState()
        {
            return new FinishState<TStackAlphabet>
            {
                IsAcceptState = true
            };
        }

        public static FinishState<TStackAlphabet> StateWithShift(
            List<TStackAlphabet> topStackSymbols)
        {
            return new FinishState<TStackAlphabet>
            {
                WithShift = true, TopStackSymbols = topStackSymbols
            };
        }

        public static FinishState<TStackAlphabet> StateWithNoShift(
            List<TStackAlphabet> topStackSymbols)
        {
            return new FinishState<TStackAlphabet>
            {
                WithShift = false, TopStackSymbols = topStackSymbols
            };
        }

        public bool IsAcceptState { get; private set; }

        public bool WithShift { get; private set; }

        public List<TStackAlphabet> TopStackSymbols { get; private set; } = 
            new List<TStackAlphabet>();
        
        
    }
}