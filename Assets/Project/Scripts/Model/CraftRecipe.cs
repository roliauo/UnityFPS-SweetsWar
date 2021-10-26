using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.SweetsWar
{
    [CreateAssetMenu(fileName = "New Craft Recipe", menuName = "Craft/New Recipe")]
    public class CraftRecipe : ScriptableObject
    {
        public short OutputItemID;
        public Ingredient[] Ingredients;
       
        /*
        public CraftRecipe(short outputItemID, Ingredient[] ingredients)
        {
            this.Ingredients = ingredients;
            this.OutputItemID = outputItemID;
        }
        */
    }

}
