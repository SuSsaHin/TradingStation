using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NUnit.Framework;
using StatesRobot;
using Tests.Tools;
using TradeTools.Events;

namespace Tests
{
    [TestClass]
	public class StrategyTests
	{
		private void RunTest(TradeParams tradeParams, StatesFactory statesFactory, HistoryRepository repository, TradesPrinter printer)
		{
			var advisor = new TradeAdvisor(repository.Days.First().FiveMins);
			var robot = new RobotContext(tradeParams, statesFactory, advisor);
			var results = new TradesResult();

			foreach (var day in repository.Days.Skip(1))
			{
				foreach (var candle in day.FiveMins)
				{
					var dealEvent = robot.Process(candle) as DealEvent;	//TODO Test advisor candles adding
					if (dealEvent != null)
					{
						results.AddDeal(dealEvent.Deal);
					}
				}
				robot.ClearHistory();
			}

			printer.AddRow(tradeParams, results);
		}
		[TestCase("Configs/main.xml")]
		public void MainTest(string filename)
		{
			var configurator = new Configurator(filename);
			var repository = configurator.Repository;
			var printer = configurator.Printer;

			configurator.Executor.Execute(tp => RunTest(tp, configurator.Factory, repository, printer));
			printer.Print("MainTest.xlsx");
		}
	}
}
