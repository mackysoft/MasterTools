using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace MackySoft.MasterTools
{
	public readonly struct TableContext
	{

		public Type DataType { get; }
		public string FilePath { get; }
		public string SheetName { get; }

		public TableContext (Type dataType, string filePath, string sheetName)
		{
			DataType = dataType;
			FilePath = filePath;
			SheetName = sheetName;
		}
	}

	public readonly struct BuildContext
	{
		public string OutputDirectoryPath { get; }

		public BuildContext (string outputDirectoryPath)
		{
			OutputDirectoryPath = !string.IsNullOrEmpty(outputDirectoryPath) ? outputDirectoryPath : throw new ArgumentException($"{nameof(outputDirectoryPath)} is null or empty.", nameof(outputDirectoryPath));
		}
	}

	public static class MasterDataBuilder
	{

		public static MasterToolsOptions Options { get; set; } = new MasterToolsOptions();

		[MenuItem("Tools/Master Tools/Import (with default options)")]
		public static void Import ()
		{
			Import(Options);
		}

		public static void Import (MasterToolsOptions options)
		{
			BuildContext buildContext = new BuildContext(
				options.GetDefaultOutputDirectoryFullPath()
			);
			IDatabaseBuilder builder = options.DatabaseBuilderFactory.Create(buildContext);

			string tablesDirectory = options.GetTablesDirectoryFullPath();
			foreach (TableReaderInfo info in GetTableReaderInfos())
			{
				TableContext tableContext = new TableContext(
					info.DataType,
					Path.Combine(tablesDirectory, info.Attribute.FilePath),
					!string.IsNullOrEmpty(info.Attribute.SheetName) ? info.Attribute.SheetName : options.DefaultSheetName
				);

				List<string> jsonData = options.TableReader.Read(tableContext);

				List<object> tableData = new List<object>();
				for (int i = 0; i < jsonData.Count; i++)
				{
					object obj = options.JsonDeserializer.Deserialize(tableContext.DataType, jsonData[i]);
					tableData.Add(obj);
				}

				builder.Append(tableContext.DataType, tableData);
			}

			try
			{
				builder.Build(buildContext);
			}
			catch (Exception e)
			{
				Debug.LogException(e);
				return;
			}

			RuntimeMasterDataNotification.NotifyImported();
		}

		static IEnumerable<TableReaderInfo> GetTableReaderInfos ()
		{
			return TypeCache.GetTypesWithAttribute<ImportTableFromAttribute>()
				.Where(x => x.IsClass && !x.IsAbstract)
				.Select(x => new TableReaderInfo(x, x.GetCustomAttribute<ImportTableFromAttribute>()));
		} 
	}
}
