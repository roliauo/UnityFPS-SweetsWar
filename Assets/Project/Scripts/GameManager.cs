using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;

namespace Game.SweetsWar
{
    public class GameManager : MonoBehaviourPunCallbacks
    {
        static public GameManager Instance;
        public GameObject Menu;
        public GameObject BackpackUI;
        public bool StopAction;

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
                
            if (playerPrefab == null) //PlayerManager.LocalPlayerInstance == null
            {
                Debug.LogFormat("Missing playerPrefab Reference");

            }
            else
            {

                if (PlayerMovementController.localPlayerInstance == null)
                {
                    Debug.LogFormat("We are Instantiating LocalPlayer from {0}", SceneManagerHelper.ActiveSceneName);

                    // generate the player : it gets synced by using PhotonNetwork.Instantiate
                    PhotonNetwork.Instantiate(this.playerPrefab.name, new Vector3(10f, 10f, 10f), Quaternion.identity, 0);
                }
                else
                {

                    Debug.LogFormat("Ignoring scene load for {0}", SceneManagerHelper.ActiveSceneName);
                }

            }

            // SET CURSOR
            Cursor.lockState = CursorLockMode.Locked;

            StopAction = false;

            /*
            // SHOW PLAYERS' NAME
            playerName.text = photonView.Owner.NickName;

            Debug.LogFormat("name: {0}, key: {1}, photonView: {2}",
                PhotonNetwork.NickName,
                PlayerPrefs.GetString(GameConstants.PLAYER_NAME_PREFAB_KEY),
                photonView.Owner.NickName
              );
            */
        }

        void Update()
        {
            // show the menu
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                //Cursor.lockState = CursorLockMode.None;
                PhotonNetwork.LeaveRoom();
            }

            /*
            if (Input.GetKeyDown(KeyCode.Tab))
            {
                
                BackpackUI.SetActive(!BackpackUI.activeInHierarchy);
                Cursor.lockState = BackpackUI.activeInHierarchy ? CursorLockMode.None : CursorLockMode.Locked;
                //需設定人物不會旋轉
                //StopAction = BackpackUI.activeInHierarchy;
            }
            */
        }

        /*
        void OnGUI()
        {
            Vector3 characterPos = Camera.main.WorldToScreenPoint(playerPrefab.transform.position);
           
            characterPos = new Vector3(Mathf.Clamp(characterPos.x, 0 + (windowWidth / 2), Screen.width - (windowWidth / 2)),
                                               Mathf.Clamp(characterPos.y, 50, Screen.height),
                                               characterPos.z);
            GUILayout.BeginArea(new Rect((characterPos.x + offsetX) - (windowWidth / 2), (Screen.height - characterPos.y) + offsetY, windowWidth, windowHeight));
            // GUI CODE GOES HERE

            GUILayout.EndArea();
        }
        */

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

        private bool CheckAllPlayerLoadedLevel()
        {
            foreach (Player p in PhotonNetwork.PlayerList)
            {
                object playerLoadedLevel;

                if (p.CustomProperties.TryGetValue(GameConstants.PLAYER_LOADED_LEVEL, out playerLoadedLevel))
                {
                    if ((bool)playerLoadedLevel)
                    {
                        continue;
                    }
                }

                return false;
            }

            return true;
        }

        public override void OnLeftRoom()
        {
            SceneManager.LoadScene(GameConstants.SCENE_LOBBY);
        }

        // Watching other Players' Connection
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

