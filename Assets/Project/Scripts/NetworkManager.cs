using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

namespace Game.SweetsWar
{
    public class NetworkManager : MonoBehaviourPunCallbacks
    {
        [SerializeField]
        private InputField input_playerName;

        private Button btn_login;
        private GameObject mainMenu;
        private GameObject title;

        [SerializeField]
        private GameObject controlPanel;

        [SerializeField]
        private GameObject loader;

        //private string gameVersion; 

        const string playerNamePrefKey = "PlayerName";

        void Awake()
        {
            // #Critical
            // this makes sure we can use PhotonNetwork.LoadLevel() on the master client and all clients in the same room sync their level automatically
            PhotonNetwork.AutomaticallySyncScene = true;
            //gameVersion = Application.version;
        }
        void Start()
        {
            string defaultName = string.Empty;
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
            Debug.Log("connectting....");
            controlPanel.SetActive(false);
            loader.SetActive(true);

            if (PhotonNetwork.IsConnected)
            {
                loader.SetActive(false);
                PhotonNetwork.JoinLobby();
            }
            else
            {
                // connect to Photon Online Server.            
                //PhotonNetwork.PhotonServerSettings.AppSettings.EnableLobbyStatistics = true;
                //PhotonNetwork.PhotonServerSettings.AppSettings.FixedRegion = "asia";
                PhotonNetwork.ConnectUsingSettings();
                PhotonNetwork.GameVersion = Application.version; // important: set the GameVersion right after calling ConnectUsingSettings
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

        

        public override void OnDisconnected(DisconnectCause cause)
        {
            Debug.LogWarningFormat("PUN: OnDisconnected() was called by PUN with reason {0}", cause);
        }

        public override void OnConnectedToMaster()
        {
            PhotonNetwork.JoinLobby();
            Debug.Log("player has connected to Photon master server. Now: " + PhotonNetwork.CountOfPlayers);
            Debug.Log("rooms: " + PhotonNetwork.CountOfRooms);
            Debug.Log("version: " + PhotonNetwork.GameVersion);
            Debug.Log("region: " + PhotonNetwork.CloudRegion);
        }

        public override void OnJoinedLobby()
        {
            SceneManager.LoadScene(GameConstants.SCENE_LOBBY);
        }

        public override void OnLeftLobby()
        {
            base.OnLeftLobby();
        }



    }
}
