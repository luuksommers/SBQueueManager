using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Linq;
using Microsoft.ServiceBus;
using Microsoft.ServiceBus.Messaging;
using NLog;

namespace SBQueueManager.Manager
{
    public class QueueManager : IQueueManager
    {
        public ObservableCollection<QueueDescription> Queues { get; set; }

        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private readonly string _connectionString;
        private readonly string _nameSpace;
        private readonly NamespaceManager _namespaceManager;
        private const string ConnectionStringAppSetting = "ServiceBus.ConnectionString";

        public QueueManager()
        {
            _connectionString = ConfigurationManager.AppSettings[ConnectionStringAppSetting];
            _namespaceManager = NamespaceManager.CreateFromConnectionString(_connectionString);
            _nameSpace = _namespaceManager.Address.AbsolutePath.TrimStart('/');

            Queues = new ObservableCollection<QueueDescription>(_namespaceManager.GetQueues());
        }

        public void CreateQueue(string name, IEnumerable<ServiceBusDomainUser> users)
        {
            if (_namespaceManager.QueueExists(name))
            {
                throw new QueueException("Queue {0} already exists", name);
            }

            var queue = new QueueDescription(name);

            foreach (var user in users)
            {
                queue.Authorization.Add(new AllowRule(_nameSpace, "nameidentifier",
                                                      user.UserName + "@" + Environment.GetEnvironmentVariable("USERDNSDOMAIN"),
                                                      user.AccessRights));
            }

            queue = _namespaceManager.CreateQueue(queue);

            Queues.Add(queue);
        }

        public void DeleteQueue(string name)
        {
            if (!_namespaceManager.QueueExists(name))
            {
                throw new QueueException("Queue {0} doesn't exists", name);
            }

            _namespaceManager.DeleteQueue(name);
            Queues.Remove(Queues.First(a => a.Path == name));
        }
    }

    public interface IQueueManager
    {
    }
}
