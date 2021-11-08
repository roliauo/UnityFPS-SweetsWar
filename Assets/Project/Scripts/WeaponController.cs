using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.SweetsWar
{
    public class WeaponController : MonoBehaviourPunCallbacks//, IPunObservable
    {
        public Weapon WeaponData;
        public float MaxPickUpDistance = 4f;

        private void OnMouseDown()
        {
            // 點擊即裝備，並丟掉目前裝備
            if (photonView.IsMine == false || BackpackManerger._instance == null)
            {
                return;
            }

            float itemDistance = Vector3.Distance(PlayerController.localPlayerInstance.transform.position, transform.position);
            Debug.Log("itemDistance: " + itemDistance);

            if ( itemDistance < MaxPickUpDistance && BackpackManerger._instance.Collect(WeaponData))
            {
                //gameObject.transform.parent = PlayerController._instance.WeaponSlot;
                //PlayerController._instance.EquipWeapon(gameObject);
                photonView.RPC("EquipWeapon", RpcTarget.All, gameObject);
                // Destroy this item in scene
                Destroy(this.gameObject);
            }
            
        }

        [PunRPC]
        private void EquipWeapon(GameObject weaponPrefab)
        {
            PlayerController._instance.EquipWeapon(weaponPrefab);
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

