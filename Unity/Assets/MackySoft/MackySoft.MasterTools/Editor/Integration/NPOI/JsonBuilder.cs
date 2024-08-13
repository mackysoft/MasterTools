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
			for (int i = valueRow.FirstCellNum; i <= lastCellNum; i++)
			{
				ICell cell = valueRow.GetCell(i);
				stringBuilder.Append(cell.GetCellValueAsFormattable());
				if (i < lastCellNum)
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
			if ((nameRow.LastCellNum - nameRow.FirstCellNum) < (valueRow.LastCellNum - valueRow.FirstCellNum))
			{
				throw new ArgumentException($"{nameof(nameRow)} must have the same or more cells than {nameof(valueRow)}.");
			}

			stringBuilder.Append('[');

			int lastCellNum = valueRow.LastCellNum - 1;
			for (int i = valueRow.FirstCellNum; i <= lastCellNum; i++)
			{
				ICell cell = valueRow.GetCell(i);
				ICell nameCell = nameRow.GetCell(i);
				
				stringBuilder
					.Append('"')
					.Append(nameCell.GetCellValue())
					.Append("\":")
					.Append(cell.GetCellValueAsFormattable());

				if (i < lastCellNum)
				{
					stringBuilder.Append(',');
				}
			}

			stringBuilder.Append(']');
			return stringBuilder;
		}
	}
}
