namespace StatesRobot
{
	
	public class TradeParams
	{
		//TODO Params
		[FieldName("StopLoss")]
		public int StopLoss;
	
		[FieldName("TrailingStopPercent")]
		public decimal TrailingStopPercent;

		[FieldName("Pegtop")]
		public decimal PegtopSize;
	}
}
