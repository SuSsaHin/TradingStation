namespace Utils.Events
{
	public class BreakevenEvent:ITradeEvent
	{
		public int Price { get; private set; }
		public bool IsTrendLong { get; private set; }

		public BreakevenEvent(int price, bool isTrendLong)
		{
			IsTrendLong = isTrendLong;
			Price = price;
		}
	}
}
