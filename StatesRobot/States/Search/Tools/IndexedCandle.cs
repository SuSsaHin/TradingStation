using System;
using TradeTools;

namespace StatesRobot.States.Search.Tools
{
	class IndexedCandle : Candle
	{
		public int Index { get; private set; }
		public IndexedCandle(DateTime dateTime, int open, int hight, int low, int close, int periodMins, int index) : base(dateTime, open, hight, low, close, periodMins)
		{
			Index = index;
		}

		public IndexedCandle(Candle candle, int index) :base(candle.DateTime, candle.Open, candle.High, candle.Low, candle.Close, candle.PeriodMins)
		{
			Index = index;
		}
	}
}
