using System.Collections.Generic;

namespace MackySoft.MasterTools
{
	public interface ITableReader
	{

		/// <summary>
		/// Reads a table file and returns a list of item json.
		/// </summary>
		List<string> Read (TableContext context);
	}
}
