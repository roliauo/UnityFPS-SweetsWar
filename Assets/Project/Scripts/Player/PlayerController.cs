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
    public class PlayerController : MonoBehaviourPunCallbacks//, IPunObservable
    {
        public static PlayerController _instance;
        public static GameObject localPlayerInstance;
        public Camera playerCamera;
        public TextMesh playerName;
        public string playerID;

        public float lookSensitivity = 200f;
        public float gravity = -20f;

        [Header("Health")]
        public float maxHealth = 100f;

        [Header("Ground Check")]
        public Transform groundCheckTransform;
        public LayerMask groundLayerMask;

        [Header("Movement")]
        public float speedNormal = 2.5f;
        public float speedMax = 4f;
        public float speedCrouching = 1f;
        public float speedSprinting = 4f;
        public float jumpHeight = 1f;

        [Header("Weapon")]
        public Transform weaponSlot;
        public GameObject[] weaponPosition;

        [Header("Audio")]
        public AudioSource audioSource;
        public AudioClip footstepSFX;
        public AudioClip jumpSFX;
        public AudioClip equipSFX;
        public AudioClip pickupSFX;

        public float health { get; set; }
        public bool isDead { get; private set; }
        public bool isGrounded { get; private set; }
        public bool isCrouching { get; private set; }
        public bool isFiring;
        public bool stopMove { get; set; }

        // private
        private string m_heldWeaponPrefabName = null; //
        private int m_heldWeaponViewID = -1;
        private Weapon m_heldWeapon;

        private CharacterController m_characterController;
        private Animator m_animator;
        private Vector3 m_velocity;  
        private Vector3 m_cameraPosition;
        private Vector3 m_cameraCrouchingPosition;
        private Vector3 m_groundNormal;
        private float m_speedPlayer;
        private float m_cameraHeightRatio = 0.9f;
        private float m_rotationX = 0f;           
        private float m_footstepDistance;
        private GameObject m_weaponPrefab;

        private const float k_groundCheckDistance = 0.05f;
        private const float k_JumpGroundingPreventionTime = 0.2f;
        private const float k_GroundCheckDistanceInAir = 0.07f;


        // constants
        private const string k_ANIMATION_SPEED = "SpeedTest"; //"Speed";
        private const string k_ANIMATION_MOVE = "Move";
        private const string k_ANIMATION_JUMP = "Jump";
        private const string k_ANIMATION_CHROUCH = "Crouch";
        private const string k_ANIMATION_EQUIP = "EquipWeapon";
        private const string k_ANIMATION_DEATH = "Death";
        private const string k_ANIMATION_BEATEN = "Beaten";
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
            health = maxHealth;

            playerName.text = photonView.Owner.NickName;
            playerID = photonView.Owner.UserId;

            InitializePlayerProps();
            
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

            float playerDistance = Vector3.Distance(Camera.main.transform.position, playerName.gameObject.transform.position);
            playerName.gameObject.SetActive(playerDistance < 4);
            

            if (photonView.IsMine == false && PhotonNetwork.IsConnected == true)
            {
                return;
            }

            //playerName.gameObject.transform.rotation = Camera.main.transform.rotation; // #local version          

            Action();

        }

        public override void OnPlayerPropertiesUpdate(Player targetPlayer, ExitGames.Client.Photon.Hashtable changedProps)
        {
            if (!photonView.IsMine && targetPlayer == photonView.Owner)
            {
                if (changedProps.TryGetValue(GameConstants.K_PROP_WEAPON_VIEW_ID, out object id) && (int)id > -1)
                {
                    EquipWeapon((int)changedProps[GameConstants.K_PROP_WEAPON_VIEW_ID]);
                }
                /*
                if (changedProps.TryGetValue(GameConstants.K_PROP_IS_DEAD, out object dead))
                {
                    GameManager.Instance.AllPlayersDataCache.Find(targetPlayer);
                }
                */
                
            }
        }

        public void EquipWeapon(int viewID) //GameObject weaponPrefab
        {

            if ((int)PhotonNetwork.LocalPlayer.CustomProperties[GameConstants.K_PROP_WEAPON_VIEW_ID] > 0)
            {
                // Switch weapon (Drop)
                // BackpackManerger - remove previous weapon 
                GameObject prevWeaponPrefab = PhotonView.Find(m_heldWeaponViewID).gameObject;

                if (photonView.IsMine)
                {
                    BackpackManerger._instance.Subtract(prevWeaponPrefab.GetComponent<WeaponController>().WeaponData);
                    
                }

                /*
                foreach (GameObject go in weapons)
                {
                    if (photonView.IsMine && go.name == m_heldWeaponPrefabName)
                    {
                        BackpackManerger._instance.Subtract(go.GetComponent<WeaponController>().WeaponData);
                        break;
                    }
                }
                */

                // Scene - drop previous weapon
                //Vector3 position = PlayerController.localPlayerInstance.transform.position;
                //GameObject prevWeaponPrefab = PhotonView.Find(m_heldWeaponViewID).gameObject;
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
            m_heldWeaponPrefabName = weaponPrefab.name.Substring(0, weaponPrefab.name.IndexOf("(")); // substring : (clone)          
            m_heldWeaponViewID = viewID;
            weaponPrefab.GetComponent<Rigidbody>().useGravity = false;
            weaponPrefab.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll; //RigidbodyConstraints.FreezePosition;

            // position
            //Transform weaponTransform;
            foreach (GameObject go in weaponPosition)
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
                if (PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue(GameConstants.K_PROP_WEAPON_VIEW_ID, out object id))
                {
                    PhotonNetwork.LocalPlayer.CustomProperties[GameConstants.K_PROP_WEAPON_VIEW_ID] = viewID;
                } else
                {
                    Hashtable hash = new Hashtable();
                    hash.Add(GameConstants.K_PROP_WEAPON_VIEW_ID, viewID);
                    PhotonNetwork.LocalPlayer.SetCustomProperties(hash);
                }
               
            }
        }
 
        [PunRPC] public void RPC_TakeDamageInPlayer(int viewID, float damage)
        {
            //photonView.ViewID: sender's id,  _instance.photonView.ViewID: player's id
            //GameObject player = PhotonView.Find(viewID).gameObject;
            Debug.Log("RPC_TakeDamageInPlayer - hit.collider viewID: " + viewID + 
                " photonView.ViewID: " + photonView.ViewID +  // sender
                " _instance.photonView.ViewID: " + _instance.photonView.ViewID + //
                " LOCAL: " + localPlayerInstance.GetPhotonView().ViewID+ 
                " health:" + health);
            
            // ####Important: use _instance to get this local player. (if no _instance, the data are sender's)
            if (_instance.photonView.ViewID == viewID)
            {
                _instance.m_animator.SetTrigger(k_ANIMATION_BEATEN); //

                // sender add damage points
                //GameManager.Instance.AddScore(photonView.Owner.UserId, GameConstants.K_PROP_DAMAGE_POINTS, damage);
                GameManager.Instance.photonView.RPC("AddScore", RpcTarget.All, photonView.Owner.UserId, GameConstants.K_PROP_DAMAGE_POINTS, damage);


                if (_instance.health <= damage)
                {
                    _instance.health = 0;

                    // sender add kills
                    //GameManager.Instance.AddScore(photonView.Owner.UserId, GameConstants.K_PROP_KILLS, 1f);
                    GameManager.Instance.photonView.RPC("AddScore", RpcTarget.All, photonView.Owner.UserId, GameConstants.K_PROP_KILLS, 1f);
                    
                    // update player state
                    GameManager.Instance.photonView.RPC("UpdatePlayerCacheState", RpcTarget.All, _instance.photonView.Owner.UserId, GameConstants.K_PROP_IS_DEAD, true);

                    Die();                  
                }
                else
                {
                    _instance.health -= damage;
                    //Debug.Log("health: " + _instance.health + " player: " + _instance.playerName.text);
                }
            }         

        }
        

        public void setPropsFloat(string key, float value)
        {
            if (PhotonNetwork.LocalPlayer.CustomProperties.ContainsKey(key))
            {
                PhotonNetwork.LocalPlayer.CustomProperties[key] = value;
            }
            else
            {
                Hashtable hash = new Hashtable();
                hash.Add(key, value);
                PhotonNetwork.LocalPlayer.SetCustomProperties(hash);
            }
        }

        public void Die()
        {
            Debug.Log("Die... ");
            //m_animator.SetTrigger(k_ANIMATION_DEATH);  // can not move
            //m_animator.Play(k_ANIMATION_DEATH);

            _instance.isDead = true;
            PhotonNetwork.LocalPlayer.CustomProperties[GameConstants.K_PROP_IS_DEAD] = true;

            //_instance.stopMove = true;
            //waitAndSee = true;
            // call Score panel or detect by GameManager
            GameManager.Instance.ShowScorePanel();
        }

        #region Private functions

        private void InitializePlayerProps()
        {
            Hashtable hash = new Hashtable();
            hash.Add(GameConstants.K_PROP_WINNER, false);
            hash.Add(GameConstants.K_PROP_IS_DEAD, false);
            hash.Add(GameConstants.K_PROP_HEALTH, maxHealth);
            hash.Add(GameConstants.K_PROP_MAX_HEALTH, maxHealth);
            hash.Add(GameConstants.K_PROP_WEAPON_VIEW_ID, -1); // no weapon: -1
            hash.Add(GameConstants.K_PROP_KILLS, 0f);
            hash.Add(GameConstants.K_PROP_DAMAGE_POINTS, 0f);
            hash.Add(GameConstants.K_PROP_CRAFT_NUMBER, 0f);
            hash.Add(GameConstants.K_PROP_SCORE, 0);

            PhotonNetwork.LocalPlayer.SetCustomProperties(hash);
        }
        private void Attack()
        {
            if (m_heldWeaponViewID < 0)
            {
                Debug.Log("Attack: whithout weapon!");
                return;
            }

            m_weaponPrefab.GetComponent<WeaponController>().Fire();
        }

        private void Action() {

            CheckGrounded();
            //GroundCheck();
            
            if (stopMove) return;
            
            /* move */
            m_animator.SetFloat(k_ANIMATION_SPEED, m_speedPlayer);
            float x = Input.GetAxis(GameConstants.HORIZONTAL);
            float y = Input.GetAxis(GameConstants.VERTICAL);
            Debug.Log("isGrounded: " + isGrounded +" " + x + ", "+y);

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
                //m_lastTimeJumped = Time.time;
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
            //isGrounded = GetComponent<CharacterController>().isGrounded;
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

        #endregion

        /*
        // TODO: need to refactor: customProps -> OnPhotonSerializeView
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
        */

        #region will be removed

        #endregion
    }
}
   

