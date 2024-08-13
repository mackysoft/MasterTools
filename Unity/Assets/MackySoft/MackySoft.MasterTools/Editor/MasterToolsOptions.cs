using System;

namespace MackySoft.MasterTools
{
	public sealed class TableReaderInfo
	{
		public Type DataType { get; }
		public TableReaderAttribute Attribute { get; }

		public TableReaderInfo (Type dataType, TableReaderAttribute attribute)
		{
			DataType = dataType;
			Attribute = attribute;
		}
	}
	public interface ITableReaderProvider
	{
		public ITableReader GetTableReader (TableReadContext context);
	}
	public static class TableReaderProvider
	{
		public static ITableReaderProvider Create (Func<TableReadContext, ITableReader> func)
		{
			if (func == null)
			{
				throw new ArgumentNullException(nameof(func));
			}
			return new AnonymousTableReaderProvider(func);
		}

		sealed class AnonymousTableReaderProvider : ITableReaderProvider
		{

			readonly Func<TableReadContext, ITableReader> m_Func;

			public AnonymousTableReaderProvider (Func<TableReadContext, ITableReader> func)
			{
				m_Func = func;
			}

			public ITableReader GetTableReader (TableReadContext context)
			{
				return m_Func(context);
			}
		}
	}
	public sealed class MasterToolsOptions
	{

		string m_TablesDirectoryPath = "../MasterData";
		string m_DefaultSheetName = "Main";
		ITableReaderProvider m_TableReaderProvider;

		public string TablesDirectoryPath
		{
			get => m_TablesDirectoryPath;
			set
			{
				if (string.IsNullOrEmpty(value))
				{
					throw new ArgumentException($"{nameof(TablesDirectoryPath)} is null or empty.", nameof(TablesDirectoryPath));
				}
				m_TablesDirectoryPath = value;
			}
		}
		public string DefaultSheetName
		{
			get => m_DefaultSheetName;
			set
			{
				if (string.IsNullOrEmpty(value))
				{
					throw new ArgumentException($"{nameof(DefaultSheetName)} is null or empty.", nameof(DefaultSheetName));
				}
				m_DefaultSheetName = value;

			}
		}

		public ITableReaderProvider TableReaderProvider { get => m_TableReaderProvider; set => m_TableReaderProvider = value; }
	}
}