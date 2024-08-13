using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using NPOI.SS.UserModel;

namespace MackySoft.MasterTools.Readers
{
	public sealed class XlsxTableReader : ITableReader
	{

		readonly string m_FilePath;
		readonly string m_SheetName;

		public XlsxTableReader (string filePath, string sheetName)
		{
			m_FilePath = !string.IsNullOrEmpty(filePath) ? filePath : throw new ArgumentException($"{nameof(filePath)} is null or empty.", nameof(filePath));
			m_SheetName = !string.IsNullOrEmpty(sheetName) ? sheetName : throw new ArgumentException($"{nameof(sheetName)} is null or empty.", nameof(sheetName));
		}

		public List<string> Read ()
		{
			string pathWithExtension = Path.ChangeExtension(m_FilePath, ".xlsx");
			using IWorkbook workbook = WorkbookFactory.Create(pathWithExtension);
			ISheet sheet = workbook.GetSheet(m_SheetName);
			if (sheet == null)
			{
				throw new Exception($"Sheet '{m_SheetName}' not found in '{pathWithExtension}'.");
			}
			IRow nameRow = sheet.GetRow(0);

			StringBuilder jsonBuilder = new();

			List<string> list = new List<string>();
			int firstValueRowNum = sheet.FirstRowNum + 1;
			for (int i = firstValueRowNum; i <= sheet.LastRowNum; i++)
			{
				IRow row = sheet.GetRow(i);
				jsonBuilder.Clear();
				jsonBuilder.AppendObjectFromRowWithName(row, nameRow);
				list.Add(jsonBuilder.ToString());
			}

			return list;
		}
	}
}
