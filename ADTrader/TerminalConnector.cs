using System;
using System.Collections.Generic;
using System.Linq;
using ADLite;
using Utils.Types;

namespace ADTrader
{
	public class TerminalConnector
	{
		private readonly AlfaDirectClass alfaDirect;	//TODO configs
		private const string account = "67693-000";
		private const string placeCode = "FORTS";
		private readonly string pCode;
		
		public bool Connected { get { return alfaDirect != null && alfaDirect.Connected; } }

		public TerminalConnector(string pCode)
		{
			this.pCode = pCode;
			alfaDirect = new AlfaDirectClass();
		}

		public List<Candle> GetCurrentDayCandles()
		{
			return GetCandles(DateTime.Now.Date);
		}

		public Candle GetLastFullCandle()
		{
			var currentMinutes = ((DateTime.Now.Minute/5 + 1)*5)%60;
			return GetCandles(DateTime.Now.AddMinutes(-10).AddHours(-2)).Last(c => c.Time.Minutes != currentMinutes);
		} 

		public int GetLastPrice()
		{
			var localDbData = alfaDirect.GetLocalDBData("fin_info", "last_price", "p_code=" + pCode);
			return int.Parse(localDbData.Substring(0, localDbData.IndexOf('|')));
		}

		public int SendOrder(bool isTrendLong, double price, int count = 1)
		{
			var endDate = DateTime.Now.AddMinutes(1);
			string type = isTrendLong ? "B" : "S";

			return alfaDirect.CreateLimitOrder(account, placeCode, pCode, endDate, "", "RUR", type, count, price,
				null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, -1);
		}

		public int SendStopOrder(bool isTrendLong, double stopPrice, int count = 1)
		{
			var endDate = DateTime.Now.AddDays(1);
			string type = isTrendLong ? "S" : "B";

			return alfaDirect.CreateStopOrder(account, placeCode, pCode, endDate, "", "RUR", type, count, stopPrice, 1000, null, 10);
		}

		public int ModifyStopOrder(int orderNumber, double stopPrice)
		{
			return alfaDirect.UpdateStopOrder(orderNumber, stopPrice, null, null, null, null, 10);
		}

		public void DropOrder(int orderNumber)
		{
			alfaDirect.DropOrder(orderNumber, null, null, null, null, null, -1);
		}

		public string GetLastResultMessage()
		{
			return alfaDirect.LastResultMsg;
		}

		private List<Candle> GetCandles(DateTime? dateFrom = null, DateTime? dateTo = null)
		{
			string result = alfaDirect.GetArchiveFinInfoFromDB(placeCode, pCode, 1, dateFrom, dateTo);
			return ParseCandles(result);
		}

		private List<Candle> ParseCandles(string requestResult)
		{
			if (requestResult == null)
				return new List<Candle>();

			var rows = requestResult.Split('\n').ToList();
			rows.RemoveAt(rows.Count - 1);
			return rows.Select(ParseCandle).ToList();
		}

		private Candle ParseCandle(string row)
		{
			var fields = row.Split('|');
			var dateTime = DateTime.Parse(fields[0]);
			var open = int.Parse(fields[1]);
			var high = int.Parse(fields[2]);
			var low = int.Parse(fields[3]);
			var close = int.Parse(fields[4]);

			return new Candle(dateTime, open, high, low, close, 5);
		}
	}
}
