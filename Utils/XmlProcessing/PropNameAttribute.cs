using System;

namespace Utils.XmlProcessing
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
