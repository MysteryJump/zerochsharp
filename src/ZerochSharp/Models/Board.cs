using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.IO;
using System.Linq;
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
        internal string GetLocalRule()
        {
            var path = $"{BOARD_SETTING_PATH}/{BoardKey}/localrule.txt";
            if (!File.Exists(path))
            {
                return null;
            }
            return File.ReadAllText(path);

        }
    }
}
