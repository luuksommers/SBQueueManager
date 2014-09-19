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
        private readonly ServiceBusManager _manager;
        public TopicDescription Instance { get; set; }
        public ObservableCollection<QueueUser> Users { get; set; }
        public AuthorizationRule SelectedAuthorization { get; set; }

        public SubscriptionDescription SelectedSubscription { get; set; }
        public ObservableCollection<SubscriptionDescription> Subscriptions { get; set; }

        public TopicViewModel(TopicDescription topicInstance, ServiceBusManager manager)
        {
            _manager = manager;
            Instance = topicInstance;
            Users = new ObservableCollection<QueueUser>(topicInstance.Authorization.Select(a => new QueueUser()
                {
                    UserName = a.ClaimValue,
                    AllowListen = a.Rights.Any(b => b == AccessRights.Listen),
                    AllowSend = a.Rights.Any(b => b == AccessRights.Send),
                }));
            Subscriptions = new ObservableCollection<SubscriptionDescription>(manager.GetSubscriptions(topicInstance));
        }

        public void Update()
        {
            try
            {
                _manager.UpdateTopic(Instance);
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, "Update failed", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public void Delete()
        {
            var result = MessageBox.Show(
                string.Format("Are you sure you want to delete the topic {0}? This action cannot be undone.", Instance.Path), 
                "Delete Queue", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                _manager.DeleteTopic(Instance.Path);
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
            Users.Add(user);
            NotifyOfPropertyChange(() => Instance);
        }

        public void Delete(QueueUser user)
        {
            try
            {
                _manager.DeleteUser(Instance, user);
                Users.Remove(user);
                NotifyOfPropertyChange(() => Instance);
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, "Delete user failed", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public void DeleteSubscription(SubscriptionDescription subscription)
        {
            try
            {
                _manager.RemoveSubscription(Instance, subscription.Name);
                _manager.UpdateTopic(Instance);

                Subscriptions.Remove(subscription);
                NotifyOfPropertyChange(() => Instance);
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, "Delete subscription failed", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public string SubscriptionName { get; set; }
        public void AddSubscription()
        {
            try
            {
                var subscription = new SubscriptionDescription(Instance.Path, SubscriptionName);
                subscription = _manager.AddSubscription(subscription);

                _manager.UpdateTopic(Instance);

                Subscriptions.Add(subscription);

                NotifyOfPropertyChange(() => Instance);
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, "Add subscription failed", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
