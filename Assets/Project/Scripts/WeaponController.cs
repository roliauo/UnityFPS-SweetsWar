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
        public ParticleSystem ps;

        [Header("Bullet")]
        public GameObject BulletPrefab;
        public GameObject FirePoint;
        public float BulletSpeed;
        public bool isInUse { get; set; } //= false 

        private AudioSource m_audioSource;
        private Animator m_animator;


        void Awake()
        {
            m_audioSource = GetComponent<AudioSource>();
            m_animator = GetComponent<Animator>();
            isInUse = false;
        }

        [PunRPC] public void Fire()
        {
            /*if (BulletPrefab)
            {
                GameObject bullet = Instantiate(BulletPrefab, FirePoint.transform.position, FirePoint.transform.rotation);
                Rigidbody rig = bullet.GetComponent<Rigidbody>();
                //bullet.velocity = transform.TransformDirection(new Vector3(0, 0, BulletSpeed));
                //bullet.position += bullet.velocity;
                rig.AddForce(transform.forward * BulletSpeed);

                //GameObject bullet = Instantiate(BulletPrefab, FirePoint.transform.position, transform.rotation);
                //bullet.GetComponent<Rigidbody>().velocity = transform.TransformDirection(new Vector3(0, 0, BulletSpeed));

                //if (bullet.co)

            } else
            {*/
                Ray ray = new Ray(Camera.main.transform.position, Camera.main.transform.forward);
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit, WeaponData.AttackRange))
                {

                    Debug.Log("hit: " + hit.collider.name);
                    if (hit.collider.tag != GameConstants.TAG_ITEM)
                    {
                        if (WeaponData.AttackSFX)
                        {
                            //m_audioSource.volume = 0.5f;
                            m_audioSource.PlayOneShot(WeaponData.AttackSFX);
                        }

                        if (m_animator && WeaponData.hasFireAnimation)
                        {
                            m_animator.SetTrigger("Fire");
                            //m_animator.Play("Fire");
                        }

                        if (ps) ps.Play();
                    }

                    if (hit.collider.tag == GameConstants.TAG_PLAYER)
                    {
                        //TODO: hit position
                        int viewID = hit.collider.gameObject.GetComponent<PhotonView>().ViewID;
                        Debug.Log("hit.collider health: " + hit.collider.gameObject.GetComponent<PlayerController>().health);

                        
                         // play animation
                        hit.collider.gameObject.GetComponent<Animator>().SetTrigger("Beaten");

                        /*
                         //FAIL
                        if (WeaponData.Damage >= hit.collider.gameObject.GetComponent<PhotonView>().v)
                        {
                            hit.collider.gameObject.GetComponent<Animator>().SetBool("Death", true);
                        }
                        */

                        PlayerController._instance.photonView.RPC("RPC_TakeDamageInPlayer", RpcTarget.AllViaServer, viewID, WeaponData.Damage);

                    }
                }
            //}
           
    
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
            PhotonNetwork.Destroy(weaponPrefab);
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

