using System.Collections.Generic;
using System.Linq;
using Utils.Events;
using Utils.Types;

namespace StatesRobot.States
{
	class SearchState : IState
	{
		private readonly ExtremumsFinder extremumFinder;

		public SearchState(RobotContext context)
		{
			extremumFinder = new ExtremumsFinder(context.PegTopSize);
		}

		public ITradeEvent Process(RobotContext context, Candle candle)
		{
			var firstLongExtremums = extremumFinder.FindFirstExtremums(context.Candles, true);
			var firstShortExtremums = extremumFinder.FindFirstExtremums(context.Candles, false);

			var secondExtremum = extremumFinder.FindLastSecondExtremum(firstLongExtremums, firstShortExtremums, context.Candles.Count - 1);
			if (secondExtremum == null)
				return new SearchInfoEvent(firstLongExtremums, firstShortExtremums);

			bool isTrendLong = IsTrendLong(context.Candles);
			if (secondExtremum.IsMinimum != isTrendLong)
				return new SecondExtremumEvent(secondExtremum, firstLongExtremums, firstShortExtremums);

			context.CurrentState = new TradeState(candle.Close, isTrendLong);
			return new DealEvent(isTrendLong, candle.Close, secondExtremum, firstLongExtremums, firstShortExtremums);
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
