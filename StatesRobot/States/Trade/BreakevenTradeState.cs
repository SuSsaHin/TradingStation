using TradeTools;
using TradeTools.Events;
using Utils.Events;

namespace StatesRobot.States.Trade
{
	class BreakevenTradeState : BasicTradeState
	{
		private bool hasBreakeven;

		public BreakevenTradeState(RobotContext context, Deal deal) : base(context, deal)
		{}

		public override ITradeEvent Process(RobotContext context, Candle candle)
		{
			var result = base.Process(context, candle);
			if (result != null)
				return result;

			if (!hasBreakeven &&
				(TrendIsLong && candle.High >= StartPrice + context.StopLossSize ||
				!TrendIsLong && candle.Low <= StartPrice - context.StopLossSize))
			{
				hasBreakeven = true;

				context.StopLossPrice = GetStopPrice(StartPrice, -context.BreakevenSize);

				context.Logger.Debug("Breakeven. StopPrice: {0}, Candle: {1}", context.StopLossPrice, candle);
				return new StopLossMovingEvent(context.StopLossPrice, TrendIsLong);
			}
			
			return null;
		}
	}
}
