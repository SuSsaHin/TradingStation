using System.Collections.Generic;
using Utils.Events;

namespace TradeTools.Events
{
	public class SecondExtremumEvent : ITradeEvent
	{
		public SecondExtremumEvent(Extremum extremum, ICollection<Extremum> firstLongExtremums, ICollection<Extremum> firstShortExtremums)
		{
			FirstShortExtremums = firstShortExtremums;
			FirstLongExtremums = firstLongExtremums;
			Extremum = extremum;
		}

		public Extremum Extremum { get; private set; }
		public ICollection<Extremum> FirstLongExtremums { get; private set; }
		public ICollection<Extremum> FirstShortExtremums { get; private set; }
	}
}
