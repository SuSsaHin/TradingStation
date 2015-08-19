using System;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows;
using GUI.Helper;
using NLog;
using NLog.Config;
using NLog.Targets.Wrappers;

namespace GUI
{
	/// <summary>
	/// Логика взаимодействия для MainWindow.xaml
	/// </summary>
	public partial class MainWindow
	{
		private readonly TradeController tradeController;
		//private TerminalConnector connector;
		//private int orderNumber = -1;

		public MainWindow()
		{
			InitializeComponent();
			
			var configurator = new Configurator("Configs/main.xml", this);
			try
			{
				tradeController = configurator.TradeController;

				Task.Run(() => tradeController.Run());
			}
			catch (Exception e)
			{
				AddLogs(e.Message);
			}
		}
		
		public void AddLogs(string text)
		{
			//TODO!!
			//LogsListBox.Items.Add(text);
		}

		public void SetState(string state)
		{
			//TODO!!
			//StateString.Content = state;
		}

		private void MainWindow_OnClosing(object sender, CancelEventArgs e)
		{
			var result = MessageBox.Show("Закрыть?", "Close", MessageBoxButton.YesNo);
			e.Cancel = result != MessageBoxResult.Yes;
			tradeController.SerializeCandlesAsync();
		}

        private void PrintExtremums_Click(object sender, RoutedEventArgs e)
        {
			tradeController.PrintExtremumsAsync();
        }
	}
}
