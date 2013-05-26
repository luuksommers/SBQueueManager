using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using Microsoft.ServiceBus;
using Microsoft.ServiceBus.Messaging;
using NLog;

namespace SBQueueManager.Manager
{
    public class QueueManager : IQueueManager
    {
        public IEnumerable<QueueDescription> Queues { get; set; }

        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private readonly string _connectionString;
        private const string ConnectionStringAppSetting = "ServiceBus.ConnectionString";

        public QueueManager()
        {
            _connectionString = ConfigurationManager.AppSettings[ConnectionStringAppSetting];
            NamespaceManager namespaceManager = NamespaceManager.CreateFromConnectionString(_connectionString);

            Queues = namespaceManager.GetQueues();
        }

        public void CreateQueue(string name, IEnumerable<ServiceBusDomainUser> users)
        {
            Logger.Debug("Creating NamespaceManager");
            NamespaceManager namespaceManager = NamespaceManager.CreateFromConnectionString(_connectionString);
            var queue = new QueueDescription(name);

            foreach (var user in users)
            {
                queue.Authorization.Add(new AllowRule("ServiceBusDefaultNamespace", "nameidentifier",
                                                      user.UserName + "@" + Environment.GetEnvironmentVariable("USERDNSDOMAIN"),
                                                      user.AccessRights));
            }

            if (namespaceManager.QueueExists(queue.Path))
            {
                throw new QueueException("Queue {0} already exists", queue.Path);
            }
            namespaceManager.CreateQueue(queue);
        }
    }

    public interface IQueueManager
    {
    }
}
