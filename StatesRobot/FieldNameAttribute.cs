using System;

namespace StatesRobot
{
	[AttributeUsage(AttributeTargets.Field)]
	public class FieldNameAttribute : Attribute
	{
		public FieldNameAttribute(string name)
		{
			Name = name;
		}

		public string Name { get; private set; }
	}
}
