using System;
using Utils.XmlProcessing;

namespace StatesRobot
{
	public class TradeParams
	{
		[PropName("StopLoss")]
		public int StopLoss { get; set; }

		[PropName("BreakevenPercent")]
		public decimal BreakevenPercent { get; set; }

		[PropName("TrailingStopPercent")]
		public decimal TrailingStopPercent { get; set; }

		[PropName("Pegtop")]
		public int PegtopSize { get; set; }

		[PropName("EndTime")]
		public TimeSpan EndTime { get; set; }

		[PropName("MaxSkippedCount")]
		public int MaxSkippedCandlesCount { get; set; }
	}
}
