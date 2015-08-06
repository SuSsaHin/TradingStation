using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NUnit.Framework;
using StatesRobot;
using Tests.Tools;
using Utils.Events;

namespace Tests
{
	[TestClass]
	public class StrategyTests
	{
		private void RunTest(TradeParams tradeParams, StatesFactory statesFactory, TradeAdvisor advisor, 
								HistoryRepository repository, EventBus evetBus)
		{
			var robot = new RobotContext(tradeParams, statesFactory, advisor);
			foreach (var day in repository.Days.Skip(1))
			{
				foreach (var candle in day.FiveMins)
				{
					var ev = robot.Process(candle);
					if (ev != null)
					{
						evetBus.FireEvent(ev);
					}
				}
			}
		}
		[TestCase("Configs/main.xml")]
		public void MainTest(string filename)
		{
			var configurator = new Configurator(filename);
			var repository = configurator.Repository;
			var advisor = new TradeAdvisor(repository.Days.First().FiveMins);


		}
	}
}
