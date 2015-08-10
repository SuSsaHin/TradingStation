using Utils.Events;

namespace TradeTools.Events
{
	public class StopLossMovingEvent:ITradeEvent
	{
		public int Price { get; private set; }
		public bool TrendIsLong { get; private set; }

		public StopLossMovingEvent(int price, bool trendIsLong)
		{
			TrendIsLong = trendIsLong;
			Price = price;
		}
	}
}
