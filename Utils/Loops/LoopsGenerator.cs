using System;
using Utils.XmlProcessing;

namespace Utils.Loops
{
	public class LoopsGenerator<TFieldsContainer> : IExecutor<TFieldsContainer> where TFieldsContainer : new()
	{
		private Func<Action<TFieldsContainer>, Action<TFieldsContainer>> loop = action => action;
		private readonly XmlToFieldsMapper<TFieldsContainer> xmlMapper = new XmlToFieldsMapper<TFieldsContainer>();	//TODO static?
		private readonly TFieldsContainer container;

		public LoopsGenerator()
		{
			container = new TFieldsContainer();
		} 

		public LoopsGenerator(TFieldsContainer container)
		{
			this.container = container;
		}

		public void AppendLoop(string fieldName, object loopStart, object loopEnd, object loopStep)
		{
			var name = fieldName.ToLower();
			if (!xmlMapper.ContainsField(name))
				throw new MissingFieldException("Can't loop field " + fieldName);

			var fieldInfo = xmlMapper.GetFieldInfo(name);

			var start = fieldInfo.PropertyType.DynamicCast(loopStart);
			var step = fieldInfo.PropertyType.DynamicCast(loopStep);
			var end = fieldInfo.PropertyType.DynamicCast(loopEnd);

			var oldLoop = loop;
			
			loop = action =>
			{
				return fieldsContainer =>
				{
					for (var i = start; i < end; i += step)
					{
						fieldInfo.SetValue(fieldsContainer, i);
						oldLoop(action)(fieldsContainer);
					}
				};
			};
		}

		public void AppendValue(string fieldName, object value)
		{
			if (!xmlMapper.ContainsField(fieldName))
				throw new MissingFieldException("Can't find loop field " + fieldName);

			var oldLoop = loop;

			loop = action =>
			{
				return fieldsContainer =>
				{
					xmlMapper.SetValue(fieldName, fieldsContainer, value);
					oldLoop(action)(fieldsContainer);
				};
			};
		}

		public void Execute(Action<TFieldsContainer> action)
		{
			loop(action)(container);
		}
	}
}
