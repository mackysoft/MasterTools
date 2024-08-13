using System;

namespace MackySoft.MasterTools
{

	[AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
	public sealed class TableReaderAttribute : Attribute
	{

		public string FilePath { get; }

		public string SheetName { get; }

		public TableReaderAttribute (string filePath, string sheetName = null)
		{
			FilePath = filePath;
			SheetName = sheetName;
		}
	}
}
