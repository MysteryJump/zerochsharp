using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ZerochSharp.Models
{
    public class Thread
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ThreadId { get; set; }
        [Required]
        public string Title { get; set; }
        [NotMapped]
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public List<Response> Responses { get; set; }
        [Required]
        public DateTime Created { get; set; }
        [Required]
        [ForeignKey("Board")]
        public string BoardKey { get; set; }
        [Required]
        public string Author { get; set; }
        [Required]
        public DateTime Modified { get; set; }

        [Required]
        public int ResponseCount { get; set; }

        [NotMapped]
        public double Influence => Created != DateTime.MinValue ? ResponseCount / ((DateTime.Now - Created).TotalSeconds / 86400.0) : -1;

        public long DatKey { get; set; }

        /// <summary>
        /// Initialize for write in Database.
        /// </summary>
        public void Initialize(string ip)
        {
            var time = DateTime.Now;
            Modified = Created = time;
            ResponseCount = 1;
            Author = Models.Author.GenerateAuthorId(ip, BoardKey);
            DatKey = new DateTimeOffset(time, new TimeSpan(+9, 0, 0)).ToUnixTimeSeconds();

        }
    }

    public class ClientThread
    {
        [Required]
        public ClientResponse Response { get; set; }
        [Required]
        public string Title { get; set; }
    }
}
