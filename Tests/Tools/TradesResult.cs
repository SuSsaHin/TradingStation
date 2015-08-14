using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using MathNet.Numerics.Distributions;
using MathNet.Numerics.Statistics;
using TradeTools;
using Utils;
using Utils.XmlProcessing;

namespace Tests.Tools
{
	internal class TradesResult 	//TODO FieldNames
	{
		private readonly List<Trade> trades = new List<Trade>();
		private readonly Stack<Deal> deals = new Stack<Deal>(); 
		
		private const int startDepoSize = 30000;	//TODO config

		private int globalMaximumIndex;

		private int depoSize = startDepoSize;
		private int maxDepoSize = startDepoSize;

		public const int Comission = 30;
		public IReadOnlyList<Trade> Trades => trades;

		[PropName("MaxDropdownPercent")]
		public double MaxDropdownPercent { get; private set; }

		[PropName("MaxDropdownLength")]
		public int MaxDropdownLength { get; private set; }

		[PropName("Profit")]
		public int Profit => trades.Sum(d => d.Profit);

		[PropName("ProfitMean")]
		public double ProfitMean => trades.Average(d => d.Profit);

		[PropName("DealsCount")]
		public int DealsCount => trades.Count;

		[PropName("GoodCount")]
		public int GoodCount => trades.Count(d => d.IsGood);

		public int BadCount => trades.Count(d => !d.IsGood);

		public int Volume => trades.Sum(deal => Math.Abs(deal.Profit));

		[PropName("MaxLoss")]
		public int MaxLoss
		{
			get
			{
				var badDeals = trades.Where(d => !d.IsGood).ToList();
				return badDeals.Any() ? Math.Abs(badDeals.Min(d => d.Profit)) : 0;
			}
		}

		public int MaxProfit
		{
			get
			{
				var goodDeals = trades.Where(d => d.IsGood).ToList();
				return goodDeals.Any() ? goodDeals.Max(d => d.Profit) : 0;
			}
		}

		[PropName("GoodDealsAverage")]
		public double GoodDealsAverage
		{
			get
			{
				var goodDeals = trades.Where(d => d.IsGood).ToList();
				return goodDeals.Any() ? goodDeals.Average(d => d.Profit) : 0;
			}
		}

		public double LossAverage
		{
			get
			{
				var badDeals = trades.Where(d => !d.IsGood).ToList();
				return badDeals.Any() ? badDeals.Average(d => d.Profit) : 0;
			}
		}

		public int LongGoodCount => trades.Count(d => d.IsGood && d.IsLong);

		public int ShortGoodCount => trades.Count(d => d.IsGood && !d.IsLong);

		public bool DealsAreClosed => !deals.Any();

		[PropName("ProbabilityLevel")]
		public double ProbabilityLevel => Math.Round(GetProbabilityLevel(), 1);

		public static List<string> GetHeaders() //TODO FieldNames
	    {
            return new List<string>{"Good", "Bad", "Profit", "Volume", "Profit percent", 
                                    "Max loss", "Max profit", "Max dropdown", "Max dropdown length", 
                                    "Profit average", "Loss average", 
                                    "Long good", "Short good"};
	    }

	    public List<string> GetTableRow()
	    {
            return new List<string>{GoodCount.ToString(), BadCount.ToString(), Profit.ToString(), Volume.ToString(), (100.0*Profit / Volume).ToEnString(), 
								MaxLoss.ToString(), MaxProfit.ToString(), MaxDropdownPercent.ToEnString(2), MaxDropdownLength.ToString(),
								GoodDealsAverage.ToEnString(2), LossAverage.ToEnString(2), 
                                LongGoodCount.ToString(), ShortGoodCount.ToString()};
	    }

		public override string ToString()
		{
			return
				$"Good: {GoodCount}, Bad: {BadCount}, Profit: {Profit}, Volume: {Volume}, Profit percent: {(100.0*Profit/Volume).ToEnString()},\n" +
				$"Max loss: {MaxLoss}, Max profit: {MaxProfit}, Max dropdown: {MaxDropdownPercent.ToEnString(2)}, Max dropdown length: {MaxDropdownLength},\n" +
				$"Profit average: {GoodDealsAverage.ToEnString(2)}, Loss average: {LossAverage.ToEnString(2)}, Long good: {LongGoodCount}, short good: {ShortGoodCount}";
		}

		private void AddTrade(Trade trade)
		{
			trades.Add(trade);

			depoSize += trade.Profit;
			if (depoSize >= maxDepoSize)
			{
				maxDepoSize = depoSize;
				globalMaximumIndex = trades.Count - 1;
			}
			else
			{
				double currentDropdown = 100 * (maxDepoSize - depoSize) / (double)(maxDepoSize);
				int currentDropdownLength = trades.Count - 1 - globalMaximumIndex;
				MaxDropdownPercent = Math.Max(currentDropdown, MaxDropdownPercent);
				MaxDropdownLength = Math.Max(currentDropdownLength, MaxDropdownLength);
			}
		}

		public void AddDeal(Deal deal)
		{
			if (!deals.Any() || deals.Peek().IsBuy == deal.IsBuy)
			{
				deals.Push(deal);
				return;
			}
			var prevDeal = deals.Pop();
			var profit = (deal.Price - prevDeal.Price)*(prevDeal.IsBuy ? 1 : -1) - Comission;
			AddTrade(new Trade(profit, prevDeal.IsBuy, deal.DateTime - prevDeal.DateTime, prevDeal.Advice));
		}

		public List<int> GetDepositSizes()
		{
			int sum = startDepoSize;
			var depo = new List<int>{sum};
			foreach (var trade in trades)
			{
				sum += trade.Profit;
				depo.Add(sum);
			}
			return depo;
		}

		private double GetProbabilityLevel()
		{
			var standardDeviation = Trades.Select(t => (double)t.Profit).StandardDeviation();
			var meansStdDev = standardDeviation / Math.Sqrt(Trades.Count);

			var cdf = Normal.CDF(0, meansStdDev, ProfitMean);
			return 1 - Math.Min(1 - cdf, cdf)*2;
		}

		#region Unused

		private bool IsProfitStatisticallySignificant(double probabilityLevel)
		{
			var standardDeviation = Trades.Select(t => (double)t.Profit).StandardDeviation();
			var meansStdDev = standardDeviation/Math.Sqrt(Trades.Count);
			var quantile = Normal.InvCDF(0, meansStdDev, (1 - probabilityLevel)/2);
			return ProfitMean > Math.Abs(quantile);
		}

		public void PrintDeals()
		{
			for (int i = 0; i < trades.Count; ++i)
			{
				Console.WriteLine("{0}: {1}", i, trades[i].Profit);
			}
		}

		public void PrintDeals(string filename)
		{
			File.WriteAllLines(filename, trades.ConvertAll(d => d.Profit.ToString()));
		}

		#endregion
	}
}
