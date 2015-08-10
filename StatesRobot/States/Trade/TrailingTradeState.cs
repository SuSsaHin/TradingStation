using TradeTools;
using TradeTools.Events;
using Utils.Events;

namespace StatesRobot.States.Trade
{
	class TrailingTradeState : BreakevenTradeState
	{
		public TrailingTradeState(RobotContext context, Deal deal) : base(context, deal)
		{}

		public override ITradeEvent Process(RobotContext context, Candle candle)
		{
			var result = base.Process(context, candle);
			if (result != null)
				return result;

			if (TrendIsLong && candle.High - context.StopLossPrice >= context.TrailingStopLoss ||
				!TrendIsLong && candle.Low + context.TrailingStopLoss <= context.TrailingStopLoss)
			{
				context.StopLossPrice = GetStopPrice(TrendIsLong ? candle.High : candle.Low, context.TrailingStopLoss);
				return new StopLossMovingEvent(context.StopLossPrice, TrendIsLong);
			}
			
			return null;
		}
	}
}
