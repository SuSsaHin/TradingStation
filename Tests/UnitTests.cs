using System;
using System.Collections.Generic;
using System.Xml.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using StatesRobot;
using Tests.Tools;
using Utils;
using Utils.Events;
using Utils.Types;
using Assert = NUnit.Framework.Assert;

namespace Tests
{
	[TestClass]
	public class UnitTests
	{
		[TestMethod]
		public void TestRefs()
		{
			var c = new Candle(new DateTime(), 100, 120, 20, 100, 5);
			var d = c;
			c = new Candle(new DateTime(), 120, 120, 20, 120, 5);
			Console.WriteLine(c);
			Console.WriteLine(d);
		}
		/*
		[TestMethod]
		public void TestLoopsGenerator()
		{
			var lg = new LoopsGenerator();
			var loops = new List<Loop>
			{
				new Loop {Start = 0, End = 10, Step = 2, FieldName = "StopLoss"},
				new Loop {Start = 5, End = 15, Step = 1, FieldName = "TrailingStopPercent"}
			};

			var result = new List<List<decimal>> { new List<decimal>(), new List<decimal>() };
			lg.RunLoops(loops, tp =>
			{
				result[0].Add(tp.StopLoss);
				result[1].Add(tp.TrailingStopPercent);
			});

			int resInd = 0;
			for (decimal sl = loops[0].Start; sl <= loops[0].End; sl += loops[0].Step)
			{
				for (decimal dsl = loops[1].Start; dsl <= loops[1].End; dsl += loops[1].Step)
				{
					Assert.That(result[0][resInd] == sl && result[1][resInd] == dsl);
					resInd++;
				}
			}
		}*/

		[TestMethod]
		public void TestLoopsGeneratorBadNames()
		{
			/*var lg = new LoopsGenerator();
			var loops = new List<Loop>
			{
				new Loop {Start = 0, End = 10, Step = 2, FieldName = "Stop S Loss"},
			};
			Assert.Throws<MissingFieldException>(() => lg.GenerateLoops(loops, x => { }));

			loops = new List<Loop>
			{
				new Loop {Start = 0, End = 10, Step = 2, FieldName = "stoploss"},
			};
			Assert.DoesNotThrow(() => lg.GenerateLoops(loops, x => { }));*/
		}

		[TestMethod]
		public void TestConfigsReader()
		{
			/*var cr = new ConfigsReader("Configs/TestConfig.xml");
			Assert.That(cr.Factory.TradeStateType == StatesFactory.TradeStateTypes.Trailing);
			Assert.That(cr.Loops[0].FieldName == "StopLoss");
			Assert.That(cr.Loops[0].Start == 200);
			Assert.That(cr.Loops[0].End == 1000);
			Assert.That(cr.Loops[0].Step == 200);

			Assert.That(cr.Loops[2].FieldName == "Pegtop");
			Assert.That(cr.Loops[2].Start == 70);
			Assert.That(cr.Loops[2].End == 70);
			Assert.That(cr.Loops[2].Step == decimal.MaxValue);*/
		}

		[TestMethod]
		public void TestTimeParse()
		{
			var type = typeof (StatesFactory);
			var method = type.GetMethod("Parse", new Type[] {typeof (string), typeof (IFormatProvider)});
			var res = Convert.ChangeType("23:30", typeof (TimeSpan));
			res.GetType();
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
	}
}
