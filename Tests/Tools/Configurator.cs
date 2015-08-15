using System;
using System.Configuration;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using StatesRobot;
using Tests.Tools.History;
using Tests.Tools.Printing;
using Tests.Tools.Printing.Mappers;
using Utils.Loops;
using Utils.TableWriter;

using TradeParamsMapper = Utils.FieldsMapping.XmlToFieldsMapper<StatesRobot.TradeParams>;

namespace Tests.Tools
{
	class Configurator
	{
		private const string defaultConfigsPath = "Configs/default.xml";

		private string printsDir;

		private LoopsGenerator<TradeParams> loopsExecutor;
		private string workingDirectory;

		public StatesFactory Factory { get; private set; }
		public IExecutor<TradeParams> Executor => loopsExecutor;
		public HistoryRepository Repository { get; private set; }
		public TradesPrinter Printer { get; private set; }

		public Configurator(string configsName)
		{
			var document = XDocument.Load(configsName);
			var testNode = document.Descendants("Test").Single();
			var paramsNode = testNode.Descendants("Params").Single();

			InitDefaults();
			InitFactory(testNode.Descendants("Types").Single());
			InitLoops(paramsNode);
			InitRepository(testNode.Descendants("Tool").Single());
			InitTradesPrinter(paramsNode);

			CultureInfo.DefaultThreadCurrentCulture = new CultureInfo("en-us");
		}

		private void InitTradesPrinter(XElement parmeters)
		{
			var paramsFieldNames = parmeters.Descendants().Select(el => el.Name.LocalName).ToList();
			var resultsFieldNames = TradeResultsFieldsMapper.FieldNames;
			Printer = new TradesPrinter(new ExcelWriter(), paramsFieldNames, resultsFieldNames, workingDirectory, printsDir);
		}

		private void InitRepository(XElement tool)
		{
			var isTicks = tool.Attribute("IsTicks");
			Repository = new HistoryRepository(tool.Value, isTicks != null && (bool)isTicks);
		}

		private void InitDefaults()
		{
			var defaults = XDocument.Load(defaultConfigsPath);

			var paramsNode = defaults.Descendants("Params").Single();
			loopsExecutor = new LoopsGenerator<TradeParams>(GetDefaultTradeParams(paramsNode));

			var pathsNode = defaults.Descendants("Paths").Single();
			workingDirectory = pathsNode.Descendants("WorkingDirectory").Single().Value;
			if (string.IsNullOrWhiteSpace(workingDirectory))
				throw new Exception("You should specify working directory");

			Directory.CreateDirectory(workingDirectory);

			printsDir = pathsNode.Descendants("Prints").Single().Value;
			Directory.CreateDirectory(Path.Combine(workingDirectory, printsDir));
		}

		private void InitLoops(XElement parameters)
		{
			foreach (var p in parameters.Descendants())
			{
				var sizeAttr = p.Attribute("Size");

				if (sizeAttr != null)
				{
					loopsExecutor.AppendValue(p.Name.LocalName, sizeAttr);
				}
				else
				{
					loopsExecutor.AppendLoop(p.Name.LocalName, p.Attribute("Start"),
						p.Attribute("End"), p.Attribute("Step"));
				}
			}
		}

		private void InitFactory(XElement types)
		{
			var stopType = types.Descendants("Stop").Single().Value.ToLower();
			StatesFactory.TradeStateTypes tsType;
			switch (stopType)
			{
				case "breakeven":
					tsType = StatesFactory.TradeStateTypes.Breakeven;
					break;
				case "trailing":
					tsType = StatesFactory.TradeStateTypes.Trailing;
					break;
				default:
					tsType = StatesFactory.TradeStateTypes.Basic;
					break;
			}
			Factory = new StatesFactory(tsType);
		}

		private TradeParams GetDefaultTradeParams(XElement paramsNode)
		{
			var tradeParams = new TradeParams();
			foreach (var param in paramsNode.Descendants())
			{
				var name = param.Name.LocalName;
				if (!TradeParamsMapper.HasField(name))
					throw new SettingsPropertyNotFoundException($"Can't found property {name}");

				TradeParamsMapper.SetValue(name, tradeParams, param.Attribute("Size"));
			}
			return tradeParams;
		}
	}
}
