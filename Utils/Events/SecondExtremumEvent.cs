using System.Collections.Generic;
using Utils.Types;

namespace Utils.Events
{
	public class SecondExtremumEvent : ITradeEvent
	{
		public SecondExtremumEvent(Extremum extremum, IReadOnlyList<Extremum> firstLongExtremums, IReadOnlyList<Extremum> firstShortExtremums)
		{
			FirstShortExtremums = firstShortExtremums;
			FirstLongExtremums = firstLongExtremums;
			Extremum = extremum;
		}

		public Extremum Extremum { get; private set; }
		public IReadOnlyList<Extremum> FirstLongExtremums { get; private set; }
		public IReadOnlyList<Extremum> FirstShortExtremums { get; private set; }
	}
}
