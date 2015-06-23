using System;
using System.Globalization;
using Utils.Types;

namespace Utils
{
	public static class Utils
	{
		public static bool IsInner(this Candle current, Candle previous)
		{
			return current.High < previous.High && current.Low > previous.Low;
		}

		public static bool IsOuter(this Candle current, Candle previous)
		{
			return current.High > previous.High && current.Low < previous.Low;
		}

		public static int Middle(this Candle candle)
		{
			return (candle.Close + candle.Open)/2;
		}

		public static string ToEnString(this double num, int decimals = 6)
		{
			return Math.Round(num, decimals).ToString(new CultureInfo("en-us"));
		}
	}
}
