using System;
using Caliburn.Micro;
using Microsoft.ServiceBus.Messaging;
using SBQueueManager.Manager;

namespace SBQueueManager.ViewModels
{
    public class QueueViewModel : PropertyChangedBase
    {
        private readonly QueueManager _manager;
        public QueueDescription Instance { get; set; }
        public AuthorizationRule SelectedAuthorization { get; set; }

        public QueueViewModel(QueueDescription queueInstance, QueueManager manager)
        {
            _manager = manager;
            Instance = queueInstance;
        }

        public void Delete()
        {
            _manager.DeleteQueue(Instance.Path);
        }

        public void AddUser()
        {
            
        }

        public void DeleteUser()
        {
            
        }
    }
}
