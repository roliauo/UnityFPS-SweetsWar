using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.SweetsWar
{
    [RequireComponent(typeof(AudioSource))]
    public class WeaponController : MonoBehaviourPunCallbacks//, IPunObservable
    {
        public Weapon WeaponData;
        public float MaxPickUpDistance = 3f;
        public bool isInUse = false;

        AudioSource m_ShootAudioSource;
        private Animator m_animator;
        

        void Awake()
        {
            m_ShootAudioSource = GetComponent<AudioSource>();
            m_animator = GetComponent<Animator>();
        }
        void OnFire()
        {
            if (WeaponData.fireSFX) // && !ContinuousShootSound
            {
                m_ShootAudioSource.PlayOneShot(WeaponData.fireSFX);
            }

            if (m_animator && WeaponData.k_AnimationName != null)
            {
                m_animator.SetTrigger(WeaponData.k_AnimationName);
            }
        }
        private void OnMouseDown()
        {
            // 點擊即裝備，並丟掉目前裝備
            if (isInUse || PhotonNetwork.LocalPlayer.IsLocal == false || BackpackManerger._instance == null)
            {
                return;
            }

            float itemDistance = Vector3.Distance(PlayerController.localPlayerInstance.transform.position, transform.position);
            //Debug.Log("itemDistance: " + itemDistance);

            if ( itemDistance < MaxPickUpDistance && BackpackManerger._instance.Collect(WeaponData)) 
            {
                // TODO: (GUI) set Weapon Slot
                PlayerController._instance.EquipWeapon(photonView.ViewID);
                //PlayerController._instance.EquipWeapon_SetActive(name.Substring(0, name.IndexOf("("))); // prefab name: replace "(Clone)"
                //photonView.RPC("RPC_ForceMasterDestoryWeapon", RpcTarget.MasterClient, photonView.ViewID);
            }
            
        }

        [PunRPC] void RPC_ForceMasterDestoryWeapon(int viewID)
        {
            GameObject weaponPrefab = PhotonView.Find(viewID).gameObject;
            //PlayerController._instance.EquipWeapon(weaponPrefab);
            PhotonNetwork.Destroy(weaponPrefab);
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

