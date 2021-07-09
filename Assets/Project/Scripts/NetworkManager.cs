using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;

namespace Game.SweetsWar
{
    public class NetworkManager : MonoBehaviourPunCallbacks
    {
        [SerializeField]
        public string gameVersion = "20210705";   
        private byte maxPlayersPerRoom = 4;

        public InputField input_playerName;
        public Button btn_login;
        const string playerNamePrefKey = "PlayerName";

        void Awake()
        {
            // #Critical
            // this makes sure we can use PhotonNetwork.LoadLevel() on the master client and all clients in the same room sync their level automatically
            PhotonNetwork.AutomaticallySyncScene = true;
        }
        void Start()
        {
            // Title scene
            string defaultName = string.Empty;
            // InputField _inputField = this.GetComponent<InputField>();
            if (input_playerName != null)
            {
                if (PlayerPrefs.HasKey(playerNamePrefKey))
                {
                    defaultName = PlayerPrefs.GetString(playerNamePrefKey);
                    input_playerName.text = defaultName;
                }
            }

            PhotonNetwork.NickName = defaultName;
        }

        public void Connect()
        {
            if (PhotonNetwork.IsConnected)
            {
                PhotonNetwork.JoinLobby();
            }
            else
            {
                // connect to Photon Online Server.
                PhotonNetwork.GameVersion = gameVersion;
                //PhotonNetwork.PhotonServerSettings.AppSettings.EnableLobbyStatistics = true;
                //PhotonNetwork.PhotonServerSettings.AppSettings.FixedRegion = "asia";
                PhotonNetwork.ConnectUsingSettings();
            }
        }

        public void SetPlayerName()
        {
            string name = input_playerName.text;

            if (string.IsNullOrEmpty(name))
            {
                Debug.LogError("Player Name is null or empty");
                return;
            }
            Debug.Log(PlayerPrefs.GetString(playerNamePrefKey));
            PhotonNetwork.NickName = name;
            PlayerPrefs.SetString(playerNamePrefKey, name);
        }

        public void Login()
        {
            Connect();
        }

        public void OnPlayButtonClicked()
        {
            // btn_play.SetActive(false);
            // btn_cancel.SetActive(true);
            PhotonNetwork.JoinRandomRoom();
            //PhotonNetwork.
        }

        public void createRoom()
        {
            //int randomRoomName = Random.Range(0, 10000);
            // string roomName = "Let's play";
            PhotonNetwork.CreateRoom(null, new RoomOptions
            {
                MaxPlayers = maxPlayersPerRoom,
                IsVisible = true,
                IsOpen = true,
                PublishUserId = true,
            });
        }

        public override void OnDisconnected(DisconnectCause cause)
        {
            Debug.LogWarningFormat("PUN: OnDisconnected() was called by PUN with reason {0}", cause);
        }

        public override void OnConnectedToMaster()
        {
            Debug.Log("player has connected to Photon master server. Now: " + PhotonNetwork.CountOfPlayers);
            PhotonNetwork.JoinLobby();
            Debug.Log("rooms: " + PhotonNetwork.CountOfRooms);
        }

        public override void OnJoinedRoom()
        {
            //PhotonNetwork.LoadLevel(GameConstants.SCENE_GAME);
            PhotonNetwork.LoadLevel(GameConstants.SCENE_GAME);
        }


        public override void OnJoinRandomFailed(short returnCode, string message)
        {
            Debug.Log("OnJoinRandomFailed() was called by PUN. No random room available, so we create one.\nCalling: PhotonNetwork.CreateRoom");
            createRoom();
        }

    }
}
