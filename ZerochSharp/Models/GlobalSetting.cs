using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ZerochSharp.Models
{
    public class GlobalSetting
    {
        [JsonIgnore]
        [Key]
        public  int Id { get; set; }
        public bool IsInitializedElasticsearchService { get; set; }
    }
}
