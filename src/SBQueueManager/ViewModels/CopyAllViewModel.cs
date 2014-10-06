using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Caliburn.Micro;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using Microsoft.ServiceBus.Messaging;
using SBQueueManager.Helpers;
using SBQueueManager.Manager;

namespace SBQueueManager.ViewModels
{
    public class CopyAllViewModel : Screen
    {
        private readonly ServiceBusManager _fromManager;
        private ServiceBusManager _toManager;

        private string _message;
        private string _connectionString;

        public string ConnectionString
        {
            get { return _connectionString; }
            set
            {
                _connectionString = value;
                NotifyOfPropertyChange(() => CanVerify);
            }
        }

        public bool CanVerify
        {
            get { return !string.IsNullOrWhiteSpace(ConnectionString); }
        }

        public bool CanCopy
        {
            get { return _toManager != null; }
        }

        public string Message
        {
            get { return _message; }
            set
            {
                if (value == _message) return;
                _message = value;
                NotifyOfPropertyChange(() => Message);
            }
        }
        public CopyAllViewModel(ServiceBusManager fromManager)
        {
            _fromManager = fromManager;
        }

        public void Verify()
        {
            try
            {
                _toManager = new ServiceBusManager(ConnectionString);
                Message = string.Empty;
                VerifyQueues(_fromManager.Queues, _toManager.Queues);
                VerifyTopics(_fromManager.Topics, _toManager.Topics);

            }
            catch (Exception e)
            {
                Message = e.Message;
            }
            NotifyOfPropertyChange(() => CanCopy);
        }

        public async void Copy()
        {
            var metroWindow = (Application.Current.MainWindow as MetroWindow);
            var progressDialogController = await metroWindow.ShowProgressAsync("Copying...", "");

            try
            {
                Message = string.Empty;
                await CopyQueues(progressDialogController);
                await CopyTopics(progressDialogController);
                Message = "Copy OK";
            }
            catch (Exception e)
            {
                Message = e.Message;
            }

            await progressDialogController.CloseAsync();
        }

        private void VerifyQueues(ObservableCollection<QueueDescription> from, ObservableCollection<QueueDescription> to)
        {
            var queuesToAdd = from.Except(to, new LambdaComparer<QueueDescription>((a, b) => a.Path == b.Path, a => 1));

            var sb = new StringBuilder();
            sb.AppendLine("Queues to add:");
            foreach (var queue in queuesToAdd)
            {
                sb.AppendLine(queue.Path);
            }

            sb.AppendLine("");
            sb.AppendLine("Queues to update:");
            var queuesToUpdate = from.Where(a => to.Select(b => b.Path).Contains(a.Path));
            foreach (var queue in queuesToUpdate)
            {
                sb.AppendLine(queue.Path);
            }

            Message += sb.ToString();
        }

        private void VerifyTopics(ObservableCollection<TopicDescription> from, ObservableCollection<TopicDescription> to)
        {
            var topicsToAdd = from.Except(to, new LambdaComparer<TopicDescription>((a, b) => a.Path == b.Path, a => 1));

            var sb = new StringBuilder();
            sb.AppendLine("Topics to add:");
            foreach (var topic in topicsToAdd)
            {
                sb.AppendLine(topic.Path);
            }
            sb.AppendLine("");
            sb.AppendLine("Topics to update:");
            var topicsToUpdate = from.Where(a => to.Select(b => b.Path).Contains(a.Path));
            foreach (var topic in topicsToUpdate)
            {
                sb.AppendLine(topic.Path);
            }

            Message += sb.ToString();
        }


        private async Task CopyQueues(ProgressDialogController controller)
        {
            var queuesToUpdate = _fromManager.Queues.Where(a => _toManager.Queues.Select(b => b.Path).Contains(a.Path)).ToList();
            foreach (var queue in queuesToUpdate)
            {
                controller.SetMessage(string.Format("Updating: {0}", queue.Path));
                await Task.Run(() =>
                {
                    _toManager.UpdateQueue(queue);
                });
            }

            var queuesToAdd = _fromManager.Queues.Except(_toManager.Queues, new LambdaComparer<QueueDescription>((a, b) => a.Path == b.Path, a => 1)).ToList();
            foreach (var queue in queuesToAdd)
            {
                controller.SetMessage(string.Format("Adding: {0}", queue.Path));
                await Task.Run(() =>
                {
                    _toManager.CreateQueue(queue);
                });
            }
        }

        private async Task CopyTopics(ProgressDialogController controller)
        {
            var topicsToUpdate = _fromManager.Topics.Where(a => _toManager.Topics.Select(b => b.Path).Contains(a.Path)).ToList();
            foreach (var topic in topicsToUpdate)
            {
                controller.SetMessage(string.Format("Updating: {0}", topic.Path));
                await Task.Run(() =>
                {
                    _toManager.UpdateTopic(topic);

                    // Add missing subscription
                    foreach (var subscription in _fromManager.GetSubscriptions(topic).Where(a => !_toManager.GetSubscriptions(topic).Select(b => b.Name).Contains(a.Name)))
                    {
                        _toManager.AddSubscription(subscription);
                    }
                });
            }

            var topicsToAdd = _fromManager.Topics.Except(_toManager.Topics, new LambdaComparer<TopicDescription>((a, b) => a.Path == b.Path, a => 1)).ToList();
            foreach (var topic in topicsToAdd)
            {
                controller.SetMessage(string.Format("Adding: {0}", topic.Path));
                await Task.Run(() =>
                {
                    _toManager.CreateTopic(topic);

                    // Add missing subscription
                    foreach (var subscription in _fromManager.GetSubscriptions(topic))
                    {
                        _toManager.AddSubscription(subscription);
                    }
                });
            }
        }
    }
}