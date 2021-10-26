using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Game.SweetsWar
{
    public class CraftUIManager : MonoBehaviour
    {
        public static CraftUIManager _instance;
        public Inventory inventory;
        public GameObject SlotContainer;
        public Button Button_SlotPrefab;
        public Button Button_Mix;
        public Button Button_Close;
        public Button Button_Info;

        private void Awake()
        {
            if (_instance != null)
            {
                Destroy(this);
            }
            _instance = this;
        }
        void Start()
        {
            Button_Close.onClick.AddListener(() =>
            {
                FridgeBehavior._instance.OpenFridge(false);
            });

            Button_Mix.onClick.AddListener(Mix);

            Button_Info.onClick.AddListener(ShowInfo);

            //Button_SlotPrefab.onClick.AddListener((Item item) => RemoveFromCraftSlots);

        }

        void OnDestroy()
        {
            Debug.Log("Craft OnDestroy");
            _instance.inventory.ItemList.Clear();
        }

        public void AddToCraftSlots(Item item)
        {          
            if (_instance.inventory.ItemList.Count < _instance.inventory.InventoryCapacity)
            {
                _instance.inventory.ItemList.Add(item);
            }
            
            Button slot = Instantiate(Button_SlotPrefab);
            slot.transform.SetParent(SlotContainer.transform);
            slot.transform.localScale = new Vector3(1, 1, 1);
            slot.GetComponent<CraftSlotPrefab>().SetItem(item);

        }

        public void RemoveFromCraftSlots(Item item, GameObject obj)
        {
            // move into the backpack
            BackpackManerger._instance.Collect(item);
            _instance.inventory.ItemList.Remove(item); // list.remove: only remove the first item.
            Destroy(obj);
        }     

        private void Mix() { 
        
        }

        private void ShowInfo()
        {

        }
    }
}

