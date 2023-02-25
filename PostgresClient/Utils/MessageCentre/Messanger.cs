using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PostgresClient.Utils.MessageCentre
{
    public static class Messenger
    {

        private static Dictionary<string, List<EventHandler<Message>>> Events = new Dictionary<string, List<EventHandler<Message>>>();
        public static void Subscribe(string text, EventHandler<Message> handler)
        {
            Events.TryAdd(text, new List<EventHandler<Message>>());

            if (!Events[text].Contains(handler))
                Events[text].Add(handler);
        }

        public static void Unsubscribe(string text, EventHandler<Message> handler)
        {
            if (Events.TryGetValue(text, out var value))
            {
                value.Remove(handler);
                if (!value.Any())
                    Events.Remove(text);
            }
        }

        public static void Send(Message messange, object sender)
        {
            Task.Run(() =>
            {
                if (Events.TryGetValue(messange.MessageText, out var handlers))
                    handlers.ForEach(o => o.Invoke(sender, messange));
            });
        }
    }
}
