using System;
using System.Threading;
using Microsoft.ServiceBus.Messaging;
using NLog;

namespace SBQueueManager.Manager
{
    public class TopicWorker<T>
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        public event MessageReceiveDelegate<T> MessageRecieved;

        private readonly AutoResetEvent _isStopped = new AutoResetEvent(false);
        private readonly TopicClient _myTopicClient;
        private readonly SubscriptionClient _mySubscriptionClient;
        private readonly string _connectionString;
        private readonly string _topicName;
        private Thread _readThread;

        public TopicWorker(string connectionString, string topicName)
        {
            Logger.Debug("Starting LongRunningJob");
            _connectionString = connectionString;
            _topicName = topicName;

            Logger.Debug("Using Topic {0}", _topicName);
            Logger.Debug("Using ConnectionString {0} from the AppSetting: {1}", _connectionString, _connectionString);

            Logger.Debug("Creating MessagingFactory");
            MessagingFactory messageFactory = MessagingFactory.CreateFromConnectionString(_connectionString);
            
            Logger.Debug("Creating CreateTopicClient");
            _myTopicClient = messageFactory.CreateTopicClient(_topicName);

            Logger.Debug("Creating CreateTopicClient");
            _mySubscriptionClient = messageFactory.CreateSubscriptionClient(_topicName,_topicName);
        }

        public void Write(T obj)
        {
            Logger.Trace("Sending: {0}", obj.ToString());
            var message = new BrokeredMessage(obj);
            _myTopicClient.Send(message);
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
                BrokeredMessage receivedMessage = _mySubscriptionClient.Receive(TimeSpan.FromSeconds(1));
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
}