using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ZerochSharp.Models
{
    public class Session
    {
        public string SessionToken { get; set; }
        [Key]
        public long Id { get; set; }
        public DateTime Created { get; set; }
        public DateTime Expired { get; set; }
        public string SettingData { get; set; }
        public Session(DateTime date, string identify) => SessionToken = HashGenerator.GenerateSHA512(date + identify);

        public Session()
        {

        }
    }
}
