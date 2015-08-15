using System.Collections.Generic;
using System.Linq;
using StatesRobot;
using Tests.Tools.Printing.Mappers;
using Utils.FieldsMapping;

namespace Tests.Tools.Printing
{
	static class Printer
	{
		public static List<string> GetTableRow(this TradesResult result, List<string> headers)
		{
			return headers.Select(h => TradeResultsFieldsMapper.GetValue(h, result)).ToList();
		}

		public static List<string> GetTableRow(this TradeParams parameters, List<string> headers)
		{
			return headers.Select(h => XmlToFieldsMapper<TradeParams>.GetValue(h, parameters).ToString()).ToList();
		}
	}
}
