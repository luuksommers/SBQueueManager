using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Windows;
using Caliburn.Micro;
using Microsoft.ServiceBus.Messaging;
using SBQueueManager.Manager;
using SBQueueManager.Properties;

namespace SBQueueManager.ViewModels
{
    public class ShellViewModel : Screen
    {
        private ServiceBusManager _manager;
        public ObservableCollection<QueueDescription> Queues { get; set; }
        public ObservableCollection<TopicDescription> Topics { get; set; }

        // Dynamic content object
        public object ContentViewModel { get; set; }

        public bool CanAddNew
        {
            get { return _manager != null; }
        }

        public ShellViewModel()
        {
            DisplayName = "Service Bus For Windows Queue Manager";
        }

        protected override void OnViewLoaded(object view)
        {
            base.OnViewLoaded(view);
            string connectionString = Settings.Default.ConnectionString ?? System.Configuration.ConfigurationManager.AppSettings["ServiceBus.ConnectionString"];
            if (string.IsNullOrWhiteSpace(connectionString) || !SetManager(connectionString))
                OpenConnectionStringManager();
        }

        public bool SetManager(string connectionString)
        {
            try
            {
                if (_manager != null)
                {
                    Queues.CollectionChanged -= Queues_CollectionChanged;
                    Topics.CollectionChanged -= Topics_CollectionChanged;
                }

                _manager = new ServiceBusManager(connectionString);
                Queues = _manager.Queues;
                Topics = _manager.Topics;
                Queues.CollectionChanged += Queues_CollectionChanged;
                Topics.CollectionChanged += Topics_CollectionChanged;
                NotifyOfPropertyChange(() => CanAddNew);
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, "Exception occured", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
            return true;
        }

        void Queues_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add)
                OpenEntity((QueueDescription)e.NewItems[0]);
            else
                OpenEntity(null);
        }

        void Topics_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add)
                OpenEntity((TopicDescription)e.NewItems[0]);
            else
                OpenEntity(null);
        }

        public void OpenEntity(object entity)
        {
            if (entity == null)
                ContentViewModel = null;
            else if (entity is QueueDescription)
                ContentViewModel = new QueueViewModel((QueueDescription)entity, _manager);
            else if (entity is TopicDescription)
                ContentViewModel = new TopicViewModel((TopicDescription)entity, _manager);
            NotifyOfPropertyChange(() => ContentViewModel);
        }

        public void AddNew()
        {
            ContentViewModel = new CreateEntityViewModel(_manager);
            NotifyOfPropertyChange(() => ContentViewModel);
        }

        public void OpenConnectionStringManager()
        {
            ContentViewModel = new ConnectionStringViewModel(this);
            NotifyOfPropertyChange(() => ContentViewModel);
        }

        public void OpenHelp()
        {
            ContentViewModel = new HelpViewModel();
            NotifyOfPropertyChange(() => ContentViewModel);
        }

        public void OpenCertificateManager()
        {
            Process.Start("mmc.exe");
        }
    }
}
