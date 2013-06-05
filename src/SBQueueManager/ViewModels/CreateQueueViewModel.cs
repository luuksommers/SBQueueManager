﻿using System;
using System.Collections.ObjectModel;
using System.Linq;
using Caliburn.Micro;
using SBQueueManager.Manager;

namespace SBQueueManager.ViewModels
{
    public class CreateQueueViewModel : PropertyChangedBase
    {
        private readonly QueueManager _manager;
        private string _path;

        public CreateQueueViewModel(QueueManager manager)
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

        public void Save()
        {
            _manager.CreateQueue(Path, Users);
        }
    }
}