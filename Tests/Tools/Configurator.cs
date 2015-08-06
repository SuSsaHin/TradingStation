using System;
using System.Linq;
using System.Xml.Linq;
using StatesRobot;

namespace Tests.Tools
{
	class Configurator
	{
		public StatesFactory Factory { get; private set; }
		public Func<Action<TradeParams>, Action<TradeParams>> Loop { get; private set; }

		public Configurator(string configsName)
		{
			var document = XDocument.Load(configsName);
			var test = document.Descendants("Test").Single();
			InitFactory(test.Descendants("Types").Single());
			InitLoops(test.Descendants("Params").Single());
		}

		private void InitLoops(XElement parameters)
		{
			Loop = action => action;
			foreach (var p in parameters.Descendants())
			{
				var sizeAttr = p.Attribute("Size");
				var localP = p;
				/*var parseMethod = typeof (T).GetMethod("op_Addition", BindingFlags.Static | BindingFlags.Public); //"Parse"*/
				if (sizeAttr != null)
				{
					Loop = LoopsGenerator<TradeParams>.AppendValue(Loop, localP.Name.LocalName, sizeAttr);
				}
				else
				{
					Loop = LoopsGenerator<TradeParams>.AppendLoop(Loop, localP.Name.LocalName, localP.Attribute("Start"),
						localP.Attribute("End"), localP.Attribute("Step"));
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
