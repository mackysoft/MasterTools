using System;
using System.Collections.Generic;

namespace MackySoft.MasterTools
{
	/// <summary>
	/// テーブルを読み込んで、JSONのリストに変換します。
	/// </summary>
	public interface ITableReader
	{
		List<string> Read (TableContext context);
	}

	public interface IJsonDeserializer
	{
		object Deserialize (Type type, string json);
	}

	public interface IDatabaseBuilder
	{
		void Append (Type dataType, IList<object> tableData);
		void Build (BuildContext context);
	}

	
}
