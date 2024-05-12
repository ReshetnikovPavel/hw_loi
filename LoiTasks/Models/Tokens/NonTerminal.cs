using System.Collections.Generic;

namespace LoiTasks.Models.Tokens
{
    public class NonTerminal : Token
    {
        public static Dictionary<string, int> NewNonTerminalsCount = new Dictionary<string, int>();

        public NonTerminal(string value = "", int num = 0) : base("NonTerminal", value, num) { }
        
        public NonTerminal GetNewNonTerminal()
        {
            if (!NewNonTerminalsCount.ContainsKey(Value)) 
                NewNonTerminalsCount[Value] = 0;
            NewNonTerminalsCount[Value]++;

            return new NonTerminal(Value, NewNonTerminalsCount[Value]);
        }
    }
}
