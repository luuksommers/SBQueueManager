using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
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
            var dataAccess = Assembly.GetExecutingAssembly();

            builder.RegisterType<QueueManager>();

            builder.RegisterAssemblyTypes(dataAccess)
                .Where(t => t.Name.EndsWith("ViewModel"))
                .AsSelf();

            base.Configure();
        }

        protected override object GetInstance(Type service, string key)
        {
            if (Container.IsRegistered(service))
                return Container.Resolve(service);

            return base.GetInstance(service, key);
        }
    }
}
