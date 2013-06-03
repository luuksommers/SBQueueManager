using System;
using Caliburn.Micro;

namespace SBQueueManager.Helpers
{
    public class DialogHelper
    {
        public static void ShowWindow<T>() where T : class
        {
            var windowManager = new WindowManager();
            T viewModel = IoC.Get<T>();

            windowManager.ShowWindow(viewModel);
        }

        public static void ShowWindow<T>(T viewModel) where T : class
        {
            var windowManager = new WindowManager();
            windowManager.ShowWindow(viewModel);
        }

        public static void ShowDialog<T>() where T : class
        {
            var windowManager = new WindowManager();
            T viewModel = IoC.Get<T>();

            windowManager.ShowDialog(viewModel);
        }

        public static void ShowDialog<T>(T viewModel) where T : class
        {
            var windowManager = new WindowManager();
            windowManager.ShowDialog(viewModel);
        }
    }
}
