using System;
using System.Collections.Generic;
using System.Linq;
using Utils.Types;

namespace StatesRobot
{
	public class TradeAdvisor
	{
		private readonly List<Candle> history;

		public TradeAdvisor(List<Candle> history = null)
		{
			this.history = history ?? new List<Candle>();
		}

		public double GetMovingAverage(int length)
		{
			return GetMovingAverage(length, x => x.Close);
		}

		public double GetMovingAverage(int length, Func<Candle, double> selector)
		{
			if (!history.Any())
				return 0;

			if (history.Count < length)
				return history.Average(selector);

			return history.Skip(history.Count - length).Average(selector);
		}

		public void AddCandle(Candle candle)
		{
			if (history.Any() && history[history.Count - 1].DateTime >= candle.DateTime)
				throw new Exception("Last candle in history is later then added");

			history.Add(candle);
		}
	}
}
