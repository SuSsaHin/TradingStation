using System;

namespace TradeTools
{
    public class Extremum
	{
		public readonly DateTime DateTime;
		public int Value { get; private set; }
		public int CheckerIndex { get; private set; }
		public bool IsMinimum { get; private set; }

		public bool CanBeSecond { get; set; }

		public Extremum(int value, int checkerIndex, DateTime dateTime, bool isMinimum)
		{
			DateTime = dateTime;
			Value = value;
			CheckerIndex = checkerIndex;
			IsMinimum = isMinimum;
			CanBeSecond = true;
		}

		public Extremum(Candle extremumCandle, int checkerIndex, bool isMinimum) 
			: this(isMinimum ? extremumCandle.Low : extremumCandle.High, checkerIndex, extremumCandle.DateTime, isMinimum)
		{ }

        public override string ToString()
        {
            return DateTime + " (" + CheckerIndex +  "): " + Value + ", " + (IsMinimum ? "min" : "max") + ", " + (CanBeSecond ? "Can be second" : "Can't be second");
        }
	}
}
