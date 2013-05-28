using System.Collections.Generic;
using Microsoft.ServiceBus.Messaging;

namespace SBQueueManager.Manager
{
    public class QueueUser
    {
        /// <summary>
        ///     Username without domain
        /// </summary>
        public string UserName { get; set; }

        public bool AllowListen { get; set; }

        public bool AllowSend { get; set; }

        /// <summary>
        ///     Returns the user rights (Read and/or write)
        /// </summary>
        public IEnumerable<AccessRights> GetAccessRights()
        {
            if (AllowListen)
                yield return AccessRights.Listen;
            if (AllowSend)
                yield return AccessRights.Send;
        }
    }
}