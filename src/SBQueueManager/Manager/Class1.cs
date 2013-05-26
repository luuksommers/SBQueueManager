using System;
using System.Collections.Generic;
using System.Configuration;
using System.Threading;
using Microsoft.ServiceBus;
using Microsoft.ServiceBus.Messaging;
using NLog;

namespace SBQueueManager.Manager
{
    public class ServiceBusDomainUser
    {
        /// <summary>
        /// Username without domain
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// Add the user rights (Read and/or write)
        /// </summary>
        public List<AccessRights> AccessRights { get; set; }

        public ServiceBusDomainUser()
        {
            AccessRights = new List<AccessRights>();
        }
    }

    public class ServiceBusBase<T>
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        public event MessageReceiveDelegate<T> MessageRecieved;

        private readonly AutoResetEvent _isStopped = new AutoResetEvent(false);
        private readonly QueueClient _myQueueClient;
        private readonly string _connectionString;
        private readonly string _queueName;
        private const string ConnectionStringAppSetting = "RoutingWorker.ServiceBus.ConnectionString";
        private Thread _readThread;

        public ServiceBusBase(string queueName)
        {
            Logger.Debug("Starting LongRunningJob");
            _connectionString = ConfigurationManager.AppSettings[ConnectionStringAppSetting];
            _queueName = queueName;

            Logger.Debug("Using Queue {0}", _queueName);
            Logger.Debug("Using ConnectionString {0} from the AppSetting: {1}", _connectionString, ConnectionStringAppSetting);

            Logger.Debug("Creating MessagingFactory");
            MessagingFactory messageFactory = MessagingFactory.CreateFromConnectionString(_connectionString);

            Logger.Debug("Creating CreateQueueClient");
            _myQueueClient = messageFactory.CreateQueueClient(_queueName);
        }

        /// <summary>
        /// Creates a queue, note that only the owner users of a queue can create one
        /// </summary>
        /// <param name="serviceBusUsers">List of read and write users for the queue</param>
        public void CreateQueue(IEnumerable<ServiceBusDomainUser> serviceBusUsers)
        {
            Logger.Debug("Creating NamespaceManager");
            NamespaceManager namespaceManager = NamespaceManager.CreateFromConnectionString(_connectionString);

            if (!namespaceManager.QueueExists(_queueName))
            {
                Logger.Debug("Creating Queue {0}", _queueName);

                var queue = new QueueDescription(_queueName);

                foreach (var user in serviceBusUsers)
                {
                    queue.Authorization.Add(new AllowRule("ServiceBusDefaultNamespace", "nameidentifier",
                                                          user.UserName + "@" + Environment.GetEnvironmentVariable("USERDNSDOMAIN"),
                                                          user.AccessRights));
                }
                namespaceManager.CreateQueue(queue);
            }
        }

        public void Write(T obj)
        {
            Logger.Trace("Sending: {0}", obj.ToString());
            var message = new BrokeredMessage(obj);
            _myQueueClient.Send(message);
        }

        public void StopReadThread()
        {
            _isStopped.Set();
        }

        public void StartReadThread()
        {
            _readThread = new Thread(ReadThread);
            _readThread.Start();
        }

        private void ReadThread()
        {
            while (!_isStopped.WaitOne(100))
            {
                BrokeredMessage receivedMessage = _myQueueClient.Receive(TimeSpan.FromSeconds(1));
                if (receivedMessage != null)
                {
                    var receivedBody = receivedMessage.GetBody<T>();
                    Logger.Debug("Message received: {0}", receivedBody);
                    OnMessageReceived(receivedBody);
                    receivedMessage.Complete();
                }
            }
        }

        private void OnMessageReceived(T receivedBody)
        {
            var handler = MessageRecieved;
            if (handler != null)
            {
                handler(this, new MessageReceiveDelegateArgs<T>(receivedBody));
            }
        }
    }

    public delegate void MessageReceiveDelegate<T>(object sender, MessageReceiveDelegateArgs<T> args);

    public class MessageReceiveDelegateArgs<T>
    {
        public MessageReceiveDelegateArgs(T receivedBody)
        {
        }
    }
}
