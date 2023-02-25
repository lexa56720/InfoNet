namespace PostgresClient.Utils.MessageCentre
{
    public class Message
    {
        public string MessageText { get; }

        public Message(string message)
        {
            MessageText = message;
        }
    }
}
