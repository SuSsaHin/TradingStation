using System;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows;
using NLog;

namespace GUI
{
	/// <summary>
	/// Логика взаимодействия для MainWindow.xaml
	/// </summary>
	public partial class MainWindow
	{
		private readonly TradeController tradeController;
		private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
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
				Logger.Error(e);
            }
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
