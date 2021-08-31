using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.SweetsWar
{
    [CreateAssetMenu(fileName = "New Inventory", menuName = "Inventory/New Inventory")]
    public class Inventory : ScriptableObject
    {
        public List<Item> ItemList = new List<Item>();

        public void AddItems(Item item)
        {
            if (ItemList.Contains(item))
            {
                if (item.Number < item.MaxNumber)
                {
                    item.Number += 1;
                }
                else
                {
                    Debug.Log("道具滿了!");
                }
            }
            else
            {
                if (ItemList.Count < ItemList.Capacity)
                {
                    ItemList.Add(item);
                    item.Number += 1;
                }
                else
                {
                    Debug.Log("背包滿了!");
                }
            }
        }
    }
}
