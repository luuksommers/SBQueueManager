using System;
using Microsoft.ServiceBus.Messaging;
using SBQueueManager.Manager;

namespace SBQueueManager.ViewModels
{
    public class TopicUsersViewModel : UsersViewModel<TopicDescription>
    {
        private readonly ServiceBusManager _manager;

        public TopicUsersViewModel(ServiceBusManager manager, TopicDescription instance)
            : base(instance)
        {
            _manager = manager;
        }

        protected override AuthorizationRules GetAuthorizationRules(TopicDescription instance)
        {
            return instance.Authorization;
        }

        protected override void AddUser(TopicDescription instance, QueueUser user)
        {
            _manager.AddUser(instance, user);
        }

        protected override void DeleteUser(TopicDescription instance, QueueUser user)
        {
            _manager.DeleteUser(instance, user);
        }
    }
}