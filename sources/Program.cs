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
using Microsoft.ServiceBus;
using Microsoft.ServiceBus.Messaging;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Text;
using System.Threading.Tasks;

using Newtonsoft.Json;
using Microsoft.Azure.Devices;
using Microsoft.Azure.Devices.Client;
using Microsoft.Azure.Devices.Common.Exceptions;
using telcodatagen.Services;
using telcodatagen.Models;

namespace sensordatagen
{
    class Program
    {
        #region Fields
        static IoTHub iot;
        #endregion
       

        static void Main(string[] args)
        {
            iot = new IoTHub();
            
            // Generate the data
            GenerateData(args);

            Console.ReadKey();
        }

        


        // Generate data
        static void GenerateData(string[] args)
        { 
            int i = 0;
            int j = 0;

            Plant plant = new Plant();

            for (i=0; i < 1000; i++)
            {
                plant.Next();
                foreach (var sensor in plant.SensorsList)
                {
                    iot.outputrecs(sensor);
                    Console.Error.WriteLine(JsonConvert.SerializeObject(sensor));
                }
                
                System.Threading.Thread.Sleep(60000);
                Console.Error.WriteLine(" ");
            }
           
             

        }

        
    }
}
