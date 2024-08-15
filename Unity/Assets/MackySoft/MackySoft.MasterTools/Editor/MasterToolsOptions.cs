using System;
using System.IO;
using UnityEngine;

namespace MackySoft.MasterTools
{

	public sealed class MasterToolsOptions
	{

		string m_TablesDirectoryPath = "../MasterData";
		string m_DefaultOutputDirectoryPath = "MasterData";
		string m_DefaultSheetName = "Main";
		IDatabaseBuilderFactory m_DatabaseBuilderFactory;
		ITableReader m_TableReader;
		IJsonDeserializer m_JsonDeserializer;

		/// <summary>
		/// Relative path of the table directory, with <see cref="Application.dataPath"/> as root.
		/// </summary>
		public string TablesDirectoryPath
		{
			get => m_TablesDirectoryPath;
			set => m_TablesDirectoryPath = !string.IsNullOrEmpty(value) ? value : throw new ArgumentException($"{nameof(TablesDirectoryPath)} is null or empty.", nameof(TablesDirectoryPath));
		}


		/// <summary>
		/// Relative path of the output directory, with <see cref="Application.dataPath"/> as root.
		/// </summary>
		public string DefaultOutputDirectoryPath
		{
			get => m_DefaultOutputDirectoryPath;
			set => m_DefaultOutputDirectoryPath = !string.IsNullOrEmpty(value) ? value : throw new ArgumentException($"{nameof(DefaultOutputDirectoryPath)} is null or empty.", nameof(DefaultOutputDirectoryPath));
		}

		/// <summary>
		/// Default sheet name to read.
		/// </summary>
		public string DefaultSheetName
		{
			get => m_DefaultSheetName;
			set => m_DefaultSheetName = !string.IsNullOrEmpty(value) ? value : throw new ArgumentException($"{nameof(DefaultSheetName)} is null or empty.", nameof(DefaultSheetName));
		}

		public IDatabaseBuilderFactory DatabaseBuilderFactory { get => m_DatabaseBuilderFactory; set => m_DatabaseBuilderFactory = value; }
		public ITableReader TableReader { get => m_TableReader; set => m_TableReader = value; }
		public IJsonDeserializer JsonDeserializer { get => m_JsonDeserializer; set => m_JsonDeserializer = value; }

		/// <summary>
		/// Validate the options.
		/// </summary>
		/// <exception cref="NullReferenceException"></exception>
		public void Validate ()
		{
			if (m_DatabaseBuilderFactory == null)
			{
				throw new NullReferenceException($"{nameof(DatabaseBuilderFactory)} is null.");
			}
			if (m_TableReader == null)
			{
				throw new NullReferenceException($"{nameof(TableReader)} is null.");
			}
			if (m_JsonDeserializer == null)
			{
				throw new NullReferenceException($"{nameof(JsonDeserializer)} is null.");
			}
		}

		/// <summary>
		/// Get the full path of the tables directory.
		/// </summary>
		public string GetTablesDirectoryFullPath ()
		{
			return Path.GetFullPath(Path.Combine(Application.dataPath, m_TablesDirectoryPath));
		}

		/// <summary>
		/// Get the full path of the default output directory.
		/// </summary>
		public string GetDefaultOutputDirectoryFullPath ()
		{
			return Path.GetFullPath(Path.Combine(Application.dataPath, m_DefaultOutputDirectoryPath));
		}
	}
}