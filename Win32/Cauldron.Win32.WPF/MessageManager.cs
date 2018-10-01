using Cauldron.Collections;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Cauldron
{
    /// <summary>
    /// Manages the messaging system
    /// </summary>
    public static class MessageManager
    {
        private static ConcurrentCollection<MessageObject> messages = new ConcurrentCollection<MessageObject>();

        /// <summary>
        /// Sends a message to all message subscribers
        /// </summary>
        /// <param name="args">The argument of the message</param>
        /// <returns>A collection of listeners that subscribed to this message</returns>
        /// <exception cref="ArgumentNullException"><paramref name="args"/> is null</exception>
        public static IEnumerable<object> Send(MessagingArgs args)
        {
            if (args == null)
                throw new ArgumentNullException(nameof(args));

            var receiver = messages.Where(x => x.MessageType == args.GetType() && x.Handler != null);

            foreach (var item in receiver)
                item.Handler(args);

            return receiver.Select(x => x.Subscriber);
        }

        /// <summary>
        /// Subscribes to a message. If the subscriber implements the <see cref="IDisposableObject"/> interface, the <see cref="MessageManager"/> will
        /// automatically add the <see cref="Unsubscribe(object)"/> method to the dispose event
        /// </summary>
        /// <typeparam name="T">The type of message to subscribe to</typeparam>
        /// <param name="subscriber">The object that subscribes to a message</param>
        /// <param name="subscriptionHandler">The handler that will be invoked on message recieve</param>
        /// <exception cref="ArgumentNullException"><paramref name="subscriber"/> is null</exception>
        /// <exception cref="ArgumentNullException"><paramref name="subscriptionHandler"/> is null</exception>
        public static void Subscribe<T>(object subscriber, Action<T> subscriptionHandler)
            where T : MessagingArgs
        {
            if (subscriber == null)
                throw new ArgumentNullException(nameof(subscriber));

            if (subscriptionHandler == null)
                throw new ArgumentNullException(nameof(subscriptionHandler));

            messages.Add(new MessageObject { MessageType = typeof(T), Handler = new Action<MessagingArgs>(x => subscriptionHandler(x as T)), Subscriber = subscriber });

            // we don't need multiple unsubscribs
            if (!messages.Any(x => subscriber == x))
            {
                var disposable = subscriber as IDisposableObject;
                if (disposable != null)
                    disposable.Disposed += (s, e) => MessageManager.Unsubscribe(s);
            }
        }

        /// <summary>
        /// Unsubscribs all subscriptions from the defined subscriber
        /// </summary>
        /// <param name="subscriber">The subscriber</param>
        /// <exception cref="ArgumentNullException"><paramref name="subscriber"/> is null</exception>
        public static void Unsubscribe(object subscriber)
        {
            if (subscriber == null)
                throw new ArgumentNullException(nameof(subscriber));

            messages.Remove(x => x.Subscriber == subscriber);
        }

        /// <summary>
        /// Unsubscribs all subscriptions
        /// </summary>
        public static void Unsubscribe() => messages.Clear();

        /// <summary>
        /// Unsubscribs all subscriptions from the defined subscriber for the given message type
        /// </summary>
        /// <typeparam name="T">The message type that will be unsubscribed</typeparam>
        /// <param name="subscriber">The subscriber</param>
        /// <exception cref="ArgumentNullException"><paramref name="subscriber"/> is null</exception>
        public static void Unsubscribe<T>(object subscriber)
        {
            if (subscriber == null)
                throw new ArgumentNullException(nameof(subscriber));

            messages.Remove(x => x.Subscriber == subscriber && x.MessageType == typeof(T));
        }
    }
}