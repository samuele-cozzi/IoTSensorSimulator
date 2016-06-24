using Microsoft.Azure.Devices;
using Microsoft.ServiceBus;
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
    public class ServiceBus
    {
        private RegistryManager registryManager;
        private string eventHubName;
        private EventHubClient client;

        public ServiceBus()
        {
            ServiceBusEnvironment.SystemConnectivity.Mode = Microsoft.ServiceBus.ConnectivityMode.Http;

            // Setup service bus
            string connectionString = GetServiceBusConnectionString();
            NamespaceManager namespaceManager = NamespaceManager.CreateFromConnectionString(connectionString);
            eventHubName = ConfigurationManager.AppSettings["EventHubName"];

            registryManager = RegistryManager.CreateFromConnectionString(connectionString);

            client = EventHubClient.Create(eventHubName);
        }

        private string GetServiceBusConnectionString()
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


        public void outputrecs(Sensor r)
        {
            try
            {
                List<Task> tasks = new List<Task>();
                var serializedString = JsonConvert.SerializeObject(r);
                EventData data = new EventData(Encoding.UTF8.GetBytes(serializedString));
                
                // Send the metric to Event Hub
                tasks.Add(client.SendAsync(data));
                
                Task.WaitAll(tasks.ToArray());
            }
            catch (Exception e)
            {
                Console.WriteLine("Error on send: " + e.Message);
            }
        }
    }
}
