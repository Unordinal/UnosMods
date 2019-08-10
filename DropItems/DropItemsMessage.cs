using UnityEngine.Networking;

namespace UnosMods.DropItems
{
    public class DropItemsMessage : MessageBase
    {
        private string message;

        public DropItemsMessage(string msgIn = "DropItemsMessage")
        {
            message = msgIn;
        }

        public override void Serialize(NetworkWriter writer)
        {
            writer.Write(message);
        }
        public override void Deserialize(NetworkReader reader)
        {
            message = reader.ReadString();
        }

        public override string ToString() => message;
    }
}
