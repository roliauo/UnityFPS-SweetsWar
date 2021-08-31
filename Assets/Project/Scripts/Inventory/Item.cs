using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.SweetsWar
{
    [CreateAssetMenu(fileName = "New Item", menuName = "Inventory/New Item")]
    public class Item: ScriptableObject
    {
        public short ID;
        public string DisplayName;
        public Sprite Image;
        public byte Number = 0;
        public byte MaxNumber = 10;

        [TextArea]
        public string Description;      
    }
}

