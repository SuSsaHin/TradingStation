using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using TradeTools;
using TradeTools.Events;
using Utils.Events;
 using NLogger = NLog.Logger;

namespace GUI
{
	class MyLogger
	{
		private readonly string logFilePath;
		private readonly string debugFilePath;
		private readonly MainWindow window;

		private readonly string directoryName;

		private const string longExtrName = "firstLong.txt";
		private const string shortExtrName = "firstShort.txt";

		private readonly Mutex mutex = new Mutex();

		private static readonly NLogger logger = NLog.LogManager.GetCurrentClassLogger();

		public MyLogger(EventBus eventBus, string logsDir, string logFilename, string debugFilename, MainWindow window)
		{
			eventBus.AddCallback(typeof(StopLossMovingEvent), LogBreakeven);
			eventBus.AddCallback(typeof(DealEvent), LogDeal);
			eventBus.AddCallback(typeof(EndEvent), LogEnd);
			eventBus.AddCallback(typeof(StopLossEvent), LogStopLoss);
			eventBus.AddCallback(typeof(SecondExtremumEvent), LogSecondExtremum);
			//eventBus.AddCallback(typeof(SearchInfoEvent), LogSearchInfo);

			var now = DateTime.Now;
			directoryName = Path.Combine(logsDir, now.Day + "_" + now.Hour + "_" + now.Minute + @"\");

			logFilePath = Path.Combine(directoryName, logFilename);
			debugFilePath = Path.Combine(directoryName, debugFilename);
			this.window = window;

			CreateLogFiles();
		}

		private void CreateLogFiles()
		{
			Directory.CreateDirectory(directoryName);
			File.WriteAllText(logFilePath, "");
			File.WriteAllText(debugFilePath, "");
		}

		private void LogFirstExtremums(IEnumerable<Extremum> firstLongExtremums, IEnumerable<Extremum> firstShortExtremums)
		{
			if (firstLongExtremums != null)
			{
				File.WriteAllLines(directoryName + longExtrName, firstLongExtremums.Select(ex => ex.ToString()));
			}

			if (firstShortExtremums != null)
			{
				File.WriteAllLines(directoryName + shortExtrName, firstShortExtremums.Select(ex => ex.ToString()));
			}
		}

		private void LogSearchInfo(ITradeEvent logged)
		{
			throw new NotImplementedException();
			//var info = (SearchInfoEvent) logged;
			//LogFirstExtremums(info.FirstLongExtremums, info.FirstShortExtremums);
		}

		private void LogSecondExtremum(ITradeEvent logged)
		{
			var secondExtremum = (SecondExtremumEvent)logged;
			Log("Second extremum found: " + secondExtremum.Extremum);
			LogFirstExtremums(secondExtremum.FirstLongExtremums, secondExtremum.FirstShortExtremums);
		}

		private void LogBreakeven(ITradeEvent logged)
		{
			var breakeven = (StopLossMovingEvent)logged;
			Log("Breakeven: " + breakeven.Price);
		}

		private void LogDeal(ITradeEvent logged)
		{
			var deal = ((DealEvent)logged).Deal;
			Log("New deal: price" + deal.Price + ", " + (deal.IsBuy ? "long" : "short"));
			//LogFirstExtremums(deal.FirstLongExtremums, deal.FirstShortExtremums);
		}

		private void LogEnd(ITradeEvent logged)
		{
			Log("End of trading");
		}

		private void LogStopLoss(ITradeEvent logged)
		{
			var stopLoss = ((StopLossEvent)logged).Deal;
			Log("StopLoss: price" + stopLoss.Price + ", " + (stopLoss.IsBuy ? "long" : "short"));
		}

		public void Log(string logged)
		{
			mutex.WaitOne();
			window.Dispatcher.InvokeAsync(() => window.AddLogs(DateTime.Now.TimeOfDay.ToString(@"hh\:mm\:ss") + @": " + logged));
			File.AppendAllText(logFilePath, DateTime.Now + @": " + logged + "\n");
			mutex.ReleaseMutex();
		}

		/// <summary>
		/// Вызывать только в одном потоке?
		/// </summary>
		/// <param name="logged"></param>
		public void Debug(string logged)
		{
			File.AppendAllText(debugFilePath, DateTime.Now + @": " + logged + "\n");
		}

		public void SetState(string state)
		{
			logger.Info(state);
			mutex.WaitOne();
			window.Dispatcher.InvokeAsync(() => window.SetState(state));
			mutex.ReleaseMutex();
		}

		public void PrintToFile(string fileName, IEnumerable<string> text)
		{
			var filePath = Path.Combine(directoryName, fileName);
			File.WriteAllLines(filePath, text);
		}
	}
}
