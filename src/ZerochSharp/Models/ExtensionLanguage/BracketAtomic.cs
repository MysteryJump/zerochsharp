namespace ZerochSharp.Models.ExtensionLanguage
{
    class BracketAtomic : Atomic
    {
        public BracketType BracketType { get; }
        public int BracketLevel { get; }
        public BracketAtomic(BracketType type, int level = -1)
        {
            BracketType = type;
            atomicString = type.ToString();
            BracketLevel = level;
        }
    }
}
