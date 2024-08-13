using System;

namespace MackySoft.MasterTools
{

	[AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
	public sealed class ImportTableFromAttribute : Attribute
	{

		public string FilePath { get; }

		public string SheetName { get; }

		public ImportTableFromAttribute (string filePath, string sheetName = null)
		{
			FilePath = filePath;
			SheetName = sheetName;
		}
	}
}
