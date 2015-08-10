using System;

namespace Tests.Tools
{
	internal class Trade
	{
		public Trade(int profit, bool isLong, TimeSpan duration)
		{
			Profit = profit;
			IsLong = isLong;
			Duration = duration;
		}

		public int Profit { get; }
		public TimeSpan Duration { get; private set; }
		public bool IsLong { get; private set; }
		public bool IsGood => Profit > 0;
	}
}
