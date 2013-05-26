using System;
using Caliburn.Micro;
using Microsoft.ServiceBus.Messaging;

namespace SBQueueManager.ViewModels
{
    public class QueueViewModel : PropertyChangedBase
    {
        public QueueDescription Instance { get; set; }
        public AuthorizationRule SelectedAuthorization { get; set; }

        public QueueViewModel(QueueDescription queueInstance)
        {
            Instance = queueInstance;
        }

        public void AddUser()
        {
            
        }

        public void DeleteUser()
        {
            
        }
    }
}
