using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Microsoft.ServiceBus;
using Microsoft.ServiceBus.Messaging;
using NLog;

namespace SBQueueManager.Manager
{
    public class QueueManager
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private readonly string _connectionString;
        private readonly string _nameSpace;
        private readonly NamespaceManager _namespaceManager;
        public ObservableCollection<QueueDescription> Queues { get; set; }

        public QueueManager(QueueConnectionStringProvider provider)
        {
            _connectionString = provider.GetConnectionString();
            _namespaceManager = NamespaceManager.CreateFromConnectionString(_connectionString);
            _nameSpace = _namespaceManager.Address.AbsolutePath.TrimStart('/');

            Queues = new ObservableCollection<QueueDescription>(_namespaceManager.GetQueues());
        }

        public void CreateQueue(string path, IEnumerable<QueueUser> users)
        {
            if (_namespaceManager.QueueExists(path))
            {
                throw new QueueException("Queue {0} already exists", path);
            }

            var queue = new QueueDescription(path);

            foreach (QueueUser user in users)
            {
                queue.Authorization.Add(new AllowRule(_nameSpace, "nameidentifier",
                                                      user.UserName + "@" +
                                                      Environment.GetEnvironmentVariable("USERDNSDOMAIN"),
                                                      user.GetAccessRights()));
            }

            queue = _namespaceManager.CreateQueue(queue);

            Queues.Add(queue);
        }

        public void DeleteQueue(string path)
        {
            if (!_namespaceManager.QueueExists(path))
            {
                throw new QueueException("Queue {0} doesn't exists", path);
            }

            _namespaceManager.DeleteQueue(path);
            Queues.Remove(Queues.First(a => a.Path == path));
        }

        public QueueWorker<T> GetQueueWorker<T>(string path)
        {
            return new QueueWorker<T>(_connectionString, path);
        }
    }
}