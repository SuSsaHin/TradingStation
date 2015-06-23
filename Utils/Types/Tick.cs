using System;

namespace Utils.Types
{
	public class Tick
	{
		private DateTime dateTime;

		public Tick(DateTime dateTime, int value)
		{
			this.dateTime = dateTime;
			Value = value;
		}

		public DateTime Date { get { return dateTime.Date; } }
		public TimeSpan Time { get { return dateTime.TimeOfDay; } }
		public DateTime DateTime { get { return dateTime; } }
		public int Value { get; private set; }

		public override bool Equals(object obj)
		{
			var tick = (Tick) obj;
			return (tick.Date == Date) && (tick.Time == Time) && (tick.Value == Value);
		}
	}
}
