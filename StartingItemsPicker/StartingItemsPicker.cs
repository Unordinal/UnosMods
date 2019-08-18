using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BepInEx;
using BepInEx.Configuration;
using LeTai.Asset.TranslucentImage;
using MiniRpcLib;
using MiniRpcLib.Action;
using R2API.Utils;
using RoR2;
using RoR2.UI;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Networking;
using UnityEngine.UI;

namespace UnosMods.StartingItemsPicker
{
    [BepInDependency(R2API.R2API.PluginGUID)]
    [BepInPlugin(PluginGUID, PluginName, PluginVersion)]
    public class StartingItemsPicker : BaseUnityPlugin
    {
        public const string PluginGUID = "com.unordinal.startingitemspicker";
        public const string PluginName = "Starting Items Picker";
        public const string PluginVersion = "1.0.0";
        private const string PluginRpcGUID = "UnosMods.StartingItemsPicker";
        public static ConfigWrapper<ushort> NumberOfItems { get; private set; }
        public static ConfigWrapper<ushort> AllowedStack { get; private set; }

        IRpcAction<Action<NetworkWriter>> NetDisplayItemPickerAction;
        IRpcAction<Action<NetworkWriter>> NetItemsPickedAction;

        public void Awake()
        {
            NumberOfItems = Config.Wrap<ushort>(
                "Settings", 
                "NumberOfItems", 
                "The number of items players can pick at the start of the run. (Default: 2)", 
                2
                );
            AllowedStack = Config.Wrap<ushort>(
                "Settings", 
                "AllowedStack", 
                "How many items of the same type players can receive. (Default: 1)", 
                1
                );

            var miniRpc = MiniRpc.CreateInstance(PluginRpcGUID);
            NetDisplayItemPickerAction = miniRpc.RegisterAction(Target.Client, NetDisplayItemPicker);
            NetItemsPickedAction = miniRpc.RegisterAction(Target.Server, NetItemsPicked);
            On.RoR2.Run.BeginStage += Run_BeginStage;
        }

        private void Run_BeginStage(On.RoR2.Run.orig_BeginStage orig, Run self)
        {
            orig(self);
            if (NetworkServer.active)
            {
                if ((NumberOfItems.Value > 0 && AllowedStack.Value > 0) && self.stageClearCount == 0)
                {
                    List<ItemIndex> availableItems = Run.instance.availableTier1DropList.Select(x => x.itemIndex).Where(x => x != ItemIndex.None).ToList();
                    CallNetDisplayItemPicker(NumberOfItems.Value, AllowedStack.Value, availableItems);
                }
            }
        }

        // Attributes for clarity only, unaffects .dll when compiled with VS.
        [Server]
        public void CallNetDisplayItemPicker(ushort numberOfItems, ushort allowedStack, List<ItemIndex> availableItems)
        {
            Logger.LogDebug(string.Join(",", availableItems));
            NetDisplayItemPickerAction.Invoke(w =>
            {
                w.Write(numberOfItems);
                w.Write(allowedStack);
                w.Write(availableItems.Count);
                foreach (var item in availableItems)
                    w.WriteItemIndex(item);
            });
        }

        [Client]
        private void NetDisplayItemPicker(NetworkUser user, NetworkReader reader)
        {
            var numOfItems = reader.ReadUInt16();
            var allowedStack = reader.ReadUInt16();
            var itemCount = reader.ReadInt32();
            var items = new List<ItemIndex>();
            for (int i = 0; i < itemCount; i++)
                items.Add(reader.ReadItemIndex());
            DisplayItemPicker(items, numOfItems, allowedStack, CallNetItemsPicked);
        }

        [Client]
        public void CallNetItemsPicked(List<ItemIndex> items)
        {
            NetItemsPickedAction.Invoke(w =>
            {
                w.Write(items.Count);
                foreach (var item in items)
                    w.WriteItemIndex(item);
            });
        }

        [Server]
        private void NetItemsPicked(NetworkUser user, NetworkReader reader)
        {
            var itemCount = reader.ReadInt32();
            var items = new List<ItemIndex>();
            for (int i = 0; i < itemCount; i++)
                items.Add(reader.ReadItemIndex());
            var inv = user?.master?.inventory;
            if (inv)
            {
                foreach (var item in items)
                {
                    inv.GiveItem(item);
                }
            }
        }

        public delegate void ItemsCallback(List<ItemIndex> items);

        public async Task<GameObject> GetItemInventoryDisplayAsync()
        {
            GameObject IID;
            do {
                Logger.LogInfo("Waiting for client to spawn ItemInventoryDisplay...");
                await Task.Delay(150);
                IID = GameObject.Find("ItemInventoryDisplay");
            } while (IID == null);
            return IID;
        }

        // Async because ItemInventoryDisplay isn't immediately spawned at the start and I didn't want to use another hook.
        // Had a coroutine but I find await cleaner in this case.
        public async void DisplayItemPicker(List<ItemIndex> availableItems, ushort numOfItems, ushort allowedStack, ItemsCallback itemsCB)
        {
            Debug.Log("Building and displaying Item Picker Window...");
            var itemInventoryDisplay = await GetItemInventoryDisplayAsync();
            float minSize = 400f;
            float maxSize = 1000f;
            float uiWidth = Mathf.Clamp(availableItems.Count * 20f, minSize, maxSize);
            Logger.LogDebug("itemInventoryDisplay: " + (itemInventoryDisplay != null).ToString());
            
            GameObject pickerUI = new GameObject();
            pickerUI.name = "ItemPickerUI";
            pickerUI.layer = 5; // UI layer
            pickerUI.AddComponent<Canvas>().renderMode = RenderMode.ScreenSpaceOverlay;
            pickerUI.GetComponent<Canvas>().sortingOrder = -1; // Required or the UI will render over pause and tooltips.
            pickerUI.AddComponent<GraphicRaycaster>();
            pickerUI.AddComponent<MPEventSystemProvider>().fallBackToMainEventSystem = true;
            pickerUI.AddComponent<MPEventSystemLocator>();
            pickerUI.AddComponent<CursorOpener>();
            Logger.LogDebug("pickerUI: " + (pickerUI != null).ToString());

            GameObject ctr = new GameObject();
            ctr.name = "Container";
            ctr.transform.SetParent(pickerUI.transform, false);
            ctr.AddComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, uiWidth);
            Logger.LogDebug("ctr: " + (ctr != null).ToString());

            GameObject bg = new GameObject();
            bg.name = "Background";
            bg.transform.SetParent(ctr.transform, false);
            bg.AddComponent<TranslucentImage>().color = new Color(0f, 0f, 0f, 1f);
            bg.GetComponent<TranslucentImage>().raycastTarget = true;
            bg.GetComponent<TranslucentImage>().material = Resources.Load<GameObject>("Prefabs/UI/Tooltip").GetComponentInChildren<TranslucentImage>(true).material;
            bg.GetComponent<RectTransform>().anchorMin = new Vector2(0f, 0f);
            bg.GetComponent<RectTransform>().anchorMax = new Vector2(1f, 1f);
            bg.GetComponent<RectTransform>().sizeDelta = new Vector2(0f, 0f);
            Logger.LogDebug("bg: " + (bg != null).ToString());

            GameObject bgSprite = new GameObject();
            bgSprite.name = "BackgroundSprite";
            bgSprite.transform.SetParent(ctr.transform, false);
            bgSprite.AddComponent<Image>().sprite = itemInventoryDisplay.GetComponent<Image>().sprite;
            bgSprite.GetComponent<Image>().type = Image.Type.Sliced;
            bgSprite.GetComponent<RectTransform>().anchorMin = new Vector2(0f, 0f);
            bgSprite.GetComponent<RectTransform>().anchorMax = new Vector2(1f, 1f);
            bgSprite.GetComponent<RectTransform>().sizeDelta = new Vector2(0f, 0f);
            Logger.LogDebug("bgSprite: " + (bgSprite != null).ToString());

            GameObject header = new GameObject();
            header.name = "Header";
            header.transform.SetParent(ctr.transform, false);
            header.transform.localPosition = new Vector2(0f, 0f);
            header.AddComponent<HGTextMeshProUGUI>().fontSize = 30f;
            header.GetComponent<HGTextMeshProUGUI>().text = "Select Items";
            header.GetComponent<HGTextMeshProUGUI>().color = Color.white;
            header.GetComponent<HGTextMeshProUGUI>().alignment = TMPro.TextAlignmentOptions.Center;
            header.GetComponent<RectTransform>().anchorMin = new Vector2(0f, 1f);
            header.GetComponent<RectTransform>().anchorMax = new Vector2(1f, 1f);
            header.GetComponent<RectTransform>().pivot = new Vector2(0.5f, 1f);
            header.GetComponent<RectTransform>().sizeDelta = new Vector2(0f, 90f);
            Logger.LogDebug("header: " + (header != null).ToString());

            GameObject subheader = new GameObject();
            subheader.name = "Subheader";
            subheader.transform.SetParent(ctr.transform, false);
            subheader.transform.localPosition = new Vector2(0f, -30f);
            subheader.AddComponent<HGTextMeshProUGUI>().fontSize = 18f;
            subheader.GetComponent<HGTextMeshProUGUI>().text = $"Items Allowed: 0\\{numOfItems}\t\tStacks Allowed: {allowedStack}";
            subheader.GetComponent<HGTextMeshProUGUI>().color = Color.white;
            subheader.GetComponent<HGTextMeshProUGUI>().alignment = TMPro.TextAlignmentOptions.Center;
            subheader.GetComponent<RectTransform>().anchorMin = new Vector2(0f, 1f);
            subheader.GetComponent<RectTransform>().anchorMax = new Vector2(1f, 1f);
            subheader.GetComponent<RectTransform>().pivot = new Vector2(0.5f, 1f);
            subheader.GetComponent<RectTransform>().sizeDelta = new Vector2(0f, 90f);
            Logger.LogDebug("header: " + (subheader != null).ToString());

            GameObject itemCtr = new GameObject();
            itemCtr.name = "ItemContainer";
            itemCtr.transform.SetParent(ctr.transform, false);
            itemCtr.transform.localPosition = new Vector2(0f, -100f);
            itemCtr.AddComponent<GridLayoutGroup>().childAlignment = TextAnchor.UpperCenter;
            itemCtr.GetComponent<GridLayoutGroup>().cellSize = new Vector2(60f, 60f);
            itemCtr.GetComponent<GridLayoutGroup>().spacing = new Vector2(12f, 12f);
            itemCtr.GetComponent<RectTransform>().anchorMin = new Vector2(0f, 1f);
            itemCtr.GetComponent<RectTransform>().anchorMax = new Vector2(1f, 1f);
            itemCtr.GetComponent<RectTransform>().pivot = new Vector2(0.5f, 1f);
            itemCtr.GetComponent<RectTransform>().sizeDelta = new Vector2(-16f, 0f);
            itemCtr.AddComponent<ContentSizeFitter>().verticalFit = ContentSizeFitter.FitMode.PreferredSize;
            Logger.LogDebug("itemCtr: " + (itemCtr != null).ToString());

            GameObject itemIconPrefab = itemInventoryDisplay.GetComponent<ItemInventoryDisplay>().itemIconPrefab;
            Logger.LogDebug("itemIconPrefab: " + (itemIconPrefab != null).ToString());

            var pickedItems = new List<ItemIndex>();

            foreach (ItemIndex idx in availableItems)
            {
                if (idx == ItemIndex.None)
                    continue;
                var item = Instantiate(itemIconPrefab, itemCtr.transform).GetComponent<ItemIcon>();

                item.SetItemIndex(idx, 1);
                item.SetFieldValue<int>("itemCount", 0);
                item.stackText.text = "x0";
                item.stackText.enabled = true;

                var itemButton = item.gameObject.AddComponent<GUIClickController>();

                ColorBlock colors = itemButton.colors;
                colors.normalColor = new Color(1, 1, 1, 0.35f);
                colors.highlightedColor = new Color(1, 1, 1, 1f);
                colors.pressedColor = new Color(1, 1, 1, 0.35f);
                itemButton.colors = colors;

                itemButton.onClick.AddListener((button) =>
                {
                    if (button == PointerEventData.InputButton.Left)
                    {
                        var itemCount = item.GetFieldValue<int>("itemCount");
                        if (itemCount < allowedStack && pickedItems.Count < numOfItems)
                        {
                            item.SetFieldValue("itemCount", itemCount + 1);
                            pickedItems.Add(idx);
                            item.stackText.text = "x" + (itemCount + 1);
                            Logger.LogInfo($"Item added: {idx}");
                        }
                    }
                    else if (button == PointerEventData.InputButton.Right)
                    {
                        var itemCount = item.GetFieldValue<int>("itemCount");
                        if (itemCount > 0)
                        {
                            item.SetFieldValue("itemCount", itemCount - 1);
                            pickedItems.Remove(idx);
                            item.stackText.text = "x" + (itemCount - 1);
                            Logger.LogInfo($"Item removed: {idx}");
                        }
                    }
                    subheader.GetComponent<HGTextMeshProUGUI>().text = $"Items Allowed: {pickedItems.Count}\\{numOfItems}\t\tStacks Allowed: {allowedStack}";
                });
            }

            GameObject confirmText = new GameObject();
            confirmText.name = "ConfirmText";
            confirmText.transform.SetParent(ctr.transform, false);
            confirmText.transform.localPosition = new Vector2(0f, 0f);
            confirmText.AddComponent<HGTextMeshProUGUI>().fontSize = 24f;
            confirmText.GetComponent<HGTextMeshProUGUI>().text = "Confirm";
            confirmText.GetComponent<HGTextMeshProUGUI>().color = Color.white;
            confirmText.GetComponent<HGTextMeshProUGUI>().alignment = TMPro.TextAlignmentOptions.Center;
            confirmText.GetComponent<RectTransform>().anchorMin = new Vector2(0f, 0f);
            confirmText.GetComponent<RectTransform>().anchorMax = new Vector2(1f, 0f);
            confirmText.GetComponent<RectTransform>().pivot = new Vector2(0.5f, 1f);
            //confirmText.GetComponent<RectTransform>().sizeDelta = new Vector2(0f, 90f);

            var confirmButton = confirmText.AddComponent<GUIClickController>();
            confirmButton.onLeft.AddListener(() =>
            {
                Logger.LogInfo("Items picked: " + string.Join(",", pickedItems));
                Destroy(pickerUI);
                itemsCB(pickedItems);
            });
            Logger.LogDebug("receiveButton: " + (confirmText != null).ToString());
            LayoutRebuilder.ForceRebuildLayoutImmediate(itemCtr.GetComponent<RectTransform>());
            ctr.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical,
                itemCtr.GetComponent<RectTransform>().sizeDelta.y + 100f + 30f);
        }
    }
}
