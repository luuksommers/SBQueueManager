using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using Caliburn.Micro;
using Microsoft.ServiceBus.Messaging;
using SBQueueManager.Manager;

namespace SBQueueManager.ViewModels
{
    public abstract class UsersViewModel<T> : PropertyChangedBase
    {
        private T _instance;

        public AuthorizationRule SelectedAuthorization { get; set; }
        public ObservableCollection<QueueUser> Users { get; set; }
        public string UserName { get; set; }
        public bool UserAllowListen { get; set; }
        public bool UserAllowSend { get; set; }
        public bool UserAllowManage { get; set; }

        protected UsersViewModel(T instance)
        {
            _instance = instance;
            Users = new ObservableCollection<QueueUser>(GetAuthorizationRules(_instance).Select(a => new QueueUser()
            {
                UserName = a.ClaimValue,
                AllowListen = a.Rights.Any(b => b == AccessRights.Listen),
                AllowSend = a.Rights.Any(b => b == AccessRights.Send),
                AllowManage = a.Rights.Any(b => b == AccessRights.Manage),
            }));
        }

        protected abstract AuthorizationRules GetAuthorizationRules(T instance);
        protected abstract void AddUser(T instance, QueueUser user);
        protected abstract void DeleteUser(T instance, QueueUser user);
        

        public void AddUser()
        {
            var user = new QueueUser();
            user.UserName = UserName;
            user.AllowListen = UserAllowListen;
            user.AllowSend = UserAllowSend;
            user.AllowManage = UserAllowManage;

            AddUser(_instance, user);

            Users.Add(user);
            NotifyOfPropertyChange(() => _instance);
        }

        public void Delete(QueueUser user)
        {
            try
            {
                DeleteUser(_instance, user);
                Users.Remove(user);
                NotifyOfPropertyChange(() => _instance);
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, "Delete user failed", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
