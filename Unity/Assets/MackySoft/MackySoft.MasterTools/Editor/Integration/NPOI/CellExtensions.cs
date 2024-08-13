using NPOI.SS.UserModel;
using System;

namespace MackySoft.MasterTools
{
	public static class CellExtensions
	{

		const string Null = "null";

		public static string GetCellValue (this ICell cell)
		{
			return cell?.CellType switch
			{
				CellType.Numeric => cell.NumericCellValue.ToString(),
				CellType.String => cell.StringCellValue,
				CellType.Formula => cell.CellFormula,
				CellType.Blank => string.Empty,
				CellType.Boolean => cell.BooleanCellValue.ToString(),
				CellType.Error => cell.ErrorCellValue.ToString(),
				_ => string.Empty,
			} ?? throw new ArgumentNullException(nameof(cell));
		}

		public static string GetCellValueAsFormattable (this ICell cell)
		{
			return cell?.CellType switch
			{
				CellType.Numeric => cell.NumericCellValue.ToString(),
				CellType.String => $@"""{cell.StringCellValue}""",
				CellType.Formula => cell.CellFormula,
				CellType.Blank => Null,
				CellType.Boolean => cell.BooleanCellValue.ToString(),
				CellType.Error => cell.ErrorCellValue.ToString(),
				_ => string.Empty,
			} ?? throw new ArgumentNullException(nameof(cell));
		}
	}
}
