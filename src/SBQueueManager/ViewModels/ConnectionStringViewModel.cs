using System;
using System.Configuration;
using Caliburn.Micro;
using SBQueueManager.Properties;

namespace SBQueueManager.ViewModels
{
    public class ConnectionStringViewModel : Screen
    {
        private readonly ShellViewModel _shellViewModel;
        public string ConnectionString { get; set; }

        public ConnectionStringViewModel(ShellViewModel shellViewModel)
        {
            _shellViewModel = shellViewModel;
            DisplayName = "Set connectionstring";
            ConnectionString = Settings.Default.ConnectionString;
        }

        public void Set()
        {
            Settings.Default.ConnectionString = ConnectionString;
            Settings.Default.Save();
            _shellViewModel.SetManager(ConnectionString);
        }
    }
}
