using System;
using System.Collections.ObjectModel;
using System.Linq;
using Caliburn.Micro;
using SBQueueManager.Manager;

namespace SBQueueManager.ViewModels
{
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
            }
        }

        public bool IsNameValid { get; set; }
        public string UserName { get; set; }
        public bool UserAllowListen { get; set; }
        public bool UserAllowSend { get; set; }
        public ObservableCollection<QueueUser> Users { get; set; }

        public bool CanSave()
        {
            return true;
        }

        public void AddUser()
        {
            var user = new QueueUser();

            user.UserName = UserName;
            user.AllowListen = UserAllowListen;
            user.AllowSend = UserAllowSend;

            Users.Add(user);
        }

        public void SaveQueue()
        {
            _manager.CreateQueue(Path, Users);
        }

        public void SaveTopic()
        {
            _manager.CreateTopic(Path, Users);
        }
    }
}