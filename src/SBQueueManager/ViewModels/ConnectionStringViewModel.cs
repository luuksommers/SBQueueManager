using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using Caliburn.Micro;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using SBQueueManager.Properties;

namespace SBQueueManager.ViewModels
{
    public class ConnectionStringViewModel : Screen
    {
        private readonly ShellViewModel _shellViewModel;
        private string _message;
        private string _connectionString;

        public string ConnectionString
        {
            get { return _connectionString; }
            set
            {
                if (value == _connectionString) return;
                _connectionString = value;
                NotifyOfPropertyChange(() => ConnectionString);
            }
        }

        public BindableCollection<string> History { get; set; }
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

        public override string DisplayName
        {
            get { return "Set connectionstring"; }
            set { }
        }

        public ConnectionStringViewModel(ShellViewModel shellViewModel)
        {
            _shellViewModel = shellViewModel;
            ConnectionString = Settings.Default.ConnectionString;
            if (Settings.Default.History == null)
            {
                Settings.Default.History = new StringCollection();
            }
            History = new BindableCollection<string>(Settings.Default.History.Cast<string>());
        }

        public async void Delete(string connectionString)
        {
            var metroWindow = (Application.Current.MainWindow as MetroWindow);
            var metroDialogSettings = new MetroDialogSettings()
            {
                AffirmativeButtonText = "OK",
                NegativeButtonText = "CANCEL",
                AnimateHide = true,
                AnimateShow = true,
                ColorScheme = MetroDialogColorScheme.Theme,
            };
            
            var result = await metroWindow.ShowMessageAsync("Delete", "Are you sure?", MessageDialogStyle.AffirmativeAndNegative, metroDialogSettings);
            if (result == MessageDialogResult.Affirmative)
            {
                Settings.Default.History.Remove(connectionString);
                History.Remove(connectionString);
                Settings.Default.Save();
            }
        }

        public async void Set()
        {
            Settings.Default.ConnectionString = ConnectionString;
            if (!Settings.Default.History.Contains(ConnectionString)) 
            {
                Settings.Default.History.Add(ConnectionString);
                History.Add(ConnectionString);
            }
            
            Settings.Default.Save();

            var metroWindow = (Application.Current.MainWindow as MetroWindow);
            var controller = await metroWindow.ShowProgressAsync("Refreshing...", "");

            await Task.Run(() => _shellViewModel.SetManager(ConnectionString));

            await controller.CloseAsync();
        }
    }
}
