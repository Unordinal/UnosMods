using RoR2;
using System;
using System.Collections.Generic;
using System.Text;

namespace SoulLink
{
    public class CharacterSoulLink
    {
        public Inventory SharedInventory { get; } = new Inventory();
        public Dictionary<CharacterBody, Inventory> LinkedCharacters { get; } = new Dictionary<CharacterBody, Inventory>();
        
        /// <summary>
        /// Create a soul link between characters.
        /// </summary>
        /// <param name="other">The <see cref="CharacterBody"/> to link to.</param>
        public CharacterSoulLink(CharacterBody other) => LinkCharacter(other);
        
        /// <summary>
        /// Create a soul link between characters.
        /// </summary>
        /// <param name="others">The characters to link to.</param>
        public CharacterSoulLink(List<CharacterBody> others) => LinkCharacters(others);

        public bool LinkCharacter(CharacterBody other)
        {
            if (!SoulLink.LocalCharacter || !other || other?.inventory == null)
                return false;

            if (!LinkedCharacters.ContainsKey(other))
            {
                var oldInventory = new Inventory();
                oldInventory.CopyItemsFrom(other.inventory);
                oldInventory.CopyEquipmentFrom(other.inventory);

                SharedInventory.CopyItemsFrom(oldInventory);
                other.inventory.RemoveAllItems();
                other.inventory.CopyItemsFrom(SharedInventory);

                LinkedCharacters.Add(other, oldInventory);
                return true;
            }
            return false;
        }

        public int LinkCharacters(List<CharacterBody> others)
        {
            int failCount = 0;
            foreach (var body in others)
                if (LinkCharacter(body) == false)
                    failCount++;
            return failCount;
        }

        protected virtual void OnCharactersLinked(CharacterLinkEventArgs e) =>
            CharacterLink?.Invoke(this, e);

        public class CharacterLinkEventArgs : EventArgs
        {
            public List<CharacterBody> Characters { get; set; }
            /// <summary>
            /// Whether the characters in the list were linked or unlinked.
            /// </summary>
            public bool Linked { get; set; }
        }

        public delegate void CharacterLinkEvent(object sender, CharacterLinkEventArgs e);
        public event CharacterLinkEvent CharacterLink;
    }
}
