using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using NPOI.SS.UserModel;

namespace MackySoft.MasterTools
{
	public sealed class XlsxTableReader : ITableReader
	{

		public List<string> Read (TableContext context)
		{
			string pathWithExtension = Path.ChangeExtension(context.FilePath, ".xlsx");
			using IWorkbook workbook = WorkbookFactory.Create(pathWithExtension);
			ISheet sheet = workbook.GetSheet(context.SheetName);
			if (sheet == null)
			{
				throw new InvalidDataException($"Sheet '{context.SheetName}' not found in '{pathWithExtension}'.");
			}
			IRow nameRow = sheet.GetRow(0);

			StringBuilder jsonBuilder = new();

			List<string> list = new List<string>();
			int firstValueRowNum = sheet.FirstRowNum + 1;
			for (int i = firstValueRowNum; i <= sheet.LastRowNum; i++)
			{
				IRow row = sheet.GetRow(i);
				jsonBuilder.Clear();
				jsonBuilder.AppendObjectFromRow(row);

				list.Add(jsonBuilder.ToString());
			}

			return list;
		}
	}
}
