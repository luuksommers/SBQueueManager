﻿using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Diagnostics;
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
        }

        public void OpenQueue(QueueDescription queue)
        {
            SelectedQueue = new QueueViewModel(queue, _manager);
            NotifyOfPropertyChange(() => SelectedQueue);
        }

        public void CreateQueue()
        {
            DialogHelper.ShowWindow(new CreateQueueViewModel(_manager));
        }

        public void OpenSettings()
        {
            
        }

        public void OpenCertificateManager()
        {
            Process.Start("mmc.exe");
        }
    }
}
