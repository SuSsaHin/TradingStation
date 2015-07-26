using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
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

		[TestMethod]
		public void TestConfigsReader()
		{
			var cr = new ConfigsReader();
			var loops = new List<Loop>
			{
				new Loop {Start = 0, End = 10, Step = 2, FieldName = "Stop Loss"},
				new Loop {Start = 5, End = 15, Step = 1, FieldName = "Dynamic Stop Loss"}
			};

			var result = new List<List<int>>{new List<int>(), new List<int>()};
			cr.RunLoops(loops, tp =>
			{
				result[0].Add(tp.StopLoss);
				result[1].Add(tp.DynamicStopLoss);
			});

			int resInd = 0;
			for (decimal sl = loops[0].Start; sl <= loops[0].End; sl += loops[0].Step)
			{
				for (decimal dsl = loops[1].Start; dsl <= loops[1].End; dsl += loops[1].Step)
				{
					Assert.That(result[0][resInd] == (int)sl && result[1][resInd] == (int)dsl);
					resInd++;
				}
			}
		}

		[TestMethod]
		public void TestConfigsReaderBadNames()
		{
			var cr = new ConfigsReader();
			var loops = new List<Loop>
			{
				new Loop {Start = 0, End = 10, Step = 2, FieldName = "Stop S Loss"},
			};
			Assert.Throws<MissingFieldException>(() => cr.GenerateLoops(loops, x => { }));

			loops = new List<Loop>
			{
				new Loop {Start = 0, End = 10, Step = 2, FieldName = "stop loss"},
			};
			Assert.DoesNotThrow(() => cr.GenerateLoops(loops, x => { }));
		}
	}
}
