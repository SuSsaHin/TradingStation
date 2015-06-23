using System.Collections.Generic;
using Utils.Types;

namespace Utils.Events
{
	public class SearchInfoEvent : ITradeEvent
	{
		public IReadOnlyList<Extremum> FirstLongExtremums { get; private set; }
		public IReadOnlyList<Extremum> FirstShortExtremums { get; private set; }

		public SearchInfoEvent(IReadOnlyList<Extremum> firstLongExtremums, IReadOnlyList<Extremum> firstShortExtremums)
		{
			FirstShortExtremums = firstShortExtremums;
			FirstLongExtremums = firstLongExtremums;
		}
	}
}
