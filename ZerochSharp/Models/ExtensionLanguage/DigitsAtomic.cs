namespace ZerochSharp.Models.ExtensionLanguage
{
    class DigitsAtomic : Atomic
    {
        private int number;
        public int Digits => number;
        public DigitsAtomic(int number)
        {
            this.number = number;
            atomicString = number.ToString();
        }
    }
}
