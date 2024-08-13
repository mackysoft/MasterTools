using System.Collections.Generic;

namespace MackySoft.MasterTools
{
	/// <summary>
	/// テーブルを読み込んで、JSONのリストに変換します。
	/// </summary>
	public interface ITableReader
	{
		List<string> Read ();
	}
}
