using R2API.Networking.Interfaces;
using RoR2;
using System.Collections.Generic;
using UnityEngine.Networking;

namespace Unordinal.StartingItemsPicker.Networking
{
    internal struct ItemPickerInfoMessage : INetMessage
    {
        public byte MaxItems { get; set; }
        public byte MaxStack { get; set; }
        public List<ItemIndex> AllowedItems { get; set; }

        public void Serialize(NetworkWriter writer)
        {
            writer.Write(MaxItems);
            writer.Write(MaxStack);
            writer.Write(AllowedItems.Count);
            foreach (var item in AllowedItems)
            {
                writer.Write(item);
            }

            StartingItemsPicker.Logger.LogInfo($"Serialized {nameof(ItemPickerInfoMessage)}.");
            StartingItemsPicker.Logger.LogInfo($"{nameof(MaxItems)}: {MaxItems}");
            StartingItemsPicker.Logger.LogInfo($"{nameof(MaxStack)}: {MaxStack}");
            StartingItemsPicker.Logger.LogInfo($"{nameof(AllowedItems)} ({AllowedItems.Count}): {string.Join(", ", AllowedItems)}");
        }

        public void Deserialize(NetworkReader reader)
        {
            MaxItems = reader.ReadByte();
            MaxStack = reader.ReadByte();
            AllowedItems = new List<ItemIndex>();
            int allowedItemListCount = reader.ReadInt32();
            for (int i = 0; i < allowedItemListCount; i++)
            {
                AllowedItems.Add(reader.ReadItemIndex());
            }

            StartingItemsPicker.Logger.LogInfo($"Deserialized {nameof(ItemPickerInfoMessage)}.");
            StartingItemsPicker.Logger.LogInfo($"{nameof(MaxItems)}: {MaxItems}");
            StartingItemsPicker.Logger.LogInfo($"{nameof(MaxStack)}: {MaxStack}");
            StartingItemsPicker.Logger.LogInfo($"{nameof(AllowedItems)} ({AllowedItems.Count}): {string.Join(", ", AllowedItems)}");
        }

        public void OnReceived()
        {
            if (!StartingItemsPicker.pickedItems)
            {
                StartingItemsPicker.DisplayItemPicker(MaxItems, MaxStack, AllowedItems);
                StartingItemsPicker.pickedItems = true;
            }
        }
    }
}
