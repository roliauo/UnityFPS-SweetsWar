using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.SweetsWar
{
    public class BackpackManerger : MonoBehaviour
    {
        public static BackpackManerger _instance;

        public Inventory inventory;
        public RectTransform inventoryUI;
        public Grid gridPrefab;

        private void Awake()
        {
            if (_instance != null)
            {
                Destroy(this);
            }
            _instance = this;
        }

        public static void SetupGrid(Item item)
        {
            /*
            Grid grid = Instantiate(_instance.gridPrefab, _instance.inventoryUI.transform);
            grid.gridImage.sprite = item.Image;
            grid.girdNum.text = item.Number.ToString();
            */
        }


        public void AddOneItem(Item item)
        {
            string msg = _instance.inventory.Add(item);
            UnityEngine.UI.LayoutRebuilder.ForceRebuildLayoutImmediate(inventoryUI);
        }
       
       
        /*
        public void AddOneItem(Item item, Inventory inventory)
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
        */
    }
}
    
