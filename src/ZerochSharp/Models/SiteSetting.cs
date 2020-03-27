using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ZerochSharp.Models
{
    public class SiteSetting
    {
        public string SiteName { get; set; }
        [Key]
        [JsonIgnore]
        public int Id { get; set; }
    }
}
