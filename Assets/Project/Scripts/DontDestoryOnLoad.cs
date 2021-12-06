using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.SweetsWar
{
    public class DontDestoryOnLoad : MonoBehaviour
    {        
        private void Start()
        {
            GameObject[] objs = GameObject.FindGameObjectsWithTag(tag);

            if (objs.Length > 1)
            {
                Destroy(this.gameObject);
            }

            DontDestroyOnLoad(gameObject);
        }

    }
}
