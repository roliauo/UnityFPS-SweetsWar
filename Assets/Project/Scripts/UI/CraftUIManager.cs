using Photon.Pun;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Game.SweetsWar
{
    public class CraftUIManager : MonoBehaviour
    {
        public static CraftUIManager _instance;
        public int CraftID; // photonView.viewID
        public Inventory inventory; 
        public GameObject SlotContainer;
        public GameObject InfoPanel;
        public Dictionary<int, Inventory> AllPlayerCraftingInventories;

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

        public Item[] OutputItems;

        private Dictionary<short, short> m_itemCount;

        void Start()
        {
            /*
            if (!PhotonNetwork.IsConnected)
            {
                return;
            }
            */

            if (_instance != null)
            {
                Destroy(this);
            }

            _instance = this;
            AllPlayerCraftingInventories = new Dictionary<int, Inventory>();

            Button_Close.onClick.AddListener(() =>
            {
                //FridgeBehavior._instance.OpenFridge(false);
                PhotonView.Find(CraftID).gameObject.GetComponent<FridgeBehavior>().OpenFridge(false);
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
            //Debug.Log("Craft OnDestroy"); //yes
            //Clear();
        }

        public void SetData(int id)
        {
            CraftID = id;
            if (AllPlayerCraftingInventories.TryGetValue(id, out Inventory box))
            {             
                inventory = box;
            }
            else
            {
                AllPlayerCraftingInventories.Add(id, new Inventory(9));
            }
            
        }

        public void Clear()
        {
            if (!inventory || !GameManager.Instance)
            {
                return;
            }
            GameManager.Instance.GetComponent<PhotonView>().RPC("RPC_CraftingClear", RpcTarget.Others, CraftID);

            // local data
            _instance.inventory.ItemList.Clear();

            Inventory box;
            AllPlayerCraftingInventories.TryGetValue(CraftID, out box);
            box.ItemList.Clear();

            foreach (Transform child in SlotContainer.transform)
            {
                Destroy(child.gameObject);
            }
            ValidMix();
        }

        public void AddToCraftSlots(Item item)
        {
            Inventory box = _instance.inventory;
            if (box.ItemList.Count < box.InventoryCapacity)
            {
                box.ItemList.Add(item);
                Button slot = Instantiate(Button_SlotPrefab);
                slot.transform.SetParent(SlotContainer.transform);
                slot.transform.localScale = new Vector3(1, 1, 1);
                slot.GetComponent<CraftSlotPrefab>().SetItem(item);
                ValidMix();

                Debug.Log("PhotonNetwork.LocalPlayer.UserId: " + PhotonNetwork.LocalPlayer.UserId);
                // use FridgeBehavior._instance.ID to get the data
                GameManager.Instance.GetComponent<PhotonView>().RPC("RPC_SyncCraftingInventories", RpcTarget.Others, CraftID, item.ID, true);              

            }

        }

        public void RemoveFromCraftSlots(Item item, GameObject obj)
        {
            // move into the backpack
            BackpackManerger._instance.Collect(item);
            _instance.inventory.ItemList.Remove(item); // list.remove: only remove the first item.
            GameManager.Instance.GetComponent<PhotonView>().RPC("RPC_SyncCraftingInventories", RpcTarget.Others, CraftID, item.ID, false);
            Destroy(obj);
            ValidMix();
        }

        public void UpdateView()
        {
            //ClearSlots();

            foreach (Transform child in SlotContainer.transform)
            {
                Destroy(child.gameObject);
            }

            // build
            foreach (Item item in _instance.inventory.ItemList)
            {
                Button slot = Instantiate(Button_SlotPrefab);
                slot.transform.SetParent(SlotContainer.transform);
                slot.transform.localScale = new Vector3(1, 1, 1);
                slot.GetComponent<CraftSlotPrefab>().SetItem(item);
                //m_prefabDict.Add(slot.gameObject);
            }
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
                //GameManager.Instance.Craft(outputItem.name);
                Vector3 position = PlayerController.localPlayerInstance.transform.position;
                GameManager.Instance.photonView.RPC("RPC_CraftForMasterClient", RpcTarget.MasterClient, outputItem.name, position.x, position.y, position.z);
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

