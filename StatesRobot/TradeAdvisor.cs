using System;
using System.Collections.Generic;
using System.Linq;
using MathNet.Numerics.Statistics;
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

		public Advice GetAdvice(double dealPrice, bool trendIsLong)
		{
			return null;	//TODO rmove it!!!
			const int maLength = 163;

			if (!history.Any())
				throw new Exception("Empty history for advice");

			var lastCandle = history[history.Count - 1];
			var currentDayCandles = history.Where(c => c.Date == lastCandle.Date).ToList();

			var prevDate = lastCandle.Date.AddDays(-1);
			var prevDayCandles = history.Where(c => c.Date == prevDate).ToList();

			if (!prevDayCandles.Any() || !currentDayCandles.Any())
				throw new Exception("Empty days history for advice");

			var distFromMA = dealPrice - GetMovingAverage(maLength);

			var currentStdDev = currentDayCandles.Select(c => (double)(c.Close - c.Open)).StandardDeviation();
			var prevStdDev = prevDayCandles.Select(c => (double) (c.Close - c.Open)).StandardDeviation();

			var sameDirCandlesCount = currentDayCandles.Count(c => c.IsLong == trendIsLong);
			var invDirCandlesCount = currentDayCandles.Count - sameDirCandlesCount;

			return new Advice(distFromMA, currentStdDev, prevStdDev, currentDayCandles.Count, sameDirCandlesCount, invDirCandlesCount);
		}
	}
}
