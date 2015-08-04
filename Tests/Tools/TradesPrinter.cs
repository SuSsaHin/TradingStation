using System;
using System.Collections.Generic;
using System.Linq;
using StatesRobot;
using TradeTools;
using Utils.TableWriter;
using Utils.XmlProcessing;

namespace Tests.Tools
{
	class TradesPrinter
	{
		private readonly ITableWriter writer;
		private readonly List<string> paramsFieldNames;
		private readonly List<string> resultsFieldNames;
		private readonly XmlToFieldsMapper<TradeParams> paramsXmlMapper;
		private readonly XmlToFieldsMapper<TradesResult> resultsXmlMapper;

		private readonly List<string> headers;
		private readonly List<List<string>> table;

		public TradesPrinter(ITableWriter writer, IEnumerable<string> paramsFieldNames, IEnumerable<string> resultsFieldNames)
		{
			this.writer = writer;
			this.paramsFieldNames = paramsFieldNames as List<string> ?? paramsFieldNames.ToList();
			this.resultsFieldNames = resultsFieldNames as List<string> ?? resultsFieldNames.ToList();

			table = new List<List<string>>();
			paramsXmlMapper = new XmlToFieldsMapper<TradeParams>(); //TODO см. static field
			
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
				.Select(name => paramsXmlMapper.GetValue(name, parameters).ToString())
				.Concat(resultsFieldNames
					.Select(name => resultsXmlMapper.GetValue(name, result).ToString()))
				.ToList();

			table.Add(row);
		}

		public void Print(string filename)
		{
			writer.Print(filename, headers, table);
		}

		public void Clear()
		{
			table.Clear();
		}
	}
}
