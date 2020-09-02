using R2API.Networking.Interfaces;
using System.Collections.Generic;
using UnityEngine.Networking;

namespace Unordinal.UnoUtilities.Networking
{
    internal struct TestMessage : INetMessage
    {
        public string TestMessageStr { get; set; }
        public List<int> TestMessageIntList { get; set; }

        public void Serialize(NetworkWriter writer)
        {
            writer.Write(TestMessageStr);
            writer.Write(TestMessageIntList.Count);
            foreach (var item in TestMessageIntList)
            {
                writer.Write(item);
            }

            UnoUtilities.Logger.LogDebug($"Serialized {nameof(TestMessage)}.");
            LogContents();
        }

        public void Deserialize(NetworkReader reader)
        {
            TestMessageStr = reader.ReadString();
            TestMessageIntList = new List<int>();
            foreach (var item in TestMessageIntList)
            {
                TestMessageIntList.Add(reader.ReadInt32());
            }

            UnoUtilities.Logger.LogDebug($"Deserialized {nameof(TestMessage)}.");
            LogContents();
        }

        public void OnReceived()
        {
            UnoUtilities.Logger.LogDebug($"Received {nameof(TestMessage)}.");
            LogContents();
        }

        private void LogContents()
        {
            UnoUtilities.Logger.LogDebug($"{nameof(TestMessageStr)}: {TestMessageStr}");
            UnoUtilities.Logger.LogDebug($"{nameof(TestMessageIntList)} ({TestMessageIntList.Count}): {string.Join(", ", TestMessageIntList)}");
        }
    }
}
