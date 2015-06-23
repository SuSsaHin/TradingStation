using System;
using System.Linq;
using Utils.Events;
using Utils.Types;

namespace StatesRobot.States
{
	class TradeState : IState
	{
		private readonly int startPrice;
		private readonly bool isTrendLong;
		private bool hasBreakeven;
		private readonly TimeSpan endTime = new TimeSpan(23, 30, 0);

		public TradeState(int startPrice, bool isTrendLong)
		{
			this.startPrice = startPrice;
			this.isTrendLong = isTrendLong;
		}

		public ITradeEvent Process(RobotContext context, Candle candle)
		{
			if (candle.Time >= endTime)
			{
				context.CurrentState = new EndState();
				return new DealEvent(!isTrendLong, candle.Close);
			}

			int endPrice;
			if (isTrendLong && candle.Low <= (endPrice = startPrice - context.StopLoss) ||
				!isTrendLong && candle.High >= (endPrice = startPrice + context.StopLoss))
			{
				context.CurrentState = new EndState();
				return new StopLossEvent(isTrendLong, endPrice);
			}

			if (!hasBreakeven &&
				(isTrendLong && candle.High >= startPrice + context.StopLoss ||
				!isTrendLong && candle.Low <= startPrice - context.StopLoss))
			{
				hasBreakeven = true;

				context.StopLoss = -(int)(context.StopLoss * context.BreakevenPercent);
				return new BreakevenEvent(isTrendLong ? startPrice - context.StopLoss : startPrice + context.StopLoss, isTrendLong);
			}
			
			return null;
		}

		public ITradeEvent StopTrading(RobotContext context)
		{
			context.CurrentState = new EndState();
			return new DealEvent(!isTrendLong, context.Candles.Last().Close);
		}
	}
}
