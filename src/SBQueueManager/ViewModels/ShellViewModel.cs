using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Linq;
using Caliburn.Micro;
using Microsoft.ServiceBus.Messaging;
using SBQueueManager.Manager;

namespace SBQueueManager.ViewModels
{
    public class ShellViewModel : PropertyChangedBase
    {
        private readonly ServiceBusManager _manager;
        public ObservableCollection<QueueDescription> Queues { get; set; }
        public ObservableCollection<TopicDescription> Topics { get; set; }

        // Dynamic content object
        public object ContentViewModel { get; set; }

        public ShellViewModel(ServiceBusManager manager)
        {
            _manager = manager;
            Queues = _manager.Queues;
            Topics = _manager.Topics;
            _manager.Queues.CollectionChanged += Queues_CollectionChanged;
            _manager.Topics.CollectionChanged += Topics_CollectionChanged;
        }

        void Queues_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add)
                OpenEntity((QueueDescription) e.NewItems[0]);
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
            else if(entity is QueueDescription)
                ContentViewModel = new QueueViewModel((QueueDescription)entity, _manager);
            else if(entity is TopicDescription)
                ContentViewModel = new TopicViewModel((TopicDescription)entity, _manager);
            NotifyOfPropertyChange(() => ContentViewModel);
        }

        public void AddNew()
        {
            ContentViewModel = new CreateEntityViewModel(_manager);
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
