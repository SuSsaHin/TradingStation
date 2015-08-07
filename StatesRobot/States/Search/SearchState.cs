using System.Collections.Generic;
using System.Linq;
using StatesRobot.States.End;
using StatesRobot.States.Search.Tools;
using TradeTools;
using TradeTools.Events;
using Utils;
using Utils.Events;

namespace StatesRobot.States.Search	//TODO !!документация
{
	class SearchState : IState
	{
		private readonly LinkedList<CandleNode> searchTree;
		private readonly int maxSkippedCandlesCount;

		public SearchState(RobotContext context)
		{
			searchTree = new LinkedList<CandleNode>();
			for (int i = 0; i < context.Candles.Count; ++i)
			{
				AppendToTree(context.Candles[i], i);
			}
			maxSkippedCandlesCount = context.MaxSkippedCandlesCount;
		}

		public ITradeEvent StopTrading(RobotContext context)
		{
			context.CurrentState = new EndState();
			return new EndEvent();
		}

		public ITradeEvent Process(RobotContext context, Candle candle)
		{
			int currentIndex = context.Candles.Count;
			var bestSecondExtremum = AppendToTree(candle, currentIndex);

			if (bestSecondExtremum == null)
				return null;	//TODO SearchInfoEvent

			if (NeedToTrade(context, bestSecondExtremum))
				return new DealEvent(new Deal(candle.Close, candle.DateTime, bestSecondExtremum.IsMinimum));

			return new SecondExtremumEvent(secondExtremum, extremumsRepo.FirstMaximums, extremumsRepo.FirstMinimums);
		}

		private Extremum AppendToTree(Candle candle, int currentIndex)
		{
			var leftIter = searchTree.First;
			Extremum bestSecondExtremum = null;
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
						continue;

					var extremum = TryGetExtremum(leftIter.Value.Candle, midIter.Value.Candle, candle, currentIndex);
					if (extremum == null)
					{
						leftIter = searchTree.RemoveElement(leftIter);
						goto NextIteration;
					}

					var secondExtremum = extremumsRepo.AddExtremum(extremum);
					if (secondExtremum != null && (bestSecondExtremum == null || bestSecondExtremum.DateTime < secondExtremum.DateTime))
					{
						bestSecondExtremum = secondExtremum;
					}
				}

				if (IsSkipped(leftIter.Value.Candle, candle))
				{
					leftIter.Value.Children.AddLast(new CandleNode(candle, currentIndex));
				}
				leftIter = leftIter.Next;
			NextIteration:;
			}

			searchTree.AddLast(new CandleNode(candle, currentIndex));
			return bestSecondExtremum;
		}

		private bool TryRemoveUnusefull(ref LinkedListNode<CandleNode> node, int currentIndex)
		{
			if (node.Value.HasChildren() || currentIndex - node.Value.CandleIndex < maxSkippedCandlesCount)
				return false;

			node = node.List.RemoveElement(node);
			return true;
		}

		private bool NeedToTrade(RobotContext context, Extremum secondExtremum)
		{
			return secondExtremum.IsMinimum == IsTrendLong(context.Candles);
		}

		private bool IsSkipped(Candle previous, Candle current)
		{
			return !current.IsOuterTo(previous) && !current.IsInnerTo(previous);
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
	}
}
