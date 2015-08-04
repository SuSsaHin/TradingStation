using System;
using System.Reflection;
using Utils.XmlProcessing;

namespace Tests.Tools
{
	static class LoopsGenerator<TFieldsContainer>
	{
		/// <summary>
		/// 
		/// </summary>
		/// <typeparam name="T">T should be a type of field</typeparam>
		/// <param name="func"></param>
		/// <param name="loop"></param>
		/// <returns></returns>
		public static Action<TFieldsContainer> AppendLoop<T>(Action<TFieldsContainer> func, Loop<T> loop) where T : IComparable
		{
			var xmlMapper = new XmlToFieldsMapper<TFieldsContainer>();
			var name = loop.FieldName.ToLower();
			if (!xmlMapper.ContainsField(name))
				throw new MissingFieldException("Can't loop field " + loop.FieldName);

			var addOp = typeof (T).GetMethod("op_Addition", BindingFlags.Static | BindingFlags.Public);
			if (addOp == null)
				throw new ArithmeticException("Type " + typeof(T).Name + " doesn't contains + operator");

			return tp =>
			{
				for (T i = loop.Start; i.CompareTo(loop.End) < 0; addOp.Invoke(null, new object[]{i, loop.Step}))
				{
					xmlMapper.GetFieldInfo(name).SetValue(tp, i);
					func(tp);
				}
			};
		}

		/// <summary>
		/// 
		/// </summary>
		/// <typeparam name="T">T should be a type of field</typeparam>
		/// <param name="func"></param>
		/// <param name="fieldName"></param>
		/// <param name="value"></param>
		/// <returns></returns>
		public static Action<TFieldsContainer> AppendValue<T>(Action<TFieldsContainer> func, string fieldName, T value)
		{
			var xmlMapper = new XmlToFieldsMapper<TFieldsContainer>();	//TODO убрать static (или добавить static поле--)
			if (!xmlMapper.ContainsField(fieldName))
				throw new MissingFieldException("Can't find loop field " + fieldName);

			return tp =>
			{
				xmlMapper.GetFieldInfo(fieldName).SetValue(tp, value);
				func(tp);
			};
		}
	}
}
