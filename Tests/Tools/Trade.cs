using System;

namespace Tests.Tools
{
	class Trade
	{
		public Trade(int profit, bool isTrendLong, TimeSpan duration)
		{
			Profit = profit;
			IsTrendLong = isTrendLong;
			Duration = duration;
		}

		public int Profit { get; private set; }
		public TimeSpan Duration { get; private set; }
		public bool IsTrendLong { get; private set; }
		public bool IsGood { get { return Profit > 0; } }
	}
}
