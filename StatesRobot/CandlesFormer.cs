using System;
using System.Collections.Generic;
using Utils.Types;

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
			catch (Exception ex)	
			{
				//TODO Logs
			}

			return candle;
		}
	}
}
