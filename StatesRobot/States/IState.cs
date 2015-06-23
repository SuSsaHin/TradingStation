using Utils.Events;
using Utils.Types;

namespace StatesRobot.States
{
	interface IState
	{
		ITradeEvent Process(RobotContext context, Candle candle);
		ITradeEvent StopTrading(RobotContext context);
	}
}
