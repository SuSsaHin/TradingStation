namespace TradeTools
{
	public class Advice
	{
		public Advice(double distFromMovigAverage, double currentDayStdDev, double previousDayStdDev, int distFromOpen, int sameDirectionCandlesCount, int otherDirectionCandlesCount)
		{
			DistFromMovingAverage = distFromMovigAverage;
			CurrentDayStdDev = currentDayStdDev;
			PreviousDayStdDev = previousDayStdDev;
			DistFromOpen = distFromOpen;
			SameDirectionCandlesCount = sameDirectionCandlesCount;
			OtherDirectionCandlesCount = otherDirectionCandlesCount;
		}

		public double DistFromMovingAverage { get; }
		public double CurrentDayStdDev { get; }
		public double PreviousDayStdDev { get; }
		public int DistFromOpen { get; }
		public int SameDirectionCandlesCount { get; }
		public int OtherDirectionCandlesCount { get; }
	}
}
