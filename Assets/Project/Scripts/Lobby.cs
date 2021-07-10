using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.SweetsWar
{
    public class Lobby : MonoBehaviourPunCallbacks
    {
        private List<RoomInfo> roomList = new List<RoomInfo>();
        // private Dictionary<string, RoomInfo> cachedRoomList = new Dictionary<string, RoomInfo>();
        void Start()
        {

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
            PhotonNetwork.JoinRandomRoom();
        }

        public void CreateRoom()
        {
            //int randomRoomName = Random.Range(0, 10000);
            //string roomName = "Let's play";
            PhotonNetwork.CreateRoom(null, new RoomOptions
            {
                MaxPlayers = GameConstants.maxPlayersPerRoom,
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
            //PhotonNetwork.LoadLevel(GameConstants.SCENE_GAME);
            PhotonNetwork.LoadLevel(GameConstants.SCENE_GAME);
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
            JoinedLobbyGUI();
        }

        public override void OnJoinedLobby()
        {
            roomList.Clear();
        }

        public override void OnLeftLobby()
        {
            roomList.Clear();
        }

        public override void OnDisconnected(DisconnectCause cause)
        {
            Debug.Log("OnDisconnected()");
            roomList.Clear();
        }
    }
}

