﻿using System;
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
        public GameObject InfoPanel;

        [Header("Result")]
        public GameObject ResultPanel;
        public GameObject ResultSuccess;
        public GameObject ResultFail;
        public Image ResultImg;

        [Header("Buttons")]
        public Button Button_SlotPrefab;
        public Button Button_Mix;
        public Button Button_Close;
        public Button Button_Info;
        public Button Button_CloseInfo;

        //public GameObject[] OutputItemPrefabs; //del
        public Item[] OutputItems;

        private Dictionary<short, short> m_itemCount;

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
            Button_Info.onClick.AddListener(() => ShowInfo(true));
            Button_CloseInfo.onClick.AddListener(() => ShowInfo(false));
            //m_itemCount = new Dictionary<short, short>();
            ValidMix();
        }
        
        void Update()
        {
            if (ResultPanel.activeInHierarchy && Input.anyKeyDown)
            {
                ResultPanel.SetActive(false);
            }
        }
        
        void OnDestroy()
        {
            Debug.Log("Craft OnDestroy");
            Clear();
        }

        public void Clear()
        {
            _instance.inventory.ItemList.Clear();
            foreach (Transform child in SlotContainer.transform)
            {
                Destroy(child.gameObject);
            }
            ValidMix();
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
            ValidMix();
        }

        public void RemoveFromCraftSlots(Item item, GameObject obj)
        {
            // move into the backpack
            BackpackManerger._instance.Collect(item);
            _instance.inventory.ItemList.Remove(item); // list.remove: only remove the first item.
            Destroy(obj);
            ValidMix();
        }  
        
        private void ValidMix()
        {
            Button_Mix.interactable = (_instance.inventory.ItemList.Count > 1);
        }

        private void Mix() {
            Ingredient[] ingredients = { };
            Item outputItem = null;

            m_itemCount = new Dictionary<short, short>();

            // count
            foreach (Item item in _instance.inventory.ItemList)
            {
                if (m_itemCount.ContainsKey(item.ID))
                {
                    m_itemCount[item.ID] += 1;
                } else
                {
                    m_itemCount.Add(item.ID, 1);
                }
            }
            /*
            foreach(KeyValuePair<short, short> dic in m_itemCount)
            {
                Debug.Log("key: " + dic.Key + " value: " + dic.Value);
            }
            */
            // Matching
            foreach(Item itemData in OutputItems)
            {
                if (m_itemCount.Count == itemData.Ingredients.Length)
                {
                    foreach (Ingredient ingre in itemData.Ingredients)
                    {
                        if (!m_itemCount.ContainsKey(ingre.ItemID) || m_itemCount[ingre.ItemID] != ingre.Number)
                        {
                            outputItem = null;
                            break; // 一項成份不匹配即跳出
                        }
                        outputItem = itemData;
                    }
                }

                if (outputItem != null)
                {
                    break; // 已有配對物即跳出
                }
            }

            if (outputItem != null)
            {
                ResultImg.sprite = outputItem.Icon;
                // generated by GameManager
                GameManager.Instance.Craft(outputItem.name);
            }

            // result panel
            ResultPanel.SetActive(true);
            ResultSuccess.SetActive(outputItem != null);
            ResultFail.SetActive(outputItem == null);
            
            Clear();

        }

        private void ShowInfo(bool state)
        {
            InfoPanel.SetActive(state);
        }
    }
}

