using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Utils.Types;

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
	}
}
