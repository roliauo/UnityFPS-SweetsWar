using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;
using System.Linq;
using Hashtable = ExitGames.Client.Photon.Hashtable;
using Photon.Pun.UtilityScripts;
//using System;

namespace Game.SweetsWar
{
    public class GameManager : MonoBehaviourPunCallbacks
    {
        static public GameManager Instance;

        public GameObject Menu;
        public GameObject CraftPanel;
        public GameObject AimTarget;
        public GameObject ScorePanel;
        public Image Image_Win;

        [Header("Items")]
        public List<GameObject> ItemPrefabs;
        public int[] MinMaxX = { 2, 55 };
        public int[] MinMaxZ = { 2, 80 };
        public int[] ItemNumberRange = { 10, 20 };

        [Header("Player")]
        public List<Transform> PlayerLocations;
        [SerializeField] private GameObject PlayerPrefab;
        public List<Player> AllPlayersDataCache; // for score(PLAYER LEAVE)
        public float PlayerMaxHealthSum { get; private set; }

        [Header("Fridge")]
        public List<Transform> FridgeLocations;
        public GameObject FridgePrefab;

        // for show/hide backpack
        //public GameObject BackpackUI;
        //public bool StopAction;

        [Header("Craft")]
        public List<GameObject> CraftItemPrefabs;
        public int TreasureGoalID { get; private set; }

        private short m_RandomItemNumber;
        private List<GameObject> m_TreasureList;

        void Start()
        {
            if (!PhotonNetwork.IsConnected)
            {
                SceneManager.LoadScene(GameConstants.SCENE_TITLE);
                return;
            }

            Instance = this;

            if (GameConstants.DebugMode)
            {
                // SET PLAYER 
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

                if (IsPlayerReady()) // Ready then setup
                {
                    // way1: photon instantiate
                    GenerateItems();

                }
            }

            if (PhotonNetwork.IsMasterClient)
            {
                //MovePlayersToGameStage_Test();
                //photonView.RPC("RPC_InitializePlayerPosition", RpcTarget.All);

            }
            RPC_InitializePlayerPosition();
            SetTreasureGoal();
            SetCursorMode(false);
            GenerateItems();
            CachePlayersData();

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

            if (!ScorePanel.activeInHierarchy)
            {
                checkGameOver();
            }
            

            // show the menu
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                //Cursor.lockState = CursorLockMode.None;
                //PhotonNetwork.LeaveRoom();
                Menu.SetActive(!Menu.activeInHierarchy);
                SetCursorMode(Menu.activeInHierarchy);
            }

            if (Image_Win.gameObject.activeInHierarchy && Input.anyKeyDown)
            {
                Image_Win.gameObject.SetActive(false);
            }


            /*
            if (Input.GetKeyDown(KeyCode.Tab))
            {
                // show backpack
                BackpackUI.SetActive(!BackpackUI.activeInHierarchy);
                Cursor.lockState = BackpackUI.activeInHierarchy ? CursorLockMode.None : CursorLockMode.Locked;
                //需設定人物不會旋轉
                //StopAction = BackpackUI.activeInHierarchy;
            }
            */
        }

        void OnDestroy()
        {
            SetCursorMode(true);
            ClearGameData();
        }

        public void SetTreasureGoal()
        {
            TreasureGoalID = Random.Range(GameConstants.TREASURE_ID_MIN, GameConstants.TREASURE_ID_MAX + 1);
            InfoManager._instance.SetTreasureGoal(TreasureGoalID);
        }

        public void checkGameOver()
        {
            foreach (Player p in AllPlayersDataCache)
            {
                if ((bool)p.CustomProperties[GameConstants.K_PROP_WINNER] == true && 
                    p.UserId != PhotonNetwork.LocalPlayer.UserId)
                {
                    ShowScorePanel();
                    break;
                }

            }

            if (PlayerController._instance.isDead)
            {
                ShowScorePanel();
            }
            else if ((bool)PhotonNetwork.LocalPlayer.CustomProperties[GameConstants.K_PROP_WINNER]) {
                Win();
            }
            /*
            int alivePlayer = 0;
            foreach(Player p in AllPlayersDataCache)
            {
                if ((bool)p.CustomProperties[GameConstants.K_PROP_IS_DEAD] == false)
                {
                    alivePlayer++;
                }

                if ((bool)p.CustomProperties[GameConstants.K_PROP_WINNER] == true && p.UserId != PhotonNetwork.LocalPlayer.UserId)
                {
                    ShowScorePanel();
                    break;
                }
                
            }
            if (alivePlayer == 1 && (bool)PhotonNetwork.LocalPlayer.CustomProperties[GameConstants.K_PROP_IS_DEAD] == false)
            {
                Debug.Log("AllPlayersDataCache.count" + AllPlayersDataCache.Count());
                Win();
            }
            */
            
        }

        public void Win()
        {
            ShowScorePanel();
            Image_Win.gameObject.SetActive(true);
            Instance.photonView.RPC("UpdatePlayerCacheState", RpcTarget.All, PhotonNetwork.LocalPlayer.UserId, GameConstants.K_PROP_WINNER, true);
        }

        public void SetCursorMode(bool show)
        {
            if(PlayerController._instance) PlayerController._instance.stopMove = show;
            Cursor.visible = show;
            Cursor.lockState = show ? CursorLockMode.None : CursorLockMode.Locked;
        }

        public void SetCraftPanel(bool state, int craftID)
        {
            //Debug_ShowAllPlayerCraftingInventories();
            CraftUIManager._instance.SetData(craftID);
            
            AimTarget.SetActive(!state);
            CraftPanel.SetActive(state);
            if (state) CraftUIManager._instance.UpdateView();

            SetCursorMode(state);
        }

        public void ShowScorePanel()
        {
            // end panel: show score, can close and back to game to see others (transfer camera, and can't move)
            SetCursorMode(true);
            ScorePanelManager._instance.UpdateScoreView();
            ScorePanel.SetActive(true);          
        }

        public void CachePlayersData()
        {
            PlayerMaxHealthSum = 0;
            AllPlayersDataCache = new List<Player>();
            foreach (Player p in PhotonNetwork.PlayerList)
            {
                AllPlayersDataCache.Add(p);
                PlayerMaxHealthSum += (float)p.CustomProperties[GameConstants.K_PROP_MAX_HEALTH];
            }

        }

        [PunRPC] public void AddScore(string userID, string key, float value)
        {

            foreach (Player p in Instance.AllPlayersDataCache)
            {
                if (p.UserId == userID && p.CustomProperties.TryGetValue(key, out object v))
                {
                    p.CustomProperties[key] = value + (float)v;
                }
            }
            
        }

        [PunRPC] public void UpdatePlayerCacheState(string userID, string key, bool value)
        {

            foreach (Player p in Instance.AllPlayersDataCache)
            {
                if (p.UserId == userID && p.CustomProperties.TryGetValue(key, out object v))
                {
                    p.CustomProperties[key] = value;
                }
            }

        }

        #region RPC
        [PunRPC] public void RPC_InitializePlayerPosition()
        {
            //PlayerController.localPlayerInstance.transform.localPosition = PlayerLocations[Random.Range(0, PlayerLocations.Count - 1)].position;
            PlayerController.localPlayerInstance.transform.localPosition = PlayerLocations[(int)PhotonNetwork.LocalPlayer.CustomProperties[GameConstants.K_PROP_PLAYER_INDEX]].position;
        }

        [PunRPC] public void RPC_CraftForMasterClient(string prefabName, float x, float y, float z) //string userID //int actorNum
        {
            foreach (GameObject obj in CraftItemPrefabs)
            {
                if (obj.name.Equals(prefabName))
                {
                    //Instantiate(obj, PlayerController._instance.transform.position + new Vector3(0, 10f, 0), Quaternion.identity);

                    PhotonNetwork.InstantiateRoomObject(prefabName, new Vector3(x, y, z), Quaternion.identity);
                    break;
                }
            }

        }

        [PunRPC] public void RPC_SyncCraftingInventories(int craftID, short itemID, bool add)
        {
            // ERROR: RPC can't recieve object: Item
            Debug.Log("RPC_SyncCraftingInventories: " + craftID);
            GameObject targetPrefab = ItemPrefabs.Find(v => v.GetComponent<ItemBehavior>().item.ID == itemID);
            if (targetPrefab == null) return;
            Item item = targetPrefab.GetComponent<ItemBehavior>().item;
            if (CraftUIManager._instance.AllPlayerCraftingInventories.TryGetValue(craftID, out Inventory box))
            {
                if (add)
                {
                    box.ItemList.Add(item);
                }
                else
                {
                    box.ItemList.Remove(item);
                }               
            }
            else
            {
                CraftUIManager._instance.AllPlayerCraftingInventories.Add(craftID, new Inventory(9));
            }

            CraftUIManager._instance.UpdateView();
            //Debug_ShowAllPlayerCraftingInventories();
            
        }
        [PunRPC] public void RPC_CraftingClear(int craftID)
        {
            if (CraftUIManager._instance.AllPlayerCraftingInventories.TryGetValue(craftID, out Inventory box))
            {
                box.ItemList.Clear();
                CraftUIManager._instance.UpdateView();
            }
        }
        #endregion

        public void Debug_ShowAllPlayerCraftingInventories()
        {
            if (CraftUIManager._instance == null) return;
            // DEBUG
            Debug.Log("AllPlayerCraftingInventories: " + CraftUIManager._instance.AllPlayerCraftingInventories.Count);

            foreach (KeyValuePair<int, Inventory> dic in CraftUIManager._instance.AllPlayerCraftingInventories)
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

            foreach (GameObject obj in CraftItemPrefabs)
            {
                Item item;
                // ###NEED TO MODIFY
                if (obj.TryGetComponent<WeaponController>(out WeaponController w))
                {
                    item = obj.GetComponent<WeaponController>().WeaponData;
                }
                else
                {
                    item = obj.GetComponent<ItemBehavior>().item;
                }
                
                item.Number = 0;
            }
        }

        private bool IsPlayerReady()
        {
            // use MasterClient to update the scene
            // TODO: need to modify number of player (2~4)
            // 時間: 1分鐘內, 
            // 個人戰: 人數>1, 團體: 人數==MAX_PLAYERS_PER_ROOM
            //SceneManagerHelper.ActiveSceneName
            return (
                PhotonNetwork.IsMasterClient && 
                PhotonNetwork.CurrentRoom.PlayerCount == GameConstants.MAX_PLAYERS_PER_ROOM
                );
        }

        private void MovePlayersToGameStage_Test() //PhotonNetwork.Instantiate
        {
            int index = PhotonNetwork.CurrentRoom.PlayerCount - 1; //Random.Range(0, PlayerLocations.Count);

            // generate the player : it gets synced by using PhotonNetwork.Instantiate
            PhotonNetwork.Instantiate(PlayerPrefab.name, PlayerLocations[index].position, Quaternion.identity, 0);

            Debug.Log("=========MovePlayersToGameStage TEST=====");
        }

        private void GenerateItems()
        {
            if (!PhotonNetwork.IsMasterClient) return;

            // Fridge
            for (short i = 0; i < PhotonNetwork.CurrentRoom.PlayerCount; i++)
            {
                //Debug_ShowAllPlayerCraftingInventories();
                int colorID = (int)PhotonNetwork.PlayerList[i].CustomProperties[GameConstants.K_PROP_PLAYER_COLOR];
                PhotonNetwork.InstantiateRoomObject(FridgePrefab.name + colorID, FridgeLocations[i].position, Quaternion.identity, 0);
            }

            //int RandomObjects = Random.Range(0, ItemPrefabList.Length);
            float RandomX;
            float RandomZ;

            foreach (GameObject obj in ItemPrefabs)
            {
                m_RandomItemNumber = (short)Random.Range(ItemNumberRange[0], ItemNumberRange[1]);
                //Debug.LogFormat("GenerateItems: {0}, {1}", obj.name, m_RandomItemNumber);

                for (short i = 0; i < m_RandomItemNumber; i++)
                {
                    RandomX = Random.Range(MinMaxX[0], MinMaxX[1]);
                    RandomZ = Random.Range(MinMaxZ[0], MinMaxZ[1]);

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
                return;
            }
            */
  
            PhotonNetwork.LoadLevel(GameConstants.GetSceneByGameMode((string)PhotonNetwork.CurrentRoom.CustomProperties[GameConstants.GAME_MODE]));

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

        #region will be removed
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
        }
        #endregion
    }
}

