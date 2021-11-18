using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using Hashtable = ExitGames.Client.Photon.Hashtable;

namespace Game.SweetsWar
{
    [RequireComponent(typeof(CharacterController), typeof(AudioSource), typeof(Animator))]
    public class PlayerController : MonoBehaviourPunCallbacks, IPunObservable
    {
        public static PlayerController _instance;
        public static GameObject localPlayerInstance;
        public Camera playerCamera;
        public TextMesh playerName;       

        public float lookSensitivity = 200f;
        public float gravity = -20f;

        [Header("Health")]
        public float health = 100f;

        [Header("Ground Check")]
        public Transform groundCheckTransform;
        public LayerMask groundLayerMask;

        [Header("Movement")]
        public float speedNormal = 1f;
        public float speedMax = 4f;
        public float speedCrouching = 0.5f;
        public float speedSprinting = 3f;
        public float jumpHeight = 1f;

        [Header("Weapon")]
        public Transform weaponSlot;
        public GameObject[] weapons;

        [Header("Audio")]
        public AudioSource audioSource;
        public AudioClip footstepSFX;
        public AudioClip jumpSFX;
        public AudioClip equipSFX;
        public AudioClip pickupSFX;

        public bool isDead { get; private set; }
        public bool isGrounded { get; private set; }
        public bool isCrouching { get; private set; }
        public bool isFiring;
        public bool stopMove { get; set; }

        // private
        private string m_heldWeaponPrefabName = null;
        private int m_heldWeaponViewID = -1;
        private Weapon m_heldWeapon;
        //private short m_heldWeaponID = -1;
        private CharacterController m_characterController;
        private Animator m_animator;
        private Vector3 m_velocity;  
        private Vector3 m_cameraPosition;
        private Vector3 m_cameraCrouchingPosition;
        private Vector3 m_groundNormal;
        private float m_speedPlayer;
        private float m_cameraHeightRatio = 0.9f;
        private float m_rotationX = 0f;           
        private float m_lastTimeJumped = 0f;
        private float m_footstepDistance;
        private GameObject m_weaponPrefab;

        private const float k_groundCheckDistance = 0.05f;
        private const float k_JumpGroundingPreventionTime = 0.2f;
        private const float k_GroundCheckDistanceInAir = 0.07f;
        private const string k_weaponViewID = "weaponViewID";

        // Animation
        private const string k_ANIMATION_SPEED = "SpeedTest"; //"Speed";
        private const string k_ANIMATION_MOVE = "Move";
        private const string k_ANIMATION_JUMP = "Jump";
        private const string k_ANIMATION_CHROUCH = "Crouch";
        private const string k_ANIMATION_EQUIP = "EquipWeapon";
        private const string k_TAKE_DAMAGE = "TakeDamage";

        public void Awake()
        {
            // #Important
            // used in GameManager.cs: we keep track of the localPlayer instance to prevent instanciation when levels are synchronized
            if (photonView.IsMine)
            {
                _instance = this;
                localPlayerInstance = gameObject;
                playerCamera.gameObject.SetActive(true);
            }

            // #Critical
            // giving a seamless experience when levels load.
            DontDestroyOnLoad(gameObject);
        }
        void Start()
        {         
            m_animator = GetComponent<Animator>();
            m_characterController = GetComponent<CharacterController>();
            m_speedPlayer = speedNormal;
            m_cameraPosition = Camera.main.transform.localPosition; 
            m_cameraCrouchingPosition = new Vector3(m_cameraPosition.x, m_cameraPosition.y / 2, m_cameraPosition.z);
            stopMove = false;

            playerName.text = photonView.Owner.NickName;

            /*
            Debug.LogFormat("name: {0}, key: {1}, photonView: {2}",
                PhotonNetwork.LocalPlayer.NickName, 
                PlayerPrefs.GetString(GameConstants.PLAYER_NAME_PREFAB_KEY), 
                photonView.Owner.NickName
              );
            */
        }

        void Update()
        {
            // the player's name always faces the main camera : use Camera.main to get the main
            playerName.gameObject.transform.rotation = Camera.main.transform.rotation; // #photon version: Before photon.IsMine

            if (photonView.IsMine == false && PhotonNetwork.IsConnected == true)
            {
                return;
            }

            //playerName.gameObject.transform.rotation = Camera.main.transform.rotation; // #local version


            /*
            if (Input.GetKeyDown(KeyCode.Tab) || Input.GetKeyDown(KeyCode.Escape))
            {
                stop = !stop;
            }
            */

            Action();

        }
      
        public void EquipWeapon(int viewID) //GameObject weaponPrefab
        {
            if (m_heldWeaponPrefabName != null)
            {
                // Switch weapon (Drop)
                // BackpackManerger - remove previous weapon 
                foreach (GameObject go in weapons)
                {
                    if (photonView.IsMine && go.name == m_heldWeaponPrefabName)
                    {
                        BackpackManerger._instance.Subtract(go.GetComponent<WeaponController>().WeaponData);
                        break;
                    }
                }
                // Scene - drop previous weapon
                //Vector3 position = PlayerController.localPlayerInstance.transform.position;
                GameObject prevWeaponPrefab = PhotonView.Find(m_heldWeaponViewID).gameObject;
                prevWeaponPrefab.GetComponent<Rigidbody>().useGravity = true;
                prevWeaponPrefab.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
                prevWeaponPrefab.transform.parent = null;
                prevWeaponPrefab.GetComponent<WeaponController>().isInUse = false;
            }
            
            // setup
            GameObject weaponPrefab = PhotonView.Find(viewID).gameObject;
            weaponPrefab.GetComponent<WeaponController>().isInUse = true;
            m_weaponPrefab = weaponPrefab;
            m_heldWeapon = weaponPrefab.GetComponent<WeaponController>().WeaponData;
            m_heldWeaponPrefabName = weaponPrefab.name.Substring(0, weaponPrefab.name.IndexOf("("));           
            m_heldWeaponViewID = viewID;
            weaponPrefab.GetComponent<Rigidbody>().useGravity = false;
            weaponPrefab.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll; //RigidbodyConstraints.FreezePosition;

            // position
            //Transform weaponTransform;
            foreach (GameObject go in weapons)
            {
                if (go.name == m_heldWeaponPrefabName)
                {
                    //weaponTransform = go.transform;
                    weaponPrefab.transform.position = go.transform.position;
                    weaponPrefab.transform.rotation = go.transform.rotation;
                    weaponPrefab.transform.parent = gameObject.transform; // outside
                    weaponPrefab.transform.parent = weaponSlot; // inside

                    //m_animator.Play(GameConstants.ANIMATION_EQUIP);
                    m_animator.SetBool(k_ANIMATION_EQUIP, true);
                    audioSource.PlayOneShot(equipSFX);

                    break;
                }
            }
            
            if (photonView.IsMine)
            {
                Hashtable hash = new Hashtable();
                hash.Add("weaponViewID", viewID);
                PhotonNetwork.LocalPlayer.SetCustomProperties(hash);
            }
        }

        /*
        public void EquipWeapon_SetActive(short weaponID)
        {
            if (m_heldWeaponID != -1)
            {
                // has weapon -> drop
                Vector3 position = PlayerController.localPlayerInstance.transform.position;
                //GameManager.Instance.photonView.RPC("RPC_CraftForMasterClient", RpcTarget.MasterClient, outputItem.name, position.x, position.y, position.z);
            }
            m_heldWeaponID = weaponID;
            foreach (GameObject go in weapons)
            {
                go.SetActive(false); // switch weapons
                if (go.GetComponent<WeaponController>().WeaponData.ID == weaponID)
                {
                    go.SetActive(true);
                }
            }

            //m_animator.Play(GameConstants.ANIMATION_EQUIP);
            m_animator.SetBool(GameConstants.ANIMATION_EQUIP, true);
            audioSource.PlayOneShot(equipSFX);

            if (photonView.IsMine)
            {
                Hashtable hash = new Hashtable();
                hash.Add("weaponID", m_heldWeaponID);
                PhotonNetwork.LocalPlayer.SetCustomProperties(hash);
            }
        }
        */

        public void EquipWeapon_SetActive(string weaponPrefabName)
        {
            if (photonView.IsMine && m_heldWeaponPrefabName != null)
            {
                // switch weapon (drop previous weapon)
                Vector3 position = PlayerController.localPlayerInstance.transform.position;
                GameManager.Instance.photonView.RPC("RPC_CraftForMasterClient", RpcTarget.MasterClient, m_heldWeaponPrefabName, position.x, position.y + 3f, position.z);
            }
            m_heldWeaponPrefabName = weaponPrefabName;
            //WeaponData data;
            foreach (GameObject go in weapons)
            {
                // switch weapons
                go.SetActive(go.name == weaponPrefabName);
                //Debug.Log("go.name: " + go.name + " prefab: " + weaponPrefabName);

                if (photonView.IsMine && go.name == weaponPrefabName)
                {
                    BackpackManerger._instance.Subtract(go.GetComponent<WeaponController>().WeaponData);
                    break;
                }
                
            }

            m_animator.SetBool(k_ANIMATION_EQUIP, true);
            audioSource.PlayOneShot(equipSFX);
          
            if (photonView.IsMine)
            {
                Hashtable hash = new Hashtable();
                hash.Add("weaponPrefabName", weaponPrefabName);
                PhotonNetwork.LocalPlayer.SetCustomProperties(hash);
            }
        }

        public void TakeDamage(float h)
        {
            health -= h;
            Debug.Log("health: " + health);
            //m_animator.SetTrigger(k_TAKE_DAMAGE);

            if (health <= 0)
            {
                Die();
            }
        }

        void Die()
        {
            // play animation
            GameManager.Instance.showEndPanel();
        }

        public override void OnPlayerPropertiesUpdate(Player targetPlayer, ExitGames.Client.Photon.Hashtable changedProps)
        {
            if(!photonView.IsMine && targetPlayer == photonView.Owner)
            {
                if (changedProps.TryGetValue(k_weaponViewID, out object id)) EquipWeapon((int)changedProps["weaponViewID"]);
                //EquipWeapon_SetActive((string)changedProps["weaponPrefabName"]); //m_heldWeaponID
            }
        }

        private void Attack()
        {
            if (m_heldWeaponViewID < 0)
            {
                Debug.Log("Attack: whithout weapon!");
                return;
            }

            m_weaponPrefab.GetComponent<WeaponController>().Fire();

            /*
            Ray ray = new Ray(Camera.main.transform.position, Camera.main.transform.forward);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit) && hit.distance <= m_heldWeapon.AttackRange)
            {
                Debug.Log("hit: " + hit.collider.name);
                //audioSource.PlayOneShot(m_heldWeapon.AttackSFX);
                m_weaponPrefab.GetComponent<WeaponController>().Fire();
                if (hit.collider.tag == GameConstants.TAG_PLAYER)
                {
                    hit.transform.GetComponent<PlayerController>().TakeDamage(m_heldWeapon.Damage);
                    
                }
            }
            */
        }

        private void Action() {

            CheckGrounded();
            //GroundCheck();

            if (stopMove) return;

            /* move */
            m_animator.SetFloat(k_ANIMATION_SPEED, m_speedPlayer);
            float x = Input.GetAxis(GameConstants.HORIZONTAL);
            float y = Input.GetAxis(GameConstants.VERTICAL);
            if (x == 0 && y == 0)
            {
                m_animator.SetBool(k_ANIMATION_MOVE, false);
            }
            else
            {
                Vector3 move = transform.right * x + transform.forward * y;
                m_characterController.Move(move * m_speedPlayer * Time.deltaTime);
                m_animator.SetBool(k_ANIMATION_MOVE, true);

                // sound                
                if (m_footstepDistance >= 1f)
                {
                    m_footstepDistance = 0f;
                    audioSource.PlayOneShot(footstepSFX);
                }

                m_footstepDistance += m_velocity.magnitude * Time.deltaTime;
            }    

            /* sprint */
            m_speedPlayer = isGrounded && Input.GetButton(GameConstants.BUTTON_SPRINT) ? speedSprinting : speedNormal;

            /* crouch */
            if (Input.GetButtonDown(GameConstants.BUTTON_CROUCH) && isGrounded)
            {
                isCrouching = !isCrouching;
                m_animator.SetBool(k_ANIMATION_CHROUCH, isCrouching);
                
                // #TODO: NEED TO SET TIMEOUT
                
                m_speedPlayer = isCrouching ? speedCrouching : speedNormal;

                // change player's height
                groundCheckTransform.position += isCrouching ? new Vector3(0, 0.1f, 0) : new Vector3(0, -0.1f, 0);
                m_characterController.height = isCrouching ? 1.2f : 1.5f;
                //m_characterController.center = isCrouching ? new Vector3(0, -0.2f, 0) : new Vector3(0, 0, 0);
                //playerCamera.transform.localPosition = isCrouching ? m_cameraCrouchingPosition : m_cameraPosition;

                

            }

            /* jump */
            if (Input.GetButtonDown(GameConstants.BUTTON_JUMP) && isGrounded)
            {
                m_velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
                m_animator.SetTrigger(k_ANIMATION_JUMP);
                m_lastTimeJumped = Time.time;
                audioSource.PlayOneShot(jumpSFX);

                /*
                // Force grounding to false
                isGrounded = false;
                m_GroundNormal = Vector3.up;
                */
            }

            /* fire */
            if (Input.GetButtonDown(GameConstants.BUTTON_FIRE))
            {
                //Debug.Log("BUTTON_FIRE");
                Attack();
            }

            /* look */
            float mouseX = Input.GetAxis(GameConstants.MOUSE_X) * lookSensitivity * Time.deltaTime;
            float mouseY = Input.GetAxis(GameConstants.MOUSE_Y) * lookSensitivity * Time.deltaTime;
            m_rotationX -= mouseY;
            m_rotationX = Mathf.Clamp(m_rotationX, -90f, 90f); // limit -90~90

            playerCamera.transform.localRotation = Quaternion.Euler(m_rotationX, 0f, 0f);
            transform.Rotate(Vector3.up * mouseX);

            
        }

        private void CheckGrounded()
        {
            isGrounded = Physics.CheckSphere(groundCheckTransform.position, k_groundCheckDistance, groundLayerMask);

            /*
            if (!isGrounded)
            {
                Debug.Log("m_characterController.y: " + m_characterController.transform.localPosition.y);
            }
            else
            {
                //m_playerPositionY = m_characterController.transform.localPosition.y
                Debug.Log("isGrounded: " + m_characterController.transform.localPosition.y);
            }
            */

            if (isGrounded && m_velocity.y < 0)
            {
                m_velocity.y = -2f;
            }
            m_velocity.y += gravity * Time.deltaTime;
            m_characterController.Move(m_velocity * Time.deltaTime);

            /*
            if (m_characterController.transform.localPosition.y > 3) //TEST
            {
                m_velocity.y += gravity * Time.deltaTime;
                m_characterController.Move(m_velocity * Time.deltaTime);
            }
            */
            
        }

        
        private void GroundCheck()
        {
            // Make sure that the ground check distance while already in air is very small, to prevent suddenly snapping to ground
            float chosenGroundCheckDistance = isGrounded ? (m_characterController.skinWidth + k_groundCheckDistance) : k_GroundCheckDistanceInAir;

            // reset values before the ground check
            isGrounded = false;
            m_groundNormal = Vector3.up;

            // only try to detect ground if it's been a short amount of time since last jump; otherwise we may snap to the ground instantly after we try jumping
            if (Time.time >= m_lastTimeJumped + k_JumpGroundingPreventionTime)
            {
                // if we're grounded, collect info about the ground normal with a downward capsule cast representing our character capsule
                if (Physics.CapsuleCast(GetCapsuleBottomHemisphere(), GetCapsuleTopHemisphere(m_characterController.height), m_characterController.radius, Vector3.down, out RaycastHit hit, chosenGroundCheckDistance, groundLayerMask, QueryTriggerInteraction.Ignore))
                {
                    // storing the upward direction for the surface found
                    m_groundNormal = hit.normal;

                    // Only consider this a valid ground hit if the ground normal goes in the same direction as the character up
                    // and if the slope angle is lower than the character controller's limit
                    if (Vector3.Dot(hit.normal, transform.up) > 0f &&
                        IsNormalUnderSlopeLimit(m_groundNormal))
                    {
                        isGrounded = true;

                        // handle snapping to the ground
                        if (hit.distance > m_characterController.skinWidth)
                        {
                            m_characterController.Move(Vector3.down * hit.distance);
                        }
                    }
                }
            }
        }

        private Vector3 GetCapsuleBottomHemisphere()
        {
            // Gets the center point of the bottom hemisphere of the character controller capsule    
            return transform.position + (transform.up * m_characterController.radius);
        }

        
        private Vector3 GetCapsuleTopHemisphere(float atHeight)
        {
            // Gets the center point of the top hemisphere of the character controller capsule    
            return transform.position + (transform.up * (atHeight - m_characterController.radius));
        }

        
        bool IsNormalUnderSlopeLimit(Vector3 normal)
        {
            // Returns true if the slope angle represented by the given normal is under the slope angle limit of the character controller
            return Vector3.Angle(transform.up, normal) <= m_characterController.slopeLimit;
        }


        #region IPunObservable implementation
        public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
        {
            if (stream.IsWriting)
            {
                // We own this player: send the others our data
                stream.SendNext(isFiring);
                stream.SendNext(health);
            }
            else
            {
                // Network player, receive data
                isFiring = (bool)stream.ReceiveNext();
                health = (float)stream.ReceiveNext();
            }
        }
        #endregion
    }
}
   

