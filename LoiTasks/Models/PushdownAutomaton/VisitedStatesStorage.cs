using System;
using System.Collections.Generic;

namespace LoiLL.Models.PushdownAutomaton
{
    public class VisitedStatesStorage
    {
        public bool IsVisited(string stackSuffix, string inputSuffix)
        {
            var key = new Tuple<string, string>(stackSuffix, inputSuffix);
            return storage.Contains(key);
        }

        public void Visit(string stackSuffix, string inputSuffix)
        {
            var key = new Tuple<string, string>(stackSuffix, inputSuffix);
            storage.Add(key);
        }
        
        private readonly HashSet<Tuple<string, string>> storage = 
            new HashSet<Tuple<string, string>>();
    }
}