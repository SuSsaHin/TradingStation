using System.Linq;
using TradeTools;
using TradeTools.Events;
using Utils.Events;

namespace StatesRobot.States.Trade
{
	class BasicTradeState : IState
	{
		protected int StartPrice { get; }
		protected bool TrendIsLong { get; }

		public BasicTradeState(RobotContext context, Deal deal)
		{
			StartPrice = deal.Price;
			TrendIsLong = deal.IsBuy;
			context.StopLossPrice = GetStopPrice(StartPrice, context.StopLossSize);
		}

		public virtual ITradeEvent Process(RobotContext context, Candle candle)
		{
			if (candle.Time >= context.EndTime)
			{
				context.CurrentState = context.Factory.GetEndState();
				return new DealEvent(new Deal(candle.Close, candle.DateTime, !TrendIsLong));
			}

			if (TrendIsLong && candle.Low <= context.StopLossPrice ||
				!TrendIsLong && candle.High >= context.StopLossPrice)
			{
				context.CurrentState = context.Factory.GetEndState();
				return new StopLossEvent(new Deal(context.StopLossPrice, candle.DateTime, !TrendIsLong));
			}
			
			return null;
		}

		public virtual ITradeEvent StopTrading(RobotContext context)
		{
			context.CurrentState = context.Factory.GetEndState();
			var lastCandle = context.Candles.Last();
			return new DealEvent(new Deal(lastCandle.Close, lastCandle.DateTime, !TrendIsLong));
		}

		protected int GetStopPrice(int startValue, int offset)
		{
			return startValue + (TrendIsLong ? -offset : offset);
		}
	}
}
