using Utils.Events;
using Utils.Types;

namespace StatesRobot.States.Trade
{
	class BreakevenTradeState : BasicTradeState
	{
		private bool hasBreakeven;

		public BreakevenTradeState(int startPrice, bool isTrendLong)
			: base(startPrice, isTrendLong)
		{
		}

		public override ITradeEvent Process(RobotContext context, Candle candle)
		{
			var result = base.Process(context, candle);
			if (result != null)
				return result;

			if (!hasBreakeven &&
				(IsTrendLong && candle.High >= StartPrice + context.StopLoss ||
				!IsTrendLong && candle.Low <= StartPrice - context.StopLoss))
			{
				hasBreakeven = true;

				context.StopLoss = -context.BreakevenSize;
				return new StopLossMovingEvent(IsTrendLong ? StartPrice - context.StopLoss : StartPrice + context.StopLoss, IsTrendLong);
			}
			
			return null;
		}
	}
}
