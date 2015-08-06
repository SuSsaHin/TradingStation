using System;
using System.Xml.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Tests.Tools;
using TradeTools;
using TradeTools.Events;
using Utils;
using Utils.Events;
using Assert = NUnit.Framework.Assert;

namespace Tests
{
	[TestClass]
	public class UnitTests
	{
		[TestMethod]
		public void TestAddDeal()
		{
			var results = new TradesResult();
			results.AddDeal(new Deal(100, new DateTime(1, 1, 1, 10, 0, 0), true));
			results.AddDeal(new Deal(150, new DateTime(1, 1, 1, 15, 0, 0), false));
			Assert.That(results.Profit == 50 - TradesResult.Comission);
			Assert.That(results.DealsAreClosed);
		}

		[TestMethod]
		public void TestConfigurator()
		{
			var cr = new Configurator("Configs/TestConfig.xml");
			cr.Executor.Execute(tp => Console.WriteLine("{0}, {1}, {2}", tp.PegtopSize, tp.TrailingStopPercent, tp.StopLoss));
		}

		[TestMethod]
		public void TestFireEvent()
		{
			EventBus eb = new EventBus();
			int counter = 0;
			eb.AddCallback(typeof(StopLossMovingEvent), ev => { ++counter; });
			eb.FireEvent(new StopLossMovingEvent(0, true));
			Assert.That(counter == 1);
			eb.AddCallback(typeof(StopLossMovingEvent), ev => { ++counter; });
			eb.FireEvent(new StopLossMovingEvent(0, false));
			Assert.That(counter == 3);
		}

		[TestMethod]
		public void TestDynamicCastXelements()
		{
			var test = new TimeSpan(0, 10, 0);
			var testEl = new XElement("N1", test);
			Assert.That(typeof(TimeSpan).DynamicCast(testEl) == test);
		}
	}
}
