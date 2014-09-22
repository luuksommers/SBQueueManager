using System;
using System.Windows;
using Caliburn.Micro;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using Microsoft.ServiceBus.Messaging;
using SBQueueManager.Manager;

namespace SBQueueManager.ViewModels
{
    public class QueueViewModel : PropertyChangedBase
    {
        private readonly ServiceBusManager _manager;
        public QueueDescription Instance { get; set; }
        public QueueUsersViewModel Users { get; set; }

        public QueueViewModel(QueueDescription queueInstance, ServiceBusManager manager)
        {
            _manager = manager;
            Instance = queueInstance;
            Users = new QueueUsersViewModel(manager, queueInstance);
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

        public async void Update()
        {
            try
            {
                var metroWindow = (Application.Current.MainWindow as MetroWindow);

                _manager.UpdateQueue(Instance);

                await metroWindow.ShowMessageAsync("Updating", "Success");
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, "Update failed", MessageBoxButton.OK, MessageBoxImage.Error);
            }
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
