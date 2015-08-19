using System;
using System.Runtime.InteropServices;
using System.Xml.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using StatesRobot.States.Search.Tools;
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
			var results = new TradesResult(0, 30);
			results.AddDeal(new Deal(100, new DateTime(1, 1, 1, 10, 0, 0), true));
			results.AddDeal(new Deal(150, new DateTime(1, 1, 1, 15, 0, 0), false));
			Assert.That(results.Profit == 50 - results.Comission);
			Assert.That(results.DealsAreClosed);
			Assert.That(results.DealsCount == 1);
		}

		[TestMethod]
		public void TestConfigurator()
		{
			var cr = new Configurator("Configs/main.xml");
			cr.Executor.Execute(tp => Console.WriteLine("{0}, {1}, {2}", tp.PegtopSize, tp.TrailingStopPercent, tp.StopLoss));
		}

		[TestMethod]
		public void TestFireEvent()
		{
			EventBus eb = new EventBus();
			int counter = 0;
			eb.AddCallback(typeof(DealEvent), ev => { ++counter; });
			eb.AddCallback(typeof(StopLossEvent), ev => { ++counter; });
			eb.FireEvent(new StopLossEvent(new Deal(0, DateTime.Now, false)));
			Assert.That(counter == 1);
			eb.AddCallback(typeof(StopLossEvent), ev => { ++counter; });
			eb.FireEvent(new StopLossEvent(new Deal(0, DateTime.Now, false)));
			Assert.That(counter == 3);

			eb.FireEvent(new DealEvent(new Deal(0, DateTime.Now, false)));
			Assert.That(counter == 4);
		}

		[TestMethod]
		public void TestDynamicCastXelements()
		{
			var test = new TimeSpan(0, 10, 0);
			var testEl = new XElement("N1", test);
			//Console.WriteLine(testEl.Value);
			Assert.That(typeof(TimeSpan).DynamicCast(testEl) == test);
		}

		[TestMethod]
		public void TestAddExtremum()
		{
			var repo = new ExtremumsRepository();

			var secondMin = repo.AddExtremum(new Extremum(100, 1, new DateTime(10, 10, 10), true));
			Assert.IsNull(secondMin);
			secondMin = repo.AddExtremum(new Extremum(50, 2, new DateTime(10, 10, 11), true));
			Assert.IsNull(secondMin);
			secondMin = repo.AddExtremum(new Extremum(100, 3, new DateTime(10, 10, 12), true));
			Assert.IsNotNull(secondMin);
			Assert.That(secondMin.Value == 50 && secondMin.IsMinimum && secondMin.DateTime == new DateTime(10, 10, 11) && secondMin.CheckerIndex == 3);

			var secondMax = repo.AddExtremum(new Extremum(100, 1, new DateTime(10, 10, 10), false));
			Assert.IsNull(secondMax);
			secondMax = repo.AddExtremum(new Extremum(50, 2, new DateTime(10, 10, 11), false));
			Assert.IsNull(secondMax);
			secondMax = repo.AddExtremum(new Extremum(30, 4, new DateTime(10, 10, 11), false));
			Assert.IsNull(secondMax);
			secondMax = repo.AddExtremum(new Extremum(100, 3, new DateTime(10, 10, 12), false));
			Assert.IsNull(secondMax);
			secondMax = repo.AddExtremum(new Extremum(80, 5, new DateTime(10, 10, 14), false));
			Assert.IsNotNull(secondMax);
			Assert.That(secondMax.Value == 100 && !secondMax.IsMinimum && secondMax.DateTime == new DateTime(10, 10, 12) && secondMax.CheckerIndex == 5);
		}

		[TestMethod]
		public void TestAddSameExtremum()
		{
			var repo = new ExtremumsRepository();

			repo.AddExtremum(new Extremum(50, 2, new DateTime(10, 10, 11), false));
			Assert.That(repo.FirstMaximums.Count == 1);
			repo.AddExtremum(new Extremum(30, 4, new DateTime(10, 10, 11), false));
			Assert.That(repo.FirstMaximums.Count == 1);
		}
	}
}
