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
    public class FooterManager : MonoBehaviour
    {
       public Text PlayerName;

        void Start()
        {
            if (!PhotonNetwork.IsConnected)
            {
                SceneManager.LoadScene(GameConstants.SCENE_TITLE);
                return;
            }
            
            PlayerName.text = PhotonNetwork.LocalPlayer.NickName;  //PlayerController._instance.photonView.Owner.NickName;
            //PlayerName.color = GameConstants.GetColor(PhotonNetwork.LocalPlayer.GetPlayerNumber()); //PhotonNetwork.LocalPlayer.CustomProperties()
        }


    }
}
