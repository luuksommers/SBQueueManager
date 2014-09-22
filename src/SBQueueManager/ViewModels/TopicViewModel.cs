using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using Caliburn.Micro;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using Microsoft.ServiceBus.Messaging;
using SBQueueManager.Manager;

namespace SBQueueManager.ViewModels
{
    public class TopicViewModel : PropertyChangedBase
    {
        private readonly ServiceBusManager _manager;
        public TopicDescription Instance { get; set; }
        public TopicUsersViewModel Users { get; set; }

        public SubscriptionDescription SelectedSubscription { get; set; }
        public ObservableCollection<SubscriptionDescription> Subscriptions { get; set; }

        public TopicViewModel(TopicDescription topicInstance, ServiceBusManager manager)
        {
            _manager = manager;
            Instance = topicInstance;
            Users = new TopicUsersViewModel(manager, topicInstance);
            Subscriptions = new ObservableCollection<SubscriptionDescription>(manager.GetSubscriptions(topicInstance));
        }

        public async void Update()
        {
            try
            {
                var metroWindow = (Application.Current.MainWindow as MetroWindow);

                _manager.UpdateTopic(Instance);

                await metroWindow.ShowMessageAsync("Updating", "Success");

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
