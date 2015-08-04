using TradeTools;
using Utils.Events;

namespace StatesRobot.States
{
	public interface IState
	{
		ITradeEvent Process(RobotContext context, Candle candle);
		ITradeEvent StopTrading(RobotContext context);
	}
}
