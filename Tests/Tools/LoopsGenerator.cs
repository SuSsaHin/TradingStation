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
			var name = loop.FieldName.ToLower();
			if (!XmlToFieldsMapper.Fields.ContainsKey(name))
				throw new MissingFieldException("Can't loop field " + loop.FieldName);

			var addOp = typeof (T).GetMethod("op_Addition", BindingFlags.Static | BindingFlags.Public);
			if (addOp == null)
				throw new ArithmeticException("Type " + typeof(T).Name + " doesn't contains + operator");

			return tp =>
			{
				for (T i = loop.Start; i.CompareTo(loop.End) < 0; addOp.Invoke(null, new object[]{i, loop.Step}))
				{
					XmlToFieldsMapper.Fields[name].SetValue(tp, i);
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
			if (!XmlToFieldsMapper.Fields.ContainsKey(fieldName))
				throw new MissingFieldException("Can't loop field " + fieldName);

			return tp =>
			{
				XmlToFieldsMapper.Fields[fieldName].SetValue(tp, value);
				func(tp);
			};
		}
	}
}
