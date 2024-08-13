using MasterMemory;
using MessagePack;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MackySoft.MasterTools.Example
{

    [TableReader("Quest")]
    [MemoryTable("Quest")]
    [MessagePackObject]
    public sealed class QuestMasterData
    {

        [PrimaryKey]
        [Key(0)]
        public int Id { get; private set; }

        [Key(1)]
        public string Name { get; private set; }
    }
}