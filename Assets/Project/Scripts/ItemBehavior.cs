using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.SweetsWar
{
    public class ItemBehavior : MonoBehaviourPunCallbacks//, IPunObservable
    {
        public Item item;
        public float MaxPickUpDistance = 3.5f;

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

        private void OnMouseDown()
        {
            //way1
            if (BackpackManerger._instance == null)
            {
                return;
            }

            float itemDistance = Vector3.Distance(PlayerController.localPlayerInstance.transform.position, transform.position);

            if (itemDistance < MaxPickUpDistance && BackpackManerger._instance.Collect(item))
            {
                PlayerController._instance.audioSource.PlayOneShot(PlayerController._instance.pickupSFX);
                // Destroy this item in scene
                photonView.RPC("RPC_ForceMasterClientDestroy", RpcTarget.MasterClient, photonView.ViewID);
            }

            /*
            // way2
            if (BackpackManerger._instance == null)
            {
                return;
            }

            float itemDistance = Vector3.Distance(PlayerController.localPlayerInstance.transform.position, transform.position);

            if (itemDistance < MaxPickUpDistance && BackpackManerger._instance.Collect(item))
            {
                // Destroy this item in scene
                Destroy(this.gameObject);
            }
            */
        }
        [PunRPC] void RPC_ForceMasterClientDestroy(int viewID)
        {
            PhotonNetwork.Destroy(PhotonView.Find(viewID).gameObject);
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

