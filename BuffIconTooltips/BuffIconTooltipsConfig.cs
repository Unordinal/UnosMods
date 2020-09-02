using BepInEx.Configuration;

namespace Unordinal.BuffIconTooltips
{
    public static class BuffIconTooltipsConfig
    {
        private static ConfigFile config;
        private static ConfigEntry<int> testInt;
        private static ConfigEntry<string> testStr;

        public static int TestInt => testInt?.Value ?? 0;
        public static string TestStr => testStr?.Value ?? "<null>";

        internal static void Initialize(ConfigFile configFile)
        {
            config = configFile;
            testInt = config.Bind("BuffIconTooltips", "TestInt", 0, "This is a test config entry of type int32.");
            testStr = config.Bind("BuffIconTooltips", "TestStr", "Hello World!", "This is a test config entry of type string.");

            BuffIconTooltips.Logger.LogDebug("Initialized configuration file.");
        }
    }
}
