using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;

namespace Game.SweetsWar
{
    [RequireComponent(typeof(CharacterController), typeof(AudioSource), typeof(Animator))]
    public class PlayerMovementController : MonoBehaviourPunCallbacks, IPunObservable
    {
        public static PlayerMovementController _instance;
        public static GameObject localPlayerInstance;
        public Camera playerCamera;
        public TextMesh playerName;
        public AudioSource playerAudioSource;

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

        public bool isDead { get; private set; }
        public bool isGrounded { get; private set; }
        public bool isCrouching { get; private set; }
        public bool isFiring;
        public bool stopMove { get; set; }

        // private
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

        private const float k_groundCheckDistance = 0.05f;
        private const float k_JumpGroundingPreventionTime = 0.2f;
        private const float k_GroundCheckDistanceInAir = 0.07f;

        public void Awake()
        {
            // #Important
            // used in GameManager.cs: we keep track of the localPlayer instance to prevent instanciation when levels are synchronized
            if (photonView.IsMine)
            {
                localPlayerInstance = gameObject;
                playerCamera.gameObject.SetActive(true);
            }

            // #Critical
            // giving a seamless experience when levels load.
            DontDestroyOnLoad(gameObject);
        }
        void Start()
        {
            _instance = this;
            m_animator = GetComponent<Animator>();
            m_characterController = GetComponent<CharacterController>();
            m_speedPlayer = speedNormal;
            m_cameraPosition = Camera.main.transform.localPosition; 
            m_cameraCrouchingPosition = new Vector3(m_cameraPosition.x, m_cameraPosition.y / 2, m_cameraPosition.z);
            stopMove = false;

            playerName.text = photonView.Owner.NickName;
            Debug.LogFormat("name: {0}, key: {1}, photonView: {2}",
                PhotonNetwork.NickName, 
                PlayerPrefs.GetString(GameConstants.PLAYER_NAME_PREFAB_KEY), 
                photonView.Owner.NickName
              );

            // the player's name always faces the main camera : use Camera.main to get the main
            //playerName.gameObject.transform.rotation = Camera.main.transform.rotation;
            
        }

        void Update()
        {
            //bool StopAction = (Input.GetKeyDown(KeyCode.Tab) || Input.GetKeyDown(KeyCode.Escape));
            // the player's name always faces the main camera : use Camera.main to get the main
            playerName.gameObject.transform.rotation = Camera.main.transform.rotation;   

            if (photonView.IsMine == false && PhotonNetwork.IsConnected == true)
            {
                return;
            }

            /*
            if (Input.GetKeyDown(KeyCode.Tab) || Input.GetKeyDown(KeyCode.Escape))
            {
                stop = !stop;
            }
            */

            Action();

        }

        private void Action() {

            CheckGrounded();
            //GroundCheck();

            if (stopMove) return;

            /* move */
            m_animator.SetFloat(GameConstants.ANIMATION_SPEED, m_speedPlayer);
            float x = Input.GetAxis(GameConstants.HORIZONTAL);
            float y = Input.GetAxis(GameConstants.VERTICAL);
            if (x == 0 && y == 0)
            {
                m_animator.SetBool(GameConstants.ANIMATION_MOVE, false);
            }
            else
            {
                Vector3 move = transform.right * x + transform.forward * y;
                m_characterController.Move(move * m_speedPlayer * Time.deltaTime);
                m_animator.SetBool(GameConstants.ANIMATION_MOVE, true);
            }    

            /* sprint */
            m_speedPlayer = isGrounded && Input.GetButton(GameConstants.BUTTON_SPRINT) ? speedSprinting : speedNormal;

            /* crouch */
            if (Input.GetButtonDown(GameConstants.BUTTON_CROUCH) && isGrounded)
            {
                isCrouching = !isCrouching;
                m_animator.SetBool(GameConstants.ANIMATION_CHROUCH, isCrouching);
                
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
                m_animator.SetTrigger(GameConstants.ANIMATION_JUMP);
                m_lastTimeJumped = Time.time;

                /*
                // Force grounding to false
                isGrounded = false;
                m_GroundNormal = Vector3.up;
                */
            }

            /* fire */
            if (Input.GetButtonDown(GameConstants.BUTTON_FIRE))
            {
                Debug.Log("BUTTON_FIRE");
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
   

