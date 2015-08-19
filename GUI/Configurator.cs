using System;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using System.Xml.XPath;
using ADTrader;
using GUI.Helper;
using NLog;
using NLog.Config;
using NLog.Targets.Wrappers;
using StatesRobot;
using Utils.Events;

using TradeParamsMapper = Utils.FieldsMapping.XmlToFieldsMapper<StatesRobot.TradeParams>;

namespace GUI
{
	class Configurator
	{
		public TerminalConnector Connector { get; private set; }
		public RobotContext RobotContext { get; }
		public TradeController TradeController { get; private set; }

		public Configurator(string configsName, MainWindow window)
		{
			var document = XDocument.Load(configsName);

			var eventBus = new EventBus();

			var logsDirectory = document.XPathSelectElement(@"Configs/Logs/Directory")?.Value;
			if (string.IsNullOrWhiteSpace(logsDirectory))
				throw new Exception("You should specify logs directory");

			Directory.CreateDirectory(logsDirectory);

			InitLogs(logsDirectory, window);
			InitConnector(document.Descendants("Connection").Single());

			var factory = GetFactory(document.Descendants("Types").Single());
			var tradeParams = GetParams(document.Descendants("TradeParams").Single());

			var currentDayCandles = Connector.GetCurrentDayCandles();
			var advisor = new TradeAdvisor(currentDayCandles);	//TODO больше истории

			RobotContext = new RobotContext(tradeParams, factory, advisor, currentDayCandles);
			InitTradeController(document.Descendants("TradeController").Single(), eventBus, logsDirectory);
		}

		private void InitTradeController(XElement controllerNode, EventBus eventBus, string workingDirectory)
		{
			var updatePeriod = (int)controllerNode.Descendants("UpdatePeriod").Single();
			var serverCooldownPeriod = (int)controllerNode.Descendants("ServerCooldownPeriod").Single();
			var tradeStartTime = (TimeSpan)controllerNode.Descendants("TradeStartTime").Single();

			TradeController = new TradeController(eventBus, Connector, RobotContext, workingDirectory, updatePeriod, serverCooldownPeriod, tradeStartTime);
		}

		private void InitConnector(XElement connection)
		{
			Connector = new TerminalConnector(
								connection.Descendants("Account").Single().Value,
								connection.Descendants("PlaceCode").Single().Value,
								connection.Descendants("Tool").Single().Value
                            );
		}

		private void InitLogs(string logsDirectory, MainWindow window)
		{
			var textTarget = new WpfRichTextBoxTarget
			{
				Name = "WindowText",
				Layout = "${date:format=HH.mm.ss}: ${message}",
				ControlName = window.LogsRichTextBox.Name,
				FormName = GetType().Name,
				AutoScroll = true,
				MaxLines = 100000,
				UseDefaultRowColoringRules = true,
				Width = (int)window.LogsRichTextBox.Width
			};

			var stateTarget = new WpfRichTextBoxTarget
			{
				Name = "WindowState",
				Layout = "${message}",
				ControlName = window.StateString.Name,
				FormName = GetType().Name,
				MaxLines = 1,
				UseDefaultRowColoringRules = true,
				Width = (int)window.StateString.Width
			};

			var asyncTextWrapper = new AsyncTargetWrapper { Name = "WindowTextAsync", WrappedTarget = textTarget };
			var asyncStateWrapper = new AsyncTargetWrapper { Name = "WindowStateAsync", WrappedTarget = stateTarget };

			LogManager.Configuration.AddTarget(asyncTextWrapper.Name, asyncTextWrapper);
			LogManager.Configuration.LoggingRules.Add(new LoggingRule("*", LogLevel.Info, asyncTextWrapper));

			LogManager.Configuration.AddTarget(asyncStateWrapper.Name, asyncStateWrapper);
			LogManager.Configuration.LoggingRules.Insert(0, new LoggingRule("StateLogger", LogLevel.Trace, asyncStateWrapper) {Final = true});
			
			LogManager.Configuration.Variables["WorkingDirectory"] = logsDirectory;
			LogManager.ReconfigExistingLoggers();
		}

		private StatesFactory GetFactory(XElement types)
		{
			var stopType = types.Descendants("Stop").Single().Value.ToLower();
			StatesFactory.TradeStateTypes tsType;
			switch (stopType)
			{
				case "breakeven":
					tsType = StatesFactory.TradeStateTypes.Breakeven;
					break;
				case "trailing":
					tsType = StatesFactory.TradeStateTypes.Trailing;
					break;
				default:
					tsType = StatesFactory.TradeStateTypes.Basic;
					break;
			}
			return new StatesFactory(tsType);
		}

		private TradeParams GetParams(XElement parameters)
		{
			var tradeParams = new TradeParams();
			foreach (var param in parameters.Descendants())
			{
				var name = param.Name.LocalName;
				if (!TradeParamsMapper.HasField(name))
					throw new SettingsPropertyNotFoundException($"Can't found property {name}");

				TradeParamsMapper.SetValue(name, tradeParams, param.Attribute("Size"));
			}

			return tradeParams;
		}
	}
}
