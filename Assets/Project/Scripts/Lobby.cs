using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
//using ExitGames.Client.Photon;
using Hashtable = ExitGames.Client.Photon.Hashtable;

namespace Game.SweetsWar
{
    public class Lobby : MonoBehaviourPunCallbacks
    {
        public GameObject MainPanel;

        [Header("Room List Panel")]
        public GameObject RoomListPanel;
        public GameObject RoomListContent;
        public GameObject RoomListItemPrefab;
        public InputField Input_RoomName;  // for creating or searching the room

        [Header("Player List Panel")]
        public GameObject PlayerListPanel;
        public GameObject PlayerListItemPrefab;

        [Header("Player Information Panel")]
        public GameObject PlayerInfoPanel;

        [Header("Setting Panel")]
        public GameObject SettingPanel;

        
        private Dictionary<string, RoomInfo> cachedRoomList;
        private Dictionary<string, GameObject> roomListItems;
        private Dictionary<int, GameObject> playerListItems;

        private Hashtable CustomRoomProperties = new Hashtable() {
                { GameConstants.GAME_MODE, GameConstants.GAME_MODE_PERSONAL_BATTLE }
            };
        void Start()
        {
            if (!PhotonNetwork.IsConnected)
            {
                SceneManager.LoadScene(GameConstants.SCENE_TITLE);
                return;
            }

            cachedRoomList = new Dictionary<string, RoomInfo>();
            roomListItems = new Dictionary<string, GameObject>();

            Debug.Log("InLobby: " + PhotonNetwork.InLobby);
        }

        /*
        private void OnGUI() {
            JoinedLobbyGUI();
        }
        */

        #region UI

        public void QuickMatchPersonalBattle()
        {
            // btn_play.SetActive(false);
            // btn_cancel.SetActive(true);
            QuickMatch(GameConstants.GAME_MODE_PERSONAL_BATTLE);
        }

        public void QuickMatchTeamFight()
        {
            QuickMatch(GameConstants.GAME_MODE_TEAM_FIGHT);
        }

        public void ShowPlayerInfoPanel()
        {
            SetActivePanel(PlayerInfoPanel.name);
        }

        public void ShowSettingPanel()
        {
            SetActivePanel(SettingPanel.name);
        }

        public void ShowRoomListPanel() 
        {
            if (!PhotonNetwork.InLobby)
            {
                PhotonNetwork.JoinLobby();
            }

            SetActivePanel(RoomListPanel.name);
        }

        public void OnBackButtonClicked()
        {
            if (PhotonNetwork.InLobby)
            {
                PhotonNetwork.LeaveLobby();
            }

            SetActivePanel(MainPanel.name);
        }

        public void LeaveRoom()
        {
            PhotonNetwork.LeaveRoom();
        }

        public void LeaveLobby()
        {
            if (PhotonNetwork.InLobby)
            {
                PhotonNetwork.LeaveLobby();
            }
            PhotonNetwork.Disconnect();
            SceneManager.LoadScene(GameConstants.SCENE_TITLE);
        }

        public void PlayerUpdateReadyState()
        {
            CheckPlayersReady();
            //StartGameButton.gameObject.SetActive(CheckPlayersReady());
        }

        #endregion

        #region Private function

        private void QuickMatch(string gameMode)
        {
            Debug.Log("IsConnectedAndReady: " + PhotonNetwork.IsConnectedAndReady);

            // to update the room list
            if (PhotonNetwork.InLobby)
            {
                PhotonNetwork.LeaveLobby();
            }

            CustomRoomProperties = new Hashtable() {
                { GameConstants.GAME_MODE, gameMode }
            };

            if (PhotonNetwork.IsConnectedAndReady)
            {
                PhotonNetwork.JoinRandomRoom(CustomRoomProperties, 0);
            }
        }

        private void CreateRoom()
        {
            string roomName = Input_RoomName.text;
            roomName = (roomName.Equals(string.Empty)) ? "Let's play " + Random.Range(1, 10000) : roomName;

            //string roomName = "Let's play " + Random.Range(1, 10000);

            //byte maxPlayers;
            //byte.TryParse(MaxPlayersInputField.text, out maxPlayers);
            //maxPlayers = (byte)Mathf.Clamp(maxPlayers, 2, 8);
            //RoomOptions options = new RoomOptions { MaxPlayers = maxPlayers, PlayerTtl = 10000 };
            //PhotonNetwork.CreateRoom(roomName, options, null);

            //int randomRoomName = Random.Range(0, 10000);
            //string roomName = "Let's play";
            Debug.Log("CreateRoom - GAME_MODE: " + CustomRoomProperties[GameConstants.GAME_MODE]);
            PhotonNetwork.CreateRoom(
                roomName, //null, 
                new RoomOptions
                {
                    MaxPlayers = GameConstants.MAX_PLAYERS_PER_ROOM,
                    IsVisible = true,
                    IsOpen = true,
                    PublishUserId = true,
                    CustomRoomProperties = CustomRoomProperties,
                },
                TypedLobby.Default
            );
        }

        private void JoinedLobbyGUI()
        {
            // connection info
            GUILayout.Label("player has connected to Photon master server. Now: " + PhotonNetwork.CountOfPlayers);
            GUILayout.Label("rooms: " + PhotonNetwork.CountOfRooms + ", roomList.Count: " + cachedRoomList.Count);
            GUILayout.Label("version: " + PhotonNetwork.GameVersion);
            GUILayout.Label("region: " + PhotonNetwork.CloudRegion);

            GUILayout.Label("---------------------");

            short index = 0;
            foreach(KeyValuePair<string, RoomInfo> r in cachedRoomList)
            {
                GUILayout.Label("Room：" + r.Value.Name);
                GUILayout.Label("players：" + r.Value.PlayerCount + "/" + r.Value.MaxPlayers);
                if (GUILayout.Button("Join this room"))
                {
                    PhotonNetwork.JoinRoom(r.Value.Name);
                }

                if (index != cachedRoomList.Count - 1)
                {
                    GUILayout.Label("*************");
                }

                index++;
            }

            GUILayout.Label("---------------------");

            if (GUILayout.Button("Leave Lobby"))
            {
                PhotonNetwork.LeaveLobby();
            }
        }

        private void UpdateCachedRoomList(List<RoomInfo> roomList)
        {
            /*foreach (RoomInfo info in roomList)
            {
                if (info.RemovedFromList) //!info.IsOpen || !info.IsVisible || 
                {
                    if (cachedRoomList.ContainsKey(info.Name))
                    {
                        cachedRoomList.Remove(info.Name);
                    }

                    continue;
                }


                if (cachedRoomList.ContainsKey(info.Name))
                {
                    cachedRoomList[info.Name] = info;
                }
                else
                {
                    cachedRoomList.Add(info.Name, info);
                }
            }*/

            for (int i = 0; i < roomList.Count; i++)
            {
                RoomInfo targetInfo = roomList[i];
                if (targetInfo.RemovedFromList) //!info.IsOpen || !info.IsVisible || 
                {
                    cachedRoomList.Remove(targetInfo.Name);
                }
                else
                {
                    cachedRoomList[targetInfo.Name] = targetInfo;
                }
            }

        }

        private void UpdateRoomListView()
        {
            foreach (RoomInfo info in cachedRoomList.Values)
            {
                string v = (string)info.CustomProperties[GameConstants.GAME_MODE];
                //info.CustomProperties.
                Debug.Log("info: " + info);
                GameObject item = Instantiate(RoomListItemPrefab);
                item.transform.SetParent(RoomListContent.transform);
                item.transform.localScale = Vector3.one;
                item.GetComponent<RoomListItem>().Initialize(info);
                roomListItems.Add(info.Name, item);
            }
        }

        private void ClearRoomListView()
        {
            foreach (GameObject entry in roomListItems.Values)
            {
                Destroy(entry.gameObject);
            }

            roomListItems.Clear();
        }

        private void SetActivePanel(string activePanel)
        {
            MainPanel.SetActive(activePanel.Equals(MainPanel.name));
            RoomListPanel.SetActive(activePanel.Equals(RoomListPanel.name));    
            PlayerListPanel.SetActive(activePanel.Equals(PlayerListPanel.name));
            PlayerInfoPanel.SetActive(activePanel.Equals(PlayerInfoPanel.name));
            SettingPanel.SetActive(activePanel.Equals(SettingPanel.name));
        }

        private bool CheckPlayersReady()
        {
            if (!PhotonNetwork.IsMasterClient)
            {
                return false;
            }

            foreach (Player p in PhotonNetwork.PlayerList)
            {
                object isPlayerReady;
                if (p.CustomProperties.TryGetValue(GameConstants.IS_PLAYER_READY, out isPlayerReady))
                {
                    if (!(bool)isPlayerReady)
                    {
                        return false;
                    }
                }
                else
                {
                    return false;
                }
            }

            return true;
        }

        #endregion

        #region PUN CALLBACKS
        public override void OnJoinedRoom()
        {
            cachedRoomList.Clear();
            //SetActivePanel(InsideRoomPanel.name);

            if (playerListItems == null)
            {
                playerListItems = new Dictionary<int, GameObject>();
            }
            /*
            foreach (Player p in PhotonNetwork.PlayerList)
            {
                GameObject entry = Instantiate(PlayerListEntryPrefab);
                entry.transform.SetParent(InsideRoomPanel.transform);
                entry.transform.localScale = Vector3.one;
                entry.GetComponent<PlayerListEntry>().Initialize(p.ActorNumber, p.NickName);

                object isPlayerReady;
                if (p.CustomProperties.TryGetValue(AsteroidsGame.PLAYER_READY, out isPlayerReady))
                {
                    entry.GetComponent<PlayerListEntry>().SetPlayerReady((bool)isPlayerReady);
                }

                playerListEntries.Add(p.ActorNumber, entry);
            }
            */
            //StartGameButton.gameObject.SetActive(CheckPlayersReady());

            // debug
            // #Critical: We only load if we are the first player, else we rely on PhotonNetwork.AutomaticallySyncScene to sync our instance scene.
            if (PhotonNetwork.CurrentRoom.PlayerCount == 1)
            {
                Debug.Log("First Player -> Load Level");
                
                //PhotonNetwork.LoadLevel(GameConstants.SCENE_GAME + CustomRoomProperties[GameConstants.GAME_MODE]);
                PhotonNetwork.LoadLevel((string)CustomRoomProperties[GameConstants.GAME_MODE]);
            }

            /*
            if (PhotonNetwork.IsMasterClient && PhotonNetwork.CurrentRoom.PlayerCount == GameConstants.MAX_PLAYERS_PER_ROOM)
            {
                PhotonNetwork.LoadLevel((string)CustomRoomProperties[GameConstants.GAME_MODE]);
            }
            */
        }

        public override void OnJoinRandomFailed(short returnCode, string message)
        {
            Debug.Log("OnJoinRandomFailed()");
            CreateRoom();
        }

        public override void OnRoomListUpdate(List<RoomInfo> roomList)
        {
            // only show the updated room
            ClearRoomListView();

            UpdateCachedRoomList(roomList);
            UpdateRoomListView();
        }
        
        public override void OnJoinedLobby()
        {
            Debug.Log("OnJoinedLobby");
            //base.OnJoinedLobby();
            cachedRoomList.Clear();
            ClearRoomListView();
        }
        
        public override void OnLeftLobby()
        {
            Debug.Log("OnLeftLobby");
            cachedRoomList.Clear();
            ClearRoomListView();
            //PhotonNetwork.Disconnect();
            //SceneManager.LoadScene(GameConstants.SCENE_TITLE);
        }

        public override void OnDisconnected(DisconnectCause cause)
        {
            Debug.Log("Lobby: OnDisconnected()");
            cachedRoomList.Clear();
            ClearRoomListView();
        }

        #endregion
    }
}

