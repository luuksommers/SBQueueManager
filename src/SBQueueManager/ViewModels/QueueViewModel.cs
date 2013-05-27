using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Caliburn.Micro;
using Microsoft.ServiceBus.Messaging;
using SBQueueManager.Manager;

namespace SBQueueManager.ViewModels
{
    public class QueueViewModel : PropertyChangedBase
    {
        private readonly QueueManager _manager;
        public QueueDescription Instance { get; set; }
        public ObservableCollection<ServiceBusUser> Users { get; set; }
        public AuthorizationRule SelectedAuthorization { get; set; }

        public QueueViewModel(QueueDescription queueInstance, QueueManager manager)
        {
            _manager = manager;
            Instance = queueInstance;
            Users = new ObservableCollection<ServiceBusUser>(queueInstance.Authorization.Select(a => new ServiceBusUser()
                {
                    UserName = a.ClaimValue,
                    AllowListen = a.Rights.Any(b => b == AccessRights.Listen),
                    AllowSend = a.Rights.Any(b => b == AccessRights.Send),
                }));
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
