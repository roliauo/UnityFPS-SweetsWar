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
        public byte MaxNumber = 10;
        public Ingredient[] Ingredients;

        [TextArea]
        public string Description;

        // [SerializeField]
        // public Dictionary<Item, byte> IngredientDict = new Dictionary<Item, byte>();    
    }
}

