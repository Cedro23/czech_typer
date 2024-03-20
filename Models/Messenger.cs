using System;

namespace CzechTyper.Models
{
    internal enum MessageType
    {
        Closing
    }

    internal static class Messenger
    {
        public static event Action<MessageType> MessageReceived;

        public static void SendMessage(MessageType message)
        {
            MessageReceived?.Invoke(message);
        }
    }
}
