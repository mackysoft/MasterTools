using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace MackySoft.MasterTools
{
	public readonly struct TableImportInfo
	{
		public Type DataType { get; }
		public ImportTableFromAttribute Attribute { get; }

		public TableImportInfo (Type dataType, ImportTableFromAttribute attribute)
		{
			DataType = dataType;
			Attribute = attribute;
		}
	}

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

		public string TablesDirectoryPath { get; }
		public string OutputDirectoryPath { get; }

		public BuildContext (string tablesDirectoryPath, string outputDirectoryPath)
		{
			TablesDirectoryPath = !string.IsNullOrEmpty(tablesDirectoryPath) ? tablesDirectoryPath : throw new ArgumentException($"{nameof(tablesDirectoryPath)} is null or empty.", nameof(tablesDirectoryPath));
			OutputDirectoryPath = !string.IsNullOrEmpty(outputDirectoryPath) ? outputDirectoryPath : throw new ArgumentException($"{nameof(outputDirectoryPath)} is null or empty.", nameof(outputDirectoryPath));
		}
	}

	public static class MasterDataBuilder
	{

		public static MasterToolsOptions DefaultOptions { get; set; } = new MasterToolsOptions();

		[MenuItem("Tools/Master Tools/Import (with default options)")]
		public static void ImportWithDefaultOptions ()
		{
			if (DefaultOptions == null)
			{
				throw new InvalidOperationException($"{nameof(DefaultOptions)} is null. Please set the default options.");
			}
			Import(DefaultOptions);
		}

		public static void Import (MasterToolsOptions options)
		{
			if (options == null)
			{
				throw new ArgumentNullException(nameof(options));
			}
			options.Validate();

			BuildContext buildContext = new BuildContext(
				options.GetTablesDirectoryFullPath(),
				options.GetDefaultOutputDirectoryFullPath()
			);

			Debug.Log($"[MasterTools] Start import tables from \'{buildContext.TablesDirectoryPath}\'");

			IDatabaseBuilder builder = null;
			try
			{
				builder = options.DatabaseBuilderFactory.Create(buildContext);
				if (builder == null)
				{
					throw new NullReferenceException($"Created builder is null.");
				}
			}
			catch
			{
				Debug.LogError("[MasterTools] Failed to create database builder. See the following log for details.");
				throw;
			}

			StringBuilder importLogBuilder = new StringBuilder();
			foreach (TableImportInfo info in GetTableReaderInfos())
			{
				TableContext tableContext = new TableContext(
					info.DataType,
					Path.Combine(buildContext.TablesDirectoryPath, info.Attribute.FilePath),
					!string.IsNullOrEmpty(info.Attribute.SheetName) ? info.Attribute.SheetName : options.DefaultSheetName
				);

				try
				{
					importLogBuilder.Clear();
					importLogBuilder.Append($"[MasterTools] Import \'{tableContext.DataType.Name}\' table from sheet '{tableContext.SheetName}' in \'{tableContext.FilePath}\'");

					// Read table data.
					List<string> jsonData = options.TableReader.Read(tableContext);
					if (jsonData.Count > 0)
					{
						importLogBuilder.AppendLine();
						importLogBuilder.AppendLine(jsonData.Count + " items found.");
						importLogBuilder.Append("- ");
						importLogBuilder.AppendJoin("\n- ", jsonData);
					}
					else
					{
						importLogBuilder.AppendLine("No items found.");
					}

					// Deserialize table data.
					List<object> tableData = new List<object>();
					for (int i = 0; i < jsonData.Count; i++)
					{
						object obj = options.JsonDeserializer.Deserialize(tableContext.DataType, jsonData[i]);
						tableData.Add(obj);
					}

					builder.Append(tableContext.DataType, tableData);

					Debug.Log(importLogBuilder.ToString());
				}
				catch
				{
					Debug.LogError($"[MasterTools] Failed to import \'{tableContext.DataType.Name}\' table from \'{tableContext.SheetName}\' in \'{tableContext.FilePath}\'. See the following log for details.");
					throw;
				}
			}

			try
			{
				builder.Build(buildContext);
			}
			catch
			{
				Debug.LogError("[MasterTools] Failed to build tables. See the following log for details.");
				throw;
			}

			Debug.Log($"[MasterTools] Successfully imported tables.");

			RuntimeMasterDataNotification.NotifyImported();
		}

		static IEnumerable<TableImportInfo> GetTableReaderInfos ()
		{
			return TypeCache.GetTypesWithAttribute<ImportTableFromAttribute>()
				.Where(x => x.IsClass && !x.IsAbstract)
				.Select(x => new TableImportInfo(x, x.GetCustomAttribute<ImportTableFromAttribute>()));
		} 
	}
}
