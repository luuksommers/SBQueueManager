﻿using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows;
using Caliburn.Micro;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using Microsoft.ServiceBus.Messaging;
using SBQueueManager.Manager;
using SBQueueManager.Properties;

namespace SBQueueManager.ViewModels
{
    public class ShellViewModel : Screen
    {
        private ServiceBusManager _manager;
        private object _contentViewModel;
        public ObservableCollection<QueueDescription> Queues { get; set; }
        public ObservableCollection<TopicDescription> Topics { get; set; }

        // Dynamic content object
        public object ContentViewModel
        {
            get { return _contentViewModel; }
            set
            {
                if (Equals(value, _contentViewModel)) return;
                _contentViewModel = value;
                NotifyOfPropertyChange(() => ContentViewModel);
            }
        }

        public bool CanAddNew
        {
            get { return _manager != null; }
        }

        public bool CanCopyAll
        {
            get { return _manager != null; }
        }

        public bool Connected
        {
            get { return _manager != null; }
        }

        public bool CanRefreshList
        {
            get { return _manager != null; }
        }

        public override string DisplayName
        {
            get { return "Service Bus For Windows Queue Manager"; }
            set { }
        }

        protected override async void OnViewLoaded(object view)
        {
            base.OnViewLoaded(view);
            var metroWindow = (Application.Current.MainWindow as MetroWindow);
            var controller = await metroWindow.ShowProgressAsync("Initializing...", "");
            await Task.Run(() => LoadOrSetConnection());
            await controller.CloseAsync();
        }

        private void LoadOrSetConnection()
        {
            var connectionString = string.IsNullOrWhiteSpace(Settings.Default.ConnectionString)
                ? System.Configuration.ConfigurationManager.AppSettings["Microsoft.ServiceBus.ConnectionString"]
                : Settings.Default.ConnectionString;
            
            if (string.IsNullOrWhiteSpace(connectionString))
                OpenConnectionStringManager();
            else
                SetManager(connectionString);
        }

        public void SetManager(string connectionString)
        {
            try
            {
                if (_manager != null)
                {
                    Queues.CollectionChanged -= Queues_CollectionChanged;
                    Topics.CollectionChanged -= Topics_CollectionChanged;
                }

                _manager = new ServiceBusManager(connectionString);
                Queues = _manager.Queues;
                Topics = _manager.Topics;
                Queues.CollectionChanged += Queues_CollectionChanged;
                Topics.CollectionChanged += Topics_CollectionChanged;

                ContentViewModel = null;
            }
            catch (Exception e)
            {
                _manager = null;
                Queues = null;
                Topics = null;

                OpenConnectionStringManager(e.Message);
            }

            NotifyOfPropertyChange(() => CanAddNew);
            NotifyOfPropertyChange(() => CanCopyAll);
            NotifyOfPropertyChange(() => Connected);
            NotifyOfPropertyChange(() => CanRefreshList);
            NotifyOfPropertyChange(() => Queues);
            NotifyOfPropertyChange(() => Topics);
        }

        void Queues_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add)
                OpenEntity((QueueDescription)e.NewItems[0]);
            else
                OpenEntity(null);
        }

        void Topics_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add)
                OpenEntity((TopicDescription)e.NewItems[0]);
            else
                OpenEntity(null);
        }

        public void OpenEntity(object entity)
        {
            if (entity == null)
                ContentViewModel = null;
            else if (entity is QueueDescription)
                ContentViewModel = new QueueViewModel((QueueDescription)entity, _manager);
            else if (entity is TopicDescription)
                ContentViewModel = new TopicViewModel((TopicDescription)entity, _manager);
        }

        public async void RefreshList()
        {
            var metroWindow = (Application.Current.MainWindow as MetroWindow);
            var controller = await metroWindow.ShowProgressAsync("Please wait", "Refreshing the list");

            await Task.Run(() => LoadOrSetConnection());

            await controller.CloseAsync();
        }

        public void AddNew()
        {
            ContentViewModel = new CreateEntityViewModel(_manager);
        }

        public void CopyAll()
        {
            ContentViewModel = new CopyAllViewModel(_manager);
        }

        public void OpenConnectionStringManager(string message = null)
        {
            ContentViewModel = new ConnectionStringViewModel(this);
            ((ConnectionStringViewModel)ContentViewModel).Message = message;
        }

        public void OpenHelp()
        {
            ContentViewModel = new HelpViewModel();
        }

        public void OpenCertificateManager()
        {
            Process.Start("mmc.exe");
        }
    }
}
