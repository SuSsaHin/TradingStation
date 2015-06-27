using System.Collections.Generic;
using StatesRobot.States;
using StatesRobot.States.Search;
using Utils.Events;
using Utils.Types;

namespace StatesRobot
{
	public class RobotContext
	{
		private readonly CandlesFormer candlesFormer = new CandlesFormer();
		internal StatesFactory Factory { get; private set; }
		internal TradeAdvisor Advisor { get; private set; }
		internal int StopLoss { get; set; }
		internal double BreakevenPercent { get; private set; }
		internal int PegTopSize { get; private set; }
		
		internal List<Candle> Candles { get; private set; }

		internal IState CurrentState { get; set; }

		public RobotContext(TradeParams tradeParams, StatesFactory factory, TradeAdvisor advisor, List<Candle> history)	//TODO Params
		{
			Candles = history;
			Advisor = advisor;
			Factory = factory;

			CurrentState = new SearchState(this);
		}

		public ITradeEvent Process(Candle candle)
		{
			var result = CurrentState.Process(this, candle);
			Advisor.AddCandle(candle);
			Candles.Add(candle);
			return result;
		}

		public ITradeEvent Process(Tick tick)
		{
			return Process(candlesFormer.AddTick(tick));
		}

		public ITradeEvent StopTrading()
		{
			return CurrentState.StopTrading(this);
		}
	}
}
