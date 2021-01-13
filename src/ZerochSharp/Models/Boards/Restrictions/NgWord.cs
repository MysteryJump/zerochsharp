using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Newtonsoft.Json;

namespace ZerochSharp.Models.Boards.Restrictions
{
    public class NgWord : PlainRegexPattern
    {
        [Key] 
        [JsonIgnore]
        public int Id { get; set; }
        public string BoardKey { get; set; }
        protected override bool IsMatchPlainPattern(IEnumerable<string> target)
        {
            return target.Any(x => x.Contains(Pattern));
        }
    }
}
