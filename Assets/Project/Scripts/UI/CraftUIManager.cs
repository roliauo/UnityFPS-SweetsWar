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
        public GameObject InfoPanel;
        public Button Button_SlotPrefab;
        public Button Button_Mix;
        public Button Button_Close;
        public Button Button_Info;
        public Button Button_CloseInfo;
        public GameObject[] OutputItemPrefabs;

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
        }

        void OnDestroy()
        {
            Debug.Log("Craft OnDestroy");
            Clear();
        }

        public void Clear()
        {
            _instance.inventory.ItemList.Clear();
            m_itemCount.Clear();
            foreach (Transform child in SlotContainer.transform)
            {
                Destroy(child.gameObject);
            }
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
            Ingredient[] ingredients = { };
            short outputItemID = -1;
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
            foreach(GameObject obj in OutputItemPrefabs)
            {
                //outputItemID = -1;
                //Ingredient[] ingredients = obj.GetComponent<WeaponBehavior>().WeaponData.Ingredients;
                if (obj.TryGetComponent<WeaponBehavior>(out WeaponBehavior behavior_w))
                {
                    ingredients = behavior_w.WeaponData.Ingredients;
                    
                    
                }              
                else if (obj.TryGetComponent<ItemBehavior>(out ItemBehavior behavior_i))
                {
                    ingredients = behavior_i.item.Ingredients;

                }

                if (m_itemCount.Count == ingredients.Length)
                {
                    foreach (Ingredient ingre in ingredients)
                    {
                        /*if (m_itemCount.ContainsKey(ingre.ItemID) && m_itemCount[ingre.ItemID] == ingre.Number)
                        {

                        }*/
                        if (!m_itemCount.ContainsKey(ingre.ItemID) || m_itemCount[ingre.ItemID] != ingre.Number)
                        {
                            outputItemID = -1;
                            break; // 一項成份不匹配即跳出
                        }
                        outputItemID = behavior_w.WeaponData.ID;
                    }
                }

                if (outputItemID > 0)
                {
                    break; // 已有配對物即跳出
                }
                
            }

            Debug.Log("MIX------outputItemID: "+ outputItemID);

        }

        private void ShowInfo(bool state)
        {
            InfoPanel.SetActive(state);
        }
    }
}

