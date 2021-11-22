using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;
using System.Linq;
using Hashtable = ExitGames.Client.Photon.Hashtable;

namespace Game.SweetsWar
{
    public class GameManager : MonoBehaviourPunCallbacks
    {
        static public GameManager Instance;

        public GameObject Menu;
        public GameObject CraftPanel;
        public GameObject AimTarget;
        public GameObject EndPanel;

        [Header("Items")]
        public List<GameObject> ItemPrefabs;
        public int[] MinMaxX = { 2, 55 };
        public int[] MinMaxZ = { 2, 80 };
        public int[] ItemNumberRange = { 10, 20 };

        [Header("Player")]
        public List<Transform> PlayerLocations;
        [SerializeField] private GameObject PlayerPrefab;

        [Header("Fridge")]
        public List<Transform> FridgeLocations;
        public GameObject FridgePrefab;

        // for show/hide backpack
        //public GameObject BackpackUI;
        //public bool StopAction;

        [Header("Craft")]
        public GameObject[] CraftItemPrefabs;

        private short m_RandomItemNumber;

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
                /*
                int index, playerCount = PhotonNetwork.CurrentRoom.PlayerCount;
                int[] indexList = new int[playerCount];
                bool[] indexTagList = new bool[playerCount];
                Hashtable hash = new Hashtable();
                index = Random.Range(0, indexList.Length);

                for (int i = 0; i < playerCount; i++)
                {
                    do
                    {                      
                        if (!indexTagList[index])
                        {
                            indexTagList[index] = true;
                            if (PhotonNetwork.PlayerList[i].CustomProperties.TryGetValue(GameConstants.K_PROP_PLAYER_INDEX, out object x))
                            {
                                PhotonNetwork.PlayerList[i].CustomProperties[GameConstants.K_PROP_PLAYER_INDEX] = index;
                            }
                            else
                            {
                                hash.Add(GameConstants.K_PROP_PLAYER_INDEX, index);
                                PhotonNetwork.PlayerList[i].SetCustomProperties(hash);
                            }
                            
                            Debug.Log("player: " + PhotonNetwork.PlayerList[i].NickName + ", index:" + PhotonNetwork.PlayerList[i].CustomProperties[GameConstants.K_PROP_PLAYER_INDEX]);
                        }

                        index = Random.Range(0, indexList.Length);
                    } while (indexTagList[index]);

                }
              */
                /*
                 for (int i = 0; i < PhotonNetwork.PlayerList.Length; i++)
                 {
                     Hashtable hash = new Hashtable();
                     hash.Add(GameConstants.K_PROP_PLAYER_INDEX, i);
                     PhotonNetwork.PlayerList[i].SetCustomProperties(hash);
                     Debug.Log("player: " + PhotonNetwork.PlayerList[i].NickName + ", index:" + PhotonNetwork.PlayerList[i].CustomProperties[GameConstants.K_PROP_PLAYER_INDEX]);
                 }
                 */

                photonView.RPC("RPC_InitializePlayerPosition", RpcTarget.All);

                /*
                 //FAIL
                for (int i = 0; i < PhotonNetwork.PlayerList.Length; i++)
                {
                    PlayerController.localPlayerInstance.transform.localPosition = PlayerLocations[i].position;
                    Debug.Log("player: " + PhotonNetwork.PlayerList[i].NickName + ", index:" + i);
                }
                */

            }

            SetCursorMode(false);
            GenerateItems();


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
            SetCursorMode(true);
            ClearGameData();
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

        public void showEndPanel()
        {
            EndPanel.SetActive(true);
            SetCursorMode(true);
            // end panel: show score, can close and back to game to see others (transfer camera, and can't move)
        }

        public void End()
        {
            PhotonNetwork.LoadLevel(GameConstants.SCENE_END);
        }

        #region RPC
        [PunRPC] public void RPC_InitializePlayerPosition()
        {
            //PlayerController.localPlayerInstance.transform.localPosition = PlayerLocations[Random.Range(0, PlayerLocations.Count - 1)].position;
            PlayerController.localPlayerInstance.transform.localPosition = PlayerLocations[(int)PhotonNetwork.LocalPlayer.CustomProperties[GameConstants.K_PROP_PLAYER_INDEX]].position;

        }

        [PunRPC] public void RPC_InitializePlayerPositionByIndex(int index)
        {
            PlayerController.localPlayerInstance.transform.localPosition = PlayerLocations[index].position;
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
                // ###NEED TO MODIFY
                Item item = obj.GetComponent<WeaponController>().WeaponData; 
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
                //m_gameState == false &&
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

        private void GenerateItems()
        {
            if (!PhotonNetwork.IsMasterClient) return;

            // Fridge
            for (short i = 0; i < PhotonNetwork.CurrentRoom.PlayerCount; i++)
            {
                //Debug_ShowAllPlayerCraftingInventories();
                PhotonNetwork.InstantiateRoomObject(FridgePrefab.name, FridgeLocations[i].position, Quaternion.identity, 0);
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
                Debug.LogError("PhotonNetwork : Trying to Load a level but we are not the master Client");
                return;
            }
            */
  
            //PhotonNetwork.LoadLevel(GameConstants.SCENE_GAME); //TODO: game mode
            PhotonNetwork.LoadLevel(GameConstants.GetSceneByGameMode((string)PhotonNetwork.CurrentRoom.CustomProperties[GameConstants.GAME_MODE]));
            //PhotonNetwork.LoadLevel(GameConstants.SCENE_READY);

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

