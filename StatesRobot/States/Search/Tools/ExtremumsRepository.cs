using System;
using System.Collections.Generic;
using TradeTools;
using Wintellect.PowerCollections;

namespace StatesRobot.States.Search.Tools
{
	internal class ExtremumsRepository
	{
		private readonly OrderedSet<Extremum> firstMaximums;
		private readonly OrderedSet<Extremum> firstMinimums;

		private readonly OrderedSet<Extremum> secondMaximums;
		private readonly OrderedSet<Extremum> secondMinimums;

		public ICollection<Extremum> FirstMaximums { get { return firstMaximums.AsReadOnly(); } }

		public ICollection<Extremum> FirstMinimums { get { return firstMinimums.AsReadOnly(); } }

		public ExtremumsRepository()
		{
			firstMaximums = new OrderedSet<Extremum>(CompareExtremums);
			firstMinimums = new OrderedSet<Extremum>(CompareExtremums);

			secondMaximums = new OrderedSet<Extremum>(CompareExtremums);
			secondMinimums = new OrderedSet<Extremum>(CompareExtremums);
		}

		public Extremum AddExtremum(Extremum extremum)
		{
			var firstExtremums = extremum.IsMinimum ? firstMinimums : firstMaximums;

			if (firstExtremums.Contains(extremum))
				return null;

			firstExtremums.Add(extremum);

			if (CompareExtremums(firstExtremums.GetLast(), extremum) != 0)
				return null;

			if (firstExtremums.Count < 3)
				return null;

			if (extremum.IsMinimum)
				return TryGetSecondMinimum();

			return TryGetSecondMaximum();
		}

		private Extremum TryGetSecondMaximum()
		{
			int last = firstMinimums.Count - 1;
			if (firstMinimums[last - 1].Value > firstMinimums[last].Value &&
					(firstMinimums[last - 1].Value > firstMinimums[last - 2].Value ||
					firstMinimums[last - 1].Value == firstMinimums[last - 2].Value &&
					last - 3 >= 0 && firstMinimums[last - 1].Value > firstMinimums[last - 3].Value))
				return SaveSecondExtremum(firstMinimums[last - 1], firstMinimums[last]);

			return null;
		}

		private Extremum TryGetSecondMinimum()
		{
			int last = firstMinimums.Count - 1;
			if (firstMinimums[last - 1].Value < firstMinimums[last].Value &&
					(firstMinimums[last - 1].Value < firstMinimums[last - 2].Value ||
					firstMinimums[last - 1].Value == firstMinimums[last - 2].Value &&
					last - 3 >= 0 && firstMinimums[last - 1].Value < firstMinimums[last - 3].Value))
				return SaveSecondExtremum(firstMinimums[last - 1], firstMinimums[last]);

			return null;
		}

		private Extremum SaveSecondExtremum(Extremum mid, Extremum right)
		{
			var extremum = new Extremum(mid.Value, right.CheckerIndex, mid.DateTime, mid.IsMinimum);
			(mid.IsMinimum ? secondMinimums : secondMaximums).Add(extremum);
			return extremum;
		}

		private static int CompareExtremums(Extremum left, Extremum right)
		{
			if (left.DateTime > right.DateTime)
				return 1;

			if (left.DateTime < right.DateTime)
				return -1;

			if (left.CheckerIndex > right.CheckerIndex)
				return 1;

			if (left.CheckerIndex < right.CheckerIndex)
				return -1;

			return 0;
		}
	}
}