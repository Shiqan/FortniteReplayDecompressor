using Unreal.Core.Attributes;
using Unreal.Core.Contracts;
using Unreal.Core.Models;
using Unreal.Core.Models.Enums;

namespace FortniteReplayReader.Models.NetFieldExports
{
    [NetFieldExportClassNetCache("FortInventory_ClassNetCache")]
    public class FortInventoryCache
    {
        [NetFieldExportRPCStruct("Inventory", "/Script/FortniteGame.FortInventory")]
        public FortInventory Inventory { get; set; }
    }

    [NetFieldExportGroup("/Script/FortniteGame.FortInventory")]
    public class FortInventory : INetFieldExportGroup
    {
        [NetFieldExport("RemoteRole", RepLayoutCmdType.Ignore)]
        public uint? RemoteRole { get; set; }

        [NetFieldExport("Owner", RepLayoutCmdType.PropertyObject)]
        public uint? Owner { get; set; }

        [NetFieldExport("Role", RepLayoutCmdType.Ignore)]
        public uint? Role { get; set; }

        [NetFieldExport("Count", RepLayoutCmdType.PropertyInt)]
        public int? Count { get; set; }

        [NetFieldExport("ItemDefinition", RepLayoutCmdType.PropertyObject)]
        public uint? ItemDefinition { get; set; }

        [NetFieldExport("OrderIndex", RepLayoutCmdType.PropertyUInt16)]
        public ushort? OrderIndex { get; set; }

        [NetFieldExport("Durability", RepLayoutCmdType.PropertyFloat)]
        public float? Durability { get; set; }

        [NetFieldExport("Level", RepLayoutCmdType.PropertyInt)]
        public int? Level { get; set; }

        [NetFieldExport("LoadedAmmo", RepLayoutCmdType.PropertyInt)]
        public int? LoadedAmmo { get; set; }

        [NetFieldExport("A", RepLayoutCmdType.PropertyInt)]
        public int? A { get; set; }

        [NetFieldExport("B", RepLayoutCmdType.PropertyInt)]
        public int? B { get; set; }

        [NetFieldExport("C", RepLayoutCmdType.PropertyInt)]
        public int? C { get; set; }

        [NetFieldExport("D", RepLayoutCmdType.PropertyInt)]
        public int? D { get; set; }

        [NetFieldExport("inventory_overflow_date", RepLayoutCmdType.PropertyBool)]
        public bool? InventoryOverflowDate { get; set; }

        [NetFieldExport("bWasGifted", RepLayoutCmdType.PropertyBool)]
        public bool? bWasGifted { get; set; }

        [NetFieldExport("bIsReplicatedCopy", RepLayoutCmdType.PropertyBool)]
        public bool? bIsReplicatedCopy { get; set; }

        [NetFieldExport("bIsDirty", RepLayoutCmdType.PropertyBool)]
        public bool? bIsDirty { get; set; }

        [NetFieldExport("bUpdateStatsOnCollection", RepLayoutCmdType.PropertyBool)]
        public bool? bUpdateStatsOnCollection { get; set; }

        [NetFieldExport("StateValues", RepLayoutCmdType.DynamicArray)]
        public FortItemEntryStateValue[] StateValues { get; set; }

        [NetFieldExport("ParentInventory", RepLayoutCmdType.PropertyObject)]
        public uint? ParentInventory { get; set; }

        [NetFieldExport("Handle", RepLayoutCmdType.PropertyInt)]
        public int? Handle { get; set; }

        [NetFieldExport("AlterationInstances", RepLayoutCmdType.Ignore)]
        public DebuggingObject[] AlterationInstances { get; set; }

        [NetFieldExport("GenericAttributeValues", RepLayoutCmdType.Ignore)]
        public DebuggingObject[] GenericAttributeValues { get; set; }

        [NetFieldExport("ReplayPawn", RepLayoutCmdType.PropertyObject)]
        public uint? ReplayPawn { get; set; }
    }
}