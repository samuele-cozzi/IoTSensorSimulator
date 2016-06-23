//*********************************************************
//
//    Copyright (c) Microsoft. All rights reserved.
//    This code is licensed under the Microsoft Public License.
//    THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
//    ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
//    IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
//    PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
//
//*********************************************************
using System;
using System.Collections;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace sensordatagen
{
    
    [DataContract]
    public class pollrecord
    {
        public string deviceId { get; set; }
        [DataMember]
        public DateTime PollTime { get; set; }

        [DataMember]
        public String SensorName {get; set; }

        [DataMember]
        public double SensorVal {get; set; }

        
        static string[] columns = {"PollTime","SensorName","SensorVal"};
        static string[] SensorNameList = { "LBA:AI:OFDS.CDWPI10108.PV", "LBA:AI:OFDS.CDWPI10104A.PV", "LBA:AI:OFDS.CDWPI10104B.PV", "LBA:AI:OFDS.CDWHIC10101.PV", "LBA:AI:OFDS.CDWUI10103.PV", "LBA:AI:OFDS.CDWXV10101.PV"};
        
     
         Hashtable data;

        public pollrecord(DateTime dt, int s, double state)
        {
            data = new Hashtable();
          
            data.Add("SensorName", SensorNameList[s]);
            this.SensorName = SensorNameList[s];

            data.Add("PollTime", DateTime.Now);
            this.PollTime = dt;

            data.Add("SensorVal", state);
            this.SensorVal = state;
        }


        override public String ToString()
        {
            StringBuilder sb = new StringBuilder();

            for (int i = 0; i < columns.Length; i++)
            {
                if (!data.ContainsKey(columns[i]) || data[columns[i]] == null)
                    sb.Append("");
                else
                    sb.Append(data[columns[i]]);

                if (i < columns.Length - 1)
                    sb.Append(",");
            }

            return sb.ToString();            
        }
    }
}
