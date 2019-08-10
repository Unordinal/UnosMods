using System;
using System.Collections.Generic;
using BepInEx;
using BepInEx.Configuration;
using LeTai.Asset.TranslucentImage;
using RoR2;
using RoR2.UI;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

namespace UnosMods.StartingItemsPicker
{
    [BepInDependency("com.bepis.r2api")]
    [BepInPlugin("com.unordinal.startingitempicker", "Starting Item Picker", "1.0.0")]

    public class StartingItemsPicker : BaseUnityPlugin
    {
        private static ConfigWrapper<int> ItemPicks { get; set; }

        private IRpcAction<Action<NetworkWriter>> NetDisplayItemPickerAction;
        private IRpcAction<Action<NetworkWriter>> NetItemPickedAction;

        public void Awake()
        {
            ItemPicks = Config.Wrap(
                "StartingItemsPicker", 
                "ItemPicks", 
                "The number of items players can pick at the start of the run. (Default: 3)", 
                3
                );
            On.RoR2.Run.Start += Run_Start;
        }

        private void Run_Start(On.RoR2.Run.orig_Start orig, Run self)
        {
            orig(self);
            List<PickupIndex> availablePickups = Run.instance.availableTier1DropList;
            DisplayItemPicker(availablePickups, ItemPicks);
        }

        private void DisplayItemPicker(List<PickupIndex> availablePickups, int numOfItems, ItemsCallback cb)
        {
            Debug.Log("Building and displaying Item Picker Window...");
            GameObject itemInventoryDisplay = GameObject.Find("ItemInventoryDisplay");
            float minSize = 300f;
            float maxSize = 1000f;
            float uiWidth = Mathf.Clamp(availablePickups.Count * 10f + 100f, minSize, maxSize);

            GameObject pickerUI = new GameObject();
            pickerUI.name = "ItemPickerUI";
            pickerUI.layer = 5; // UI layer
            pickerUI.AddComponent<Canvas>().renderMode = RenderMode.ScreenSpaceOverlay;
            pickerUI.GetComponent<Canvas>().sortingOrder = -1; // Required or the UI will render over pause and tooltips.
            pickerUI.AddComponent<GraphicRaycaster>();
            pickerUI.AddComponent<MPEventSystemProvider>().fallBackToMainEventSystem = true;
            pickerUI.AddComponent<MPEventSystemLocator>();
            pickerUI.AddComponent<CursorOpener>();

            GameObject ctr = new GameObject();
            ctr.name = "Container";
            ctr.transform.SetParent(pickerUI.transform, false);
            ctr.AddComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, uiWidth);

            GameObject bg = new GameObject();
            bg.name = "Background";
            bg.transform.SetParent(ctr.transform, false);
            bg.AddComponent<TranslucentImage>().color = new Color(0f, 0f, 0f, 1f);
            bg.GetComponent<TranslucentImage>().raycastTarget = true;
            bg.GetComponent<TranslucentImage>().material = Resources.Load<GameObject>("Prefabs/UI/Tooltip").GetComponentInChildren<TranslucentImage>(true).material;
            bg.GetComponent<RectTransform>().anchorMin = new Vector2(0f, 0f);
            bg.GetComponent<RectTransform>().anchorMax = new Vector2(1f, 1f);
            bg.GetComponent<RectTransform>().sizeDelta = new Vector2(0f, 0f);

            GameObject bgSprite = new GameObject();
            bgSprite.name = "BackgroundSprite";
            bgSprite.transform.SetParent(ctr.transform, false);
            bgSprite.AddComponent<Image>().sprite = pickerUI.GetComponent<Image>().sprite;
            bgSprite.GetComponent<Image>().type = Image.Type.Sliced;
            bgSprite.GetComponent<RectTransform>().anchorMin = new Vector2(0f, 0f);
            bgSprite.GetComponent<RectTransform>().anchorMax = new Vector2(1f, 1f);
            bgSprite.GetComponent<RectTransform>().sizeDelta = new Vector2(0f, 0f);

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

            GameObject itemCtr = new GameObject();
            itemCtr.name = "ItemContainer";
            itemCtr.transform.SetParent(ctr.transform, false);
            itemCtr.transform.localPosition = new Vector2(0f, -100f);
            itemCtr.AddComponent<GridLayoutGroup>().childAlignment = TextAnchor.UpperCenter;
            itemCtr.GetComponent<GridLayoutGroup>().cellSize = new Vector2(50f, 50f);
            itemCtr.GetComponent<GridLayoutGroup>().spacing = new Vector2(8f, 8f);
            itemCtr.GetComponent<RectTransform>().anchorMin = new Vector2(0f, 1f);
            itemCtr.GetComponent<RectTransform>().anchorMax = new Vector2(1f, 1f);
            itemCtr.GetComponent<RectTransform>().pivot = new Vector2(0.5f, 1f);
            itemCtr.GetComponent<RectTransform>().sizeDelta = new Vector2(-16f, 0f);
            itemCtr.AddComponent<ContentSizeFitter>().verticalFit = ContentSizeFitter.FitMode.PreferredSize;

            GameObject itemIconPrefab = pickerUI.GetComponent<ItemInventoryDisplay>().itemIconPrefab;
            foreach (PickupIndex idx in availablePickups)
            {
                if (idx.itemIndex == ItemIndex.None)
                    continue;
                var item = Instantiate(itemIconPrefab, itemCtr.transform).GetComponent<ItemIcon>();
                item.SetItemIndex(idx.itemIndex, 1);
                item.gameObject.AddComponent<Button>().onClick.AddListener(() =>
                {
                    Logger.LogInfo($"Item picked: {idx}");
                    Destroy(pickerUI);
                    cb(idx);
                });
            }
        }

        public delegate void ItemsCallback(List<PickupIndex> idx);

        [Server]
        private void CallNetShowItemPicker(NetworkUser user, List<PickupIndex> pickupList)
        {
            NetDisplayItemPickerAction.Invoke(w =>
            {
                w.Write(pickupList.Count);
                foreach (var idx in pickupList)
                    PickupIndex.WriteToNetworkWriter(w, idx);
            }, user);
        }

        [Server]
        private void NetItemsPicked(NetworkUser user, NetworkReader reader)
        {

        }

        [Client]
        private void NetDisplayItemPicker (NetworkUser user, NetworkReader reader)
        {
            int count = reader.ReadInt32();
            List<PickupIndex> pickupList = new List<PickupIndex>(count);
            for (int i = 0; i < count; i++)
                pickupList.Add(PickupIndex.ReadFromNetworkReader(reader));
            DisplayItemPicker(pickupList, 3, x => CallNetItemsPicked(x));
        }

        [Client]
        private void CallNetItemsPicked(List<PickupIndex> selectedPickups)
        {
            NetItemsPickedAction.Invoke(w =>
            {
                foreach (PickupIndex idx in selectedPickups)
                    PickupIndex.WriteToNetworkWriter(w, idx);
            });
        }
    }
}
