using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.SweetsWar
{
    [CreateAssetMenu(fileName = "New Item", menuName = "Inventory/New Item")]
    public class Item: ScriptableObject
    {
        public int ID;
        public string DisplayName;
        public Sprite Image;
        public int Number;

        [TextArea]
        public string Description;
    }
}

