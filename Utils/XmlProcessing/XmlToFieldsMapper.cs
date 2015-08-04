using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Utils.XmlProcessing
{
	public class XmlToFieldsMapper<T>
	{
		private readonly Dictionary<string, FieldInfo> fields;

		public XmlToFieldsMapper()
		{
			fields = new Dictionary<string, FieldInfo>();

			var namedFields = typeof(T).GetFields().Where(f => f.GetCustomAttributes(typeof (FieldNameAttribute)).Any());
			foreach (var field in namedFields)
			{
				var name = ((FieldNameAttribute)(field.GetCustomAttribute(typeof(FieldNameAttribute)))).Name.ToLower();
				fields[name] = field;
			}
		}

		public FieldInfo GetFieldInfo(string name)
		{
			return fields[name.ToLower()];
		}

		public object GetValue(string fieldName, T obj)
		{
			return GetFieldInfo(fieldName).GetValue(obj);
		}

		public bool ContainsField(string name)
		{
			return fields.ContainsKey(name.ToLower());
		}
	}
}
