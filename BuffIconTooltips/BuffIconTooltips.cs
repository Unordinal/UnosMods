using BepInEx;
using BepInEx.Logging;
using R2API.Utils;
using RoR2;
using RoR2.UI;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace Unordinal.BuffIconTooltips
{
    [BepInDependency(R2API.R2API.PluginGUID)]
    [BepInPlugin(PluginGUID, PluginName, PluginVersion)]
    [NetworkCompatibility(CompatibilityLevel.NoNeedForSync)]
    public class BuffIconTooltips : BaseUnityPlugin
    {
        public const string PluginGUID = "Unordinal.BuffIconTooltips";
        public const string PluginName = "Buff Icon Tooltips";
        public const string PluginVersion = "1.0.0";
        public static new ManualLogSource Logger { get; private set; }


        // Called when the script instance is initialized. Only called once during the lifetime of the script.
        internal void Awake()
        {
            Logger = base.Logger;
            // BuffIconTooltipsConfig.Initialize(Config);

            On.RoR2.UI.BuffIcon.Awake += BuffIcon_Awake;
            On.RoR2.UI.BuffIcon.UpdateIcon += BuffIcon_UpdateIcon;
        }

        internal void Start()
        {
            BuffInfoProvider.Init();
        }

        private void BuffIcon_Awake(On.RoR2.UI.BuffIcon.orig_Awake orig, BuffIcon self)
        {
            orig(self);
            self.gameObject.AddComponent<GraphicRaycaster>();
            self.gameObject.AddComponent<TooltipProvider>();
        }

        private void BuffIcon_UpdateIcon(On.RoR2.UI.BuffIcon.orig_UpdateIcon orig, BuffIcon self)
        {
            orig(self);
            var tooltipProvider = self.GetComponent<TooltipProvider>();
            if (tooltipProvider != null)
            {
                BuffInfo buffInfo = BuffInfoProvider.GetBuffInfoFromIndex(self.buffIndex);
                string title;
                string body;
                Color titleColor = Color.black;
                Color bodyColor = new Color(0.6f, 0.6f, 0.6f, 1f);

                if (buffInfo != null)
                {
                    title = buffInfo.Name;
                    body = buffInfo.Description;
                    titleColor = buffInfo.Color;
                }
                else
                {
                    title = BuffCatalog.GetBuffDef(self.buffIndex)?.name;
                    body = "";
                }

                TooltipContent ttContent = new TooltipContent
                {
                    titleToken = title,
                    bodyToken = body,
                    titleColor = titleColor,
                    bodyColor = bodyColor
                };

                tooltipProvider.SetContent(ttContent);
            }
        }
    }
}
