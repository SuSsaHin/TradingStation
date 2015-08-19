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
			context.Logger.Debug("Change state to {0}", GetType().Name);
			StartPrice = deal.Price;
			TrendIsLong = deal.IsBuy;
			context.StopLossPrice = GetStopPrice(StartPrice, context.StopLossSize);
		}

		public virtual ITradeEvent Process(RobotContext context, Candle candle)
		{
			if (candle.Time >= context.EndTime)
			{
				context.Logger.Debug("End time exit. StopPrice: {0}, Candle: {1}", context.StopLossPrice, candle);
				context.CurrentState = context.Factory.GetEndState(context);
				return new DealEvent(new Deal(candle.Close, candle.DateTime, !TrendIsLong));
			}

			if (TrendIsLong && candle.Low <= context.StopLossPrice ||
				!TrendIsLong && candle.High >= context.StopLossPrice)
			{
				context.Logger.Debug("Stop has catched. StopPrice: {0}, Candle: {1}", context.StopLossPrice, candle);
				context.CurrentState = context.Factory.GetEndState(context);
				return new StopLossEvent(new Deal(context.StopLossPrice, candle.DateTime, !TrendIsLong));
			}
			
			return null;
		}

		public virtual ITradeEvent StopTrading(RobotContext context)
		{
			var lastCandle = context.Candles.Last();
			context.Logger.Debug("Stop trading command. Last candle: {0}", lastCandle);
			context.CurrentState = context.Factory.GetEndState(context);
			return new DealEvent(new Deal(lastCandle.Close, lastCandle.DateTime, !TrendIsLong));
		}

		protected int GetStopPrice(int startValue, int offset)
		{
			return startValue + (TrendIsLong ? -offset : offset);
		}
	}
}
