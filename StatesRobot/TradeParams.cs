using System;

namespace StatesRobot
{

	public class TradeParams
	{
		[FieldName("StopLoss")]
		public int StopLoss;

		[FieldName("BreakevenPercent")]
		public decimal BreakevenPercent;

		[FieldName("TrailingStopPercent")]
		public decimal TrailingStopPercent;

		[FieldName("Pegtop")]
		public int PegtopSize;

		[FieldName("EndTime")]
		public TimeSpan EndTime;
	}
}
