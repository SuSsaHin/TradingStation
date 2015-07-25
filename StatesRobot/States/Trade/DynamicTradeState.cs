using Utils.Events;
using Utils.Types;

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
			if (IsTrendLong && candle.High >= (dynamicPrice = StartPrice + context.DynamicStopLoss) ||
				!IsTrendLong && candle.Low <= (dynamicPrice = StartPrice - context.DynamicStopLoss))
			{
				context.StopLoss = StartPrice - dynamicPrice;
				return new StopLossMovingEvent(dynamicPrice, IsTrendLong);
			}
			
			return null;
		}
	}
}
