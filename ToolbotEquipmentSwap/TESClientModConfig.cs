using UnityEngine.Networking;

namespace UnosMods.ToolbotEquipmentSwap
{
    public class TESClientModConfig : MessageBase
    {
        public string EquipSwapKeyStr { get; private set; }
        public bool StopAutoSwap { get; private set; }

        public TESClientModConfig(string equipSwapKeyStr = "", bool stopAutoSwap = false)
        {
            EquipSwapKeyStr = equipSwapKeyStr;
            StopAutoSwap = stopAutoSwap;
        }

        public override void Serialize(NetworkWriter writer)
        {
            writer.Write(EquipSwapKeyStr);
            writer.Write(StopAutoSwap);
        }
        public override void Deserialize(NetworkReader reader)
        {
            EquipSwapKeyStr = reader.ReadString();
            StopAutoSwap = reader.ReadBoolean();
        }

        public override string ToString() => $"{EquipSwapKeyStr}, {StopAutoSwap}";
    }
}
