using System;
using System.Collections.Generic;
using System.Linq;
using StatesRobot.States.End;
using StatesRobot.States.Trade;
using Utils.Events;
using Utils.Types;

namespace StatesRobot.States.Search
{
	class SearchState : IState
	{
		public SearchState(RobotContext context)
		{
			throw new NotImplementedException();
		}

		public ITradeEvent Process(RobotContext context, Candle candle)
		{
			throw new NotImplementedException();

			//var firstLongExtremums = extremumFinder.FindFirstExtremums(context.Candles, true);
			//var firstShortExtremums = extremumFinder.FindFirstExtremums(context.Candles, false);

			//var secondExtremum = extremumFinder.FindLastSecondExtremum(firstLongExtremums, firstShortExtremums, context.Candles.Count - 1);
			//if (secondExtremum == null)
			//	return new SearchInfoEvent(firstLongExtremums, firstShortExtremums);

			//bool isTrendLong = IsTrendLong(context.Candles);
			//if (secondExtremum.IsMinimum != isTrendLong)
			//	return new SecondExtremumEvent(secondExtremum, firstLongExtremums, firstShortExtremums);

			//context.CurrentState = new BasicTradeState(candle.Close, isTrendLong);
			//return new DealEvent(isTrendLong, candle.Close, secondExtremum, firstLongExtremums, firstShortExtremums);
		}

		public ITradeEvent StopTrading(RobotContext context)
		{
			context.CurrentState = new EndState();
			return new EndEvent();
		}
		
		private bool IsTrendLong(List<Candle> candles)
		{
			return candles[candles.Count - 1].Close > candles.First().Open;
		}
	}
}
