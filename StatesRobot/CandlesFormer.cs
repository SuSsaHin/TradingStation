using System;
using System.Collections.Generic;
using Utils.Types;

namespace StatesRobot
{
	class CandlesFormer
	{
		private Candle candle;
		private const int periodMins = 5;

		public Candle AddTick(Tick tick)
		{
			try
			{
				candle.AppendTick(tick);
			}
			catch (Exception)
			{
				var res = candle;
				candle = new Candle(new List<Tick>{tick}, periodMins);
				return res;
			}

			return candle;
		}
	}
}
