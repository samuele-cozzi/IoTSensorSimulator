using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace telcodatagen.Models
{
    public class Alert
    {
        public int id { get; set; }
        public DateTime date { get; set; }
        public string message { get; set; }
    }
}
