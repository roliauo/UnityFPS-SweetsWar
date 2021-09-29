using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 GameManager.prefab
 */
namespace Game.SweetsWar
{
    public class BackpackManerger : MonoBehaviour
    {
        public static BackpackManerger _instance;

        public Inventory inventory;
        //public RectTransform inventoryUI;
        //public Grid gridPrefab;
        public GameObject BackpackUI;
        public GameObject SlotPrefab;
        private Dictionary<short, GameObject> m_prefabDict;

        private void Awake()
        {
            if (_instance != null)
            {
                Destroy(this);
            }
            _instance = this;
            m_prefabDict = new Dictionary<short, GameObject>();
        }

        public static void SetupGrid(Item item)
        {
            /*
            Grid grid = Instantiate(_instance.gridPrefab, _instance.inventoryUI.transform);
            grid.gridImage.sprite = item.Image;
            grid.girdNum.text = item.Number.ToString();
            */
        }


        public bool Collect(Item item)
        {
            string msg = _instance.inventory.Add(item);
            UpdateView();

            return msg == null; // Success
            //UnityEngine.UI.LayoutRebuilder.ForceRebuildLayoutImmediate(inventoryUI);
        }

        private void UpdateView()
        {
            // clear
            foreach (GameObject item in m_prefabDict.Values)
            {
                Destroy(item.gameObject);
            }

            m_prefabDict.Clear();

            // build
            foreach (Item item in _instance.inventory.ItemList)
            {
                GameObject slot = Instantiate(SlotPrefab);
                slot.transform.SetParent(BackpackUI.transform);
                //slot.transform.localScale = Vector3.one;
                slot.GetComponent<BackpackSlotPrefab>().SetItem(item);
                m_prefabDict.Add(item.ID, slot);
            }
        }

        void OnDestroy()
        {
            Debug.Log("Backpack OnDestroy");
            _instance.inventory.ItemList.Clear();
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
    
