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
        //public List<Item> ItemList = new List<Item>(new Item[6]);

        public byte InventoryCapacity = 6;

        public string Add(Item item, byte num = 1) // Backpack: 之後可放到相對應的Manager去處理
        {
            string errMsg = null;
            if (ItemList.Contains(item))
            {
                if (item.Number < item.MaxNumber)
                {
                    item.Number += num;
                }
                else
                {
                    Debug.Log("道具數量已達上限!");
                    errMsg = "道具數量已達上限!";
                }
            }
            else
            {
                if (ItemList.Count < InventoryCapacity)
                {
                    ItemList.Add(item);
                    item.Number += num;
                    //result = true;
                }
                else
                {
                    Debug.Log("沒有空間了!" + ItemList.Count + "/" + ItemList.Capacity);
                    errMsg = "沒有空間了!";
                }
            }
            return errMsg;
        }

        /*
        public bool Add(Item item, byte num = 1) 
        {
            bool result = false;

            if (ItemList.Contains(item))
            {
                if (item.Number < item.MaxNumber)
                {
                    item.Number += num;
                    result = true;
                }              
            }
            else
            {
                if (ItemList.Count < InventoryCapacity)
                {
                    ItemList.Add(item);
                    item.Number += num;
                    result = true;
                }
               
            }
            return result;
        }
    }
        */
    }
}
