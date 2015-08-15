using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;

namespace Utils.FieldsMapping
{
	[SuppressMessage("ReSharper", "StaticMemberInGenericType")]
	public static class XmlToFieldsMapper<T>
	{
		private readonly static Dictionary<string, PropertyInfo> properties;
		public static IReadOnlyList<string> FieldNames { get; }

		static XmlToFieldsMapper()
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

		public static PropertyInfo GetFieldInfo(string name)
		{
			return properties[name.ToLower()];
		}

		public static object GetValue(string fieldName, T obj)
		{
			return GetFieldInfo(fieldName).GetValue(obj);
		}

		public static void SetValue(string fieldName, T obj, object value)
		{
			var fieldInfo = GetFieldInfo(fieldName);
			fieldInfo.SetValue(obj, fieldInfo.PropertyType.DynamicCast(value));
		}

		public static bool HasField(string fieldName)
		{
			return properties.ContainsKey(fieldName.ToLower());
		}
	}
}
