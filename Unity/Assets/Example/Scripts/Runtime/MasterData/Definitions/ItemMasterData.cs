using MasterMemory;
using MessagePack;

namespace MackySoft.MasterTools.Example
{
	[ImportTableFrom("Item")]
	[MemoryTable("Item")]
	[MessagePackObject]
	public sealed class ItemMasterData
    {

        [PrimaryKey]
		[Key(0)]
		public int Id { get; private set; }

		[Key(1)]
		public string Name { get; private set; }

        public ItemMasterData (int Id, string Name)
        {
            this.Id = Id;
            this.Name = Name;
        }
    }
}