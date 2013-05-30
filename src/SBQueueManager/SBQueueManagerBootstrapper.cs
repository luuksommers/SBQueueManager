using System;
using System.Reflection;
using Autofac;
using Caliburn.Micro;
using SBQueueManager.Manager;
using SBQueueManager.ViewModels;

namespace SBQueueManager
{
    public class SBQueueManagerBootstrapper : Bootstrapper<ShellViewModel>
    {
        private static IContainer Container { get; set; }

        protected override void Configure()
        {
            var builder = new ContainerBuilder();

            builder.RegisterType<QueueConnectionStringProvider>();
            builder.RegisterType<QueueManager>();
            builder.RegisterType<WindowManager>().AsImplementedInterfaces();
            builder.RegisterType<EventAggregator>().AsImplementedInterfaces();

            Assembly dataAccess = Assembly.GetExecutingAssembly();
            builder.RegisterAssemblyTypes(dataAccess)
                   .Where(t => t.Name.EndsWith("ViewModel"))
                   .AsSelf();

            Container = builder.Build();

            base.Configure();
        }

        protected override object GetInstance(Type service, string key)
        {
            if (Container != null && Container.IsRegistered(service))
                return Container.Resolve(service);

            return base.GetInstance(service, key);
        }
    }
}