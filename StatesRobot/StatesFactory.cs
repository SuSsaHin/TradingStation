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
			Classic,
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
				case TradeStateTypes.Breakeven:
					throw new NotImplementedException("Not implemented trade state type");
				case TradeStateTypes.Dynamic:
					throw new NotImplementedException("Not implemented trade state type");
				case TradeStateTypes.Classic:
					return new BasicTradeState(startPrice, isTrendLong);
				default:
					throw new NotImplementedException("Not expected trade state type");
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
