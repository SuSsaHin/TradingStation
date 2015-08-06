using System.Linq;
using System.Xml.Linq;
using StatesRobot;
using Utils.Loops;

namespace Tests.Tools
{
	class Configurator
	{
		private LoopsGenerator<TradeParams> loopsExecutor; 
		public StatesFactory Factory { get; private set; }
		public IExecutor<TradeParams> Executor { get { return loopsExecutor; } }

		public Configurator(string configsName)
		{
			var document = XDocument.Load(configsName);
			var test = document.Descendants("Test").Single();
			InitFactory(test.Descendants("Types").Single());
			InitLoops(test.Descendants("Params").Single());
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
