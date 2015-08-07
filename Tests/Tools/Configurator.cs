using System.Linq;
using System.Xml.Linq;
using StatesRobot;
using Utils.Loops;
using Utils.TableWriter;
using Utils.XmlProcessing;

namespace Tests.Tools
{
	class Configurator
	{
		private LoopsGenerator<TradeParams> loopsExecutor; 
		public StatesFactory Factory { get; private set; }
		public IExecutor<TradeParams> Executor { get { return loopsExecutor; } }
		public HistoryRepository Repository { get; private set; }
		public TradesPrinter Printer { get; private set; }

		public Configurator(string configsName)
		{
			var document = XDocument.Load(configsName);
			var testNode = document.Descendants("Test").Single();
			var paramsNode = testNode.Descendants("Params").Single();

			InitFactory(testNode.Descendants("Types").Single());
			InitLoops(paramsNode);
			InitRepository(testNode.Descendants("Tool").Single());
			InitPrinter(paramsNode);
		}

		private void InitPrinter(XElement parmeters)
		{
			var paramsFieldNames = parmeters.Descendants().Select(el => el.Name.LocalName).ToList();
			var resultsFieldNames = (new XmlToFieldsMapper<TradesResult>()).FieldNames;
			Printer = new TradesPrinter(new ExcelWriter(), paramsFieldNames, resultsFieldNames);
		}

		private void InitRepository(XElement tool)
		{
			var isTicks = tool.Attribute("IsTicks");
			Repository = new HistoryRepository(tool.Value, isTicks != null && (bool)isTicks);
		}

		private void InitLoops(XElement parameters)
		{
			loopsExecutor = new LoopsGenerator<TradeParams>();
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
	}
}
