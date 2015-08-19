using System;
using System.Collections.Generic;
using NLog;
using TradeTools;

namespace StatesRobot
{
    public class CandlesFormer
    {
	    private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
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
			catch (Exception ex)	
			{
				Logger.Warn(ex);
            }

			return candle;
		}
	}
}
