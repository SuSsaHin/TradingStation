using System;
using System.Collections.Generic;
using Utils;

namespace Tests.Tools.Printing.Mappers
{
	static class TradeResultsFieldsMapper
	{
		private static readonly IReadOnlyDictionary<string, Func<TradesResult, string>> mapper = new Dictionary<string, Func<TradesResult, string>>
		{
			{"BadCount", tp => tp.BadCount.ToString()},
			{"DealsCount", tp => tp.DealsCount.ToString()},
			{"GoodCount", tp => tp.GoodCount.ToString()},
			{"MaxDropdownLength", tp => tp.MaxDropdownLength.ToString()},
			{"MaxDropdownPercent", tp => tp.MaxDropdownPercent.ToEnString(2)},
			{"MaxLoss", tp => tp.MaxLoss.ToString()},
			{"MaxProfit", tp => tp.MaxProfit.ToString()},
			{"ProbabilityLevel", tp => tp.ProbabilityLevel.ToEnString(2)},
			{"Profit", tp => tp.Profit.ToString()},
			{"ProfitMean", tp => tp.ProfitMean.ToEnString(1)},
			{"Volume", tp => tp.Volume.ToString()}
		};

		public static IEnumerable<string> FieldNames => mapper.Keys;

		public static string GetValue(string fieldName, TradesResult result) => mapper[fieldName](result);

		public static bool HasField(string fieldName) => mapper.ContainsKey(fieldName);
	}
}
