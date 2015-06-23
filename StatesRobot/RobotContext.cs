using System.Collections.Generic;
using StatesRobot.States;
using Utils.Events;
using Utils.Types;

namespace StatesRobot
{
	public class RobotContext
	{
		private readonly CandlesFormer candlesFormer = new CandlesFormer();
		//TODO Advisor
		internal int StopLoss { get; set; }
		internal double BreakevenPercent { get; private set; }
		internal int PegTopSize { get; private set; }

		internal List<Candle> Candles { get; private set; }

		internal IState CurrentState { get; set; }

		public RobotContext(int stopLoss, double breakevenPercent, int pegTopSize, List<Candle> history)	//TODO Params, Fabric, History
		{
			PegTopSize = pegTopSize;
			BreakevenPercent = breakevenPercent;
			StopLoss = stopLoss;

			Candles = history;
			CurrentState = new SearchState(this);
		}

		public ITradeEvent Process(Candle candle)
		{
			var result = CurrentState.Process(this, candle);
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
