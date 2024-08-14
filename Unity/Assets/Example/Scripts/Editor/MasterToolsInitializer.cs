using MessagePack.Resolvers;
using MessagePack;
using UnityEditor;
using MackySoft.MasterTools.Example.MasterData;
using MackySoft.MasterTools.Example.MasterData.Resolvers;
using UnityEngine;

namespace MackySoft.MasterTools
{
	public static class MasterToolsInitializer
	{
		[InitializeOnLoadMethod]
		static void Initialize ()
		{
			try
			{
				// Register resolvers.
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

			MasterDataBuilder.Options = new MasterToolsOptions()
			{
				DefaultOutputDirectoryPath = "Example/MasterData",
				DatabaseBuilderFactory = DatabaseBuilderFactory.Create(ctx =>
				{
					return new MasterMemoryDatabaseBuilder(new DatabaseBuilder(), x => new MemoryDatabase(x).Validate());
				}),
				TableReader = new XlsxTableReader(),
				JsonDeserializer = new MessagePackJsonDeserializer(),
			};
		}
	}
}
