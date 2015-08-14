using System;
using Utils.XmlProcessing;

namespace StatesRobot
{
	public class TradeParams
	{
		public class UnavailableParameterException : Exception
		{
			public UnavailableParameterException(string msg)
				: base(msg)
			{ }
		}

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

		public void Validate()
		{
			if (StopLoss <= 0)
				throw new UnavailableParameterException("Stop loss should be positive");

			if (BreakevenPercent < 0 || BreakevenPercent >= 1)
				throw new UnavailableParameterException("Breakeven percent should be between 0.0 and 1.0");

			if (TrailingStopPercent <= 0.1m)
				throw new UnavailableParameterException("Trailing stop percent should be larger then 0.1");

			if (PegtopSize <= 0)
				throw new UnavailableParameterException("Pegtop size should be positive");

			if (EndTime.TotalDays >= 1)
				throw new UnavailableParameterException("End time can't be over 23.59");
		}
	}
}
