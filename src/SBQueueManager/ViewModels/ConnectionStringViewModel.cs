using System;
using System.Configuration;
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
        public string ConnectionString { get; set; }

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
        }

        public async void Set()
        {
            Settings.Default.ConnectionString = ConnectionString;
            Settings.Default.Save();

            var metroWindow = (Application.Current.MainWindow as MetroWindow);
            var controller = await metroWindow.ShowProgressAsync("Refreshing...", "");

            await Task.Run(() => _shellViewModel.SetManager(ConnectionString));

            await controller.CloseAsync();
        }
    }
}
