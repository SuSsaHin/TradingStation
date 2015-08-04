using System;
using System.Collections.Generic;
using StatesRobot.States;
using StatesRobot.States.Search;
using TradeTools;
using Utils.Events;

namespace StatesRobot
{
	public class RobotContext
	{
		private readonly CandlesFormer candlesFormer = new CandlesFormer();
		internal StatesFactory Factory { get; private set; }
		internal TradeAdvisor Advisor { get; private set; }
		internal int StopLoss { get; set; }
		internal int TrailingStopLoss { get; private set; }
		internal int BreakevenSize { get; private set; }
		internal int PegtopSize { get; private set; }
		internal TimeSpan EndTime { get; private set; }
		
		internal List<Candle> Candles { get; private set; }
		internal int MaxSkippedCandlesCount { get; private set; }

		internal IState CurrentState { get; set; }

		public RobotContext(TradeParams tradeParams, StatesFactory factory, TradeAdvisor advisor, List<Candle> history)
		{
			Candles = history;
			Advisor = advisor;
			Factory = factory;

			StopLoss = tradeParams.StopLoss;
			TrailingStopLoss = (int) (StopLoss*tradeParams.TrailingStopPercent);
			BreakevenSize = (int) (StopLoss*tradeParams.BreakevenPercent);
			PegtopSize = tradeParams.PegtopSize;
			EndTime = tradeParams.EndTime;
			MaxSkippedCandlesCount = tradeParams.MaxSkippedCandlesCount;

			CurrentState = new SearchState(this);
		}

		public ITradeEvent Process(Candle candle)
		{
			var result = CurrentState.Process(this, candle);
			Advisor.AddCandle(candle);	//TODO specify order
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
