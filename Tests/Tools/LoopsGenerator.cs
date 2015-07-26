using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using StatesRobot;

namespace Tests.Tools
{
	class LoopsGenerator
	{
		private readonly Dictionary<string, Action<TradeParams, decimal>> setters;
		
		public LoopsGenerator()
		{
			setters = new Dictionary<string, Action<TradeParams, decimal>>();

			var namedFields = typeof(TradeParams).GetFields().Where(f => f.GetCustomAttributes(typeof (FieldNameAttribute)).Any());
			foreach (var field in namedFields)
			{
				var locField = field;
				var name = ((FieldNameAttribute) (locField.GetCustomAttribute(typeof (FieldNameAttribute)))).Name.ToLower();
				var parseMethod = locField.FieldType.GetMethod("Parse", new[] {typeof (string), typeof (IFormatProvider)});
				if (parseMethod == null)
				{
					throw new NotImplementedException("AZAZA");
					setters[name] = (tp, d) => locField.SetValue(tp, Convert.ChangeType(d, locField.FieldType));
				}
				else
				{
					setters[name] = (tp, d) => locField.SetValue(tp, parseMethod.Invoke(null, new object[]{d, new CultureInfo("en-us") }));
				}
			}
		}

		public void RunLoops(IEnumerable<Loop> fields, Action<TradeParams> test)
		{
			var action = GenerateLoops(fields, test);
			action();
		}

		public Action GenerateLoops(IEnumerable<Loop> fields, Action<TradeParams> test)
		{
			var resFunc = fields.Reverse().Aggregate(test, AppendLoop);
			return () => resFunc(new TradeParams());
		}

		private Action<TradeParams> AppendLoop(Action<TradeParams> func, Loop loop)
		{
			var name = loop.FieldName.ToLower();
			if (!setters.ContainsKey(name))
				throw new MissingFieldException("Can't loop field " + loop.FieldName);

			return tp =>
			{
				for (decimal i = loop.Start; i <= loop.End; i += loop.Step)
				{
					setters[name](tp, i);
					func(tp);
				}
			};
		}
	}
}
