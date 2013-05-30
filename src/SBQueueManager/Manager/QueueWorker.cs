using System;
using System.Threading;
using Microsoft.ServiceBus.Messaging;
using NLog;

namespace SBQueueManager.Manager
{
    public class QueueWorker<T>
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        public event MessageReceiveDelegate<T> MessageRecieved;

        private readonly AutoResetEvent _isStopped = new AutoResetEvent(false);
        private readonly QueueClient _myQueueClient;
        private readonly string _connectionString;
        private readonly string _queueName;
        private Thread _readThread;

        public QueueWorker(string connectionString, string queueName)
        {
            Logger.Debug("Starting LongRunningJob");
            _connectionString = connectionString;
            _queueName = queueName;

            Logger.Debug("Using Queue {0}", _queueName);
            Logger.Debug("Using ConnectionString {0} from the AppSetting: {1}", _connectionString, _connectionString);

            Logger.Debug("Creating MessagingFactory");
            MessagingFactory messageFactory = MessagingFactory.CreateFromConnectionString(_connectionString);

            Logger.Debug("Creating CreateQueueClient");
            _myQueueClient = messageFactory.CreateQueueClient(_queueName);
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
}