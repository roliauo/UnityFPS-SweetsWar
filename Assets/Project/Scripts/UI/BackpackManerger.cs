using Photon.Pun;
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
        public GameObject BackpackUI;
        public GameObject SlotPrefab;
        public Alert alert;
        //public List<int> BackpackStoredItemViewID;
        public Dictionary<short, List<int>> BackpackStoredItemViewID; // key: itemID, value: List<int> viewID

        private Dictionary<short, GameObject> m_prefabDict;

        private void Awake()
        {
            /*
            if (!PhotonNetwork.IsConnected || !GameManager.Instance)
            {
                //Debug.Log("!GameManager.Instance: " + !GameManager.Instance);
                return;
            }
            */
            
            if (_instance != null)
            {
                //Debug.Log("Destroy Instance");
                Destroy(this);
            }
            _instance = this;
            m_prefabDict = new Dictionary<short, GameObject>();
            //BackpackStoredItemViewID = new List<int>();
            BackpackStoredItemViewID = new Dictionary<short, List<int>>();

            _instance.inventory.ItemList.Clear();
            ClearSlots();
        }

        void OnDestroy()
        {
            /*
            //Debug.Log("Backpack OnDestroy"); //yes
            if (!inventory || !GameManager.Instance)
            {
                return;
            }
            _instance.inventory.ItemList.Clear();
            ClearSlots();
            */
        }

        public string Add(Item item, byte num = 1) // Backpack: 之後可放到相對應的Manager去處理
        {
            string errMsg = null;
            if (_instance.inventory.ItemList.Contains(item))
            {
                if (item.Number < item.MaxNumber)
                {
                    item.Number += num;
                }
                else
                {
                    //Debug.Log("道具數量已達上限!");
                    errMsg = "道具數量已達上限!";
                }
            }
            else
            {
                if (_instance.inventory.ItemList.Count < _instance.inventory.InventoryCapacity)
                {
                    _instance.inventory.ItemList.Add(item);
                    item.Number += num;
                }
                else
                {
                    //Debug.Log("沒有空間了!" + _instance.inventory.ItemList.Count + "/" + _instance.inventory.ItemList.Capacity);
                    errMsg = "沒有空間了!";
                }
            }
            return errMsg;
        }

        public bool Collect(Item item, int viewID, byte num = 1)
        {
            
            string msg = _instance.inventory.Add(item, num);
            if (msg == null) // Success
            {
                //BackpackStoredItemViewID.Add(viewID);
                if (!BackpackStoredItemViewID.ContainsKey(item.ID))
                {
                    BackpackStoredItemViewID.Add(item.ID, new List<int>());
                }
                BackpackStoredItemViewID[item.ID].Add(viewID);
                
                UpdateView();
            } 
            else
            {
                alert.Show(msg);
            }

            return msg == null; 
            
        }

        public void Subtract(Item item, int viewID, byte num = 1)
        {
            //BackpackStoredItemViewID.Remove(viewID);
            BackpackStoredItemViewID[item.ID].Remove(viewID);
            Item target = _instance.inventory.ItemList.Find(obj => obj.ID == item.ID);
            Debug.Log("Subtract: " + item.ID);
            if (!target) return;
            target.Number -= num;
            if (target.Number == 0)
            {
                _instance.inventory.ItemList.Remove(item);
            }
            UpdateView();
        }

        public void Remove(Item item)
        {
            _instance.inventory.ItemList.Remove(item);
            UpdateView();
        }

        public void OnClickSlot(Item item) 
        {
            int itemViewID = BackpackStoredItemViewID[item.ID][BackpackStoredItemViewID[item.ID].Count - 1];
            Debug.Log("OnClickSlot-item.ID: " + item.ID + " itemViewID: " + itemViewID);
            // move into the craft box
            if (CraftUIManager._instance.AddToCraftSlots(item, itemViewID))
            {
                _instance.Subtract(item, itemViewID);

                if(item.Type == GameConstants.ITEM_TYPE_WEAPON)
                {           
                    //PlayerController._instance.storeWeaponToFridge();
                    PlayerController._instance.photonView.RPC("StoreWeaponToFridge", RpcTarget.AllBufferedViaServer, CraftUIManager._instance.CraftID);
                }
            }
            else
            {
                alert.Show("冰箱滿了!");
            }
            
        }

        private void UpdateView()
        {
            ClearSlots();

            // build
            foreach (Item item in _instance.inventory.ItemList)
            {
                GameObject slot = Instantiate(SlotPrefab);
                slot.transform.SetParent(BackpackUI.transform);
                slot.transform.localScale = new Vector3(1, 1, 1);
                slot.GetComponent<BackpackSlotPrefab>().SetItem(item);
                m_prefabDict.Add(item.ID, slot);
            }
            /*for (int i = 0; i < _instance.inventory.ItemList.Count; i++)
            {
                Item item = _instance.inventory.ItemList[i];
                GameObject slot = Instantiate(SlotPrefab);
                slot.transform.SetParent(BackpackUI.transform);
                slot.transform.localScale = new Vector3(1, 1, 1);
                slot.GetComponent<BackpackSlotPrefab>().SetItem(item);
                //slot.GetComponent<BackpackSlotPrefab>().SetItem(item, BackpackStoredItemViewID[item.ID][i]);
                m_prefabDict.Add(item.ID, slot);
            }*/
        }  
        
        private void ClearSlots()
        {
            foreach (GameObject item in m_prefabDict.Values)
            {
                Destroy(item.gameObject);
            }

            m_prefabDict.Clear();
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
    
