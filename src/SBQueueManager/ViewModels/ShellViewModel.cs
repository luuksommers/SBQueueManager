using System;
using System.Collections.ObjectModel;
using Caliburn.Micro;
using Microsoft.ServiceBus.Messaging;
using SBQueueManager.Helpers;
using SBQueueManager.Manager;

namespace SBQueueManager.ViewModels
{
    public class ShellViewModel : PropertyChangedBase
    {
        private readonly QueueManager _manager;
        public ObservableCollection<QueueDescription> Queues { get; set; }
        public QueueViewModel SelectedQueue { get; set; }

        public ShellViewModel(QueueManager manager)
        {
            _manager = manager;
            Queues = _manager.Queues;
            _manager.Queues.CollectionChanged += (sender, args) => NotifyOfPropertyChange(() => Queues);
        }

        public void OpenQueue(QueueDescription queue)
        {
            SelectedQueue = new QueueViewModel(queue, _manager);
            NotifyOfPropertyChange(() => SelectedQueue);
        }

        public void CreateQueue()
        {
            DialogHelper.ShowDialog<CreateQueueViewModel>();
        }
    }
}
