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
        private readonly QueueManager _manager;
        public ObservableCollection<QueueDescription> Queues { get; set; }

        // Dynamic content object
        public object ContentViewModel { get; set; }

        public ShellViewModel(QueueManager manager)
        {
            _manager = manager;
            Queues = _manager.Queues;
            _manager.Queues.CollectionChanged += Queues_CollectionChanged;
        }

        void Queues_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add)
                OpenQueue((QueueDescription) e.NewItems[0]);
            else if(e.Action == NotifyCollectionChangedAction.Remove)
                OpenQueue(_manager.Queues.FirstOrDefault());
        }

        public void OpenQueue(QueueDescription queue)
        {
            if (queue == null)
                ContentViewModel = null;
            else
                ContentViewModel = new QueueViewModel(queue, _manager);
            
            NotifyOfPropertyChange(() => ContentViewModel);
        }

        public void CreateQueue()
        {
            ContentViewModel = new CreateQueueViewModel(_manager);
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
