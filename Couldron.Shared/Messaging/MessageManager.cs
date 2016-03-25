using Couldron.Collections;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Couldron.Messaging
{
    public static class MessageManager
    {
        private static ConcurrentList<MessageObject> messages = new ConcurrentList<MessageObject>();

        public static IEnumerable<object> Message(MessagingArgs args)
        {
            var reciever = messages.Where(x => x.MessageType == args.GetType() && x.Handler != null);

            foreach (var item in reciever)
                item.Handler(args);

            return reciever.Select(x => x.Subscriber);
        }

        /// <summary>
        /// Subscribes to a message
        /// </summary>
        /// <typeparam name="T">The type of message to subscribe to</typeparam>
        /// <param name="subscriber">The object that subscribes to a message</param>
        /// <param name="subscribtionHandler">The handler that will be invoked on message recieve</param>
        public static void Subscribe<T>(object subscriber, Action<T> subscribtionHandler)
            where T : MessagingArgs
        {
            messages.Add(new MessageObject { MessageType = typeof(T), Handler = new Action<MessagingArgs>(x => subscribtionHandler(x as T)), Subscriber = subscriber });
        }

        public static void Unsubscribe(object subscriber)
        {
            messages.Remove(x => x.Subscriber == subscriber);
        }

        public static void Unsubscribe()
        {
            messages.Clear();
        }

        public static void Unsubscribe<T>(object subscriber)
        {
            messages.Remove(x => x.Subscriber == subscriber && x.MessageType == typeof(T));
        }
    }
}