namespace ZerochSharp.Models.ExtensionLanguage
{
    class ConstantsAtomic : Atomic
    {
        private string constantName;
        public string ConstantName => constantName;
        public ConstantsAtomic(string name)
        {
            constantName = AtomicString = name;
        }
    }
}
