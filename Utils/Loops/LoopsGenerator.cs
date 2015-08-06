using System;
using Utils.XmlProcessing;

namespace Utils.Loops
{
	public class LoopsGenerator<TFieldsContainer> : IExecutor<TFieldsContainer> where TFieldsContainer : new()
	{
		private Func<Action<TFieldsContainer>, Action<TFieldsContainer>> loop = action => action;
		private readonly XmlToFieldsMapper<TFieldsContainer> xmlMapper = new XmlToFieldsMapper<TFieldsContainer>();	//TODO static?

		public void AppendLoop(string fieldName, object loopStart, object loopEnd, object loopStep)
		{
			var name = fieldName.ToLower();
			if (!xmlMapper.ContainsField(name))
				throw new MissingFieldException("Can't loop field " + fieldName);

			var fieldInfo = xmlMapper.GetFieldInfo(name);

			var start = fieldInfo.FieldType.DynamicCast(loopStart);
			var step = fieldInfo.FieldType.DynamicCast(loopStep);
			var end = fieldInfo.FieldType.DynamicCast(loopEnd);

			var oldLoop = loop;
			
			loop = action =>
			{
				return tp =>
				{
					for (var i = start; i < end; i += step)
					{
						fieldInfo.SetValue(tp, i);
						oldLoop(action)(tp);
					}
				};
			};
		}

		public void AppendValue(string fieldName, object value)
		{
			if (!xmlMapper.ContainsField(fieldName))
				throw new MissingFieldException("Can't find loop field " + fieldName);

			var oldLoop = loop;

			var fieldInfo = xmlMapper.GetFieldInfo(fieldName);
			loop = action =>
			{
				return tp =>
				{
					fieldInfo.SetValue(tp, fieldInfo.FieldType.DynamicCast(value));
					oldLoop(action)(tp);
				};
			};
		}

		public void Execute(Action<TFieldsContainer> action)
		{
			loop(action)(new TFieldsContainer());
		}
	}
}
