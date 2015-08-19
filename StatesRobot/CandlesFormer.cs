using System;
using System.Collections.Generic;
using TradeTools;

namespace StatesRobot
{
    public class CandlesFormer
	{
		private Candle candle;
		private readonly int periodMins;

	    public CandlesFormer(int periodMins = 5)
	    {
		    this.periodMins = periodMins;
	    }

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
