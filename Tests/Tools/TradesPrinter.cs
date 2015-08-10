using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using StatesRobot;
using Utils;
using Utils.TableWriter;
using Utils.XmlProcessing;

namespace Tests.Tools
{
	class TradesPrinter
	{
		private readonly string outputPath;
		private readonly string depoPrintsDir;
		private readonly ITableWriter writer;
		private readonly List<string> paramsFieldNames;
		private readonly List<string> resultsFieldNames;
		private readonly XmlToFieldsMapper<TradeParams> paramsXmlMapper;	//TODO !!static
		private readonly XmlToFieldsMapper<TradesResult> resultsXmlMapper;

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
			paramsXmlMapper = new XmlToFieldsMapper<TradeParams>();
			
			string badName;
			if ((badName = this.paramsFieldNames.Find(name => !paramsXmlMapper.ContainsField(name))) != null)
				throw new ArgumentException("Can't find TradeParams field: {0}", badName);

			resultsXmlMapper = new XmlToFieldsMapper<TradesResult>();
			if ((badName = this.resultsFieldNames.Find(name => !resultsXmlMapper.ContainsField(name))) != null)
				throw new ArgumentException("Can't find TradeResults field: {0}", badName);

			headers = this.paramsFieldNames.Concat(this.resultsFieldNames).ToList();
		}

		public void AddRow(TradeParams parameters, TradesResult result)
		{
			var row = paramsFieldNames
				.Select(name => GetString(paramsXmlMapper.GetValue(name, parameters).ToString()))
				.Concat(resultsFieldNames
					.Select(name => GetString(resultsXmlMapper.GetValue(name, result))))
				.ToList();

			table.Add(row);
		}

		public void PrintDepoWithParamsName(TradeParams parameters, TradesResult result)
		{
			string filename = GetFilenameByParams(parameters);
			File.WriteAllLines(Path.Combine(outputPath, depoPrintsDir, filename), result.GetDepositSizes().Select(s => s.ToString()));
		}

		public void Print(string filename)
		{
			writer.Print(Path.Combine(outputPath, filename), headers, table);
		}

		public void Clear()
		{
			table.Clear();
		}

		private string GetFilenameByParams(TradeParams parameters)
		{
			string filename = paramsFieldNames.Aggregate("", (b, name) => b + paramsXmlMapper.GetValue(name, parameters) + "_");
			return filename.Remove(filename.Length - 1) + ".txt";
		}

		private string GetString(object value)
		{
			return (value is double) ? ((double)value).ToEnString() : value.ToString();
		}
	}
}
