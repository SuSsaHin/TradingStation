using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Xml.Linq;
using StatesRobot;

namespace Tests.Tools
{
	class ConfigsReader
	{
		public List<Loop> Loops { get; private set; }
		public StatesFactory Factory { get; private set; }

		public ConfigsReader(string configsName)
		{
			var document = XDocument.Load(configsName);
			var test = document.Descendants("Test").Single();
			InitFactory(test.Descendants("Types").Single());
			InitLoops(test.Descendants("Params").Single());
		}

		private void InitLoops(XElement parameters)
		{
			Loops = new List<Loop>();
			foreach (var p in parameters.Descendants())
			{
				var sizeAttr = p.Attribute("Size");
				if (sizeAttr != null)
				{
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
