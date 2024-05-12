using System;
using LoiTasks.Models.Tokens;

namespace LoiTasks.Models.PushdownAutomaton
{
    public class StartState<TStackAlphabet>
    {
        public StartState(Terminal inputSymbol, TStackAlphabet stackSymbol)
        {
            if (Equals(stackSymbol, Terminal.End))
            {
                throw new Exception($"Token in start state can not be {stackSymbol}");
            }
            
            this.inputSymbol = inputSymbol;
            this.stackSymbol = stackSymbol;
        }
        
        public override bool Equals(object obj)
        {
            if (this == obj)
            {
                return true;
            }

            if (!(obj is StartState<TStackAlphabet>))
            {
                return false;
            }

            var token = (StartState<TStackAlphabet>) obj;

            return Equals(inputSymbol, token.inputSymbol) && Equals(stackSymbol, token.stackSymbol);
        }

        public override int GetHashCode()
        {
            return inputSymbol.GetHashCode() * 1039 +
                   stackSymbol.GetHashCode();
        }

        public override string ToString()
        {
            return $"input: {inputSymbol}, stack: {stackSymbol}";
        }

        private readonly Terminal inputSymbol;
        private readonly TStackAlphabet stackSymbol;
    }
}