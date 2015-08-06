using System;

namespace Utils.Loops
{
	public interface IExecutor<out TFieldsContainer>
	{
		void Execute(Action<TFieldsContainer> action);
	}
}