using System;
using System.Collections.Generic;
using System.Linq;
using StatesRobot.States.Search.Tools;
using TradeTools;
using TradeTools.Events;
using Utils;
using Utils.Events;

namespace StatesRobot.States.Search
{
	class SearchState : IState
	{
	    private readonly LinkedList<CandleNode> searchTree;
	    private readonly int pegTopSize;

	    internal readonly ExtremumsRepository ExtremumsRepo;

	    public SearchState(RobotContext context)
		{
			context.Logger.Debug("Change state to Search");

			ExtremumsRepo = new ExtremumsRepository();
			searchTree = new LinkedList<CandleNode>();

			for (int i = 0; i < context.Candles.Count; ++i)
			{
				AppendToTree(context.Candles[i], i);
			}

			pegTopSize = context.PegtopSize;
		}

		public ITradeEvent StopTrading(RobotContext context)
		{
			context.Logger.Debug("Stop trading command");
			context.CurrentState = context.Factory.GetEndState(context);
			return new EndEvent();
		}

		public ITradeEvent Process(RobotContext context, Candle candle)
		{
			int currentIndex = context.Candles.Count - 1;
			var bestSecondExtremum = AppendToTree(candle, currentIndex);

			if (bestSecondExtremum == null)
				return null;

			if (NeedToTrade(context, bestSecondExtremum))
			{
				context.Logger.Debug("Need to trade, extremum: {0}", bestSecondExtremum);
				var deal = new Deal(candle.Close, candle.DateTime, bestSecondExtremum.IsMinimum, context.Advisor.GetAdvice(candle.Close, bestSecondExtremum.IsMinimum));

				context.Logger.Debug("Deal: {0}", deal);
				context.CurrentState = context.Factory.GetTradeState(context, deal);
				return new DealEvent(deal);
			}

			return new SecondExtremumEvent(bestSecondExtremum, ExtremumsRepo.FirstMaximums, ExtremumsRepo.FirstMinimums);
		}

		private Extremum AppendToTree(Candle candle, int currentIndex)
		{
			var leftIter = searchTree.First;
			Extremum lastSecondExtremum = null;
			while (leftIter != null)
			{
				var midIter = leftIter.Value.Children.First;
				while (midIter != null)
				{
					if (IsSkipped(midIter.Value.Candle, candle))
					{
						midIter = midIter.Next;
						continue;
					}

					if (IsPegTop(leftIter.Value.Candle) && IsPegTop(midIter.Value.Candle) && IsPegTop(candle))
					{
						midIter = midIter.Next;
						continue;
					}

					var extremum = TryGetExtremum(leftIter.Value.Candle, midIter.Value.Candle, candle, currentIndex);
					if (extremum == null)
					{
						leftIter = searchTree.RemoveElement(leftIter);
						goto NextIteration;
					}

					var secondExtremum = ExtremumsRepo.AddExtremum(extremum);
					if (secondExtremum != null && (lastSecondExtremum == null || lastSecondExtremum.DateTime < secondExtremum.DateTime))
					{
						lastSecondExtremum = secondExtremum;
					}

					midIter = midIter.RemoveFromList();
				}

				if (!IsSkipped(leftIter.Value.Candle, candle))
				{
					leftIter.Value.Children.AddLast(new CandleNode(candle, currentIndex));
				}
				leftIter = leftIter.Next;
			NextIteration:;
			}

			searchTree.AddLast(new CandleNode(candle, currentIndex));
			return lastSecondExtremum;
		}

		private bool NeedToTrade(RobotContext context, Extremum secondExtremum)
		{
			var isTrendLong = IsTrendLong(context.Candles);
			context.Logger.Debug("NeedToTrade? extremum:{0}, trend:{1}", secondExtremum.IsMinimum, isTrendLong);
			return secondExtremum.IsMinimum == isTrendLong;
		}

		private bool IsSkipped(Candle previous, Candle current)
		{
			return current.IsOuterTo(previous) || current.IsInnerTo(previous);
		}

		private Extremum TryGetExtremum(Candle leftCandle, Candle midCandle, Candle rightCandle, int rightIndex)
		{
			var isMinimum = IsMinimum(leftCandle, midCandle, rightCandle);
			if (!isMinimum && !IsMaximum(leftCandle, midCandle, rightCandle))
				return null;

			return new Extremum(midCandle, rightIndex, isMinimum);
		}

		private bool IsMaximum(Candle left, Candle mid, Candle right)
		{
			return mid.High > left.High && mid.High > left.High && mid.Low >= left.Low && mid.Low >= right.Low;
		}

		private bool IsMinimum(Candle left, Candle mid, Candle right)
		{
			return mid.Low < left.Low && mid.Low < left.Low && mid.High <= left.High && mid.High <= right.High;
		}
		
		private bool IsTrendLong(IReadOnlyList<Candle> candles)
		{
			return candles[candles.Count - 1].Close > candles.First().Open;
		}

		private bool IsPegTop(Candle candle)
		{
			return Math.Abs(candle.Open - candle.Close) <= pegTopSize;
		}
	}
}
