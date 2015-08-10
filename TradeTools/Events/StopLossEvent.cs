using Utils.Events;

namespace TradeTools.Events
{
	public class StopLossEvent : DealEvent
	{

		public StopLossEvent(Deal deal) : base(deal)
		{}
	}
}
