using R2API.Networking.Interfaces;
using UnityEngine.Networking;

namespace Unordinal.DebugCheats.Networking
{
    internal struct ExampleMessage : INetMessage
    {
        internal int Integer;
        internal string Str;

        public void Serialize(NetworkWriter writer)
        {
            writer.Write(Integer);
            writer.Write(Str);
        }

        public void Deserialize(NetworkReader reader)
        {
            Integer = reader.ReadInt32();
            Str = reader.ReadString();
        }

        public void OnReceived() => DebugCheats.Logger.LogWarning("int: " + Integer + " str: " + Str);
    }
}
