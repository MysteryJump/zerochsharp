using System.ComponentModel.DataAnnotations;

namespace ZerochSharp.Models.Caps
{
    public class CapGroupBoardPair
    {
        [Key]
        public int Id { get; set; }
        public int BoardId { get; set; }
        public int CapGroupId { get; set; }
    }
}
