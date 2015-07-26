using System;
using StatesRobot.States;
using StatesRobot.States.End;
using StatesRobot.States.Search;
using StatesRobot.States.Trade;

namespace StatesRobot
{
	public class StatesFactory
	{
		public enum TradeStateTypes
		{
			Basic,
			Breakeven,
			Trailing
		}

		public TradeStateTypes TradeStateType { get; private set; }

		public StatesFactory(TradeStateTypes tradeStateType)
		{
			this.TradeStateType = tradeStateType;
		}

		public IState GetTradeState(int startPrice, bool isTrendLong, RobotContext context)
		{
			switch (TradeStateType)
			{
				case TradeStateTypes.Basic:
					return new BasicTradeState(startPrice, isTrendLong);
				case TradeStateTypes.Breakeven:
					return new BreakevenTradeState(startPrice, isTrendLong);
				case TradeStateTypes.Trailing:
					return new DynamicTradeState(startPrice, isTrendLong);
				default:
					throw new ArgumentException("Not expected trade state type");
			}
		}

		public IState GetSearchState(RobotContext context)
		{
			return new SearchState(context);
		}

		public IState GetEndState()
		{
			return new EndState();
		}
	}
}
