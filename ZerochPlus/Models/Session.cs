using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ZerochPlus.Models
{
    public class Session
    {
        public string SessionToken { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime Expired { get; set; }
        [Key]
        public long Id { get; set; }
        
    }
}
