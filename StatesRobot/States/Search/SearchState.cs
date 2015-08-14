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
		private readonly ExtremumsRepository extremumsRepo;
		private readonly LinkedList<CandleNode> searchTree;
		private readonly uint maxSkippedCandlesCount;
		private readonly int pegTopSize;

		public SearchState(RobotContext context)
		{
			extremumsRepo = new ExtremumsRepository();
			searchTree = new LinkedList<CandleNode>();

			for (int i = 0; i < context.Candles.Count; ++i)
			{
				AppendToTree(context.Candles[i], i);
			}
			maxSkippedCandlesCount = context.MaxSkippedCandlesCount;
			pegTopSize = context.PegtopSize;
		}

		public ITradeEvent StopTrading(RobotContext context)
		{
			context.CurrentState = context.Factory.GetEndState();
			return new EndEvent();
		}

		public ITradeEvent Process(RobotContext context, Candle candle)
		{
			int currentIndex = context.Candles.Count - 1;
			var bestSecondExtremum = AppendToTree(candle, currentIndex);

			if (bestSecondExtremum == null)
				return null;	//TODO SearchInfoEvent

			if (NeedToTrade(context, bestSecondExtremum))
			{
				var deal = new Deal(candle.Close, candle.DateTime, bestSecondExtremum.IsMinimum);
				context.CurrentState = context.Factory.GetTradeState(context, deal);
				return new DealEvent(deal);
			}

			return new SecondExtremumEvent(bestSecondExtremum, extremumsRepo.FirstMaximums, extremumsRepo.FirstMinimums);
		}

		private Extremum AppendToTree(Candle candle, int currentIndex)
		{
			var leftIter = searchTree.First;
			Extremum lastSecondExtremum = null;
			while (leftIter != null)
			{
				if (TryRemoveUnusefull(ref leftIter, currentIndex))
					continue;

				var midIter = leftIter.Value.Children.First;
				while (midIter != null)
				{
					if (TryRemoveUnusefull(ref midIter, currentIndex))
						continue;

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

					midIter = midIter.RemoveFromList();
					var secondExtremum = extremumsRepo.AddExtremum(extremum);
					if (secondExtremum != null && (lastSecondExtremum == null || lastSecondExtremum.DateTime < secondExtremum.DateTime))
					{
						lastSecondExtremum = secondExtremum;
					}
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

		private bool TryRemoveUnusefull(ref LinkedListNode<CandleNode> node, int currentIndex)
		{
			if (node.Value.HasChildren() || currentIndex - node.Value.CandleIndex <= maxSkippedCandlesCount)
				return false;

			node = node.RemoveFromList();
			return true;
		}

		private bool NeedToTrade(RobotContext context, Extremum secondExtremum)
		{
			return secondExtremum.IsMinimum == IsTrendLong(context.Candles);
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
