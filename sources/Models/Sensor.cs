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
        public int AlertCountLast10 { get { return AlertList.Count(x => x.id > iteration - 10); } }
        public int AlertCountLast100 { get { return AlertList.Count(x => x.id > iteration - 100); } }
        public bool IsBroken {
            get {
                bool broken = false;
                if (AlertCountLast10 > 7 || AlertCountLast100 > 100)
                {
                    broken = true;
                }
                return broken;
            }
        }

        private int iteration = 0;
        private List<Alert> AlertList { get; set; }
        private double thresholdHeat = 0.9;
        private double thresholdPower = 0.1;
        private double thresholdFrequencyHigth = 0.9;
        private double thresholdFrequencyLow = 0.1;

        private Random random;
        private double heatStep = 0.05;
        private double powerStep = 0.05;
        private double frequencyStep = 0.05;

        public Sensor()
        {
            AlertList = new List<Alert>();
            System.Threading.Thread.Sleep(50);
            random = new Random();
        }

        public void Next(DateTime dt)
        {
            iteration++;
            PollTime = dt;
            HeatVal = this.Next(HeatVal, heatStep);
            AddAlert(HeatVal, -1, thresholdHeat);

            PowerVal = this.Next(PowerVal, powerStep);
            AddAlert(PowerVal, thresholdPower, 2);

            FrequencyVal = this.Next(FrequencyVal, frequencyStep);
            AddAlert(FrequencyVal, thresholdFrequencyLow, thresholdFrequencyHigth);
        }

        private double Next(double from, double step)
        {
            double to = from;
            if(random.NextDouble() > 0.5)
            {
                if (to <= 1 - step)
                {
                    to += step;
                }
            }
            else
            {
                if (to >= 0 + step) { 
                    to -= step;
                }
            }

            return to;
        }

        private void AddAlert(double value, double minThreshold, double maxThreshold)
        {
            minThreshold += random.NextDouble() - 0.5;
            maxThreshold += random.NextDouble() - 0.5;
            if (value < minThreshold || value > maxThreshold)
            {
                AlertList.Add(new Alert()
                {
                    id = iteration,
                    date = PollTime,
                    message = "Alert"
                });
            }
        }
    }
}
