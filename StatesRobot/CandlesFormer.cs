using System;
using System.Collections.Generic;
using TradeTools;

namespace StatesRobot
{
	class CandlesFormer
	{
		private Candle candle;
		private const int periodMins = 5;	//TODO configs

		public Candle AddTick(Tick tick)
		{
			try
			{
				if (candle.AppendTick(tick))
				{
					var res = candle;
					candle = new Candle(new List<Tick> { tick }, periodMins);
					return res;
				}
			}
			catch (Exception)	
			{
				//TODO Logs
			}

			return candle;
		}
	}
}
