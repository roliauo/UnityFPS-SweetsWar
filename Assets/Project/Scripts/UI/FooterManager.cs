using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Game.SweetsWar
{
    public class FooterManager : MonoBehaviour
    {
        public TMP_Text PlayerName;

        void Start()
        {
            if (!PhotonNetwork.IsConnected)
            {
                SceneManager.LoadScene(GameConstants.SCENE_TITLE);
                return;
            }
            
            PlayerName.text = PhotonNetwork.LocalPlayer.NickName;  //PlayerController._instance.photonView.Owner.NickName;
        }


    }
}
