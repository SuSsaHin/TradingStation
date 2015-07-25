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
			Dynamic
		}

		private readonly TradeStateTypes tradeStateType;

		public StatesFactory(TradeStateTypes tradeStateType)
		{
			this.tradeStateType = tradeStateType;
		}

		public IState GetTradeState(int startPrice, bool isTrendLong, RobotContext context)
		{
			switch (tradeStateType)
			{
				case TradeStateTypes.Basic:
					return new BasicTradeState(startPrice, isTrendLong);
				case TradeStateTypes.Breakeven:
					return new BreakevenTradeState(startPrice, isTrendLong);
				case TradeStateTypes.Dynamic:
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
