using RoR2.UI;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace UnosUtilities.UIHelper
{
    public static class UIHelper
    {
        public static GameObject BuildCanvasBackground(string name = "UIHelper.UIBase", Vector2 ctrSizeDelta = new Vector2(), bool usesCursor = false)
        {
            GameObject uiBase = new GameObject
            {
                name = name,
                layer = 5
            };
            var canvas = uiBase.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = -1;
            uiBase.AddComponent<GraphicRaycaster>();
            uiBase.AddComponent<MPEventSystemProvider>().fallBackToMainEventSystem = true;
            uiBase.AddComponent<MPEventSystemLocator>();
            if (usesCursor)
                uiBase.AddComponent<CursorOpener>();

            GameObject uiCtr = new GameObject
            {
                name = "UIHelper.UIContainer",
            };
            uiCtr.transform.SetParent(uiBase.transform, false);
            uiCtr.AddComponent<RectTransform>().sizeDelta = ctrSizeDelta;
            return uiBase;
        }
    }
}