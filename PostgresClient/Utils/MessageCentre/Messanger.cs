using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace PostgresClient.Utils.MessageCentre
{
    public static class Messenger
    {
        private class EventValue
        {
            public Type? Type { get; private set; }

            public List<Delegate> Delegates { get; private set; } = new();

            public EventValue()
            {

            }
            public EventValue(Type type)
            {
                Type = type;
            }
        }

        private static Dictionary<string, EventValue> Events = new();
        public static bool Subscribe(string text, Action handler)
        {
            if (!Events.ContainsKey(text))
                Events.Add(text, new EventValue());

            if (Events[text].Type == null)
            {
                Events[text].Delegates.Add(handler);
                return true;
            }
            return false;
        }
        public static void Subscribe<T>(string text, EventHandler<T> handler)
        {
            if (!Events.ContainsKey(text))
                Events.Add(text, new EventValue(typeof(T)));

            var type = Events[text].Type;
            if (type != null && type.Equals(typeof(T)))
                Events[text].Delegates.Add(handler);
        }

        public static void Unsubscribe<T>(string text, EventHandler<T> handler)
        {
            if (Events.TryGetValue(text, out var value))
            {
                value.Delegates.Remove(handler);
                if (!value.Delegates.Any())
                    Events.Remove(text);
            }
        }
        public static Task Send(string text)
        {
            return Task.Run(() =>
            {
                if (Events.TryGetValue(text, out var value) && value.Type == null)
                    value.Delegates
                    .AsParallel()
                    .ForAll(x => ((Action)x)());
            });
        }
        public static Task Send<T>(string text, T message, object? sender = null)
        {
            return Task.Run(() =>
            {
                if (Events.TryGetValue(text, out var value) && value.Type != null && value.Type.Equals(typeof(T)))
                    value.Delegates
                    .AsParallel()
                    .ForAll(x => ((EventHandler<T>)x)(sender, message));
            });
        }
    }
}
