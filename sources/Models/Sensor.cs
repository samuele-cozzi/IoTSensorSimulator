using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace telcodatagen.Models
{
    public class Sensor
    {
        public string deviceId { get; set; }
        public DateTime PollTime { get; set; }
        public String SensorName { get; set; }
        public double HeatVal { get; set; }
        public double PowerVal { get; set; }
        public double FrequencyVal { get; set; }
        public int AlertCountLast10 { get; set; }
        public int AlertCountLast100 { get; set; }
        public bool IsBroken { get; set; }

        private List<Alert> AlertList { get; set; }

        public void Next(DateTime dt)
        {

        }
    }
}
