using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
//using ExitGames.Client.Photon;
using Hashtable = ExitGames.Client.Photon.Hashtable;

namespace Game.SweetsWar
{
    public class Lobby : MonoBehaviourPunCallbacks
    {
        private List<RoomInfo> roomList = new List<RoomInfo>();
        // private Dictionary<string, RoomInfo> cachedRoomList = new Dictionary<string, RoomInfo>();
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
            Debug.Log("InLobby: " + PhotonNetwork.InLobby);
        }

        // Update is called once per frame
        void Update()
        {

        }

        private void OnGUI() {
            JoinedLobbyGUI();
        }

        public void OnPlayButtonClicked()
        {
            // btn_play.SetActive(false);
            // btn_cancel.SetActive(true);
            QuickMatch(GameConstants.GAME_MODE_PERSONAL_BATTLE);
        }

        public void OnTeamFightButtonClicked()
        {
            QuickMatch(GameConstants.GAME_MODE_TEAM_FIGHT);
        }

        public void LeaveLobby()
        {
            PhotonNetwork.LeaveLobby();
        }

        private void QuickMatch(string gameMode)
        {
            Debug.Log("IsConnectedAndReady: " + PhotonNetwork.IsConnectedAndReady);

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
            //int randomRoomName = Random.Range(0, 10000);
            //string roomName = "Let's play";
            PhotonNetwork.CreateRoom(null, new RoomOptions
            {
                MaxPlayers = GameConstants.MAX_PLAYERS_PER_ROOM,
                IsVisible = true,
                IsOpen = true,
                PublishUserId = true,
                CustomRoomProperties = CustomRoomProperties,
            });
        }

        private void JoinedLobbyGUI()
        {
            GUILayout.Label("---------------------");
            for (int i = 0; i < roomList.Count; i++)
            {
                GUILayout.Label("Room：" + roomList[i].Name);
                GUILayout.Label("players：" + roomList[i].PlayerCount + "/" + roomList[i].MaxPlayers);
                if (GUILayout.Button("Join this room"))
                {
                    //加入该房间
                    PhotonNetwork.JoinRoom(roomList[i].Name);
                }

                if (i != roomList.Count - 1)
                {
                    GUILayout.Label("*************");
                }
            }
            GUILayout.Label("---------------------");

            if (GUILayout.Button("Leave Lobby"))
            {
                PhotonNetwork.LeaveLobby();
            }
        }

        /*private void UpdateCachedRoomList(List<RoomInfo> roomList)
        {
            for (int i = 0; i < roomList.Count; i++)
            {
                RoomInfo targetInfo = roomList[i];
                if (targetInfo.RemovedFromList)
                {
                    cachedRoomList.Remove(targetInfo.Name);
                }
                else
                {
                    cachedRoomList[targetInfo.Name] = targetInfo;
                }
            }
        }*/

        public override void OnJoinedRoom()
        {
            
            // debug
            // #Critical: We only load if we are the first player, else we rely on PhotonNetwork.AutomaticallySyncScene to sync our instance scene.
            if (PhotonNetwork.CurrentRoom.PlayerCount == 1)
            {
                Debug.Log("First Player -> Load Level");
                
                //PhotonNetwork.LoadLevel(GameConstants.SCENE_GAME + CustomRoomProperties[GameConstants.GAME_MODE]);
                PhotonNetwork.LoadLevel((string)CustomRoomProperties[GameConstants.GAME_MODE]);
            }
            

            /*
            if (PhotonNetwork.CurrentRoom.PlayerCount > 1)
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
            base.OnRoomListUpdate(roomList);
            this.roomList = roomList;
        }
        /*
        public override void OnJoinedLobby()
        {
            Debug.Log("OnJoinedLobby");
            //base.OnJoinedLobby();
            roomList.Clear();
        }
        */
        public override void OnLeftLobby()
        {
            Debug.Log("OnLeftLobby");
            base.OnLeftLobby();
            roomList.Clear();
            PhotonNetwork.Disconnect();
            SceneManager.LoadScene(GameConstants.SCENE_TITLE);
        }

        public override void OnDisconnected(DisconnectCause cause)
        {
            Debug.Log("Lobby: OnDisconnected()");
            roomList.Clear();
        }
    }
}

