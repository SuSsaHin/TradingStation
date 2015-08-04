using Utils.Events;

namespace TradeTools.Events
{
	public class StopLossMovingEvent:ITradeEvent
	{
		public int Price { get; private set; }
		public bool IsTrendLong { get; private set; }

		public StopLossMovingEvent(int price, bool isTrendLong)
		{
			IsTrendLong = isTrendLong;
			Price = price;
		}
	}
}
