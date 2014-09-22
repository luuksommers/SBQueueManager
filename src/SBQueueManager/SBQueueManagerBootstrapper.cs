using System;
using System.Reflection;
using System.Windows;
using Autofac;
using Caliburn.Micro;
using SBQueueManager.Manager;
using SBQueueManager.ViewModels;

namespace SBQueueManager
{
    public class SBQueueManagerBootstrapper : BootstrapperBase
    {
        private static IContainer Container { get; set; }

        public SBQueueManagerBootstrapper()
        {
            Initialize();
        }

        protected override void Configure()
        {
            base.Configure();

            var builder = new ContainerBuilder();

            builder.RegisterType<ServiceBusManager>();
            builder.RegisterType<WindowManager>().AsImplementedInterfaces();
            builder.RegisterType<EventAggregator>().AsImplementedInterfaces();

            Assembly currentAssembly = Assembly.GetExecutingAssembly();
            builder.RegisterAssemblyTypes(currentAssembly)
                   .Where(t => t.Name.EndsWith("ViewModel"))
                   .AsSelf();

            Container = builder.Build();


            var baseLocate = ViewLocator.LocateTypeForModelType;

            ViewLocator.LocateTypeForModelType = (modelType, displayLocation, context) =>
            {
                Type result = baseLocate(modelType, displayLocation, context);
                if (result == null)
                {
                    modelType = modelType.BaseType;
                    result = baseLocate(modelType, displayLocation, context);
                }

                return result;
            };
        }

        protected override void OnStartup(object sender, StartupEventArgs e)
        {
            DisplayRootViewFor<ShellViewModel>();
        }

        protected override object GetInstance(Type service, string key)
        {
            if (Container != null && Container.IsRegistered(service))
                return Container.Resolve(service);

            return base.GetInstance(service, key);
        }
    }
}