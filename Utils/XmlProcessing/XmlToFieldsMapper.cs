using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Utils.XmlProcessing
{
	public class XmlToFieldsMapper<T>
	{
		private readonly Dictionary<string, PropertyInfo> properties;
		public readonly IReadOnlyList<string> FieldNames; 

		public XmlToFieldsMapper()
		{
			properties = new Dictionary<string, PropertyInfo>();

			var namedProps = typeof(T).GetProperties().Where(f => f.GetCustomAttributes(typeof (PropNameAttribute)).Any()).ToList();
			foreach (var property in namedProps)
			{
				var name = ((PropNameAttribute)(property.GetCustomAttribute(typeof(PropNameAttribute)))).Name.ToLower();
				properties[name] = property;
			}

			FieldNames = namedProps
					.Select(field => ((PropNameAttribute) (field.GetCustomAttribute(typeof (PropNameAttribute)))).Name)
					.ToList();
		}

		public PropertyInfo GetFieldInfo(string name)
		{
			return properties[name.ToLower()];
		}

		public object GetValue(string fieldName, T obj)
		{
			return GetFieldInfo(fieldName).GetValue(obj);
		}

		public void SetValue(string fieldName, T obj, object value)
		{
			var fieldInfo = GetFieldInfo(fieldName);
			fieldInfo.SetValue(obj, fieldInfo.PropertyType.DynamicCast(value));
		}

		public bool ContainsField(string name)
		{
			return properties.ContainsKey(name.ToLower());
		}
	}
}
