using System;
using System.Collections.Generic;

namespace MackySoft.MasterTools
{
	public interface IDatabaseBuilder
	{
		/// <summary>
		/// Append table data to the database.
		/// </summary>
		void Append (Type dataType, IList<object> tableData);

		/// <summary>
		/// Build the database.
		/// </summary>
		void Build (BuildContext context);
	}
}
