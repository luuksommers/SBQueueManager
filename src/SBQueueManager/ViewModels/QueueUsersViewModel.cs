using System;
using Microsoft.ServiceBus.Messaging;
using SBQueueManager.Manager;

namespace SBQueueManager.ViewModels
{
    public class QueueUsersViewModel : UsersViewModel<QueueDescription>
    {
        private readonly ServiceBusManager _manager;

        public QueueUsersViewModel(ServiceBusManager manager, QueueDescription instance) : base(instance)
        {
            _manager = manager;
        }

        protected override AuthorizationRules GetAuthorizationRules(QueueDescription instance)
        {
            return instance.Authorization;
        }

        protected override void AddUser(QueueDescription instance, QueueUser user)
        {
            _manager.AddUser(instance, user);
        }

        protected override void DeleteUser(QueueDescription instance, QueueUser user)
        {
            _manager.DeleteUser(instance, user);
        }
    }
}