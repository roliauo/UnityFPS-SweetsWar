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

        const string playerNamePrefKey = "PlayerName";

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
            ShowLoader(true);

            if (PhotonNetwork.IsConnected)
            {
                ShowLoader(false);
                //PhotonNetwork.JoinLobby();
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

            PhotonNetwork.JoinLobby(TypedLobby.Default);
            //SceneManager.LoadScene(GameConstants.SCENE_LOBBY);
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
