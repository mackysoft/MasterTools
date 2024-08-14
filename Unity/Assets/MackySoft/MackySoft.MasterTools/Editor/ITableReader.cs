using System;
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

	public interface IJsonDeserializer
	{
		/// <summary>
		/// Deserialize the json to the specified type.
		/// </summary>
		object Deserialize (Type type, string json);
	}

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
