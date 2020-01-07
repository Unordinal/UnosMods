using System;
using System.IO;
using System.Reflection;
using UnityEngine;
using BepInEx;
using BepInEx.Logging;
using System.Text;
using System.Collections.Generic;

namespace PrefabInfoExporter
{
    // What a mess

    [BepInPlugin(PluginGUID, PluginName, PluginVersion)]
    public class PluginEntry : BaseUnityPlugin
    {
        public const string PluginGUID = "com.unordinal.prefabinfoexporter";
        public const string PluginName = "PrefabInfoExporter";
        public const string PluginVersion = "1.0.0";
        public static new ManualLogSource Logger { get; private set; }

        private static string exportPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "/Exported/";

        public static class ResourcePaths
        {
            #region Materials
            public const string MaterialsPath = "Materials/";
            #endregion

            public const string NetworkSoundEventDefs = "NetworkSoundEventDefs/";

            #region Prefabs
            public const string Prefabs = "Prefabs/";
            public const string CharacterBodies = Prefabs + "CharacterBodies/";
            public const string CharacterDisplays = Prefabs + "CharacterDisplays/";
            public const string Crosshair = Prefabs + "Crosshair/";
            public const string Effects = Prefabs + "Effects/";
            public const string ImpactEffects = Effects + "ImpactEffects/";
            public const string OmniEffect = Effects + "OmniEffect/";
            public const string OrbEffects = Effects + "OrbEffects/";
            public const string Tracers = Effects + "Tracers/";
            public const string GhostProjectiles = Prefabs + "ProjectileGhosts/";
            public const string NetworkedObjects = Prefabs + "NetworkedObjects/";
            public const string Encounters = NetworkedObjects + "Encounters/";
            public const string PickupModels = Prefabs + "PickupModels/";
            public const string PositionIndicators = Prefabs + "PositionIndicators/";
            public const string Projectiles = Prefabs + "Projectiles/";
            public const string UI = Prefabs + "UI/";
            public const string Loadout = UI + "Loadout/";
            public const string Logbook = UI + "Logbook/";
            #endregion

            #region SpawnCards
            public const string SpawnCards = "SpawnCards/";
            public const string CharacterSpawnCards = SpawnCards + "CharacterSpawnCards/";
            public const string InteractableSpawnCard = SpawnCards + "InteractableSpawnCard/";
            #endregion

            #region Textures
            public const string Textures = "Textures/";
            public const string AchievementIcons = Textures + "AchievementIcons/";
            public const string BodyIcons = Textures + "BodyIcons/";
            public const string ItemIcons = Textures + "ItemIcons/";
            public const string BG = ItemIcons + "BG/";
            public const string MiscIcons = Textures + "MiscIcons/";
            #endregion
        }

        private static string ExportPath
        {
            get => exportPath;
            set
            {
                exportPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + $"/{value}";
                Logger.LogInfo("Export location set to " + exportPath);
            }
        }

        public void Awake()
        {
            Logger = base.Logger;
            Logger.LogMessage("Awake()");
        }

        public void Start()
        {
            Logger.LogMessage("Start()");
            StringBuilder exportLog = new StringBuilder(ExportPath + "\n\n");
            try
            {
                Logger.LogMessage("Attempting export of selected prefab info...");

                exportLog.AppendLine(ResourcePaths.InteractableSpawnCard);
                ExportAllPrefabInfoInPath(ResourcePaths.InteractableSpawnCard);
            }
            catch (Exception e)
            {
                string message = e.Message + "\n" + e.StackTrace;
                Logger.LogError(message);
                exportLog.AppendLine("\n" + message);
            }

            Logger.LogInfo(exportLog);
            File.WriteAllText(ExportPath + "log.txt", exportLog.ToString());
        }

        private static void ExportAllPrefabInfoInPath(string prefabsPath)
        {
            Logger.LogWarning(prefabsPath);
            Directory.CreateDirectory(ExportPath + prefabsPath);
            ExportPrefabsInPath(prefabsPath, out var typeList);
            ExportTypes(typeList, prefabsPath);
        }

        private static void ExportPrefabsInPath(string prefabsPath, out List<Type> typeList)
        {
            typeList = new List<Type>();
            if (string.IsNullOrWhiteSpace(prefabsPath)) return;
            string baseExportPath = ExportPath + prefabsPath;

            StringBuilder objInfo = new StringBuilder();
            foreach (var obj in Resources.LoadAll<UnityEngine.Object>(prefabsPath))
            {
                objInfo.Clear();
                objInfo.AppendLine(obj.name);
                
                if (obj is GameObject gameObj)
                {
                    var components = gameObj.GetComponents<Component>();
                    objInfo.AppendLine(ListComponents(components, ref typeList));
                    objInfo.AppendLine("Children\n{\n" + ListChildren(gameObj.transform, ">", "\t") + "\n}");
                }
                else
                {
                    Type type = obj.GetType();
                    objInfo.AppendLine($"[{type.FullName}]");
                    foreach (var fieldInfo in type.GetFields())
                    {
                        var fieldVal = fieldInfo.GetValue(obj).ToString();
                        objInfo.AppendLine($"{fieldInfo.Name} = {(string.IsNullOrWhiteSpace(fieldVal) ? "<null>" : fieldVal)}");
                    }
                }

                File.WriteAllText($"{baseExportPath}{obj.name}.txt", objInfo.ToString());
            }
        }

        private static void ExportTypes(IEnumerable<Type> types, string typesPath, string delimiter = ">")
        {
            string baseExportPath = ExportPath + typesPath;
            
            StringBuilder typeList = new StringBuilder("\n(Types)\n");
            foreach (var type in types)
            {
                typeList.AppendLine($"{type.Name}");
                
                var members = type.GetMembers();
                foreach (var member in members)
                {
                    bool isMonoBehaviour = member.DeclaringType.Name == "MonoBehaviour";
                    bool isComponent = member.DeclaringType.Name == "Component";
                    bool isBehaviour = member.DeclaringType.Name == "Behaviour";

                    if (!isMonoBehaviour && !isComponent && !isBehaviour)
                    {
                        typeList.AppendLine($"{delimiter} {member.MemberType}: {member.Name}");
                    }
                }
                typeList.AppendLine();
            }
            File.WriteAllText($"{baseExportPath}types.txt", typeList.ToString());
        }

        private static string ListComponents(IEnumerable<Component> components, ref List<Type> typeList, string delimiter = ">", string tabbed = "")
        {
            StringBuilder compList = new StringBuilder();
            foreach (var component in components)
            {
                Type type = component.GetType();
                compList.AppendLine($"{tabbed}[{type.FullName}]");
                if (type.FullName != "UnityEngine.Transform")
                {
                    foreach (var fieldInfo in type.GetFields())
                    {
                        compList.AppendLine($"{tabbed}{fieldInfo.Name} = {fieldInfo.GetValue(component)}");
                    }
                }
                else
                {
                    Transform transform = (Transform)component;
                    compList.AppendLine($"{tabbed}\tPos: [{transform.localPosition}]");
                    compList.AppendLine($"{tabbed}\tRot: [{transform.eulerAngles}]");
                    compList.AppendLine($"{tabbed}\tScl: [{transform.localScale}]");
                }

                if (!typeList.Contains(type))
                    typeList.Add(type);

                compList.AppendLine();
            }
            return compList.ToString();
        }

        private static string ListComponents(IEnumerable<Component> components, string delimiter = ">", string tabbed = "")
        {
            var dummyList = new List<Type>();
            return ListComponents(components, ref dummyList, delimiter, tabbed);
        }

        private static string ListChildren(Transform transform, string delimiter = ">", string tabbed = "")
        {
            StringBuilder childList = new StringBuilder();
            for (int i = 0; i < transform.childCount; i++)
            {
                var gameObj = transform.GetChild(i).gameObject;
                childList.AppendLine($"{tabbed}{gameObj.name}");

                var components = gameObj.GetComponents<Component>();
                childList.AppendLine(tabbed + ListComponents(components, delimiter, tabbed + "\t"));
                childList.AppendLine(tabbed + "Children\n" + tabbed + "{\n" + ListChildren(transform.GetChild(i), delimiter, tabbed + "\t") + "\n" + tabbed + "}");
            }

            return childList.ToString();
        }
    }
}
