using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.SweetsWar
{
    public class InventoryManerger : MonoBehaviour
    {
        static InventoryManerger instance;

        public Inventory inventory;
        public GameObject inventoryUI;
        public Grid gridPrefab;

        private void Awake()
        {
            if (instance != null)
            {
                Destroy(this);
            }
            instance = this;
        }

       /*
        public static void AddOneItem(Item item)
        {
            Grid grid = Instantiate(instance.gridPrefab, instance.inventoryUI.transform);
            grid.gridImage.sprite = item.itemImage;
            grid.girdNum.text = item.itemNum.ToString();
        }
       */

        public void AddItems(Item item)
        {
            if (inventory.ItemList.Contains(item))
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
                if (inventory.ItemList.Count < inventory.ItemList.Capacity)
                {
                    inventory.ItemList.Add(item);
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
    
