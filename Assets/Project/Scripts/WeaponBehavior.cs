using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.SweetsWar
{
    public class WeaponBehavior : MonoBehaviour//, IPunObservable
    {
        public Weapon WeaponData;
        public float MaxPickUpDistance = 4f;

        private void OnMouseDown()
        {
            // 點擊即裝備，並丟掉目前裝備
            
            if (BackpackManerger._instance == null)
            {
                Debug.Log("Without BackpackManerger._instance!");
                return;
            }

            float itemDistance = Vector3.Distance(PlayerMovementController.localPlayerInstance.transform.position, transform.position);
            Debug.Log("itemDistance: " + itemDistance);

            if ( itemDistance < MaxPickUpDistance && BackpackManerger._instance.Collect(WeaponData))
            {
                // Destroy this item in scene
                Destroy(this.gameObject);
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

