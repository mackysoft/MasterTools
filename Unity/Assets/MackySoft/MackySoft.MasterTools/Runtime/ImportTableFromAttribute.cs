using System;
using UnityEngine;

namespace MackySoft.MasterTools
{

	/// <summary>
	/// Attribute that can be assigned to data type to support import of table by MasterTools.
	/// </summary>
	[AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
	public sealed class ImportTableFromAttribute : Attribute
	{

		/// <summary>
		/// Relative path of the table, with <see cref="Application.dataPath"/> as root.
		/// </summary>
		public string FilePath { get; }

		/// <summary>
		/// Name of the sheet to reference when reading data.
		/// </summary>
		public string SheetName { get; }

		/// <summary>
		/// 
		/// </summary>
		/// <param name="filePath"> Relative path of the table, with <see cref="Application.dataPath"/> as root. </param>
		/// <param name="sheetName"></param>
		public ImportTableFromAttribute (string filePath, string sheetName = null)
		{
			FilePath = filePath;
			SheetName = sheetName;
		}
	}
}
