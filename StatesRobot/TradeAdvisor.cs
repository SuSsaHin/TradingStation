using System;
using System.Collections.Generic;
using System.Linq;
using TradeTools;
using Wintellect.PowerCollections;

namespace StatesRobot
{
	public class TradeAdvisor
	{
		private readonly Deque<Candle> history;
		private const int maxHistoryCount = 1000;

		public TradeAdvisor(List<Candle> history = null)
		{
			this.history = new Deque<Candle>(history ?? new List<Candle>());
		}

		public double GetMovingAverage(int length)
		{
			return GetMovingAverage(length, x => x.Close);
		}

		public double GetMovingAverage(int length, Func<Candle, double> selector)
		{
			if (!history.Any())
				throw new IndexOutOfRangeException("Candles list is empty");

			if (history.Count < length)
				throw new IndexOutOfRangeException("Length " + length + " is bigger then candles count");

			return history.Skip(history.Count - length).Average(selector);
		}

		public void AddCandle(Candle candle)
		{
			if (history.Any() && history.GetAtBack().DateTime >= candle.DateTime)
				throw new Exception("Last candle in history is later then added");

			history.AddToBack(candle);
			if (history.Count > maxHistoryCount)
			{
				history.RemoveFromFront();
			}
		}
	}
}
