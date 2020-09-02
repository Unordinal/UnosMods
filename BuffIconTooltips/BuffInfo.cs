using RoR2;
using System.Collections.Generic;
using UnityEngine;

namespace Unordinal.BuffIconTooltips
{
    public class BuffInfo
    {
        public string Name { get; set; }
        public BuffIndex BuffIndex { get; set; }
        public string Description { get; set; }
        public string Effect { get; set; }
        public List<string> Sources { get; set; } = new List<string>();
        public Color Color { get; set; }
        public bool IsDebuff { get; set; }

        public override string ToString()
        {
            return
                $"Buff: {Name}\n" +
                $"Description: {Description}\n" +
                $"Effect: {Effect}\n" +
                $"Sources: {string.Join(", ", Sources)}\n" +
                $"Color: {Color}";
        }
    }
}
