﻿using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using Caliburn.Micro;
using Microsoft.ServiceBus.Messaging;
using SBQueueManager.Manager;

namespace SBQueueManager.ViewModels
{
    public class QueueViewModel : PropertyChangedBase
    {
        private readonly QueueManager _manager;
        public QueueDescription Instance { get; set; }
        public ObservableCollection<QueueUser> Users { get; set; }
        public AuthorizationRule SelectedAuthorization { get; set; }

        public QueueViewModel(QueueDescription queueInstance, QueueManager manager)
        {
            _manager = manager;
            Instance = queueInstance;
            Users = new ObservableCollection<QueueUser>(queueInstance.Authorization.Select(a => new QueueUser()
                {
                    UserName = a.ClaimValue,
                    AllowListen = a.Rights.Any(b => b == AccessRights.Listen),
                    AllowSend = a.Rights.Any(b => b == AccessRights.Send),
                }));
        }

        public void Delete()
        {
            var result = MessageBox.Show(
                string.Format("Are you sure you want to delete the queue {0}? This action cannot be undone.", Instance.Path), 
                "Delete Queue", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                _manager.DeleteQueue(Instance.Path);
            }
        }
    }
}
