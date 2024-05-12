namespace LoiTasks.Models.Tokens
{
    public class Terminal : Token
    {
        public Terminal(string value = "", int num = 0) : base("Terminal", value, num) { }

        public static Terminal Lambda => new Terminal("Lambda");

        public static Terminal End => new Terminal("End");
    }
}
