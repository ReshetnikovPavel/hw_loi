namespace LoiTasks.Models.Tokens
{
    public class Token
    {
        public string Value { get; }

        public string Type { get; }

        public readonly int Num;

        private const string DefaultValue = "default";

        protected Token(string type, string value = "", int num = 0)
        {
            Type = type;
            Value = value != "" ? value : DefaultValue;
            Num = num;
        }

        public override string ToString() => $"{Value}{Num}";

        public string ToStringExtended() => $"{Type} {Value} {Num}";

        public override bool Equals(object obj)
        {
            if (this == obj)
                return true;

            if (!(obj is Token token))
                return false;

            return Value == token.Value 
                   && Type == token.Type 
                   && Num == token.Num;
        }

        public override int GetHashCode() => ToStringExtended().GetHashCode();
    }
}
