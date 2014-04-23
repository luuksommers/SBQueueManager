using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using Caliburn.Micro;
using Microsoft.ServiceBus.Messaging;
using SBQueueManager.Manager;

namespace SBQueueManager.ViewModels
{
    public class TopicViewModel : PropertyChangedBase
    {
        private readonly QueueManager _manager;
        public TopicDescription Instance { get; set; }
        public ObservableCollection<QueueUser> Users { get; set; }
        public AuthorizationRule SelectedAuthorization { get; set; }

        public TopicViewModel(TopicDescription topicInstance, QueueManager manager)
        {
            _manager = manager;
            Instance = topicInstance;
            Users = new ObservableCollection<QueueUser>(topicInstance.Authorization.Select(a => new QueueUser()
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

            _manager.UpdateTopic(Instance);
        }
    }
}
