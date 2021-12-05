using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Game.SweetsWar
{
    public class PlayerScoreBar : MonoBehaviour
    {
        public Text Rank;
        public Text Player;
        public Text Kills; //40% 
        public Text DamagePoint; //40%
        public Text CraftNumber; //20%
        public Text Score;

        private Color highlightColor = new Color(255, 90, 118); //#FF5B76 RGB:(255,90,118)

        public void SetInfo(Player player, int rank)
        {
            Color color = gameObject.GetComponent<Image>().color;
            //if self #FF5B76 RGB:(255,90,118), if dead
            if (PhotonNetwork.LocalPlayer.UserId == player.UserId)
            {
                color = highlightColor;
            }
            if ((bool)player.CustomProperties[GameConstants.K_PROP_IS_DEAD])
            {
                color.a = 0.6f;
            }
            gameObject.GetComponent<Image>().color = color;
            Rank.text = rank.ToString();
            Player.text = player.NickName;
            Kills.text = player.CustomProperties[GameConstants.K_PROP_KILLS].ToString();
            DamagePoint.text = player.CustomProperties[GameConstants.K_PROP_DAMAGE_POINTS].ToString();
            CraftNumber.text = player.CustomProperties[GameConstants.K_PROP_CRAFT_NUMBER].ToString();
            Score.text = player.CustomProperties[GameConstants.K_PROP_SCORE].ToString();
            /*
            float killScore = (float)player.CustomProperties[GameConstants.K_PROP_KILLS] / PhotonNetwork.CurrentRoom.PlayerCount * 100;
            float damagePointScore = (float)player.CustomProperties[GameConstants.K_PROP_DAMAGE_POINTS];
            float craftNumberScore = (float)player.CustomProperties[GameConstants.K_PROP_CRAFT_NUMBER];
            float totalScore = (float)(killScore * 0.4 + damagePointScore * 0.4 + craftNumberScore * 0.2);
            Total.text = totalScore.ToString();
            */

        }
    }

}
