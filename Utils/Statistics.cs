using System;
using System.Collections.Generic;
using System.Linq;

namespace Utils
{
	public static class Statistics
	{
		public static double StandardDeviation<T>(List<T> array)
		{
			return Math.Sqrt(Dispersion(array));
		}
		public static double Dispersion<T>(List<T> array)
		{
			var avg = array.Average(c =>
			{
				dynamic t = c;
				return (double) t;
			});
			var quads = array.Sum(c =>
			{
				dynamic t = c;
				double d = (double) t;
				return (d - avg)*(d - avg);
			});

			return quads/array.Count;
		}

		public static double Median(List<int> array)
		{
			var ordered = array.OrderBy(c => c).ToList();
			return array.Count % 2 == 1
				? ordered[array.Count / 2 + 1]
				: (ordered[array.Count / 2] + ordered[array.Count / 2 + 1]) / 2.0;
		}

		public static double Median(List<double> array)
		{
			var ordered = array.OrderBy(c => c).ToList();
			return array.Count % 2 == 1
				? ordered[array.Count / 2 + 1]
				: (ordered[array.Count / 2] + ordered[array.Count / 2 + 1]) / 2.0;
		}
	}
}
