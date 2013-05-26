using System;
using Caliburn.Micro;

namespace SBQueueManager.Helpers
{
    public class DialogHelper
    {
        public static void ShowDialog<T>(params Object[] param) where T : class
        {
            var windowManager = new WindowManager();
            T viewModel = IoC.Get<T>();

            windowManager.ShowWindow(viewModel);
        }
    }
}
