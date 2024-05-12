using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LoiTasks.Models.Tokens;

namespace LoiTasks.Models
{
    public class Grammar
    {
        private readonly Dictionary<NonTerminal, HashSet<Rule>> grammar =
            new Dictionary<NonTerminal, HashSet<Rule>>();
        
        public NonTerminal Axiom { get; private set; }

        public Grammar() {}

        public Grammar(NonTerminal axiom) => Axiom = axiom;

        public Grammar SetAxiom(NonTerminal axiom) 
        {
            Axiom = axiom;
            return this;
        }

        /// <summary>
        /// Расширяет грамматику
        /// </summary>
        public void Extend()
        {
            var previousSource = Axiom;
            var newSource = Axiom.GetNewNonTerminal();
            grammar[newSource] = new HashSet<Rule>
            { 
                new Rule(newSource, new List<Token> { previousSource }) 
            };
            Axiom = newSource; 
        }

        /// <summary>
        /// Возвращает все правила, которые выводятся из нетерминала
        /// </summary>
        /// <param name="nonTerminal">Нетерминал для которого нужно вернуть правила</param>
        /// <returns></returns>
        public HashSet<Rule> GetRulesBySource(NonTerminal nonTerminal)
        {
            if (nonTerminal == null || !grammar.ContainsKey(nonTerminal))
                return new HashSet<Rule>();
            return grammar[nonTerminal];
        }

        /// <summary>
        /// Возвращает все правила грамматики
        /// </summary>
        /// <returns></returns>
        public HashSet<Rule> GetAllRules()
        {
            var rules = new HashSet<Rule>();

            foreach (var item in grammar.Values) 
                rules.UnionWith(item);

            return rules;
        }

        /// <summary>
        /// Удаляет правило, если оно присутствует в грамматике
        /// </summary>
        /// <param name="rule"></param>
        /// <returns></returns>
        public bool RemoveRule(Rule rule) => grammar[rule.SourceToken].Remove(rule);

        public HashSet<NonTerminal> GetAllSources()
        {
            var nonTerminals = new HashSet<NonTerminal>();

            foreach (var nonTerminal in grammar.Keys) 
                nonTerminals.Add(nonTerminal);

            return nonTerminals;
        }
        
        public HashSet<Terminal> GetAllTerminals()
        {
            var terminals = new HashSet<Terminal>();

            foreach (var rule in GetAllRules())
            foreach (var token in rule.RuleTokens)
            {
                if (token is Terminal terminal)
                    terminals.Add(terminal);
            }

            return terminals;
        }

        public bool AddRule(Rule rule)
        {
            if (!grammar.ContainsKey(rule.SourceToken)) 
                grammar[rule.SourceToken] = new HashSet<Rule>();
            return grammar[rule.SourceToken].Add(rule);
        }

        public void AddRules(HashSet<Rule> rules)
        {
            foreach (var rule in rules) 
                AddRule(rule);
        }

        public Grammar Copy()
        {
            var grammarCopy = new Grammar(Axiom);

            foreach (var item in grammar) 
                grammarCopy.AddRules(item.Value.ToHashSet());

            return grammarCopy;
        }

        public override string ToString()
        {
            var str = new StringBuilder();
            str.Append("{\r\n");

            foreach (var rule in GetAllRules())
            {
                str.Append(rule);
                str.Append("\r\n");
            }

            str.Append("}");

            return str.ToString();
        }
        
        public string ToStringExtended()
        {
            var str = new StringBuilder();
            str.Append("{\r\n");
            str.Append($"Аксиома: {Axiom}\r\n");
            foreach (var rule in GetAllRules())
            {
                str.Append(rule.ToStringExtended());
                str.Append("\r\n");
            }

            str.Append("}");

            return str.ToString();
        }

        public override bool Equals(object obj)
        {
            if (!(obj is Grammar otherGrammar))
                return false;

            if (!Axiom.Equals(otherGrammar.Axiom))
                return false;

            var rules = GetAllRules();
            var otherRules = otherGrammar.GetAllRules();

            return rules.SetEquals(otherRules);
        }
        
        public override int GetHashCode() => ToStringExtended().GetHashCode();
    }
}
