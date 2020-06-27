using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace ZerochSharp.Models.Boards.Restrictions
{
    public abstract class PlainRegexPattern
    {
        public string Pattern { get; set; }
        public bool IsRegex { get; set; }

        protected bool IsMatchRegexPattern(IEnumerable<string> target)
        {
            if (!IsRegex)
            {
                throw new InvalidOperationException();
            }

            return target.Any(line => RegexPattern.IsMatch(line));
        }

        protected abstract bool IsMatchPlainPattern(IEnumerable<string> target);

        public bool IsMatchPattern(IEnumerable<string> target)
        {
            return IsRegex ? IsMatchRegexPattern(target) : IsMatchPlainPattern(target);
        }
        protected Regex RegexPattern
        {
            get
            {
                if (!IsRegex)
                {
                    throw new InvalidOperationException();
                }

                var regexLine = Pattern;
                if (!regexLine.StartsWith('/'))
                {
                    throw new InvalidOperationException();
                }
                regexLine = Pattern.Substring(1);
                var lastSepInd = regexLine.LastIndexOf('/');
                if (lastSepInd < 0)
                {
                    throw new InvalidOperationException();
                }
                var options = regexLine.Substring(lastSepInd + 1);
                regexLine = regexLine.Substring(0, lastSepInd);
                var regex = new Regex(regexLine, CreateRegexOptions(options));
                return regex;
            }
        }
        private RegexOptions CreateRegexOptions(string options)
        {
            var opt = RegexOptions.ECMAScript;
            foreach (var item in options)
            {
                if (item == 'i')
                {
                    opt |= RegexOptions.IgnoreCase;
                }
            }
            return opt;
        }
    }
}
