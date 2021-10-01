using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.SweetsWar
{
    public class ItemManager : MonoBehaviour
    {
        public GameObject[] ItemPrefabList;
        private short m_RandomNumber;

        void Start()
        {
            if (PhotonNetwork.IsConnected && PlayerMovementController.localPlayerInstance == null)
            {
                Debug.Log("initiate items~~~");
                generateItems();
            }

        }


        private void generateItems() {
            //int RandomObjects = Random.Range(0, ItemPrefabList.Length);
            float RandomX;
            float RandomZ;

            foreach(GameObject obj in ItemPrefabList)
            {
                m_RandomNumber = (short)Random.Range(5, 10);
                Debug.LogFormat("Instantiate: {0}, {1}", obj.name, m_RandomNumber);

                for (short j = 0; j < m_RandomNumber; j++)
                {
                    RandomX = Random.Range(2, 55);
                    RandomZ = Random.Range(2, 80);

                    Instantiate(obj, new Vector3(RandomX, 15f, RandomZ), Quaternion.identity);
                }
                
            }
        }
    }
}
    
