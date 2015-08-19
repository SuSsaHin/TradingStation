using System;
using System.Collections.Concurrent;
using NLog;

namespace Utils.Events
{
	public class EventBus
	{
		public delegate void EventCallback(ITradeEvent ev);
		private readonly ConcurrentDictionary<Type, EventCallback> delegates = new ConcurrentDictionary<Type, EventCallback>();
		private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

		public void AddCallback(Type eventType, EventCallback callback)
		{
			if (eventType.GetInterface(typeof(ITradeEvent).Name) == null)
				throw new ArgumentException("Event type should implements " + typeof(ITradeEvent).Name);

			delegates.AddOrUpdate(eventType, callback, (type, eventCallback) => eventCallback + callback);
		}

		public void FireEvent(ITradeEvent fired)
		{
			if (fired == null)
				throw new ArgumentNullException(nameof(fired));

			var key = fired.GetType();

			if (!delegates.ContainsKey(key))
				return;

			EventCallback callbacks;
			while (!delegates.TryGetValue(key, out callbacks))
			{ }

			try
			{
				callbacks.Invoke(fired);
			}
			catch (Exception ex)
			{
				Logger.Error(ex);
            }
		}
	}
}
