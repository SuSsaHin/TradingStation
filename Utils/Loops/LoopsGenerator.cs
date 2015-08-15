using System;
using Utils.FieldsMapping;

namespace Utils.Loops
{
	public class LoopsGenerator<T> : IExecutor<T> where T : new()
	{
		
		private Func<Action<T>, Action<T>> loop = action => action;
		private readonly T container;

		public LoopsGenerator()
		{
			container = new T();
		} 

		public LoopsGenerator(T container)
		{
			this.container = container;
		}

		public void AppendLoop(string fieldName, object loopStart, object loopEnd, object loopStep)
		{
			var name = fieldName.ToLower();
			if (!XmlToFieldsMapper<T>.HasField(name))
				throw new MissingFieldException("Can't loop field " + fieldName);

			var fieldInfo = XmlToFieldsMapper<T>.GetFieldInfo(name);

			var start = fieldInfo.PropertyType.DynamicCast(loopStart);
			var step = fieldInfo.PropertyType.DynamicCast(loopStep);
			var end = fieldInfo.PropertyType.DynamicCast(loopEnd);

			var oldLoop = loop;
			
			loop = action =>
			{
				return fieldsContainer =>
				{
					for (var i = start; i <= end; i += step)
					{
						fieldInfo.SetValue(fieldsContainer, i);
						oldLoop(action)(fieldsContainer);
					}
				};
			};
		}

		public void AppendValue(string fieldName, object value)
		{
			if (!XmlToFieldsMapper<T>.HasField(fieldName))
				throw new MissingFieldException("Can't find loop field " + fieldName);

			var oldLoop = loop;

			loop = action =>
			{
				return fieldsContainer =>
				{
					XmlToFieldsMapper<T>.SetValue(fieldName, fieldsContainer, value);
					oldLoop(action)(fieldsContainer);
				};
			};
		}

		public void Execute(Action<T> action)
		{
			loop(action)(container);
		}
	}
}
