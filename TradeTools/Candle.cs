using System;
using System.Collections.Generic;
using System.Linq;

namespace TradeTools
{
	public class Candle
	{
		public List<Tick> Ticks { get; }

		public int Open { get; private set; }
		public int Close { get; private set; }
		public int High { get; private set; }
		public int Low { get; private set; }

		public int PeriodMins { get; }

		private DateTime dateTime;

		public bool IsLong => Close > Open;

		public DateTime Date => dateTime.Date;

		public TimeSpan Time => dateTime.TimeOfDay;

		public DateTime DateTime => dateTime;

		public int InnerHeigth => Math.Abs(Open - Close);

		public Candle(List<Tick> ticks, int periodMins)
		{
			PeriodMins = periodMins;
			Ticks = ticks;
			dateTime = ticks.First().DateTime;

			if (!Ticks.Any() || (Ticks.Last().DateTime - Ticks.First().DateTime).TotalMinutes > PeriodMins)
				throw new Exception("Too long ticks list");

			Open = Ticks.First().Value;
			Close = Ticks.Last().Value;
			High = 0;
			Low = int.MaxValue;

			foreach (var tick in Ticks)
			{
				High = Math.Max(tick.Value, High);
				Low = Math.Min(tick.Value, Low);
			}
		}

		public Candle(DateTime dateTime, int open, int hight, int low, int close, int periodMins)
		{
			Open = open;
			High = hight;
			Low = low;
			Close = close;

			PeriodMins = periodMins;
			this.dateTime = dateTime;
			Ticks = new List<Tick>();
		}

		public bool AppendTick(Tick tick)
		{
			if ((Ticks.Any() && tick.DateTime <= Ticks[Ticks.Count - 1].DateTime))
				throw new Exception("Appended tick time is lower then last");

			if ((tick.DateTime - Ticks.First().DateTime).TotalMinutes > PeriodMins)
				return true;
				//throw new Exception("Too long ticks interval");

			Ticks.Add(tick);

			High = Math.Max(tick.Value, High);
			Low = Math.Min(tick.Value, Low);
			Close = tick.Value;

			return false;
		}

		public bool IsOuterTo(Candle c)
		{
			return High >= c.High && Low <= c.Low;
		}

		public bool IsInnerTo(Candle c)
		{
			return High <= c.High && Low >= c.Low;
		}

		public override string ToString()
		{
			return $"{DateTime.ToString("u")}\t{Open}\t{High}\t{Low}\t{Close}";
		}
	}
}
