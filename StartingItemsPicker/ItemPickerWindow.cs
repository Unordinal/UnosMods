using RoR2;
using System;
using System.Diagnostics.CodeAnalysis;
using UnityEngine;

namespace UnosMods.StartingItemsPicker
{
    public class ItemPickerWindow : MonoBehaviour
    {
        public GameObject RootObject { get; set; }
        public Func<string> GetTitle { get; set; }
        public Func<string> GetDescription { get; set; }
        public Transform Parent { get; set; }

        public void Awake()
        {
            Parent = RoR2Application.instance.mainCanvas.transform;
            RootObject = Instantiate(Resources.Load<GameObject>("Prefabs/NotificationPanel2"));
        }
    }
}
