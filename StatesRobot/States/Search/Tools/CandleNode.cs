using System.Collections.Generic;
using System.Linq;
using TradeTools;

namespace StatesRobot.States.Search.Tools
{
	class CandleNode
	{
		public Candle Candle { get; private set; }
		public int CandleIndex { get; private set; }
		public LinkedList<CandleNode> Children { get; private set; }

		public CandleNode(Candle candle, int candleIndex)
		{
			Candle = candle;
			CandleIndex = candleIndex;
			Children = new LinkedList<CandleNode>();
		}

		public bool HasChildren()
		{
			return Children.Any();
		}
	}
}
