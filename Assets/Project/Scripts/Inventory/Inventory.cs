using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.SweetsWar
{
    [CreateAssetMenu(fileName = "New Inventory", menuName = "Inventory/New Inventory")]
    public class Inventory : ScriptableObject
    {
        public List<Item> ItemList = new List<Item>();

        public string Add(Item item) // Backpack: 之後可放到相對應的Manager去處理
        {
            //bool result = false;
            string errMsg = null;

            if (ItemList.Contains(item))
            {
                if (item.Number < item.MaxNumber)
                {
                    item.Number += 1;
                    //result = true;
                }
                else
                {
                    Debug.Log("道具數量已達上限!");
                    errMsg = "道具數量已達上限!";
                }
            }
            else
            {
                if (ItemList.Count < ItemList.Capacity)
                {
                    ItemList.Add(item);
                    item.Number += 1;
                    //result = true;
                }
                else
                {
                    Debug.Log("背包滿了!");
                    errMsg = "背包滿了!";
                }
            }
            return errMsg;
        }
    }
}
