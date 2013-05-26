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
            Queues = new ObservableCollection<QueueDescription>(_manager.Queues);
        }

        public void OpenQueue(QueueDescription queue)
        {
            SelectedQueue = new QueueViewModel(queue);
            NotifyOfPropertyChange(() => SelectedQueue);
        }

        public void CreateQueue()
        {
            DialogHelper.ShowDialog<CreateQueueViewModel>();
        }
    }
}
