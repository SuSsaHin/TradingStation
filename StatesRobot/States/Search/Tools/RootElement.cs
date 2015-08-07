using System.Collections.Generic;
using System.Linq;
using TradeTools;

namespace StatesRobot.States.Search.Tools
{
	class RootElement
	{
		public IndexedCandle Candle { get; private set; }
		public LinkedList<IndexedCandle> Children { get; private set; }

		public RootElement(Candle candle, int candleIndex)
		{
			Candle = new IndexedCandle(candle, candleIndex);
			Children = new LinkedList<IndexedCandle>();
		}

		public bool HasChildren()
		{
			return Children.Any();
		}
	}
}
