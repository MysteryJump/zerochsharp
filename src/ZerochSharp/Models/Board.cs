using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ZerochSharp.Models
{
    public class Board
    {
        internal const string BOARD_SETTING_PATH = "boards";
        [Key]
        public int Id { get; set; }
        [Required]
        public string BoardKey { get; set; }
        [Required]
        [SettingTxt("BBS_TITLE")]
        public string BoardName { get; set; }
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
        public string RestrictedUsers { get; set; }
        [JsonIgnore]
        public string ProhibitedWords { get; set; }
        internal string GetLocalRule()
        {
            var path = $"{BOARD_SETTING_PATH}/{BoardKey}/localrule.txt";
            if (!File.Exists(path))
            {
                return null;
            }
            return File.ReadAllText(path);

        }
        public bool IsRestricted(IEnumerable<string> ipAddress)
        {
            if (RestrictedUsers == null)
            {
                return false;
            }
            var lines = RestrictedUsers.Split(';', StringSplitOptions.RemoveEmptyEntries);
            foreach (var line in lines)
            {
                if (line.StartsWith("regex:"))
                {
                    if (CheckRegexPattern(ipAddress, line))
                    {
                        return true;
                    }
                }
                else
                {
                    if (CheckTextRestricted(ipAddress, line))
                    {
                        return true;
                    }
                }
            }
            return false;
        }
        public bool HasProhibitedWords(string text)
        {
            if (ProhibitedWords == null)
            {
                return false;
            }
            var patterns = ProhibitedWords.Split(';', StringSplitOptions.RemoveEmptyEntries);
            foreach (var pattern in patterns)
            {
                if (pattern.StartsWith("regex:"))
                {
                    if (CheckRegexPattern(text.Split('\n', StringSplitOptions.RemoveEmptyEntries), pattern))
                    {
                        return true;
                    }
                }
                else
                {
                    foreach (var item in text.Split('\n', StringSplitOptions.RemoveEmptyEntries))
                    {
                        if (item.Contains(pattern))
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
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
            var splited = pattern.Split(new[] { '.', ':' }, StringSplitOptions.None);
            foreach (var ip in ipAddress)
            {
                var splitedIp = ip.Split(new[] { '.', ':' }, StringSplitOptions.None);
                var length = Math.Min(splitedIp.Length, splited.Length);
                var isRestricted = true;
                for (int i = 0; i < length; i++)
                {
                    if (splited[i] == "*")
                    {
                        continue;
                    }
                    else if (splited[i] != splitedIp[i])
                    {
                        isRestricted = false;
                        continue;
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
