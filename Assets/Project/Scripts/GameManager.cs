using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;
using Photon.Realtime;

namespace Game.SweetsWar
{
    public class GameManager : MonoBehaviourPunCallbacks
    {
        static public GameManager Instance;
        private GameObject m_instance;

        [SerializeField]
        private GameObject playerPrefab;
       
        void Start()
        {
            Instance = this;

            if (!PhotonNetwork.IsConnected)
            {
                SceneManager.LoadScene(GameConstants.SCENE_TITLE);
                return;
            }

            Cursor.lockState = CursorLockMode.Locked;
            /*
            if (PlayerManager.LocalPlayerInstance == null)
            {
                Debug.LogFormat("We are Instantiating LocalPlayer from {0}", SceneManagerHelper.ActiveSceneName);

                // we're in a room. spawn a character for the local player. it gets synced by using PhotonNetwork.Instantiate
                PhotonNetwork.Instantiate(this.playerPrefab.name, new Vector3(0f, 5f, 0f), Quaternion.identity, 0);
            }
            else
            {

                Debug.LogFormat("Ignoring scene load for {0}", SceneManagerHelper.ActiveSceneName);
            }
            */
        }

        void Update()
        {
            // "back" button of phone equals "Escape". quit app if that's pressed
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                PhotonNetwork.LeaveRoom();
            }
        }

        private void OnDestroy()
        {
            Cursor.lockState = CursorLockMode.None;
        }

        void LoadArena()
        {
            if (!PhotonNetwork.IsMasterClient)
            {
                Debug.LogError("PhotonNetwork : Trying to Load a level but we are not the master Client");
            }
            /*
            Debug.LogFormat("PhotonNetwork : Loading Level : {0}", PhotonNetwork.CurrentRoom.PlayerCount);
            PhotonNetwork.LoadLevel(GameConstants.SCENE_GAME_PLAYER + PhotonNetwork.CurrentRoom.PlayerCount);
            */
            PhotonNetwork.LoadLevel(GameConstants.SCENE_GAME);
        }

        public override void OnLeftRoom()
        {
            SceneManager.LoadScene(GameConstants.SCENE_LOBBY);
        }

        // Watching other Players Connection
        public override void OnPlayerEnteredRoom(Player other)
        {
            Debug.Log(other.NickName + " entered this room!"); // not seen if you're the player connecting

            if (PhotonNetwork.IsMasterClient)
            {
                Debug.LogFormat("OnPlayerEnteredRoom IsMasterClient {0}", PhotonNetwork.IsMasterClient); // called before OnPlayerLeftRoom

                LoadArena();
            }
        }
        public override void OnPlayerLeftRoom(Player other)
        {
            Debug.Log(other.NickName + " left this room!"); // seen when other disconnects

            if (PhotonNetwork.IsMasterClient)
            {
                Debug.LogFormat("OnPlayerEnteredRoom IsMasterClient {0}", PhotonNetwork.IsMasterClient); // called before OnPlayerLeftRoom

                LoadArena();
            }
        }
    }
}

