using System.Collections.Generic;
using System.IO;
using System.Linq;
using StatesRobot;
using Utils.TableWriter;

namespace Tests.Tools.Printing
{
	class TradesPrinter
	{
		private readonly string outputPath;
		private readonly string depoPrintsDir;
		private readonly ITableWriter writer;
		private readonly List<string> paramsFieldNames;
		private readonly List<string> resultsFieldNames;

		private readonly List<string> headers;
		private readonly List<List<string>> table;

		public TradesPrinter(ITableWriter writer, IEnumerable<string> paramsFieldNames, IEnumerable<string> resultsFieldNames, string outputPath, string depoPrintsDir = "")
		{
			this.writer = writer;
			this.outputPath = outputPath;
			this.depoPrintsDir = depoPrintsDir;
			this.paramsFieldNames = paramsFieldNames as List<string> ?? paramsFieldNames.ToList();
			this.resultsFieldNames = resultsFieldNames as List<string> ?? resultsFieldNames.ToList();

			table = new List<List<string>>();
			headers = this.paramsFieldNames.Concat(this.resultsFieldNames).ToList();
		}

		public void AddRow(TradeParams parameters, TradesResult result)
		{
			var row = parameters.GetTableRow(paramsFieldNames)
				.Concat(result.GetTableRow(resultsFieldNames))
				.ToList();

			table.Add(row);
		}

		public void PrintDepoWithParamsName(TradeParams parameters, TradesResult result)
		{
			string filename = GetFilenameByParams(parameters);
			File.WriteAllLines(Path.Combine(outputPath, depoPrintsDir, filename), result.GetDepositSizes().Select(s => s.ToString()));
		}

		public void PrintTable(string fileNameWithoutExtension)
		{
			writer.Print(Path.Combine(outputPath, fileNameWithoutExtension), headers, table);
		}

		public void Clear()
		{
			table.Clear();
		}

		private string GetFilenameByParams(TradeParams parameters)
		{
			string filename = parameters.GetTableRow(paramsFieldNames).Aggregate("", (b, name) => b + name + "_");
			return filename.Remove(filename.Length - 1) + ".txt";
		}
	}
}
