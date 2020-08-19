using R2API.Networking.Interfaces;
using RoR2;
using UnityEngine.Networking;

namespace Unordinal.DropItems.Networking
{
    internal struct DropLastItemMessage : INetMessage
    {
        public CharacterMaster Requester { get; set; }

        public DropLastItemMessage(CharacterMaster requester)
        {
            Requester = requester;
        }

        public void Serialize(NetworkWriter writer)
        {
            writer.Write(Requester.gameObject);
        }

        public void Deserialize(NetworkReader reader)
        {
            Requester = reader.ReadGameObject()?.GetComponent<CharacterMaster>();
        }

        public void OnReceived()
        {
            if (Requester != null)
            {
                string requesterName = Requester.playerCharacterMasterController?.GetDisplayName() ?? "<null>";
                DropItems.Logger.LogDebug($"Received '{nameof(DropLastItemMessage)}' from '{requesterName}'.");

                if (Requester.inventory != null)
                {
                    DropItems.DropLastItem(Requester);
                }
                else
                {
                    DropItems.Logger.LogWarning($"Received '{nameof(DropLastItemMessage)}' but the given requester's inventory was null!");
                }
            }
            else
            {
                DropItems.Logger.LogWarning($"Received '{nameof(DropLastItemMessage)}' but the given requester was null!");
            }
        }
    }
}
