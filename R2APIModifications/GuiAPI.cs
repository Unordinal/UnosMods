using System;
using System.Collections.Generic;
using System.Text;
using RoR2;
using BepInEx;
using R2API;
using R2API.Utils;

namespace R2API
{
    [R2APISubmodule]
    public static class GuiAPI
    {
        [R2APISubmoduleInit(Stage = InitStage.SetHooks)]
        internal static void SetHooks()
        {
            
        }

        [R2APISubmoduleInit(Stage = InitStage.UnsetHooks)]
        internal static void UnsetHooks()
        {

        }
    }
}
