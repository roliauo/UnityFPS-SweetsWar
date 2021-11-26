using Photon.Pun;
using Photon.Pun.UtilityScripts;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Game.SweetsWar
{
    public class InfoManager : MonoBehaviour
    {
        public static InfoManager _instance;

        [Header("Treasure")]
        public GameObject TreasureAnnouncement;
        public Image TreasureTip;

        [Header("player")]
        public Text PlayerName;

        [Header("Menu")]
        public GameObject Menu;      
        public TMP_Text GameMode;
        public TMP_Text Room;
        public TMP_Text PlayerCount;
        public TMP_Text Version;
        public TMP_Text Region;
        public Button Button_Help;

        [Header("Help")]
        public GameObject HelpPanel;

        private void Start()
        {
            if (!PhotonNetwork.IsConnected)
            {
                SceneManager.LoadScene(GameConstants.SCENE_TITLE);
                return;
            }

            _instance = this;
            PlayerName.text = PhotonNetwork.LocalPlayer.NickName;  //PlayerController._instance.photonView.Owner.NickName;
            PlayerName.color = GameConstants.GetColor((int)PhotonNetwork.LocalPlayer.CustomProperties[GameConstants.K_PROP_PLAYER_COLOR]);
            GameMode.text = (string)PhotonNetwork.CurrentRoom.CustomProperties[GameConstants.GAME_MODE];
            Room.text = PhotonNetwork.CurrentRoom.Name;
            PlayerCount.text = PhotonNetwork.CurrentRoom.PlayerCount + " / " + PhotonNetwork.CurrentRoom.MaxPlayers;
            Version.text = "ver. " + PhotonNetwork.GameVersion;
            Region.text = "region: " + PhotonNetwork.CloudRegion;

            Button_Help.onClick.AddListener(() =>
            {
                HelpPanel.SetActive(true);
                Menu.SetActive(false);
            });
        }

        private void Update()
        {
            if (TreasureAnnouncement.activeInHierarchy && Input.GetKeyDown(KeyCode.Space))
            {
                TreasureAnnouncement.SetActive(false);
            }
        }

        public void SetTreasureGoal(int id)
        {
            Debug.Log("SetTreasureGoal: " + id);
            foreach (Item item in CraftUIManager._instance.OutputItems)
            {
                if (item.ID == id)
                {
                    TreasureAnnouncement.GetComponentInChildren<Image>().sprite = item.Icon;
                    TreasureTip.sprite = item.TreasureTip; 
                    break;
                }

            }
            TreasureAnnouncement.SetActive(true);
            TreasureTip.gameObject.SetActive(true);
        }
    }
}
