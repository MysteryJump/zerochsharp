using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ZerochSharp.Models
{
    public class Board
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string BoardKey { get; set; }
        [Required]
        [SettingTxt("BBS_TITLE")]
        public string BoardName { get; set; }
        [NotMapped]
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public List<Thread> Child { get; set; }
        [Required]
        [SettingTxt("BBS_NONAME_NAME")]
        public string BoardDefaultName { get; set; }

        [SettingTxt("BBS_SAMBATIME")]
        public string BoardSambaTime => 30.ToString();

    }
}
