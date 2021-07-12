using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Game.SweetsWar
{
    public class Lobby : MonoBehaviourPunCallbacks
    {
        private List<RoomInfo> roomList = new List<RoomInfo>();
        // private Dictionary<string, RoomInfo> cachedRoomList = new Dictionary<string, RoomInfo>();
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
            Debug.Log("IsConnectedAndReady: " + PhotonNetwork.IsConnectedAndReady);
            if (PhotonNetwork.IsConnectedAndReady)
            {
                PhotonNetwork.JoinRandomRoom();
            }
                    
        }

        public void LeaveLobby()
        {
            PhotonNetwork.LeaveLobby();
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
            // #Critical: We only load if we are the first player, else we rely on PhotonNetwork.AutomaticallySyncScene to sync our instance scene.
            if (PhotonNetwork.CurrentRoom.PlayerCount == 1)
            {
                /*
                 // load area
                Debug.Log("OnJoinedRoom(): SCENE_GAME_PLAYER_1"); 
                PhotonNetwork.LoadLevel(GameConstants.SCENE_GAME_PLAYER + "1");
                */
                /*
                if (isTeamFight)
                {
                    PhotonNetwork.LoadLevel(GameConstants.SCENE_GAME_TEAM);
                }
                */
                PhotonNetwork.LoadLevel(GameConstants.SCENE_GAME);
            }
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

