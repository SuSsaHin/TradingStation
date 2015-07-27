using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;
using StatesRobot;
using Utils.Types;

namespace Tests.Tools
{
	class ConfigsReader
	{
		public StatesFactory Factory { get; private set; }
		public Action<TradeParams, Action<TradeParams>> LoopAction { get; private set; } 

		public ConfigsReader(string configsName)
		{
			var document = XDocument.Load(configsName);
			var test = document.Descendants("Test").Single();
			InitFactory(test.Descendants("Types").Single());
			InitLoops(test.Descendants("Params").Single());
		}

		private void InitLoops(XElement parameters)
		{
			LoopAction = (tp, act) => act(tp);
			Action<TradeParams> action;
			foreach (var p in parameters.Descendants())
			{
				var sizeAttr = p.Attribute("Size");
				/*var parseMethod = typeof (T).GetMethod("op_Addition", BindingFlags.Static | BindingFlags.Public);
				if (sizeAttr != null)
				{
					action = LoopsGenerator<TradeParams>.AppendValue(action, p.Name.LocalName, )
					decimal size = Decimal.Parse(sizeAttr.Value, new CultureInfo("en-us"));
					Loops.Add(new Loop{Start = size, End = size, Step = Decimal.MaxValue, FieldName = p.Name.LocalName});
				}
				else
				{
					Loops.Add(new Loop
					{
						Start = Decimal.Parse(p.Attribute("Start").Value, new CultureInfo("en-us")),
						End = Decimal.Parse(p.Attribute("End").Value, new CultureInfo("en-us")),
						Step = Decimal.Parse(p.Attribute("Step").Value, new CultureInfo("en-us")),
						FieldName = p.Name.LocalName
					});
				}*/
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
