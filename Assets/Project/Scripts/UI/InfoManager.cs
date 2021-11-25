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
        public GameObject TreasureAnnouncement;
        public Image TreasureTip;
        public Text PlayerName;

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
