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
        public Image TreasureAnnouncement;
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
            //PlayerName.color = GameConstants.GetColor(PhotonNetwork.LocalPlayer.GetPlayerNumber()); //PhotonNetwork.LocalPlayer.CustomProperties()
        }

        private void Update()
        {
            if (TreasureAnnouncement.gameObject.activeInHierarchy && Input.anyKeyDown)
            {
                TreasureAnnouncement.gameObject.SetActive(false);
            }
        }

        public void SetTreasureGoal(int id)
        {
            Debug.Log("SetTreasureGoal: " + id);
            foreach (Item item in CraftUIManager._instance.OutputItems)
            {
                if (item.ID == id)
                {
                    TreasureAnnouncement.sprite = item.Icon;
                    //Sprite tip = Resources.Load("CraftTip_" + id, typeof(Sprite)) as Sprite;
                    //TreasureTip.sprite = tip; //Resources.Load("CraftTip_" + id, typeof(Sprite)) as Sprite;
                    TreasureTip.sprite.name = "CraftTip_" + id;
                    break;
                }

            }
            TreasureAnnouncement.gameObject.SetActive(true);
            TreasureTip.gameObject.SetActive(true);
        }
    }
}
