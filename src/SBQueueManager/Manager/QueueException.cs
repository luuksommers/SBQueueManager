using System;

namespace SBQueueManager.Manager
{
    public class QueueException : Exception
    {
        public QueueException(string format, params object[] args) 
            : base(string.Format(format, args))
        {
            
        }
    }
}