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
        

        public QueueManager(QueueConnectionStringProvider provider)
        {
            _connectionString = provider.GetConnectionString();
            _namespaceManager = NamespaceManager.CreateFromConnectionString(_connectionString);
            _nameSpace = _namespaceManager.Address.AbsolutePath.TrimStart('/');

            Queues = new ObservableCollection<QueueDescription>(_namespaceManager.GetQueues());
            Topics = new ObservableCollection<TopicDescription>(_namespaceManager.GetTopics());
        }
        public ObservableCollection<QueueDescription> Queues { get; set; }
        public ObservableCollection<TopicDescription> Topics { get; set; }

        public void CreateQueue(string path, IEnumerable<QueueUser> users)
        {
            if (_namespaceManager.QueueExists(path))
            {
                throw new QueueException("Queue {0} already exists", path);
            }

            var queue = new QueueDescription(path);

            foreach (QueueUser user in users)
            {
                AddUser(queue, user);
            }

            queue = _namespaceManager.CreateQueue(queue);

            Queues.Add(queue);
        }

        public void CreateTopic(string path, IEnumerable<QueueUser> users)
        {
            if (_namespaceManager.TopicExists(path))
            {
                throw new QueueException("Topic {0} already exists", path);
            }

            

            var topic = new TopicDescription(path);
            foreach (QueueUser user in users)
            {
                AddUser(topic, user);
            }

            topic = _namespaceManager.CreateTopic(topic);

            _namespaceManager.CreateSubscription(topic.Path, path);

            Topics.Add(topic);
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

        internal void DeleteTopic(string path)
        {
            if (!_namespaceManager.TopicExists(path))
            {
                throw new QueueException("Topic {0} doesn't exists", path);
            }

            _namespaceManager.DeleteTopic(path);
            Topics.Remove(Topics.First(a => a.Path == path));
        }

        public void AddUser(QueueDescription queue, QueueUser user)
        {
            queue.Authorization.Add(new AllowRule(_nameSpace, "nameidentifier",
                                                     user.UserName + "@" +
                                                     Environment.GetEnvironmentVariable("USERDNSDOMAIN"),
                                                     user.GetAccessRights()));
        }

        public void AddUser(TopicDescription topic, QueueUser user)
        {
            topic.Authorization.Add(new AllowRule(_nameSpace, "nameidentifier",
                                                     user.UserName + "@" +
                                                     Environment.GetEnvironmentVariable("USERDNSDOMAIN"),
                                                     user.GetAccessRights()));
        }

        public void UpdateQueue(QueueDescription queue)
        {
            _namespaceManager.UpdateQueue(queue);
        }

        internal void UpdateTopic(TopicDescription topic)
        {
            _namespaceManager.UpdateTopic(topic);
        }
		
        public QueueWorker<T> GetQueueWorker<T>(string path)
        {
            return new QueueWorker<T>(_connectionString, path);
        }

        public void ReadMessage(QueueDescription instance)
        {
            Logger.Debug("Creating MessagingFactory");
            MessagingFactory messageFactory = MessagingFactory.CreateFromConnectionString(_connectionString);

            Logger.Debug("Creating CreateQueueClient");
            var myQueueClient = messageFactory.CreateQueueClient(instance.Path);
            
            var message = myQueueClient.Receive(TimeSpan.FromSeconds(5));
            if (message != null)
            {
                //var body = message.GetBody<object>();
                message.Complete();

                _namespaceManager.UpdateQueue(instance);
            }
        }
    }
}