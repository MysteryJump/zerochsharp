using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ZerochSharp.Models.Attributes;
using ZerochSharp.Models.Boards.Restrictions;

namespace ZerochSharp.Models.Boards
{
    public class Board
    {
        internal const string BoardSettingPath = "boards";
        [Key]
        public int Id { get; set; }
        [Required]
        public string BoardKey { get; set; }
        [Required]
        [SettingTxt("BBS_TITLE")]
        public string BoardName { get; set; }
        public string BoardCategory { get; set; }
        [NotMapped]
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public List<Thread> Children { get; set; }
        [Required]
        [SettingTxt("BBS_NONAME_NAME")]
        public string BoardDefaultName { get; set; }

        [SettingTxt("BBS_SAMBATIME")]
        public string BoardSambaTime => 30.ToString();
        [SettingTxt("BBS_SUBTITLE")]
        public string BoardSubTitle { get; set; }
        [SettingTxt("BBS_DELETE_NAME")]
        public string BoardDeleteName => "あぼーん";
        [JsonIgnore]
        public string AutoRemovingPredicate { get; set; }
        [NotMapped]
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string[] AutoArchivingPredicates => AutoRemovingPredicate?.Split(';', StringSplitOptions.RemoveEmptyEntries) ?? new string[] { };
        [NotMapped]
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public bool? IsArchivedChild { get; set; }
        [NotMapped]
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public int? Page { get; set; }
        [NotMapped]
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public int? ChildrenCount { get; set; }
        [JsonIgnore]
        [NotMapped]
        public IEnumerable<NgWord> ProhibitedWordCollection { get; private set; }
        [JsonIgnore]
        [NotMapped]
        public IEnumerable<RestrictedUser> RestrictedUserCollection { get; set; }
        public async Task InitializeForValidation(MainContext context)
        {
            ProhibitedWordCollection = await context.NgWords.Where(x => x.BoardKey == BoardKey || x.BoardKey == null).ToListAsync();
            RestrictedUserCollection = await context.RestrictedUsers
                .Where(x => x.BoardKey == BoardKey || x.BoardKey == null)
                .ToListAsync();
        }

        internal string GetLocalRule()
        {
            var path = $"{BoardSettingPath}/{BoardKey}/localrule.txt";
            if (!File.Exists(path))
            {
                return null;
            }
            return File.ReadAllText(path);

        }
        public bool IsRestricted(IEnumerable<string> ipAddress)
        {
            if (RestrictedUserCollection == null)
            {
                return false;
            }

            return RestrictedUserCollection.Any(x => x.IsMatchPattern(ipAddress));
        }
        public bool HasProhibitedWords(string text)
        {
            if (ProhibitedWordCollection == null)
            {
                return false;
            }

            var splitTextLines = text.Split('\n', StringSplitOptions.RemoveEmptyEntries);
            return ProhibitedWordCollection.Any(x => x.IsMatchPattern(splitTextLines));
        }
        private bool CheckRegexPattern(IEnumerable<string> targetLines, string pattern)
        {
            var regexLine = pattern.Remove(0, "regex:".Length);
            if (!regexLine.StartsWith('/'))
            {
                return false;
            }
            regexLine = regexLine.Substring(1);
            var lastSepInd = regexLine.LastIndexOf('/');
            if (lastSepInd < 0)
            {
                return false;
            }
            var options = regexLine.Substring(lastSepInd + 1);
            regexLine = regexLine.Substring(0, lastSepInd);
            var regex = new Regex(regexLine, CreateRegexOptions(options));
            foreach (var line in targetLines)
            {
                if (regex.IsMatch(line))
                {
                    return true;
                }
            }
            return false;
        }
        private bool CheckTextRestricted(IEnumerable<string> ipAddress, string pattern)
        {
            var split = pattern.Split(new[] { '.', ':' }, StringSplitOptions.None);
            foreach (var ip in ipAddress)
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
