namespace SBQueueManager.Manager
{
    public class MessageReceiveDelegateArgs<T>
    {
        public T ReceivedBody { get; set; }

        public MessageReceiveDelegateArgs(T receivedBody)
        {
            ReceivedBody = receivedBody;
        }
    }
}