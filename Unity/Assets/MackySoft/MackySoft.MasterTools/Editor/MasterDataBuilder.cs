using System;
using MasterMemory;
using MessagePack;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace MackySoft.MasterTools
{
	public readonly struct TableReadContext
	{

		public string FilePath { get; }
		public string SheetName { get; }

		public TableReadContext (string filePath, string sheetName)
		{
			FilePath = filePath;
			SheetName = sheetName;
		}
	}
	public static class MasterDataBuilder
	{

		public static MasterToolsOptions Options { get; set; } = new MasterToolsOptions();

		[MenuItem("Tools/Master Tools/Import")]
		public static void Import ()
		{
			// 属性に従って、ファイルを読み込む（データ型を作成->）
			IEnumerable<TableReaderInfo> infos = TypeCache.GetTypesWithAttribute<TableReaderAttribute>()
				.Where(x => x.IsClass && !x.IsAbstract)
				.Select(x => new TableReaderInfo(x, x.GetCustomAttribute<TableReaderAttribute>()));

			// 属性が付いたマスターデータ型を取得
			DatabaseBuilderBase builder = null;

			string tablesPathRoot = Path.GetFullPath(Path.Combine(Directory.GetParent(Application.dataPath).FullName, Options.TablesDirectoryPath));

			foreach (TableReaderInfo info in infos)
			{
				TableReadContext context = new TableReadContext(
					Path.Combine(tablesPathRoot, info.Attribute.FilePath),
					!string.IsNullOrEmpty(info.Attribute.SheetName) ? info.Attribute.SheetName : Options.DefaultSheetName
				);

				// ファイルからJSON形式でデータを読み取る
				List<string> list = Options.TableReaderProvider.GetTableReader(context).Read();

				// JSON->bytes->objectに変換
				List<object> tableData = new List<object>();
				for (int i = 0; i < list.Count; i++)
				{
					byte[] bytes = MessagePackSerializer.ConvertFromJson(list[i]);
					var obj = MessagePackSerializer.Deserialize<object>(bytes);
					tableData.Add(obj);
				}

				// AppendDynamicでデータを追加
				builder.AppendDynamic(info.DataType, tableData);
			}

			byte[] data = builder.Build();

			RuntimeMasterDataNotification.NotifyImported();
		}
	}
}
