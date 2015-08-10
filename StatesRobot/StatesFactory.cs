using System;
using StatesRobot.States;
using StatesRobot.States.End;
using StatesRobot.States.Search;
using StatesRobot.States.Trade;
using TradeTools;

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

		public TradeStateTypes TradeStateType { get; }

		public StatesFactory(TradeStateTypes tradeStateType)
		{
			TradeStateType = tradeStateType;
		}

		public IState GetTradeState(RobotContext context, Deal deal)
		{
			switch (TradeStateType)
			{
				case TradeStateTypes.Basic:
					return new BasicTradeState(context, deal);
				case TradeStateTypes.Breakeven:
					return new BreakevenTradeState(context, deal);
				case TradeStateTypes.Trailing:
					return new TrailingTradeState(context, deal);
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
