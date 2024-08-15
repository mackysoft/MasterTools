using UnityEditor;
using MessagePack;
using MessagePack.Resolvers;
using MackySoft.MasterTools;
using MackySoft.MasterTools.Example.MasterData;
using MackySoft.MasterTools.Example.MasterData.Resolvers;

public static class MasterToolsInitializer
{
	[InitializeOnLoadMethod]
	static void Initialize ()
	{
		MasterToolsImporter.DefaultOptions = new MasterToolsOptions
		{
			DefaultOutputDirectoryPath = "Example/MasterData",
			TablesDirectoryPath = "../../MasterData",
			DefaultSheetName = "Main",
			Processor = MasterBuilderProcessor.Create(ctx =>
			{
				try
				{
					// Initialize MessagePack
					StaticCompositeResolver.Instance.Register(
						MasterMemoryResolver.Instance,
						GeneratedResolver.Instance,
						StandardResolver.Instance
					);
					var options = MessagePackSerializerOptions.Standard.WithResolver(StaticCompositeResolver.Instance);
					MessagePackSerializer.DefaultOptions = options;
				}
				catch
				{
					// Catch and forget.
				}

				return new MasterMemoryDatabaseBuilder("database", new DatabaseBuilder(), x => new MemoryDatabase(x).Validate());
			}),
			TableReader = new XlsxTableReader(),
			JsonDeserializer = new MessagePackJsonDeserializer(),
		};
	}
}