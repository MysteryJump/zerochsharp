using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace ZerochSharp.Models.Boards.Restrictions
{
    public class RestrictedUser : PlainRegexPattern
    {
        [Key]
        [JsonIgnore]
        public int Id { get; set; }
        public string BoardKey { get; set; }
        protected override bool IsMatchPlainPattern(IEnumerable<string> target)
        {
            var split = Pattern.Split(new[] { '.', ':' }, StringSplitOptions.None);
            foreach (var ip in target)
            {
                var splitIp = ip.Split(new[] { '.', ':' }, StringSplitOptions.None);
                var length = Math.Min(splitIp.Length, split.Length);
                var isRestricted = true;
                for (int i = 0; i < length; i++)
                {
                    if (split[i] != splitIp[i] && split[i] != "*")
                    {
                        isRestricted = false;
                    }
                }
                if (isRestricted)
                {
                    return true;
                }
            }
            return false;
        }

    }
}
