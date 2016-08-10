using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace telcodatagen.Models
{
    public class Plant
    {
        private static string[] SensorNameList = { "LBA:AI:OFDS.CDWPI10108.PV", "LBA:AI:OFDS.CDWPI10104A.PV", "LBA:AI:OFDS.CDWPI10104B.PV", "LBA:AI:OFDS.CDWHIC10101.PV", "LBA:AI:OFDS.CDWUI10103.PV", "LBA:AI:OFDS.CDWXV10101.PV" };
        public List<Sensor> SensorsList { get; set; }

        public Plant()
        {
            DateTime dt = DateTime.Now;
            SensorsList = new List<Sensor>();
            for (int i = 0; i < 6; i++)
            {
                SensorsList.Add(new Sensor()
                {
                    SensorName = SensorNameList[i],
                    FrequencyVal = 0.5,
                    HeatVal = 0.5,
                    PowerVal = 0.5
                });

                
            }
        }

        public void Next()
        {
            DateTime dt = DateTime.Now;
            foreach (var sensor in SensorsList)
            {
                sensor.Next(dt);
            }
        }
    }
}
