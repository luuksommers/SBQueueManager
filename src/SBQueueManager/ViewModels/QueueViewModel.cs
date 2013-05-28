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
