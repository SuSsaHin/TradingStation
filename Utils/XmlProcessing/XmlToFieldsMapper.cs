using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Utils.Types;

namespace Utils.XmlProcessing
{
	public static class XmlToFieldsMapper
	{
		public static Dictionary<string, FieldInfo> Fields { get; private set; }

		static XmlToFieldsMapper()
		{
			Fields = new Dictionary<string, FieldInfo>();

			var namedFields = typeof(TradeParams).GetFields().Where(f => f.GetCustomAttributes(typeof (FieldNameAttribute)).Any());
			foreach (var field in namedFields)
			{
				var name = ((FieldNameAttribute)(field.GetCustomAttribute(typeof(FieldNameAttribute)))).Name.ToLower();
				Fields[name] = field;
			}
		}
	}
}
