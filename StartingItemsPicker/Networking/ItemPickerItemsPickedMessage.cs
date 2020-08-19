using R2API.Networking.Interfaces;
using RoR2;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace Unordinal.StartingItemsPicker.Networking
{
    internal struct ItemPickerItemsPickedMessage : INetMessage
    {
        public CharacterMaster Requester { get; set; }
        public List<ItemIndex> RequestedItems { get; set; }

        public void Serialize(NetworkWriter writer)
        {
            writer.Write(Requester.gameObject);
            writer.Write(RequestedItems.Count);
            foreach (var item in RequestedItems)
            {
                writer.Write(item);
            }

            string requesterName = Requester.playerCharacterMasterController?.GetDisplayName() ?? "<null>";
            StartingItemsPicker.Logger.LogInfo($"Serialized {nameof(ItemPickerItemsPickedMessage)}:");
            StartingItemsPicker.Logger.LogInfo($"{nameof(Requester)}: {requesterName}");
            StartingItemsPicker.Logger.LogInfo($"{nameof(RequestedItems)} ({RequestedItems.Count}): {string.Join(", ", RequestedItems)}");
        }

        public void Deserialize(NetworkReader reader)
        {
            Requester = reader.ReadGameObject().GetComponent<CharacterMaster>();
            RequestedItems = new List<ItemIndex>();
            int requestedItemsListCount = reader.ReadInt32();
            for (int i = 0; i < requestedItemsListCount; i++)
            {
                RequestedItems.Add(reader.ReadItemIndex());
            }

            string requesterName = Requester.playerCharacterMasterController?.GetDisplayName() ?? "<null>";
            StartingItemsPicker.Logger.LogInfo($"Deserialized {nameof(ItemPickerItemsPickedMessage)}:");
            StartingItemsPicker.Logger.LogInfo($"{nameof(Requester)}: {requesterName}");
            StartingItemsPicker.Logger.LogInfo($"{nameof(RequestedItems)} ({RequestedItems.Count}): {string.Join(", ", RequestedItems)}");
        }

        public void OnReceived()
        {
            if (NetworkServer.active)
            {
                if (Requester != null)
                {
                    string requesterName = Requester.playerCharacterMasterController?.GetDisplayName() ?? "<null>";
                    StartingItemsPicker.Logger.LogInfo($"Received {nameof(ItemPickerItemsPickedMessage)} from user '{requesterName}'.");
                    Inventory inv = Requester.inventory;
                    if (inv != null && RequestedItems != null)
                    {
                        foreach (var item in RequestedItems)
                        {
                            inv.GiveItem(item);
                        }
                    }
                    else
                    {
                        StartingItemsPicker.Logger.LogWarning($"Inventory was null for {Requester?.playerCharacterMasterController?.GetDisplayName()}!");
                        var compList = Requester.GetComponents(typeof(Component));
                        foreach (Component c in compList)
                        {
                            StartingItemsPicker.Logger.LogWarning(c.ToString() + ": " + c.name);
                        }
                    }
                }
                else StartingItemsPicker.Logger.LogWarning($"Requester for {nameof(ItemPickerItemsPickedMessage)} was null!");
            }
            else
            {
                StartingItemsPicker.Logger.LogWarning($"Wrongfully sent {nameof(ItemPickerItemsPickedMessage)} to client!");
            }
        }
    }
}
