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
		private readonly List<Candle> candles;
		internal StatesFactory Factory { get; }
		internal TradeAdvisor Advisor { get; }
	    public int StopLossPrice { get; set; }
		internal int StopLossSize { get; }
		internal int TrailingStopLoss { get;}
		internal int BreakevenSize { get; }
		internal int PegtopSize { get; }
		internal TimeSpan EndTime { get; }
		internal IState CurrentState { get; set; }

		public IReadOnlyList<Candle> Candles => candles;
		public IExtremumsRepository ExtremumsRepository { get; }

		private IState DefaultState => Factory.GetSearchState(this);

        public RobotContext(TradeParams tradeParams, StatesFactory factory, TradeAdvisor advisor, List<Candle> history = null)
		{
			candles = history ?? new List<Candle>();	//IMPROVE историю хранить не нужно (отправлять за историей к Advisor)
			Advisor = advisor;
			Factory = factory;

			StopLossSize = tradeParams.StopLoss;
			TrailingStopLoss = (int) (StopLossSize * tradeParams.TrailingStopPercent);
			BreakevenSize = (int) (StopLossSize * tradeParams.BreakevenPercent);
			PegtopSize = tradeParams.PegtopSize;
			EndTime = tradeParams.EndTime;

            CurrentState = DefaultState;
            ExtremumsRepository = ((SearchState) CurrentState).ExtremumsRepo;
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
            CurrentState = DefaultState;
        }
	}
}
