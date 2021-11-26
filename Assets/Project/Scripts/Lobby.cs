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

        [Header("Setting Panel")]
        public GameObject SettingPanel;

        [Header("Info Panel")]
        public GameObject InfoPanel;
        public Button Button_Info;

        [Header("Help Panel")]
        public GameObject HelpPanel;
        public Button Button_Help;
        public Button Button_Next;
        public Image[] PagesImage;

        private byte m_pageNumber = 1;
        private Dictionary<string, RoomInfo> m_cachedRoomList;
        private Dictionary<string, GameObject> m_roomListItems;
        private Dictionary<int, GameObject> m_playerListItems;

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
            /*
            Button_Info.onClick.AddListener(() =>
            {
                SetActivePanel(InfoPanel.name);
            });*/

            Button_Help.onClick.AddListener(() =>
            {
                SetActivePanel(HelpPanel.name);
            });

            Button_Next.onClick.AddListener(() =>
            {               
                if (m_pageNumber >= PagesImage.Length)
                {
                    PagesImage[m_pageNumber - 1].gameObject.SetActive(false);
                    m_pageNumber = 1;
                    PagesImage[0].gameObject.SetActive(true);
                    SetActivePanel(MainPanel.name);
                }
                else
                {
                    if (m_pageNumber>0) PagesImage[m_pageNumber - 1].gameObject.SetActive(false);
                    PagesImage[m_pageNumber].gameObject.SetActive(true);
                    m_pageNumber++;
                }               

            });

            m_cachedRoomList = new Dictionary<string, RoomInfo>();
            m_roomListItems = new Dictionary<string, GameObject>();

            Debug.Log("InLobby: " + PhotonNetwork.InLobby);
        }


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

        public void ShowSettingPanel()
        {
            SetActivePanel(SettingPanel.name);
        }

        public void ShowRoomListPanel() 
        {
            // for updating room list
            if (!PhotonNetwork.InLobby)
            {
                PhotonNetwork.JoinLobby(TypedLobby.Default);
            }

            SetActivePanel(RoomListPanel.name);
        }

        public void OnBackButtonClicked()
        {
            // for updating room list
            if (PhotonNetwork.InLobby)
            {
                PhotonNetwork.LeaveLobby();
            }

            SetActivePanel(MainPanel.name);

            ShowConnectionInfo();
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

        public void CreateRoom()
        {
            // TODO: Need to add game mode
            string roomName = Input_RoomName.text;
            roomName = (roomName.Equals(string.Empty)) ? "Let's play " + Random.Range(1, 10000) : roomName;

            Debug.Log("CreateRoom - GAME_MODE: " + CustomRoomProperties[GameConstants.GAME_MODE]);
            PhotonNetwork.CreateRoom(
                roomName, //null, 
                new RoomOptions
                {
                    //CleanupCacheOnLeave = false, // set false for scene objects
                    MaxPlayers = GameConstants.MAX_PLAYERS_PER_ROOM,
                    IsVisible = true,
                    IsOpen = true,
                    PublishUserId = true,
                    CustomRoomProperties = CustomRoomProperties,
                    CustomRoomPropertiesForLobby = new string[] { GameConstants.GAME_MODE } // #Important! 
                },
                TypedLobby.Default
            );
        }

        public void SearchTheRoom()
        {
            string roomName = Input_RoomName.text;
            PhotonNetwork.JoinRoom(roomName);
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

        private void ShowConnectionInfo()
        {
            // connection info
            Debug.Log("player has connected to Photon master server. Now: " + PhotonNetwork.CountOfPlayers);
            Debug.Log("rooms: " + PhotonNetwork.CountOfRooms + ", roomList.Count: " + m_cachedRoomList.Count);
            Debug.Log("version: " + PhotonNetwork.GameVersion);
            Debug.Log("region: " + PhotonNetwork.CloudRegion);
        }

        private void JoinedLobbyGUI()
        {
            // connection info
            GUILayout.Label("player has connected to Photon master server. Now: " + PhotonNetwork.CountOfPlayers);
            GUILayout.Label("rooms: " + PhotonNetwork.CountOfRooms + ", roomList.Count: " + m_cachedRoomList.Count);
            GUILayout.Label("version: " + PhotonNetwork.GameVersion);
            GUILayout.Label("region: " + PhotonNetwork.CloudRegion);

            GUILayout.Label("---------------------");

            short index = 0;
            foreach(KeyValuePair<string, RoomInfo> r in m_cachedRoomList)
            {
                GUILayout.Label("Room：" + r.Value.Name);
                GUILayout.Label("players：" + r.Value.PlayerCount + "/" + r.Value.MaxPlayers);
                if (GUILayout.Button("Join this room"))
                {
                    PhotonNetwork.JoinRoom(r.Value.Name);
                }

                if (index != m_cachedRoomList.Count - 1)
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
                    if (m_cachedRoomList.ContainsKey(info.Name))
                    {
                        m_cachedRoomList.Remove(info.Name);
                    }

                    continue;
                }


                if (m_cachedRoomList.ContainsKey(info.Name))
                {
                    m_cachedRoomList[info.Name] = info;
                }
                else
                {
                    m_cachedRoomList.Add(info.Name, info);
                }
            }*/

            for (int i = 0; i < roomList.Count; i++)
            {
                RoomInfo targetInfo = roomList[i];
                if (targetInfo.RemovedFromList) //!info.IsOpen || !info.IsVisible || 
                {
                    m_cachedRoomList.Remove(targetInfo.Name);
                }
                else
                {
                    m_cachedRoomList[targetInfo.Name] = targetInfo;
                }
            }

        }

        private void UpdateRoomListView()
        {
            foreach (RoomInfo info in m_cachedRoomList.Values)
            {
                GameObject item = Instantiate(RoomListItemPrefab);
                item.transform.SetParent(RoomListContent.transform);
                item.transform.localScale = Vector3.one;
                item.GetComponent<RoomListItem>().SetInfo(info);
                m_roomListItems.Add(info.Name, item);
            }
        }

        private void ClearRoomListView()
        {
            foreach (GameObject entry in m_roomListItems.Values)
            {
                Destroy(entry.gameObject);
            }

            m_roomListItems.Clear();
        }

        private void SetActivePanel(string activePanel)
        {
            MainPanel.SetActive(activePanel.Equals(MainPanel.name));
            RoomListPanel.SetActive(activePanel.Equals(RoomListPanel.name));
            //InfoPanel.SetActive(activePanel.Equals(InfoPanel.name));
            HelpPanel.SetActive(activePanel.Equals(HelpPanel.name));
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
            m_cachedRoomList.Clear();
            //SetActivePanel(InsideRoomPanel.name);

            if (m_playerListItems == null)
            {
                m_playerListItems = new Dictionary<int, GameObject>();
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
                if (GameConstants.DebugMode)
                {
                    PhotonNetwork.LoadLevel(GameConstants.GetSceneByGameMode((string)CustomRoomProperties[GameConstants.GAME_MODE]));
                }
                else
                {
                    PhotonNetwork.LoadLevel(GameConstants.SCENE_READY);
                }
                
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
            m_cachedRoomList.Clear();
            ClearRoomListView();
        }
        
        public override void OnLeftLobby()
        {
            Debug.Log("OnLeftLobby");
            m_cachedRoomList.Clear();
            ClearRoomListView();
            //PhotonNetwork.Disconnect();
            //SceneManager.LoadScene(GameConstants.SCENE_TITLE);
        }

        public override void OnDisconnected(DisconnectCause cause)
        {
            Debug.Log("Lobby: OnDisconnected()");
            m_cachedRoomList.Clear();
            ClearRoomListView();
        }

        #endregion
    }
}

