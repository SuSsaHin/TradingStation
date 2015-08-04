using System.Collections.Generic;
using Utils.XmlProcessing;

namespace Utils.TableWriter
{
	public interface ITableWriter
	{
		void Print(string filename, List<string> headers, List<List<string>> table);
	}
}
