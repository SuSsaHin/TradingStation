using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using StatesRobot;

namespace Tests
{
	struct Loop
	{
		public string FieldName;
		public decimal Start;
		public decimal End;
		public decimal Step;
	}
	class ConfigsReader
	{
		private readonly Dictionary<string, Action<TradeParams, decimal>> setters;

		ConfigsReader()
		{
			setters = new Dictionary<string, Action<TradeParams, decimal>>();

			var namedFields = typeof(TradeParams).GetFields().Where(f => f.GetCustomAttributes(typeof (FieldNameAttribute)).Any());
			foreach (var field in namedFields)
			{
				var locField = field;
				var name = ((FieldNameAttribute) (locField.GetCustomAttribute(typeof (FieldNameAttribute)))).Name;
				setters[name] = (tp, d) => { locField.SetValue(tp, d); };
			}
		}
		public Action GenerateLoops(IEnumerable<Loop> fields, Action<TradeParams> test)
		{
			var resFunc = fields.Reverse().Aggregate(test, AppendLoop);
			return () => resFunc(new TradeParams());
		}

		private Action<TradeParams> AppendLoop(Action<TradeParams> func, Loop loop)
		{
			if (!setters.ContainsKey(loop.FieldName))
				throw new MissingFieldException("Can't loop field " + loop.FieldName);

			return tp =>
			{
				for (decimal i = loop.Start; i <= loop.End; i += loop.Step)
				{
					setters[loop.FieldName](tp, i);
					func(tp);
				}
			};
		}
	}
}
