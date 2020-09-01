using BepInEx.Logging;

namespace Unordinal.ToolbotEquipmentSwap
{
    public static class Util
    {
        private static readonly ManualLogSource Logger;

        static Util()
        {
            Logger = ToolbotEquipmentSwap.Logger;
        }

        /// <summary>
        /// <c>[Received 'ExampleMessage' from server]</c>
        /// <br/>
        /// <c>[Received 'ExampleMessage' from client 'usernamehere']</c>
        /// </summary>
        /// <param name="netMessage"></param>
        /// <param name="source"></param>
        public static void LogMessageReceived(object netMessage, bool fromServer, string source = "")
        {
            string typeName = netMessage.GetType().Name;
            string fullSource = fromServer ? "server" : "client";

            if (!string.IsNullOrWhiteSpace(source)) fullSource += $" '{source}'";

            Logger.LogDebug($"[Received '{typeName}' from {fullSource}]");
        }

        /// <summary>
        /// <c>[Serialized 'ExampleMessage']</c>
        /// <br/>
        /// <c>[Deserialized 'ExampleMessage']</c>
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="deserialize"></param>
        public static void LogSerialization(object obj, bool deserialize)
        {
            string typeName = obj.GetType().Name;
            if (deserialize)
            {
                Logger.LogDebug($"[Deserializing '{typeName}']");
            }
            else
            {
                Logger.LogDebug($"[Serializing '{typeName}']");
            }
        }

        /// <summary>
        /// <c>[Example: exampleStr]</c>
        /// </summary>
        /// <param name="value"></param>
        /// <param name="name"></param>
        public static void LogVariable(object value, string name)
        {
            Logger.LogDebug($"[{name}: {value}]");
        }
    }
}
