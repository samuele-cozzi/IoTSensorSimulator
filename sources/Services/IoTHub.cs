using Microsoft.Azure.Devices;
using Microsoft.Azure.Devices.Client;
using Microsoft.Azure.Devices.Client.Exceptions;
using Microsoft.ServiceBus.Messaging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using telcodatagen.Models;

namespace telcodatagen.Services
{
    public class IoTHub
    {
        private string DeviceId { get; set; }
        private string DeviceKey { get; set; }

        private DeviceClient deviceClient;
        private RegistryManager registryManager;

        public IoTHub()
        {
            string connectionStringIoT = GetIoTConnectionString();
            string eventHubName = ConfigurationManager.AppSettings["EventHubName"];
            string iotHubUri = ConfigurationManager.AppSettings["IotHubUri"];

            registryManager = RegistryManager.CreateFromConnectionString(connectionStringIoT);

            AddDeviceAsync().Wait();

            deviceClient = DeviceClient.Create(iotHubUri, new DeviceAuthenticationWithRegistrySymmetricKey(DeviceId, DeviceKey));
        }

        public async Task AddDeviceAsync()
        {
            DeviceId = ConfigurationManager.AppSettings["DeviceId"];
            Device device;
            try
            {
                device = await registryManager.AddDeviceAsync(new Device(DeviceId));
            }
            catch (DeviceAlreadyExistsException)
            {
                device = await registryManager.GetDeviceAsync(DeviceId);
            }
            catch (Exception e)
            {
                device = await registryManager.GetDeviceAsync(DeviceId);
            }
            DeviceKey = device.Authentication.SymmetricKey.PrimaryKey;
            Console.WriteLine("Generated device key: {0}", device.Authentication.SymmetricKey.PrimaryKey);

            
        }


        private string GetIoTConnectionString()
        {
            string connectionString = ConfigurationManager.AppSettings["Microsoft.IoT.ConnectionString"];
            if (string.IsNullOrEmpty(connectionString))
            {
                Console.WriteLine("Did not find Service Bus connections string in appsettings (app.config)");
                return string.Empty;
            }
            return connectionString;
        }

        public void outputrecs(Sensor r)
        {

            try
            {
                List<Task> tasks = new List<Task>();
                var serializedString = JsonConvert.SerializeObject(r);
                EventData data = new EventData(Encoding.UTF8.GetBytes(serializedString));
                
                // Send the metric to IoT Hub
                r.deviceId = DeviceId;
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
