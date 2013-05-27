namespace SBQueueManager.Manager
{
    public delegate void MessageReceiveDelegate<T>(object sender, MessageReceiveDelegateArgs<T> args);
}