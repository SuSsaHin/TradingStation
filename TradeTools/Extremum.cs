using System;

namespace TradeTools
{
    public class Extremum
	{
		public readonly DateTime DateTime;
		public int Value { get; }
		public int CheckerIndex { get; }
		public bool IsMinimum { get; }
		
		public Extremum(int value, int checkerIndex, DateTime dateTime, bool isMinimum)
		{
			DateTime = dateTime;
			Value = value;
			CheckerIndex = checkerIndex;
			IsMinimum = isMinimum;
		}

		public Extremum(Candle extremumCandle, int checkerIndex, bool isMinimum) 
			: this(isMinimum ? extremumCandle.Low : extremumCandle.High, checkerIndex, extremumCandle.DateTime, isMinimum)
		{ }

	    public override string ToString()
	    {
		    var direction = IsMinimum ? "minimum" : "maxaximum";
		    return $"{DateTime}({CheckerIndex}): {Value}, {direction}";
	    }
	}
}
