using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ADTrader;
using NLog;
using StatesRobot;
using TradeTools;
using TradeTools.Events;
using Utils.Events;

namespace GUI
{
	class TradeController
	{
		private readonly int updatePeriod;
		private readonly int serverCooldownPeriod;
		private readonly TimeSpan tradeStartTime;

		private int stopOrderNumber = -1;

		private readonly EventBus eventBus;
		private readonly RobotContext robot;
		private readonly string workingDirectory;
		private readonly TerminalConnector connector;

		private readonly ConcurrentQueue<Action> actions;

		private static readonly Logger StateLogger = LogManager.GetLogger("StateLogger");
		private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

		public bool Stopped { get; private set; }
		public bool UpdateStopped { get; private set; }

		public TradeController(EventBus eventBus, TerminalConnector connector, RobotContext robot,
								string workingDirectory, int updatePeriod, int serverCooldownPeriod, TimeSpan tradeStartTime)
		{
			actions = new ConcurrentQueue<Action>();

			this.eventBus = eventBus;
			this.connector = connector;
			this.robot = robot;
			this.workingDirectory = workingDirectory;
			this.updatePeriod = updatePeriod;
			this.serverCooldownPeriod = serverCooldownPeriod;
			this.tradeStartTime = tradeStartTime;

			Stopped = true;
			UpdateStopped = true;

			eventBus.AddCallback(typeof(DealEvent), ev => AddHandler(ev, NewDeal));
			eventBus.AddCallback(typeof(EndEvent), ev => AddHandler(ev, EndTrading));
			eventBus.AddCallback(typeof(StopLossMovingEvent), ev => AddHandler(ev, StopLossMoving));
			eventBus.AddCallback(typeof(StopLossEvent), ev => AddHandler(ev, StopLoss));
		}

		public void Stop()
		{
			if (Stopped)
				return;

			Stopped = true;

			eventBus.FireEvent(robot.StopTrading());
		}

		public void Run()
		{
			if (!Stopped)
				return;

			Stopped = false;

			WaitForTime(tradeStartTime);

			UpdateStopped = false;

			Logger.Info("Controller started");
			Task.Run(() => RunLoop());
		}

		public void PrintExtremumsAsync(string fileName = "Extremums.txt") => AddAction(() => PrintExtremums(fileName));

		public void SerializeCandlesAsync(string fileName = "Candles.txt") => AddAction(() => SerializeCandles(fileName));

		private void SerializeCandles(string fileName)
		{
			PrintToFile(fileName, robot.Candles.Select(c => c.SerializeCandle()));
			var formattedCandles = new List<string> {"DateTime\tOpen\tHigh\tLow\tClose"}
									.Concat(robot.Candles.Select(c => c.ToString()));
			PrintToFile("formatted" + fileName, formattedCandles);
		}

		private void PrintExtremums(string fileName)
		{
			var text = new List<string> { "First minimums:" };
			text.AddRange(robot.ExtremumsRepository.FirstMinimums.Select(ex => ex.ToString()));
			text.Add("");
			text.Add("First maximums:");
			text.AddRange(robot.ExtremumsRepository.FirstMaximums.Select(ex => ex.ToString()));
			text.Add("");
			text.Add("Second minimums:");
			text.AddRange(robot.ExtremumsRepository.SecondMinimums.Select(ex => ex.ToString()));
			text.Add("");
			text.Add("Second maximums:");
			text.AddRange(robot.ExtremumsRepository.SecondMaximums.Select(ex => ex.ToString()));

			PrintToFile(fileName, text);
		}

		private void AddHandler(ITradeEvent ev, Action<ITradeEvent> action) => AddAction(() => action(ev));

		private void AddAction(Action action) => actions.Enqueue(action);

		private void StopLoss(ITradeEvent ev)
		{
			Stopped = true;
		}

		private void StopLossMoving(ITradeEvent ev)
		{
			var stopMovingEvent = (StopLossMovingEvent)ev;
			if (stopOrderNumber == -1)
			{
				Logger.Info("Unexpected breakeven {0}", stopMovingEvent.Price);
				return;
			}

			var trendIsLong = stopMovingEvent.TrendIsLong;

			connector.DropOrder(stopOrderNumber);
			Logger.Info(connector.LastResultMessage);

			Thread.Sleep(serverCooldownPeriod);

			stopOrderNumber = connector.SendStopOrder(trendIsLong, stopMovingEvent.Price);
			Logger.Info(connector.LastResultMessage);
		}

		private void EndTrading(ITradeEvent ev)
		{
			Stopped = true;
		}

		private void NewDeal(ITradeEvent ev)
		{
			var deal = ((DealEvent)ev).Deal;
			var isBuy = deal.IsBuy;

			connector.SendOrder(isBuy, deal.Price);
			Logger.Info(connector.LastResultMessage);

			Thread.Sleep(serverCooldownPeriod);

			stopOrderNumber = connector.SendStopOrder(isBuy, robot.StopLossPrice);
			Logger.Info(connector.LastResultMessage);
		}

		private void ProcessCandle(Candle candle)
		{
			if (Stopped)
				return;

			var ev = robot.Process(candle);
			Logger.Debug("Candle added: " + candle);

			if (ev == null)
				return;

			eventBus.FireEvent(ev);
		}

		private void RunLoop()
		{
			int lastMinute = -10;
			while (!UpdateStopped)
			{
				var loopStartTime = DateTime.Now.TimeOfDay;
				try
				{
					UpdatePrice();
					lastMinute = TryProcessCandle(lastMinute);
					ExecuteActions();
				}
				catch (Exception ex)
				{
					Logger.Warn(ex.ToString());
					Logger.Warn("Last message: " + connector.LastResultMessage);
				}

				var loopTime = (int)(DateTime.Now.TimeOfDay - loopStartTime).TotalMilliseconds;
				Thread.Sleep(Math.Max(0, updatePeriod - loopTime));
			}
		}

		private void ExecuteActions()
		{
			Action action;
			while (actions.TryDequeue(out action))
			{
				action();
			}
		}

		private int TryProcessCandle(int lastMinute)	//TODO Error with last candle in Advisor when starting at %5 minute
		{
			var currentMinute = DateTime.Now.Minute;
			if (currentMinute % 5 != 0 || currentMinute == lastMinute)
				return lastMinute;

			var candle = connector.GetLastFullCandle();

			ProcessCandle(candle);
			return currentMinute;
		}

		private void UpdatePrice()
		{
			StateLogger.Trace("Price: {0}", connector.GetLastPrice());
		}

		private void WaitForTime(TimeSpan time)
		{
			var timeOfDay = DateTime.Now.TimeOfDay;
			if (timeOfDay < time)
			{
				Thread.Sleep(time - timeOfDay);
			}
		}

		public void PrintToFile(string fileName, IEnumerable<string> text)
		{
			var filePath = Path.Combine(workingDirectory, fileName);
			File.WriteAllLines(filePath, text);
		}
	}
}
