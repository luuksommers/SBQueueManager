using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Windows;
using Caliburn.Micro;
using Microsoft.ServiceBus;
using Microsoft.ServiceBus.Messaging;

namespace SBQueueManager
{
    public class ShellViewModel : PropertyChangedBase
    {
        private string _connectionString;
        public ObservableCollection<QueueDescription> Queues { get; set; }
        public QueueViewModel SelectedQueue { get; set; }

        private const string ConnectionStringAppSetting = "RoutingWorker.ServiceBus.ConnectionString";

        public ShellViewModel()
        {
            _connectionString = ConfigurationManager.AppSettings[ConnectionStringAppSetting];
            NamespaceManager namespaceManager = NamespaceManager.CreateFromConnectionString(_connectionString);

            Queues = new ObservableCollection<QueueDescription>(namespaceManager.GetQueues());
        }

        public void OpenQueue(QueueDescription queue)
        {
            SelectedQueue = new QueueViewModel(queue);

            NotifyOfPropertyChange(() => SelectedQueue);
        }
    }
}
