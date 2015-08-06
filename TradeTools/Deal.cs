using System;

namespace TradeTools
{
	public class Deal
	{
		public Deal(int price, DateTime dateTime, bool isBuy)
		{
			Price = price;
			IsBuy = isBuy;
			DateTime = dateTime;
		}

		public bool IsBuy { get; private set; }
		public int Price { get; private set; }
		public DateTime DateTime { get; private set; }
	}
}
