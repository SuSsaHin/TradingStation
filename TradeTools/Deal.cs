using System;

namespace TradeTools
{
	public class Deal
	{
		public Advice Advice { get; }
		public bool IsBuy { get; }
		public int Price { get; }
		public DateTime DateTime { get; }

		public Deal(int price, DateTime dateTime, bool isBuy, Advice advice = null)
		{
			Price = price;
			IsBuy = isBuy;
			DateTime = dateTime;
			Advice = advice;
		}

		public override string ToString() => $"{DateTime}: {Price} {(IsBuy ? "buy" : "sell")}";
	}
}
