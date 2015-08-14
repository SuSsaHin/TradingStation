using System;

namespace TradeTools
{
	public class Deal
	{
		public Advice Advice { get; }
		public Deal(int price, DateTime dateTime, bool isBuy, Advice advice = null)
		{
			Price = price;
			IsBuy = isBuy;
			DateTime = dateTime;
			Advice = advice;
		}

		public bool IsBuy { get; private set; }
		public int Price { get; private set; }
		public DateTime DateTime { get; private set; }
	}
}
