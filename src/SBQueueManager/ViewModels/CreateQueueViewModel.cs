using System;
using System.Collections.ObjectModel;
using System.Linq;
using Caliburn.Micro;
using Microsoft.ServiceBus.Messaging;
using SBQueueManager.Manager;

namespace SBQueueManager.ViewModels
{
    public class CreateQueueViewModel : Screen
    {
        private string _name;
        public string Name 
        { 
            get { return _name; }
            set 
            { 
                _name = value; 
                IsNameValid = _manager.Queues.All(a => a.Path != _name);
                NotifyOfPropertyChange(() => IsNameValid);
            }
        }

        public bool IsNameValid { get; set; }
        public string UserName { get; set; }
        public bool UserAllowListen { get; set; }
        public bool UserAllowSend { get; set; }
        public ObservableCollection<ServiceBusDomainUser> Users { get; set; }

        private readonly QueueManager _manager;

        public CreateQueueViewModel(QueueManager manager)
        {
            _manager = manager;
            Users = new ObservableCollection<ServiceBusDomainUser>();
        }

        public bool CanSave()
        {
            return true;
        }

        public void AddUser()
        {
            var user = new ServiceBusDomainUser();
            user.UserName = UserName;

            if (UserAllowListen)
                user.AccessRights.Add(AccessRights.Listen);
            if (UserAllowSend)
                user.AccessRights.Add(AccessRights.Send);

            Users.Add(user);
        }

        public void Save()
        {
            _manager.CreateQueue(Name, Users);
            TryClose();
        }
    }
}
