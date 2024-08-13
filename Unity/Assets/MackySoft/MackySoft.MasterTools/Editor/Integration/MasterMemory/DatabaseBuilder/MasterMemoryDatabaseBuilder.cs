using MasterMemory;
using MasterMemory.Validation;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace MackySoft.MasterTools
{

	public sealed class MasterMemoryDatabaseBuilder : IDatabaseBuilder
	{

		const string DatabaseName = "master-data.bytes";

		readonly DatabaseBuilderBase m_Builder;
		readonly Func<byte[], ValidateResult> m_OnValidate;

		public MasterMemoryDatabaseBuilder (DatabaseBuilderBase builder, Func<byte[], ValidateResult> onValidate)
		{
			m_Builder = builder ?? throw new ArgumentNullException(nameof(builder));
			m_OnValidate = onValidate ?? throw new ArgumentNullException(nameof(onValidate));
		}

		public void Append (Type type, IList<object> data)
		{
			m_Builder.AppendDynamic(type, data);
		}

		public void Build (BuildContext context)
		{
			byte[] data = m_Builder.Build();
			
			// Validate database.
			ValidateResult validateResult = m_OnValidate(data);
			if (validateResult.IsValidationFailed)
			{
				Debug.LogError(validateResult.FormatFailedResults());
				return;
			}

			// Write database.
			Directory.CreateDirectory(context.OutputDirectoryPath);
			using (var stream = File.Create(Path.Combine(context.OutputDirectoryPath, DatabaseName)))
			{
				m_Builder.WriteToStream(stream);
			}

			AssetDatabase.Refresh();
		}
	}
}
