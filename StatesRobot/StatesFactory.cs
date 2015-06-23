using StatesRobot.States;

namespace StatesRobot
{
	class StatesFactory
	{
		internal enum TradeStates
		{
			Classic,
			Breakeven,
			Dynamic
		}

		private TradeStates tradeState;

		private 

		public StatesFactory(TradeStates tradeState, TradeParams tradeParams)
		{
			this.tradeState = tradeState;
		}

		public IState GetTradeState()
		{
			
		}

		public IState GetSearchState()
		{
			
		}

		public IState GetEndState()
		{
			return new EndState();
		}
	}
}
