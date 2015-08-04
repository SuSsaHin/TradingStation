namespace Tests.Tools
{
	struct Loop<T>
	{
		public Loop(string fieldName, T start, T end, T step) : this()
		{
			FieldName = fieldName;
			Start = start;
			End = end;
			Step = step;
		}

		public string FieldName { get; private set; }
		public T Start { get; private set; }
		public T End { get; private set; }
		public T Step { get; private set; }
	}
}
