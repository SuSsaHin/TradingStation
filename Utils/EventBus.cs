using System;
using System.Collections.Generic;
using System.IO;
using Utils.Events;

namespace Utils
{
	public class EventBus
	{
		private readonly Dictionary<Type, List<Action<ITradeEvent>>> callbacks = new Dictionary<Type, List<Action<ITradeEvent>>>();

		public void AddCallback(Type eventType, Action<ITradeEvent> callback)
		{
			if (!callbacks.ContainsKey(eventType))
			{
				callbacks[eventType] = new List<Action<ITradeEvent>>();
			}

			callbacks[eventType].Add(callback);
		}

		public void FireEvent(ITradeEvent fired)
		{
			Type key;
			if (fired is BreakevenEvent)
			{
				key = ((BreakevenEvent) fired).GetType();
			}
			else if (fired is DealEvent)
			{
				key = ((DealEvent) fired).GetType();
			}
			else if (fired is EndEvent)
			{
				key = ((EndEvent) fired).GetType();
			}
			else if (fired is StopLossEvent)
			{
				key = ((StopLossEvent) fired).GetType();
			}
			else if (fired is SecondExtremumEvent)
			{
				key = ((SecondExtremumEvent) fired).GetType();
			}
			else if (fired is SearchInfoEvent)
			{
				key = ((SearchInfoEvent) fired).GetType();
			}
			else
			{
				throw new ArgumentException("Unexpected event type");
			}

			if (!callbacks.ContainsKey(key))
				return;

			try
			{
				foreach (var callback in callbacks[key])
				{
					callback(fired);
				}
			}
			catch (Exception ex)
			{
				File.WriteAllText("lastEventError.txt", ex.ToString());	//TODO заменить на логгер
			}
		}
	}
}
