using System;
using System.Collections.ObjectModel;
using System.Linq;
using Caliburn.Micro;
using SBQueueManager.Manager;

namespace SBQueueManager.ViewModels
{
    public enum ServiceBusEntityType
    {
        Queue,
        Topic
    }

    public class CreateEntityViewModel : PropertyChangedBase
    {
        private readonly ServiceBusManager _manager;
        
        private string _path;

        public CreateEntityViewModel(ServiceBusManager manager)
        {
            _manager = manager;
            Users = new ObservableCollection<QueueUser>();
        }

        public string Path
        {
            get { return _path; }
            set
            {
                _path = value;
                IsNameValid = _manager.Queues.All(a => a.Path != _path);
                NotifyOfPropertyChange(() => IsNameValid);
                NotifyOfPropertyChange(() => CanSave);
            }
        }

        public ServiceBusEntityType EntityType { get; set; }
        public bool IsNameValid { get; set; }
        public string UserName { get; set; }
        public bool UserAllowListen { get; set; }
        public bool UserAllowSend { get; set; }
        public bool UserAllowManage { get; set; }
        public ObservableCollection<QueueUser> Users { get; set; }
        public bool CanSave { get { return IsNameValid; } }

        public void AddUser()
        {
            var user = new QueueUser();

            user.UserName = UserName;
            user.AllowListen = UserAllowListen;
            user.AllowSend = UserAllowSend;
            user.AllowManage = user.AllowManage;
            Users.Add(user);
        }



        public void Save()
        {
            if (EntityType == ServiceBusEntityType.Queue)
                _manager.CreateQueue(Path, Users);
            else
                _manager.CreateTopic(Path, Users);
        }
    }
}