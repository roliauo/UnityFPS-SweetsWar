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
            if (PhotonNetwork.LocalPlayer.IsLocal == false || BackpackManerger._instance == null)
            {
                return;
            }

            float itemDistance = Vector3.Distance(PlayerController.localPlayerInstance.transform.position, transform.position);
            Debug.Log("itemDistance: " + itemDistance);

            if ( itemDistance < MaxPickUpDistance && BackpackManerger._instance.Collect(WeaponData))
            {
                PlayerController._instance.EquipWeapon(gameObject);
                //photonView.RPC("RPC_ForceMasterEquipWeapon", RpcTarget.MasterClient, photonView.ViewID);
                
                //PlayerController._instance.photonView.RPC("RPC_EquipWeapon", RpcTarget.All, WeaponData.ID); //PhotonNetwork.LocalPlayer.UserId


                //gameObject.transform.parent = PlayerController._instance.WeaponSlot;
                //PlayerController._instance.EquipWeapon(gameObject);

                //photonView.RPC("RPC_ForceMasterEquipWeapon", RpcTarget.MasterClient, photonView.ViewID);
            }
            
        }

        [PunRPC] void RPC_ForceMasterEquipWeapon(int viewID)
        {
            GameObject weaponPrefab = PhotonView.Find(viewID).gameObject;
            PlayerController._instance.EquipWeapon(weaponPrefab);
            //PhotonNetwork.Destroy(weaponPrefab);
            //PlayerController._instance.EquipWeapon(WeaponData.ID);

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

