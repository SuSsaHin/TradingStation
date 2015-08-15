using System;

namespace Utils.FieldsMapping
{
	[AttributeUsage(AttributeTargets.Property)]
	public class PropNameAttribute : Attribute
	{
		public PropNameAttribute(string name)
		{
			Name = name;
		}

		public string Name { get; }
	}
}
