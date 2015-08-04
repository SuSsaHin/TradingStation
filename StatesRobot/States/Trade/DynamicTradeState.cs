using TradeTools;
using TradeTools.Events;
using Utils.Events;

namespace StatesRobot.States.Trade
{
	class DynamicTradeState : BreakevenTradeState
	{
		public DynamicTradeState(int startPrice, bool isTrendLong)
			: base(startPrice, isTrendLong)
		{
		}

		public override ITradeEvent Process(RobotContext context, Candle candle)
		{
			var result = base.Process(context, candle);
			if (result != null)
				return result;

			int dynamicPrice;
			if (IsTrendLong && candle.High >= (dynamicPrice = StartPrice + context.TrailingStopLoss) ||
				!IsTrendLong && candle.Low <= (dynamicPrice = StartPrice - context.TrailingStopLoss))
			{
				context.StopLoss = StartPrice - dynamicPrice;
				return new StopLossMovingEvent(dynamicPrice, IsTrendLong);
			}
			
			return null;
		}
	}
}
