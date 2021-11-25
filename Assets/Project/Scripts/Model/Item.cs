using UnityEngine;

namespace Game.SweetsWar
{
    [CreateAssetMenu(fileName = "New Item", menuName = "Inventory/New Item")]
    public class Item: ScriptableObject
    {
        public short ID;
        public string DisplayName;
        public Sprite Icon;
        public byte Number = 0;
        public byte MaxNumber = 5;
        public Ingredient[] Ingredients;
        public Sprite TreasureTip; // only for Treasure

        [TextArea]
        public string Description;

        // [SerializeField]
        // public Dictionary<Item, byte> IngredientDict = new Dictionary<Item, byte>();    
    }
}

