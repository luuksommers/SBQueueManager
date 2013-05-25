using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.ServiceBus.Messaging;

namespace SBQueueManager
{
    public class QueueViewModel
    {
        public QueueDescription Instance { get; set; }

        public QueueViewModel(QueueDescription queueInstance)
        {
            Instance = queueInstance;
        }
    }
}
