using System.Collections.Generic;
using Utils.Events;

namespace TradeTools.Events
{
	public class DealEvent : ITradeEvent
	{
		public bool IsTrendLong { get; private set; }
		public int Price { get; private set; }
		public Extremum SecondExtremum { get; private set; }
		public IReadOnlyList<Extremum> FirstLongExtremums { get; private set; }
		public IReadOnlyList<Extremum> FirstShortExtremums { get; private set; }

		/// <summary>
		/// 
		/// </summary>
		/// <param name="isTrendLong"></param>
		/// <param name="price"></param>
		/// <param name="secondExtremum">DEBUG</param>
		/// <param name="firstLongExtremums">DEBUG</param>
		/// <param name="firstShortExtremums">DEBUG</param>
		public DealEvent(bool isTrendLong, int price, Extremum secondExtremum = null, IReadOnlyList<Extremum> firstLongExtremums = null, IReadOnlyList<Extremum> firstShortExtremums = null)
		{
			FirstShortExtremums = firstShortExtremums;
			FirstLongExtremums = firstLongExtremums;
			SecondExtremum = secondExtremum;
			IsTrendLong = isTrendLong;
			Price = price;
		}
	}
}
