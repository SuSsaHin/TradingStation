using System;
using Utils;
using Utils.XmlProcessing;

namespace Tests.Tools
{
	static class LoopsGenerator<TFieldsContainer>
	{
		public static Func<Action<TFieldsContainer>, Action<TFieldsContainer>> AppendLoop(Func<Action<TFieldsContainer>, Action<TFieldsContainer>> func, string fieldName, object loopStart, object loopEnd, object loopStep)
		{
			var xmlMapper = new XmlToFieldsMapper<TFieldsContainer>();
			var name = fieldName.ToLower();
			if (!xmlMapper.ContainsField(name))
				throw new MissingFieldException("Can't loop field " + fieldName);

			var fieldInfo = xmlMapper.GetFieldInfo(name);

			var start = fieldInfo.FieldType.DynamicCast(loopStart);
			var step = fieldInfo.FieldType.DynamicCast(loopStep);
			var end = fieldInfo.FieldType.DynamicCast(loopEnd);
			
			return action =>
			{
				return tp =>
				{
					for (var i = start; i < end; i += step)
					{
						fieldInfo.SetValue(tp, i);
						func(action)(tp);
					}
				};
			};
		}

		public static Func<Action<TFieldsContainer>, Action<TFieldsContainer>> AppendValue(Func<Action<TFieldsContainer>, Action<TFieldsContainer>> func, string fieldName, object value)
		{
			var xmlMapper = new XmlToFieldsMapper<TFieldsContainer>();	//TODO убрать static (или добавить static поле--)
			if (!xmlMapper.ContainsField(fieldName))
				throw new MissingFieldException("Can't find loop field " + fieldName);

			var fieldInfo = xmlMapper.GetFieldInfo(fieldName);
			return action =>
			{
				return tp =>
				{
					fieldInfo.SetValue(tp, fieldInfo.FieldType.DynamicCast(value));
					func(action)(tp);
				};
			};
		}
	}
}
