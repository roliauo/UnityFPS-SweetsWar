using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.SweetsWar
{
    public class Bullet : MonoBehaviour
    {
        private void OnTriggerEnter(Collider other)
        {
            Destroy(gameObject);

            
            if (other.tag == GameConstants.TAG_PLAYER) {
                Debug.Log("hit Player!");
            }
            
        }
    }
}