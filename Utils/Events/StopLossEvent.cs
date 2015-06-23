namespace Utils.Events
{
	public class StopLossEvent : ITradeEvent
	{
		public bool IsTrendLong { get; private set; }
		public int Price { get; private set; }

		public StopLossEvent(bool isTrendLong, int price)
		{
			Price = price;
			IsTrendLong = isTrendLong;
		}
	}
}
