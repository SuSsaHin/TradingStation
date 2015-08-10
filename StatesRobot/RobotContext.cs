using System;
using System.Collections.Generic;
using StatesRobot.States;
using TradeTools;
using Utils.Events;

namespace StatesRobot
{
	public class RobotContext
	{
		private readonly List<Candle> candles;
		internal StatesFactory Factory { get; }
		internal TradeAdvisor Advisor { get; }
		internal int StopLossPrice { get; set; }
		internal int StopLossSize { get; }
		internal int TrailingStopLoss { get;}
		internal int BreakevenSize { get; }
		internal int PegtopSize { get; }
		internal TimeSpan EndTime { get; }

		internal IReadOnlyList<Candle> Candles => candles;

		internal int MaxSkippedCandlesCount { get; private set; }

		internal IState CurrentState { get; set; }

		public RobotContext(TradeParams tradeParams, StatesFactory factory, TradeAdvisor advisor, List<Candle> history = null)
		{
			candles = history ?? new List<Candle>();	//IMPROVE историю хранить не нужно
			Advisor = advisor;
			Factory = factory;

			StopLossSize = tradeParams.StopLoss;
			TrailingStopLoss = (int) (StopLossSize * tradeParams.TrailingStopPercent);
			BreakevenSize = (int) (StopLossSize * tradeParams.BreakevenPercent);
			PegtopSize = tradeParams.PegtopSize;
			EndTime = tradeParams.EndTime;
			MaxSkippedCandlesCount = tradeParams.MaxSkippedCandlesCount;

			InitState();
		}

		public ITradeEvent Process(Candle candle)
		{
			Advisor.AddCandle(candle);
			candles.Add(candle);
			var result = CurrentState.Process(this, candle);
			return result;
		}

		public ITradeEvent StopTrading()
		{
			return CurrentState.StopTrading(this);
		}

		public void Reset()
		{
			StopLossPrice = 0;
			candles.Clear();
			InitState();
		}

		private void InitState()
		{
			CurrentState = Factory.GetSearchState(this);
		}
	}
}
