namespace ZerochSharp.Models.ExtensionLanguage
{
    class DigitsAtomic : Atomic
    {
        private long number;
        public long Digits => number;
        public DigitsAtomic(long number)
        {
            this.number = number;
            atomicString = number.ToString();
        }
    }
}
