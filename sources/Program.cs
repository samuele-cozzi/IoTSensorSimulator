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


namespace sensordatagen
{
    class Program
    {
        #region Fields
        static string eventHubName;
        static EventHubClient client;

        static RegistryManager registryManager;
        static string deviceId;
        static string connectionString = "{iothub connection string}";
        static string iotHubUri = "{iot hub hostname}";
        static string deviceKey = "{device key}";
        static DeviceClient deviceClient;
        static int partitionCurrent;
        #endregion
       

        static void Main(string[] args)
        {

            ServiceBusEnvironment.SystemConnectivity.Mode = Microsoft.ServiceBus.ConnectivityMode.Http;

            // Setup service bus
            string connectionString = GetServiceBusConnectionString();
            NamespaceManager namespaceManager = NamespaceManager.CreateFromConnectionString(connectionString);

            string connectionStringIoT = GetIoTConnectionString();
            eventHubName = ConfigurationManager.AppSettings["EventHubName"];
            iotHubUri = ConfigurationManager.AppSettings["IotHubUri"];

            registryManager = RegistryManager.CreateFromConnectionString(connectionStringIoT);
            AddDeviceAsync().Wait();

            partitionCurrent = 0;

            // Assumes you already have the Event Hub created.
            client = EventHubClient.Create(eventHubName);
            deviceClient = DeviceClient.Create(iotHubUri, new DeviceAuthenticationWithRegistrySymmetricKey(deviceId, deviceKey));

            // Generate the data
            GenerateData(args);

            Console.ReadKey();
        }

        private static async Task AddDeviceAsync()
        {
            deviceId = ConfigurationManager.AppSettings["DeviceId"];
            Device device;
            try
            {
                device = await registryManager.AddDeviceAsync(new Device(deviceId));
            }
            catch (DeviceAlreadyExistsException)
            {
                device = await registryManager.GetDeviceAsync(deviceId);
            }
            deviceKey = device.Authentication.SymmetricKey.PrimaryKey;
            //Console.WriteLine("Generated device key: {0}", device.Authentication.SymmetricKey.PrimaryKey);
        }

        private static string GetServiceBusConnectionString()
        {
            string connectionString = ConfigurationManager.AppSettings["Microsoft.ServiceBus.ConnectionString"];
            if (string.IsNullOrEmpty(connectionString))
            {
                Console.WriteLine("Did not find Service Bus connections string in appsettings (app.config)");
                return string.Empty;
            }
            ServiceBusConnectionStringBuilder builder = new ServiceBusConnectionStringBuilder(connectionString);
            builder.TransportType = Microsoft.ServiceBus.Messaging.TransportType.Amqp;
            return builder.ToString();
        }

        private static string GetIoTConnectionString()
        {
            string connectionString = ConfigurationManager.AppSettings["Microsoft.IoT.ConnectionString"];
            if (string.IsNullOrEmpty(connectionString))
            {
                Console.WriteLine("Did not find Service Bus connections string in appsettings (app.config)");
                return string.Empty;
            }
            return connectionString;
        }


        // Generate data
        static void GenerateData(string[] args)
        { 
            int i = 0;
            int j = 0;
            pollrecord pr;
            DateTime dt = DateTime.Now;
            double annulusApress = 5;
            double thppress = 9;
            Random coin = new Random();
            bool isbroken = false; 

            for (i=0; i < 1000; i++)
            {

                thppress = 9 + (coin.NextDouble() - 0.3);
                
                if (isbroken)
                {
                    annulusApress = thppress-0.1;
                }
                else
                {
                  annulusApress = annulusApress + coin.NextDouble() - 0.3;
                        if (thppress-annulusApress<0.5)
                        {
                            isbroken=true;
                        }
                }


                for (j=0; j<6; j++)
                {

                    switch (j)
                    {
                        case 0: //THP                   
                            pr = new pollrecord(dt, j, thppress);
                            break;

                        case 1: //AnnulusA    
                            pr = new pollrecord(dt, j, annulusApress);
                            break;
                        
                        default:
                            pr = new pollrecord(dt, j, 5*coin.NextDouble());
                            break;
                    }
                                       
                    //outputrecs(pr);
                    Console.Error.WriteLine(pr.ToString());
                }
                //dt = dt.AddMinutes(1);
                //System.Threading.Thread.Sleep(60000);
                Console.Error.WriteLine(" ");
            }
           
             

        }

        static void GenerateData1(string[] args)
        {
            int i = 0;
            int j = 0;
            pollrecord pr;
            DateTime dt = DateTime.Now;
            double annulusApress = 5;
            double thppress = 9;
            Random coin = new Random();
            bool isbroken = false;

            for (i = 0; i < 1000; i++)
            {

                thppress = 9 + (coin.NextDouble() - 0.3);

                if (isbroken)
                {
                    annulusApress = thppress - 0.1;
                }
                else
                {
                    annulusApress = annulusApress + coin.NextDouble() - 0.3;
                    if (thppress - annulusApress < 0.5)
                    {
                        isbroken = true;
                    }
                }


                for (j = 0; j < 6; j++)
                {

                    switch (j)
                    {
                        case 0: //THP                   
                            pr = new pollrecord(dt, j, thppress);
                            break;

                        case 1: //AnnulusA    
                            pr = new pollrecord(dt, j, annulusApress);
                            break;

                        default:
                            pr = new pollrecord(dt, j, 5 * coin.NextDouble());
                            break;
                    }

                    //outputrecs(pr);
                    Console.Error.WriteLine(pr.ToString());
                }
                //dt = dt.AddMinutes(1);
                //System.Threading.Thread.Sleep(60000);
                Console.Error.WriteLine(" ");
            }



        }

        static void outputrecs(pollrecord r)
        {
        
            try
            {
                List<Task> tasks = new List<Task>();
                var serializedString = JsonConvert.SerializeObject(r);
                EventData data = new EventData(Encoding.UTF8.GetBytes(serializedString));


                // Send the metric to Event Hub
                //tasks.Add(client.SendAsync(data));

                // Send the metric to IoT Hub
                r.deviceId = deviceId;
                var messageString = JsonConvert.SerializeObject(r);
                var message = new Microsoft.Azure.Devices.Client.Message(Encoding.ASCII.GetBytes(messageString));
                tasks.Add(deviceClient.SendEventAsync(message));
                



                Task.WaitAll(tasks.ToArray());
            }
            catch (Exception e)
            {
                Console.WriteLine("Error on send: " + e.Message);
            }
        }

    }
}
