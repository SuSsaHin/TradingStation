using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Utils.Types
{
	public class TradesResult //TODO вынести в тестер
	{
		private readonly List<Deal> deals = new List<Deal>();

		private const int startDepoSize = 30000;

		private int globalMaximumIndex;

		private int depoSize = startDepoSize;
		private int maxDepoSize = startDepoSize;

		public double MaxDropdown { get; private set; }
		public int MaxDropdownLength { get; private set; }
		public int Profit 
		{
			get { return deals.Sum(d => d.Profit); } 
		}
		public int DealsCount { get { return deals.Count; } }

		public int GoodCount 
		{
			get { return deals.Count(d => d.IsGood); } 
		}

		public int BadCount
		{
			get { return deals.Count(d => !d.IsGood); }
		}

		public int Volume
		{
			get { return deals.Sum(deal => Math.Abs(deal.Profit)); }
		}

		public int MaxLoss
		{
			get
			{
				var badDeals = deals.Where(d => !d.IsGood).ToList();
				return badDeals.Any() ? Math.Abs(badDeals.Min(d => d.Profit)) : 0;
			}
		}

		public int MaxProfit
		{
			get
			{
				var goodDeals = deals.Where(d => d.IsGood).ToList();
				return goodDeals.Any() ? goodDeals.Max(d => d.Profit) : 0;
			}
		}

		public double ProfitAverage
		{
			get
			{
				var goodDeals = deals.Where(d => d.IsGood).ToList();
				return goodDeals.Any() ? goodDeals.Average(d => d.Profit) : 0;
			}
		}

		public double LossAverage
		{
			get
			{
				var badDeals = deals.Where(d => !d.IsGood).ToList();
				return badDeals.Any() ? badDeals.Average(d => d.Profit) : 0;
			}
		}

		public int LongGoodCount
		{
			get { return deals.Count(d => d.IsGood && d.IsTrendLong); }
		}

		public int ShortGoodCount
		{
			get { return deals.Count(d => d.IsGood && !d.IsTrendLong); }
		}

	    public static List<string> GetHeaders()
	    {
            return new List<string>{"Good", "Bad", "Profit", "Volume", "Profit percent", 
                                    "Max loss", "Max profit", "Max dropdown", "Max dropdown length", 
                                    "Profit average", "Loss average", 
                                    "Long good", "Short good"};
	    }

	    public List<string> GetTableRow()
	    {
            return new List<string>{GoodCount.ToString(), BadCount.ToString(), Profit.ToString(), Volume.ToString(), (100.0*Profit / Volume).ToEnString(3), 
								MaxLoss.ToString(), MaxProfit.ToString(), MaxDropdown.ToEnString(2), MaxDropdownLength.ToString(),
								ProfitAverage.ToEnString(2), LossAverage.ToEnString(2), 
                                LongGoodCount.ToString(), ShortGoodCount.ToString()};
	    }

		public override string ToString()
		{
			return string.Format("Good: {0}, Bad: {1}, Profit: {2}, Volume: {3}, Profit percent: {4},\n" +
								 "Max loss: {5}, Max profit: {6}, Max dropdown: {7}, Max dropdown length: {8},\n" +
			                     "Profit average: {9}, Loss average: {10}, Long good: {11}, short good: {12}", 
								GoodCount, BadCount, Profit, Volume, (100.0*Profit / Volume).ToEnString(3), 
								MaxLoss, MaxProfit, MaxDropdown.ToEnString(2), MaxDropdownLength,
								ProfitAverage.ToEnString(2), LossAverage.ToEnString(2), LongGoodCount, ShortGoodCount);
		}

		public void AddDeal(Deal deal)
		{
			deals.Add(deal);

			depoSize += deal.Profit;
			if (depoSize >= maxDepoSize)
			{
				maxDepoSize = depoSize;
				globalMaximumIndex = deals.Count - 1;
			}
			else
			{
				double currentDropdown = 100 * (maxDepoSize - depoSize) / (double)(maxDepoSize);
				int currentDropdownLength = deals.Count - 1 - globalMaximumIndex;
				MaxDropdown = Math.Max(currentDropdown, MaxDropdown);
				MaxDropdownLength = Math.Max(currentDropdownLength, MaxDropdownLength);
			}
		}

		public void PrintDeals()
		{
			for (int i = 0; i < deals.Count; ++i)
			{
				Console.WriteLine("{0}: {1}", i, deals[i].Profit);
			}
		}

		public void PrintDeals(string filename)
		{
			File.WriteAllLines(filename, deals.ConvertAll(d => d.Profit.ToString()));
		}

		public void PrintDepo(string filename)
		{
			var depo = new List<int>{0};
			int sum = 0;
			foreach (var deal in deals)
			{
				sum += deal.Profit;
				depo.Add(sum);
			}
			File.WriteAllLines(filename, depo.ConvertAll(d => d.ToString()));
		}
	}
}
