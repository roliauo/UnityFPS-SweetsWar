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
        
        public GameObject Title;       
        public GameObject Loader;
        public GameObject MainMenu;
        public InputField Input_playerName;
        public GameObject DevInfoPanel;
        public Text Text_Version;
        public Button Btn_Login;
        public Button Btn_DevInfo;
        public Button Btn_Quit;

        //public GameObject OptionsMenu;
        //public Button Btn_Option;

        void Awake()
        {
            // #Critical
            // this makes sure we can use PhotonNetwork.LoadLevel() on the master client and all clients in the same room sync their level automatically
            PhotonNetwork.AutomaticallySyncScene = true;
        }
        void Start()
        {
            //Connect(); // for version: solution: use Application.version

            string defaultName = string.Empty;
            if (Input_playerName != null)
            {
                if (PlayerPrefs.HasKey(GameConstants.PLAYER_NAME_PREFAB_KEY))
                {
                    defaultName = PlayerPrefs.GetString(GameConstants.PLAYER_NAME_PREFAB_KEY);
                    Input_playerName.text = defaultName;
                }
            }

            PhotonNetwork.NickName = defaultName;

            Text_Version.text = "version " + Application.version; 
            Btn_Login.onClick.AddListener(Login);
            Btn_DevInfo.onClick.AddListener(() =>
            {
                SwitchMenu(DevInfoPanel.name);
            });
            Btn_Quit.onClick.AddListener(QuitGame);
        }

        private void Update()
        {
            if (DevInfoPanel.activeInHierarchy && Input.anyKeyDown)
            {
                SwitchMenu(MainMenu.name);
            }
        }

        public void Connect()
        {
            //Debug.Log("connectting....");
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
            //SceneManager.LoadScene(GameConstants.SCENE_LOBBY);
        }

        private void SetPlayerName()
        {
            string name = Input_playerName.text;

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
            MainMenu.SetActive(!show);
            Loader.SetActive(show);
        }

        public void SwitchMenu(string activePanel)
        {
            MainMenu.SetActive(activePanel.Equals(MainMenu.name));
            DevInfoPanel.SetActive(activePanel.Equals(DevInfoPanel.name));

            //Title.SetActive(false);
            //OptionsMenu.SetActive(activePanel.Equals(OptionsMenu.name));
        }

        public void QuitGame()
        {
            //Debug.Log("quit");
            // PhotonNetwork.LeaveLobby();
            PhotonNetwork.Disconnect();
            Application.Quit();
        }

        public override void OnDisconnected(DisconnectCause cause)
        {
            Debug.LogWarningFormat("PUN: OnDisconnected()", cause);
            ShowLoader(false);
        }

        public override void OnConnectedToMaster()
        {
            Debug.Log("player has connected to Photon master server. Now: " + PhotonNetwork.CountOfPlayers);
            Debug.Log("rooms: " + PhotonNetwork.CountOfRooms);
            Debug.Log("version: " + PhotonNetwork.GameVersion);
            Debug.Log("region: " + PhotonNetwork.CloudRegion);
            ////PhotonNetwork.JoinLobby(TypedLobby.Default);
            SceneManager.LoadScene(GameConstants.SCENE_LOBBY); // press login: load scene
        }

        /*
        public override void OnCustomAuthenticationFailed(string debugMessage)
        {
            base.OnCustomAuthenticationFailed(debugMessage);
        }

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
