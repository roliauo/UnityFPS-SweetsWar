using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace Game.SweetsWar
{
    public class ScorePanelManager : MonoBehaviour
    {
        public static ScorePanelManager _instance;
        public GameObject Content;
        public GameObject PlayerScorePrefab;
        public Button Button_LeaveRoom;

        [Header("Calculate Score")]
        public float killScoreRate = 0.4f;
        public float damageScoreRate = 0.3f;
        public float craftScoreRate = 0.3f;
        public int craftFullCreditNumber = 5;

        private void Start()
        {
            _instance = this;
            Button_LeaveRoom.onClick.AddListener(() =>
            {
                Debug.Log("leave room....");
                PhotonNetwork.LeaveRoom();
            });
        }

        public void ClearView()
        {
            foreach (Transform child in Content.transform)
            {
                if (child.gameObject.TryGetComponent(out PlayerScoreBar sb))
                {
                    Destroy(child.gameObject);
                }
            }
        }

        public void UpdateScoreView()
        {
            Debug.Log("UpdateScoreView");
            ClearView();
            List<Player> players = GameManager.Instance.AllPlayersDataCache; //PhotonNetwork.PlayerList; //
            foreach (Player p in players)
            {
                float killScore = (float)p.CustomProperties[GameConstants.K_PROP_KILLS] / (players.Count - 1) * 100;
                float damagePointScore = (float)p.CustomProperties[GameConstants.K_PROP_DAMAGE_POINTS] / (GameManager.Instance.PlayerMaxHealthSum - (float)p.CustomProperties[GameConstants.K_PROP_MAX_HEALTH]) * 100;
                float craftNumberScore = (float)p.CustomProperties[GameConstants.K_PROP_CRAFT_NUMBER] / craftFullCreditNumber * 100; 
                float totalScore = (float)(killScore * killScoreRate + damagePointScore * damageScoreRate + craftNumberScore * craftScoreRate);

                p.CustomProperties[GameConstants.K_PROP_SCORE] = Math.Round(totalScore, 2, MidpointRounding.AwayFromZero);

            }

            // score rank
            //var listScoreDesc = players.OrderByDescending(p => p.CustomProperties[GameConstants.K_PROP_SCORE]).ToList();

            // live > score
            var query = from p in players
                       orderby p.CustomProperties[GameConstants.K_PROP_IS_DEAD], p.CustomProperties[GameConstants.K_PROP_SCORE] descending
                       select p;

            List<Player> list = query.ToList();
          
            for (int i = 0; i < list.Count; i++)
                {
                    GameObject item = Instantiate(PlayerScorePrefab);
                    item.transform.SetParent(Content.transform);
                    item.transform.localScale = Vector3.one;
                    item.GetComponent<PlayerScoreBar>().SetInfo(list[i], i+1);
                }

            // debug
            foreach (Player p in players)
            {
                Debug.LogFormat("{0} :  kills: {1}, damage: {2}, crafts: {3}, total: {4}, isDead: {5}  ",
                    p.NickName,
                    p.CustomProperties[GameConstants.K_PROP_KILLS],
                    p.CustomProperties[GameConstants.K_PROP_DAMAGE_POINTS],
                    p.CustomProperties[GameConstants.K_PROP_CRAFT_NUMBER],
                    p.CustomProperties[GameConstants.K_PROP_SCORE],
                    p.CustomProperties[GameConstants.K_PROP_IS_DEAD]
                    );
            }

        }
    }
}
