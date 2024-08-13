using System;
using System.IO;
using System.Text;
using NPOI.SS.UserModel;

namespace MackySoft.MasterTools
{
	public static class JsonBuilder
	{
		public static StringBuilder AppendObjectFromRow (this StringBuilder stringBuilder, IRow valueRow)
		{
			if (stringBuilder == null)
			{
				throw new ArgumentNullException(nameof(stringBuilder));
			}
			if (valueRow == null)
			{
				throw new ArgumentNullException(nameof(valueRow));
			}

			stringBuilder.Append('[');

			int lastCellNum = valueRow.LastCellNum - 1;
			for (int j = valueRow.FirstCellNum; j <= lastCellNum; j++)
			{
				ICell cell = valueRow.GetCell(j);
				stringBuilder.Append(cell.GetCellValueAsFormattable());
				if (j < lastCellNum)
				{
					stringBuilder.Append(',');
				}
			}

			stringBuilder.Append(']');
			return stringBuilder;
		}

		public static StringBuilder AppendObjectFromRowWithName (this StringBuilder stringBuilder, IRow valueRow, IRow nameRow)
		{
			if (stringBuilder == null)
			{
				throw new ArgumentNullException(nameof(stringBuilder));
			}
			if (valueRow == null)
			{
				throw new ArgumentNullException(nameof(valueRow));
			}
			if (nameRow == null)
			{
				throw new ArgumentNullException(nameof(nameRow));
			}

			stringBuilder.Append('[');

			int lastCellNum = valueRow.LastCellNum - 1;
			for (int j = valueRow.FirstCellNum; j <= lastCellNum; j++)
			{
				ICell cell = valueRow.GetCell(j);
				ICell nameCell = nameRow.GetCell(j);
				
				stringBuilder
					.Append('"')
					.Append(nameCell.GetCellValue())
					.Append("\":")
					.Append(cell.GetCellValueAsFormattable());

				if (j < lastCellNum)
				{
					stringBuilder.Append(',');
				}
			}

			stringBuilder.Append(']');
			return stringBuilder;
		}
	}
}
