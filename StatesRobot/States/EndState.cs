using Utils.Events;
using Utils.Types;

namespace StatesRobot.States
{
	class EndState : IState
	{
		public ITradeEvent Process(RobotContext context, Candle candle)
		{
			return new EndEvent();
		}

		public ITradeEvent StopTrading(RobotContext context)
		{
			return new EndEvent();
		}
	}
}
