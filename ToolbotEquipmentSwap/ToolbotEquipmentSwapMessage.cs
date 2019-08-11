using UnityEngine.Networking;

namespace UnosMods.ToolbotEquipmentSwap
{
    public class ToolbotEquipmentSwapMessage : MessageBase
    {
        private string message;

        public ToolbotEquipmentSwapMessage(string msgIn = "ToolbotEquipmentSwapMessage")
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
