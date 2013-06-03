using System;
using Caliburn.Micro;

namespace SBQueueManager.ViewModels
{
    public class DialogViewModel : Screen
    {
        public bool IsCancelled { get; set; }
        public string Message { get; set; }

        public void Cancel()
        {
            IsCancelled = true;
            TryClose();
        }

        public void Confirm()
        {
            TryClose();
        }
    }
}
