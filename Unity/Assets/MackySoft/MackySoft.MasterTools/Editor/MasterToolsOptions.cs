using System;
using static UnityEditor.Progress;
using System.IO;
using UnityEngine;

namespace MackySoft.MasterTools
{
	public readonly struct TableReaderInfo
	{
		public Type DataType { get; }
		public ImportTableFromAttribute Attribute { get; }

		public TableReaderInfo (Type dataType, ImportTableFromAttribute attribute)
		{
			DataType = dataType;
			Attribute = attribute;
		}
	}
	
	public sealed class MasterToolsOptions
	{

		string m_TablesDirectoryPath = "../MasterData";
		string m_DefaultOutputDirectoryPath = "MasterData";
		string m_DefaultSheetName = "Main";
		IDatabaseBuilderFactory m_DatabaseBuilderFactory;
		ITableReader m_TableReader;
		IJsonDeserializer m_JsonDeserializer;

		public string TablesDirectoryPath
		{
			get => m_TablesDirectoryPath;
			set => m_TablesDirectoryPath = !string.IsNullOrEmpty(value) ? value : throw new ArgumentException($"{nameof(TablesDirectoryPath)} is null or empty.", nameof(TablesDirectoryPath));
		}

		public string DefaultOutputDirectoryPath
		{
			get => m_DefaultOutputDirectoryPath;
			set => m_DefaultOutputDirectoryPath = !string.IsNullOrEmpty(value) ? value : throw new ArgumentException($"{nameof(DefaultOutputDirectoryPath)} is null or empty.", nameof(DefaultOutputDirectoryPath));
		}

		public string DefaultSheetName
		{
			get => m_DefaultSheetName;
			set => m_DefaultSheetName = !string.IsNullOrEmpty(value) ? value : throw new ArgumentException($"{nameof(DefaultSheetName)} is null or empty.", nameof(DefaultSheetName));
		}

		public IDatabaseBuilderFactory DatabaseBuilderFactory { get => m_DatabaseBuilderFactory; set => m_DatabaseBuilderFactory = value; }
		public ITableReader TableReader { get => m_TableReader; set => m_TableReader = value; }
		public IJsonDeserializer JsonDeserializer { get => m_JsonDeserializer; set => m_JsonDeserializer = value; }

		public string GetTablesDirectoryFullPath ()
		{
			return Path.GetFullPath(Path.Combine(Directory.GetParent(Application.dataPath).FullName, m_TablesDirectoryPath));
		}

		public string GetDefaultOutputDirectoryFullPath ()
		{
			return Path.GetFullPath(Path.Combine(Application.dataPath, m_DefaultOutputDirectoryPath));
		}
	}
}