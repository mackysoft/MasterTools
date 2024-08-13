using MasterMemory;
using MessagePack;

namespace MackySoft.MasterTools.Example
{

    [ImportTableFrom("Quest")]
    [MemoryTable("Quest")]
    [MessagePackObject]
    public sealed class QuestMasterData
    {

        [PrimaryKey]
        [Key(0)]
        public int Id { get; private set; }

        [Key(1)]
        public string Name { get; private set; }

        public QuestMasterData(int Id, string Name)
        {
            this.Id = Id;
            this.Name = Name;
        }
    }
}