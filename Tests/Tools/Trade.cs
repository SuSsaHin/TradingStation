using System;
using TradeTools;

namespace Tests.Tools
{
	internal class Trade
	{
		public Trade(int profit, bool isLong, TimeSpan duration, Advice advice)
		{
			Profit = profit;
			IsLong = isLong;
			Duration = duration;
			Advice = advice;
		}

		public int Profit { get; }
		public TimeSpan Duration { get; }
		public bool IsLong { get; }
		public bool IsGood => Profit > 0;
		public Advice Advice { get; }
	}
}
