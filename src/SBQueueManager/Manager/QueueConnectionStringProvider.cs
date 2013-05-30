using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;

namespace SBQueueManager.Manager
{
    public class QueueConnectionStringProvider
    {
        private const string ConnectionStringAppSetting = "ServiceBus.ConnectionString";

        public string GetConnectionString()
        {
            return ConfigurationManager.AppSettings[ConnectionStringAppSetting];
        }
    }
}
