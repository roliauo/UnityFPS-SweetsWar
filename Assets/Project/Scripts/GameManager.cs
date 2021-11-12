using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;
using System.Linq;

namespace Game.SweetsWar
{
    public class GameManager : MonoBehaviourPunCallbacks
    {
        static public GameManager Instance;

        public GameObject Menu;
        public GameObject CraftPanel;
        public GameObject AimTarget;

        [Header("Items")]
        //public GameObject[] ItemPrefabs;
        public List<GameObject> ItemPrefabs;
        //public int[] MinMaxX = { 2, 55 };
        public int MinX = 2;
        public int MaxX = 55;
        public int MinZ = 2;
        public int MaxZ = 80;

        [Header("Player")]
        //public Transform[] PlayerLocations;
        public List<Transform> PlayerLocations;
        [SerializeField]
        private GameObject PlayerPrefab;

        [Header("Fridge")]
        public List<GameObject> FridgeList;
        public List<Transform> FridgeLocations;
        public GameObject FridgePrefab;
        public Dictionary<int, Inventory> AllPlayerCraftingInventories;

        // for show/hide backpack
        //public GameObject BackpackUI;
        //public bool StopAction;

        [Header("Craft")]
        public GameObject[] CraftItemPrefabs;

        private short m_RandomItemNumber;
        private bool m_gameState;
        private byte playerIndex;

        void Start()
        {
            if (!PhotonNetwork.IsConnected)
            {
                SceneManager.LoadScene(GameConstants.SCENE_TITLE);
                return;
            }

            Instance = this;
            AllPlayerCraftingInventories = new Dictionary<int, Inventory>();

            /* SET PLAYER */
            if (PlayerPrefab == null) //PlayerManager.LocalPlayerInstance == null
            {
                Debug.LogFormat("Missing PlayerPrefab Reference");

            }
            else
            {

                if (PlayerController.localPlayerInstance == null)
                {
                    Debug.LogFormat("We are Instantiating LocalPlayer from {0}", SceneManagerHelper.ActiveSceneName);
                    ////playerIndex = PhotonNetwork.CurrentRoom.PlayerCount;
                    //GeneratePlayersInReadyStage();
                    MovePlayersToGameStage_Test();

                }
                else
                {
                    //MovePlayersToGameStage_old();
                    Debug.LogFormat("Ignoring scene load for {0}", SceneManagerHelper.ActiveSceneName);
                }

            }

            /* SET THE CURSOR MODE */
            Cursor.lockState = CursorLockMode.Locked;

            //GenerateItems();
            

            if (IsPlayerReady()) // Ready then setup
            {

                for (short i=0; i<PhotonNetwork.CurrentRoom.PlayerCount; i++)
                {
                    //AllPlayerCraftingInventories.Add(PhotonNetwork.PlayerList[i].UserId, new Inventory(9)); 
                    Debug_ShowAllPlayerCraftingInventories();

                    //object[] initData = { PhotonNetwork.PlayerList[i].UserId };
                    PhotonNetwork.InstantiateRoomObject(FridgePrefab.name, FridgeLocations[i].position, Quaternion.identity, 0);
                }
                
                // way1: photon instantiate
                GenerateItems();

            }
            

            /*
            // SHOW PLAYERS' NAME
            playerName.text = photonView.Owner.NickName;

            Debug.LogFormat("name: {0}, key: {1}, photonView: {2}",
                PhotonNetwork.NickName,
                PlayerPrefs.GetString(GameConstants.PLAYER_NAME_PREFAB_KEY),
                photonView.Owner.NickName
              );
            */
        }

        void Update()
        {
            
            if (!PhotonNetwork.IsConnected)
            {
                SceneManager.LoadScene(GameConstants.SCENE_TITLE);
                return;
            }          

            // show the menu
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                //Cursor.lockState = CursorLockMode.None;
                PhotonNetwork.LeaveRoom();
            }

            /*
            if (Input.GetKeyDown(KeyCode.Tab))
            {
                
                BackpackUI.SetActive(!BackpackUI.activeInHierarchy);
                Cursor.lockState = BackpackUI.activeInHierarchy ? CursorLockMode.None : CursorLockMode.Locked;
                //需設定人物不會旋轉
                //StopAction = BackpackUI.activeInHierarchy;
            }
            */
        }

        /*
        void OnGUI()
        {
            Vector3 characterPos = Camera.main.WorldToScreenPoint(PlayerPrefab.transform.position);
           
            characterPos = new Vector3(Mathf.Clamp(characterPos.x, 0 + (windowWidth / 2), Screen.width - (windowWidth / 2)),
                                               Mathf.Clamp(characterPos.y, 50, Screen.height),
                                               characterPos.z);
            GUILayout.BeginArea(new Rect((characterPos.x + offsetX) - (windowWidth / 2), (Screen.height - characterPos.y) + offsetY, windowWidth, windowHeight));
            // GUI CODE GOES HERE

            GUILayout.EndArea();
        }
        */

        void OnDestroy()
        {
            Cursor.lockState = CursorLockMode.None;
            ClearGameData();
        }

        public void setCraftPanel(bool state, int craftID)
        {
            Debug_ShowAllPlayerCraftingInventories();
            if (AllPlayerCraftingInventories.TryGetValue(craftID, out Inventory box))
            {
                // show the target data
                //CraftUIManager._instance.inventory = AllPlayerCraftingInventories[craftID]; // show the target data
                //CraftUIManager._instance.inventory = box; 
                CraftUIManager._instance.setData(craftID, box);
            }
            else
            {
                AllPlayerCraftingInventories.Add(craftID, new Inventory(9));
            }
            
            AimTarget.SetActive(!state);
            CraftPanel.SetActive(state);
            if (state) CraftUIManager._instance.UpdateView();
            //GetComponentInChildren<CraftUIManager>().UpdateView();
            PlayerController._instance.stopMove = state;
            //PlayerController.localPlayerInstance.GetComponent<PlayerController>().stopMove = state;
            Cursor.lockState = state ? CursorLockMode.None : CursorLockMode.Locked;
        }

        public void Craft(string prefabName)
        {
            foreach(GameObject obj in CraftItemPrefabs)
            {
                //Item data = obj.GetComponent<>
                if (obj.name.Equals(prefabName))
                {
                    //Instantiate(obj, PlayerController._instance.transform.position + new Vector3(0, 10f, 0), Quaternion.identity);
                    PhotonNetwork.InstantiateRoomObject(obj.name, PlayerController.localPlayerInstance.transform.position + new Vector3(0, 10f, 0), Quaternion.identity);
                    break;
                }
            }
            
        }
        
        #region RPC
        [PunRPC] public void RPC_SyncCraftingInventories(int craftID, short itemID, bool add)
        {
            // ERROR: RPC can't recieve object: Item
            Debug.Log("RPC_SyncCraftingInventories: " + craftID);
            Item item = ItemPrefabs.Find(v => v.GetComponent<ItemBehavior>().item.ID == itemID).GetComponent<ItemBehavior>().item;
            if (Instance.AllPlayerCraftingInventories.TryGetValue(craftID, out Inventory box))
            {
                if (add)
                {
                    box.ItemList.Add(item);
                }
                else
                {
                    box.ItemList.Remove(item);
                }
                CraftUIManager._instance.UpdateView();
            }
            else
            {
                Instance.AllPlayerCraftingInventories.Add(craftID, new Inventory(9));
            }

            Debug_ShowAllPlayerCraftingInventories();
            
        }
        #endregion
        
        public void Debug_ShowAllPlayerCraftingInventories()
        {
            // DEBUG
            Debug.Log("AllPlayerCraftingInventories: " + Instance.AllPlayerCraftingInventories.Count);

            foreach (KeyValuePair<int, Inventory> dic in Instance.AllPlayerCraftingInventories)
            {
                Debug.Log("AllPlayerCraftingInventories-KEY: " + dic.Key);
                foreach (Item v in dic.Value.ItemList)
                {
                    Debug.Log("-ItemList: " + v.DisplayName);
                }
            }
        }

        #region Private method

        private void ClearGameData() {
            ResetItemNumber();
        }

        private void ResetItemNumber() {
            foreach (GameObject obj in ItemPrefabs)
            {
                Item item = obj.GetComponent<ItemBehavior>().item;
                item.Number = 0;
            }
        }

        private void GeneratePlayersInReadyStage() // random range
        {
            // generate the player : it gets synced by using PhotonNetwork.Instantiate
            PhotonNetwork.Instantiate(PlayerPrefab.name, new Vector3(Random.Range(-80, -10), -80f, Random.Range(-80, -5)), Quaternion.identity, 0);
        }

        private bool IsPlayerReady()
        {
            // use MasterClient to update the scene
            // TODO: need to modify number of player (2~4)
            // 時間: 1分鐘內, 
            // 個人戰: 人數>1, 團體: 人數==MAX_PLAYERS_PER_ROOM

            return (
                PhotonNetwork.IsMasterClient && 
                //m_gameState == false &&
                PhotonNetwork.CurrentRoom.PlayerCount == GameConstants.MAX_PLAYERS_PER_ROOM
                );
        }

        private void MovePlayersToGameStage() // Personal
        {
            int index = Random.Range(0, PlayerLocations.Count);
            PlayerController.localPlayerInstance.transform.localPosition = PlayerLocations[index].position;
            Instantiate(FridgePrefab, FridgeLocations[index].position, Quaternion.identity);
            //FridgeLocations[index].GetComponentInChildren<GameObject>().SetActive();
            m_gameState = true;
            Debug.Log("=========MovePlayersToGameStage Personal");
        }

        private void MovePlayersToGameStage_Test() //PhotonNetwork.Instantiate
        {
            int index = PhotonNetwork.CurrentRoom.PlayerCount - 1; //Random.Range(0, PlayerLocations.Count);

            // generate the player : it gets synced by using PhotonNetwork.Instantiate
            PhotonNetwork.Instantiate(PlayerPrefab.name, PlayerLocations[index].position, Quaternion.identity, 0);

            m_gameState = true;
            Debug.Log("=========MovePlayersToGameStage TEST=====");
        }

        private void MovePlayersToGameStage_old() // player locations
        {
            int index, playerCount = PhotonNetwork.CurrentRoom.PlayerCount;
            //IEnumerable<int> tmpList = Enumerable.Range(1, max); //PlayerLocations.Count
            List<int> indexList = Enumerable.Range(0, playerCount).ToList();

            for (int i = 0; i < playerCount; i++)
            {
                index = Random.Range(0, indexList.Count);
                Debug.Log(index);
                //PlayerPrefab.transform.position = PlayerLocations[index].position;
                //FridgePrefab.transform.position = FridgeLocations[index].position;
                //PhotonNetwork.PlayerList[i]
                PlayerController.localPlayerInstance.transform.localPosition = PlayerLocations[index].position;
                Instantiate(FridgePrefab, FridgeLocations[index].position, Quaternion.identity);
                indexList.RemoveAt(index);
            }            


            Debug.Log("=========MovePlayersToGameStage _OLD");
            m_gameState = true;
        }

        /*[PunRPC]private void RPC_GenerateItems(float x, float z, short ItemPrefabIndex)
        {
            Instantiate(ItemPrefabs[ItemPrefabIndex], new Vector3(x, 10f, z), Quaternion.identity);
        }*/
        private void GenerateItems()
        {
            //int RandomObjects = Random.Range(0, ItemPrefabList.Length);
            float RandomX;
            float RandomZ;

            foreach (GameObject obj in ItemPrefabs)
            {
                m_RandomItemNumber = (short)Random.Range(8, 15);
                //Debug.LogFormat("GenerateItems: {0}, {1}", obj.name, m_RandomItemNumber);

                for (short i = 0; i < m_RandomItemNumber; i++)
                {
                    RandomX = Random.Range(MinX, MaxX);
                    RandomZ = Random.Range(MinZ, MaxZ);

                    //Instantiate(obj, new Vector3(RandomX, 10f, RandomZ), Quaternion.identity);
                    
                    //way1
                    PhotonNetwork.InstantiateRoomObject(obj.name, new Vector3(RandomX, 10f, RandomZ), Quaternion.identity);
                }

            }

        }

        private void LoadArena()
        {
            /*
            if (!PhotonNetwork.IsMasterClient)
            {
                Debug.LogError("PhotonNetwork : Trying to Load a level but we are not the master Client");
                return;
            }
            */
  
            PhotonNetwork.LoadLevel(GameConstants.SCENE_GAME); //TODO: game mode

            /*
            if (IsPlayerReady())
            {
                //PhotonNetwork.LoadLevel(GameConstants.SCENE_GAME); //TODO: game mode
                MovePlayersToGameStage_old();
                //PhotonView photonView = PhotonView.Get(this);
                //photonView.RPC("MovePlayersToGameStage", RpcTarget.All);
            }
            /*else
            {
                PhotonNetwork.LoadLevel(GameConstants.SCENE_READY);
            }*/
            
        }

        private bool CheckAllPlayerLoadedLevel()
        {
            foreach (Player p in PhotonNetwork.PlayerList)
            {
                object playerLoadedLevel;

                if (p.CustomProperties.TryGetValue(GameConstants.PLAYER_LOADED_LEVEL, out playerLoadedLevel))
                {
                    if ((bool)playerLoadedLevel)
                    {
                        continue;
                    }
                }

                return false;
            }

            return true;
        }

        #endregion

        #region PUN callback
        public override void OnLeftRoom()
        {
            SceneManager.LoadScene(GameConstants.SCENE_LOBBY);
        }

        // Watching other Players' Connection
        public override void OnPlayerEnteredRoom(Player other)
        {
            Debug.Log(other.NickName + " entered this room!"); // not seen if you're the player connecting
            Debug.Log("MasterClient.NickName: " + PhotonNetwork.MasterClient.NickName);

            if (PhotonNetwork.IsMasterClient)
            {
                LoadArena();
            }
        }
        public override void OnPlayerLeftRoom(Player other)
        {
            Debug.Log(other.NickName + " left this room!"); // seen when other disconnects
            Debug.Log("MasterClient.NickName: " + PhotonNetwork.MasterClient.NickName);

        }
        #endregion
    }
}

