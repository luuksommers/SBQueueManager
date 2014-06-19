using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using Caliburn.Micro;
using Microsoft.ServiceBus.Messaging;
using NLog;
using SBQueueManager.Manager;

namespace SBQueueManager.ViewModels
{
    public class QueueViewModel : PropertyChangedBase
    {
        private readonly ServiceBusManager _manager;
        public QueueDescription Instance { get; set; }
        public ObservableCollection<QueueUser> Users { get; set; }
        public AuthorizationRule SelectedAuthorization { get; set; }

        public QueueViewModel(QueueDescription queueInstance, ServiceBusManager manager)
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


        public string UserName { get; set; }
        public bool UserAllowListen { get; set; }
        public bool UserAllowSend { get; set; }

        public void AddUser()
        {
            var user = new QueueUser();
            user.UserName = UserName;
            user.AllowListen = UserAllowListen;
            user.AllowSend = UserAllowSend;

            _manager.AddUser(Instance, user);

            Users.Add(user);
            NotifyOfPropertyChange(() => Instance);
        }

        public void Delete(QueueUser user)
        {
            _manager.DeleteUser(Instance, user);
            Users.Remove(user);
            NotifyOfPropertyChange(() => Instance);
        }

        public void ReadMessage()
        {
            var result = _manager.ReadMessage(Instance);
            if(result)
                MessageBox.Show("Message has been read from queue", "Read Message", MessageBoxButton.OK, MessageBoxImage.Information);
            else
                MessageBox.Show("No message has been read from queue", "Read Message", MessageBoxButton.OK, MessageBoxImage.Warning);

            _manager.UpdateQueue(Instance);
            NotifyOfPropertyChange(() => Instance);
        }
    }
}
