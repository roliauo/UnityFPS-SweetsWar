using Photon.Pun;
using Photon.Pun.UtilityScripts;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Hashtable = ExitGames.Client.Photon.Hashtable;

namespace Game.SweetsWar
{
    public class ReadyStage : MonoBehaviourPunCallbacks
    {
        public int[] PositionRangeX = {20, 80};
        public int[] PositionRangeZ = {30, 70};
        public float TimeLeft = 30f;
        public float TimeLeftAlert = 10f;
        public TMP_Text CountDownText;
        public TMP_Text ReadyText;
        public Text RoomNameText;
        public Button Btn_LeaveRoom;
        public Button Btn_Start;
        public GameObject RoomListItemPrefab;

        public bool clickedStart = false;

        private void Start()
        {
            if (!PhotonNetwork.IsConnected)
            {
                SceneManager.LoadScene(GameConstants.SCENE_TITLE);
                return;
            }

            /* SET PLAYER */
            if (PlayerController.localPlayerInstance == null)
            {
                Debug.LogFormat("We are Instantiating LocalPlayer from {0}", SceneManagerHelper.ActiveSceneName);
                GeneratePlayersInReadyStage();

            }
            else
            {
                Debug.LogFormat("Ignoring scene load for {0}", SceneManagerHelper.ActiveSceneName);
            }

            SetCursorMode(false);
            //RoomNameText.text = PhotonNetwork.CurrentRoom.Name;
            RoomListItemPrefab.GetComponent<RoomListItem>().SetInfo(PhotonNetwork.CurrentRoom, false);

            Btn_LeaveRoom.onClick.AddListener(LeaveRoom);
            Btn_Start.onClick.AddListener(OnClickStart);
           
            if (PhotonNetwork.IsMasterClient && IsPlayerReady())
            {
                SetCursorMode(true);
                Btn_Start.gameObject.SetActive(true);
            }         

        }

        void Update()
        {
            if (!clickedStart && Input.GetKeyDown(KeyCode.Escape))
            {
                if (PhotonNetwork.IsMasterClient && IsPlayerReady())
                {
                    Btn_Start.gameObject.SetActive(!Btn_LeaveRoom.gameObject.activeInHierarchy);
                }

                SetCursorMode(!Btn_LeaveRoom.gameObject.activeInHierarchy);
                Btn_LeaveRoom.gameObject.SetActive(!Btn_LeaveRoom.gameObject.activeInHierarchy);  
            }
        }

        private void OnClickStart()
        {
            Btn_Start.gameObject.SetActive(false);
            Btn_LeaveRoom.gameObject.SetActive(false);
            PhotonNetwork.CurrentRoom.IsOpen = false;

            for (int i = 0; i < PhotonNetwork.PlayerList.Length; i++)
            {
                Hashtable hash = new Hashtable();
                hash.Add(GameConstants.K_PROP_PLAYER_INDEX, i);
                PhotonNetwork.PlayerList[i].SetCustomProperties(hash);
                Debug.Log("player: " + PhotonNetwork.PlayerList[i].NickName + ", index:" + PhotonNetwork.PlayerList[i].CustomProperties[GameConstants.K_PROP_PLAYER_INDEX]);
            }

            photonView.RPC("StartCountDown", RpcTarget.All);
        }

        private void SetCursorMode(bool show)
        {
            if (PlayerController._instance) PlayerController._instance.stopMove = show;
            Cursor.visible = show;
            Cursor.lockState = show ? CursorLockMode.None : CursorLockMode.Locked;
        }

        [PunRPC] private void StartCountDown()
        {
            clickedStart = true;
            InvokeRepeating("TimeCountDown", 1, 1);
        }

        private void LeaveRoom()
        {
            PhotonNetwork.LeaveRoom();
        }

        private void TimeCountDown()
        {
            //TimeLeft -= Time.deltaTime;
            TimeLeft--;
            CountDownText.text = TimeLeft.ToString();
            if (TimeLeft <= TimeLeftAlert)
            {
                ReadyText.gameObject.SetActive(true);
                CountDownText.gameObject.SetActive(true);
            }

            if (TimeLeft == 0)
            {                
                // only MasterClient 
                if (PhotonNetwork.IsMasterClient)
                {
                    PhotonNetwork.LoadLevel(GameConstants.GetSceneByGameMode((string)PhotonNetwork.CurrentRoom.CustomProperties[GameConstants.GAME_MODE]));
                }
            }
        }

        private void GeneratePlayersInReadyStage() // random range
        {
            //Debug.Log(PhotonNetwork.LocalPlayer.NickName + " GetPlayerNumber:" + PhotonNetwork.LocalPlayer.GetPlayerNumber());
            // generate the player : it gets synced by using PhotonNetwork.Instantiate
            // (PhotonNetwork.LocalPlayer.GetPlayerNumber()+1)
            int colorID = Random.Range(0, 4); //+ colorID
            PhotonNetwork.Instantiate("Player" + colorID, new Vector3(Random.Range(PositionRangeX[0], PositionRangeX[1]), 0, Random.Range(PositionRangeZ[0], PositionRangeZ[1])), Quaternion.identity, 0);
            
            Hashtable hash = new Hashtable();
            hash.Add(GameConstants.K_PROP_PLAYER_COLOR, colorID);
            PhotonNetwork.LocalPlayer.SetCustomProperties(hash);
        }

        private bool IsPlayerReady()
        {
            // use MasterClient to update the scene
            // TODO: need to modify number of player (2~4)
            // 時間: 1分鐘內, 
            // 個人戰: 人數>1, 團體: 人數==MAX_PLAYERS_PER_ROOM
            //SceneManagerHelper.ActiveSceneName
            // (string)PhotonNetwork.CurrentRoom.CustomProperties[GameConstants.GAME_MODE]

            string mode = (string)PhotonNetwork.CurrentRoom.CustomProperties[GameConstants.GAME_MODE];
            return (
                //PhotonNetwork.IsMasterClient &&
                (
                    (mode == GameConstants.GAME_MODE_PERSONAL_BATTLE && PhotonNetwork.CurrentRoom.PlayerCount > 1) ||
                    (mode == GameConstants.GAME_MODE_TEAM_FIGHT && PhotonNetwork.CurrentRoom.PlayerCount == GameConstants.MAX_PLAYERS_PER_ROOM)
                 )
                //PhotonNetwork.CurrentRoom.PlayerCount == GameConstants.MAX_PLAYERS_PER_ROOM
                );
        }

        #region PUN callback
        public override void OnLeftRoom()
        {
            SceneManager.LoadScene(GameConstants.SCENE_LOBBY);
        }

        // Watching other Players' Connection
        public override void OnPlayerEnteredRoom(Player other)
        {
            Debug.Log(other.NickName + " entered this room!"); // not seen if you're the player connecting
            Debug.Log("MasterClient.NickName: " + PhotonNetwork.MasterClient.NickName);

            if (PhotonNetwork.IsMasterClient)
            {
                PhotonNetwork.LoadLevel(GameConstants.SCENE_READY);
            }
        }
        public override void OnPlayerLeftRoom(Player other)
        {
            Debug.Log(other.NickName + " left this room!"); // seen when other disconnects
            Debug.Log("MasterClient.NickName: " + PhotonNetwork.MasterClient.NickName);

        }
        #endregion
    }
}
