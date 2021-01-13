using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ZerochSharp.Models.Caps
{
    public class Cap
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string CapName { get; set; }
        public string CapId { get; set; }
        [Required]
        public string PasswordHash { get; set; }
        public string Description { get; set; }

        
    }
}
