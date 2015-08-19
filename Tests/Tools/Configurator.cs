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

		private int startDepoSize;
		private int comission;

		public StatesFactory Factory { get; private set; }
		public IExecutor<TradeParams> Executor => loopsExecutor;
		public HistoryRepository Repository { get; private set; }
		public TradesPrinter Printer { get; private set; }
		public TradesResult GetTradeResults() => new TradesResult(startDepoSize, comission);

		public Configurator(string configsName)
		{
			var root = Init(defaultConfigsPath, true);
			var paramsNode = root.Element("Params");
			if (paramsNode == null)
				throw new Exception("Can't find Params node");

			InitDefaultParams(paramsNode);

			root = Init(configsName, true);
			paramsNode = root.Element("Params") ?? new XElement("Params");
			
			InitLoops(paramsNode);
			InitTradesPrinter(paramsNode);

			CultureInfo.DefaultThreadCurrentCulture = new CultureInfo("en-us");
		}

		private XElement Init(string configsPath, bool requireAll)
		{
			var document = XDocument.Load(defaultConfigsPath);
			var configs = document.Element("Configs");

			if (configs == null)
				throw new Exception("Can't read configs " + configsPath);

			var element = configs.Element("StateTypes");
			if (element != null)
			{
				InitFactory(element);
			}
			else if (requireAll)
			{
				throw new Exception("Can't find StateTypes node");
			}

			element = configs.Element("Tool");
			if (element != null)
			{
				InitRepository(element);
			}
			else if (requireAll)
			{
				throw new Exception("Can't find Tool node");
			}

			element = configs.Element("Paths");
			if (element != null)
			{
				InitPaths(element);
			}
			else if (requireAll)
			{
				throw new Exception("Can't find Paths node");
			}

			element = configs.Element("Circumstances");
			if (element != null)
			{
				InitTradeResultsParams(element);
			}
			else if (requireAll)
			{
				throw new Exception("Can't find Circumstances node");
			}

			return configs;
		}

		private void InitTradeResultsParams(XElement circumstances)
		{
			var element = circumstances.Element("StartDepoSize");
			if (element == null)
				throw new Exception("Can't find StartDepoSize for TradeResults");

			startDepoSize = (int) element;

			element = circumstances.Element("Comission");
			if (element == null)
				throw new Exception("Can't find Comission for TradeResults");

			comission = (int)element;
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

		private void InitDefaultParams(XElement paramsNode)
		{
			var tradeParams = new TradeParams();
			foreach (var param in paramsNode.Descendants())
			{
				var name = param.Name.LocalName;
				if (!TradeParamsMapper.HasField(name))
					throw new SettingsPropertyNotFoundException($"Can't found property {name}");

				TradeParamsMapper.SetValue(name, tradeParams, param.Attribute("Size"));
			}

			loopsExecutor = new LoopsGenerator<TradeParams>(tradeParams);
		}

		private void InitPaths(XElement pathsNode)
		{
			var node = pathsNode.Element("WorkingDirectory");
			if (node != null)
			{
				workingDirectory = node.Value;
			}

			if (string.IsNullOrWhiteSpace(workingDirectory))
				throw new Exception("You should specify working directory");

			Directory.CreateDirectory(workingDirectory);

			node = pathsNode.Element("Prints");
			if (node != null)
			{
				printsDir = node.Value;
			}

			if (string.IsNullOrWhiteSpace(workingDirectory))
				throw new Exception("You should specify prints directory");

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
			var xElement = types.Element("Stop");
			if (xElement == null)
				throw new Exception("Can't find Stop state type");

			var stopType = xElement.Value.ToLower();
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
	}
}
