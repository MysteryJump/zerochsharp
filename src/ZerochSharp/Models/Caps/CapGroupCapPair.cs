using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ZerochSharp.Models.Caps
{
    public class CapGroupCapPair
    {
        [Key]
        public int Id { get; set; }
        public int CapGroupId { get; set; }
        public int CapId { get; set; }
    }
}
