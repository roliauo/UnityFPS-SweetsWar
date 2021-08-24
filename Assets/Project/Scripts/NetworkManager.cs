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

        [SerializeField]
        private GameObject controlPanel;

        [SerializeField]
        private GameObject loader;

        void Awake()
        {
            // #Critical
            // this makes sure we can use PhotonNetwork.LoadLevel() on the master client and all clients in the same room sync their level automatically
            PhotonNetwork.AutomaticallySyncScene = true;
        }
        void Start()
        {
            string defaultName = string.Empty;
            if (input_playerName != null)
            {
                if (PlayerPrefs.HasKey(GameConstants.PLAYER_NAME_PREFAB_KEY))
                {
                    defaultName = PlayerPrefs.GetString(GameConstants.PLAYER_NAME_PREFAB_KEY);
                    input_playerName.text = defaultName;
                }
            }

            PhotonNetwork.NickName = defaultName;
        }

        public void Connect()
        {
            Debug.Log("connectting....");
            ShowLoader(true);

            if (PhotonNetwork.IsConnected)
            {
                ShowLoader(false);
            }
            else
            {
                // connect to Photon Online Server.            
                //PhotonNetwork.PhotonServerSettings.AppSettings.EnableLobbyStatistics = true;
                PhotonNetwork.PhotonServerSettings.AppSettings.FixedRegion = GameConstants.FIXED_REGION_ASIA;
                PhotonNetwork.ConnectUsingSettings();
                PhotonNetwork.GameVersion = Application.version; // #important: set the GameVersion right after calling ConnectUsingSettings
            }
        }

        public void Login()
        {
            SetPlayerName();
            Connect();
        }

        private void SetPlayerName()
        {
            string name = input_playerName.text;

            if (string.IsNullOrEmpty(name))
            {
                Debug.LogError("Player Name is null or empty");
                return;
            }
            PhotonNetwork.NickName = name;
            PlayerPrefs.SetString(GameConstants.PLAYER_NAME_PREFAB_KEY, name);
            Debug.LogFormat("SetPlayerName - PlayerPrefs: {0}, PhotonNetwork.NickName: {1}", 
                PlayerPrefs.GetString(GameConstants.PLAYER_NAME_PREFAB_KEY),
                PhotonNetwork.NickName
                );
        }

      

        private void ShowLoader(bool show)
        {
            controlPanel.SetActive(!show);
            loader.SetActive(show);
        }

        public override void OnDisconnected(DisconnectCause cause)
        {
            Debug.LogWarningFormat("PUN: OnDisconnected() was called by PUN with reason {0}", cause);
            ShowLoader(false);
        }

        public override void OnConnectedToMaster()
        {
            Debug.Log("player has connected to Photon master server. Now: " + PhotonNetwork.CountOfPlayers);
            Debug.Log("rooms: " + PhotonNetwork.CountOfRooms);
            Debug.Log("version: " + PhotonNetwork.GameVersion);
            Debug.Log("region: " + PhotonNetwork.CloudRegion);

            //PhotonNetwork.JoinLobby(TypedLobby.Default);
            SceneManager.LoadScene(GameConstants.SCENE_LOBBY);
        }

        /*
        public override void OnJoinedLobby()
        {
            SceneManager.LoadScene(GameConstants.SCENE_LOBBY);
        }

        public override void OnLeftLobby()
        {
            base.OnLeftLobby();
        }
        */      

    }
}
