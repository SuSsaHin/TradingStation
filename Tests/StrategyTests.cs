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
		private void RunTest(TradeParams tradeParams, StatesFactory statesFactory, TradeAdvisor advisor, HistoryRepository repository)
		{
			var robot = new RobotContext(tradeParams, statesFactory, advisor);
			var results = new TradesResult();
			foreach (var day in repository.Days.Skip(1))
			{
				foreach (var candle in day.FiveMins)
				{
					var dealEvent = robot.Process(candle) as DealEvent;
					if (dealEvent != null)
					{
						results.AddDeal(dealEvent.Deal);
					}
				}
				robot.ClearHistory();
			}
		}
		[TestCase("Configs/main.xml")]
		public void MainTest(string filename)
		{
			var configurator = new Configurator(filename);
			var repository = configurator.Repository;
			var advisor = new TradeAdvisor(repository.Days.First().FiveMins);

			configurator.Executor.Execute(tp => RunTest(tp, configurator.Factory, advisor, repository));
		}
	}
}
