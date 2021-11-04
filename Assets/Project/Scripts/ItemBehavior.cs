using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.SweetsWar
{
    public class ItemBehavior : MonoBehaviour//, IPunObservable
    {
        public Item item;
        public float MaxPickUpDistance = 4f;

        private void Update()
        {
            //transform.Rotate(new Vector3(0, 1, 0), Space.World);
        }
        /*
        // player touched
        private void OnTriggerEnter(Collider c) 
        {
            if (BackpackManerger._instance == null)
            {
                Debug.Log("Without BackpackManerger._instance!");
                return;
            }

            if(c.gameObject.tag == GameConstants.TAG_PLAYER)
            {
                BackpackManerger backpack = BackpackManerger._instance;
                if (backpack.Collect(item)) {
                    // Destroy the object in scene
                    Destroy(this.gameObject);
                }
            }     
        }
        */

        // on click item (collider)
        private void OnMouseDown()
        {
            if (BackpackManerger._instance == null)
            {
                Debug.Log("Without BackpackManerger._instance!");
                return;
            }

            float itemDistance = Vector3.Distance(PlayerMovementController.localPlayerInstance.transform.position, transform.position);
            Debug.Log("itemDistance: " + itemDistance);

            if ( itemDistance < MaxPickUpDistance && BackpackManerger._instance.Collect(item))
            {
                // Destroy this item in scene
                //Destroy(this.gameObject);
                PhotonNetwork.Destroy(gameObject);
            }

        }

        /*
        #region IPunObservable implementation
        public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
        {
            //throw new System.NotImplementedException();
            OnPhotonSerializeView(stream, info);
        }
        #endregion
        */
    }
}

