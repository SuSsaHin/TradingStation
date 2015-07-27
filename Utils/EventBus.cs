using System;
using System.Collections.Generic;
using System.IO;
using Utils.Events;

namespace Utils
{
	public class EventBus	//TODO добавить асинхронность
	{
		public delegate void EventCallback(ITradeEvent ev);
		private readonly Dictionary<Type, EventCallback> delegates = new Dictionary<Type, EventCallback>();

		public void AddCallback(Type eventType, EventCallback callback)
		{
			if (eventType.GetInterface(typeof(ITradeEvent).Name) == null)
				throw new ArgumentException("Event type should implements " + typeof(ITradeEvent).Name);

			if (!delegates.ContainsKey(eventType))
			{
				delegates[eventType] = callback;
				return;
			}

			delegates[eventType] += callback;
		}

		public void FireEvent(ITradeEvent fired)
		{
			var key = fired.GetType();

			if (!delegates.ContainsKey(key))
				return;

			try
			{
				delegates[key].Invoke(fired);
			}
			catch (Exception ex)
			{
				File.WriteAllText("lastEventError.txt", ex.ToString());	//TODO заменить на логгер
			}
		}
	}
}
