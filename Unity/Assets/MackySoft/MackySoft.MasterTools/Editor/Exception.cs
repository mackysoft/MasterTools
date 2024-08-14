using System;

namespace MackySoft.MasterTools
{
	public sealed class SheetNotFoundException : Exception
	{
		public SheetNotFoundException (string sheetName, string filePath) : base($"Sheet '{sheetName}' not found in '{filePath}'.") { }
	}

	public sealed class InvalidDatabaseException : Exception
	{
		public InvalidDatabaseException (string message) : base(message) { }
	}
}