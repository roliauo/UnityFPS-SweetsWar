using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Game.SweetsWar
{
    public class HealthUI : MonoBehaviour
    {
        public Image FillImage;
        public TMP_Text HP;

        private void Start()
        {
            if (!PhotonNetwork.IsConnected)
            {
                return;
            }
            //m_player = FindObjectOfType<PlayerController>();
        }

        private void Update()
        {
            if (PlayerController._instance) {
                FillImage.fillAmount = PlayerController._instance.health / PlayerController._instance.maxHealth;
                HP.text = PlayerController._instance.health.ToString();
            }

        }
    }
}
