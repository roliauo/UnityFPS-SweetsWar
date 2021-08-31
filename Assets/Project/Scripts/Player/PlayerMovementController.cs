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
        public float speedNormal = 2f;
        public float speedMax = 8f;
        public float speedCrouching = 1f;
        public float speedSprinting = 4f;
        public float jumpHeight = 1f;

        public bool isDead { get; private set; }
        public bool isGrounded { get; private set; }
        public bool isCrouching { get; private set; }
        public bool isFiring;

        // private
        private CharacterController m_characterController;
        private Animator m_animator;
        private Vector3 m_velocity;  
        private Vector3 m_cameraPosition;
        private Vector3 m_cameraCrouchingPosition;
        private float m_speedPlayer;
        private float m_cameraHeightRatio = 0.9f;
        private float m_rotationX = 0f;
        private float groundCheckDistance = 0.05f;

        

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
            m_animator = GetComponent<Animator>();
            m_characterController = GetComponent<CharacterController>();
            m_speedPlayer = speedNormal;
            m_cameraPosition = Camera.main.transform.localPosition; 
            m_cameraCrouchingPosition = new Vector3(m_cameraPosition.x, m_cameraPosition.y / 2, m_cameraPosition.z);
     

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
            // the player's name always faces the main camera : use Camera.main to get the main
            playerName.gameObject.transform.rotation = Camera.main.transform.rotation;   

            if (photonView.IsMine == false && PhotonNetwork.IsConnected == true)
            {
                return;
            }

            Action();

        }

        private void Action() {

            CheckGrounded();

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
        }

        private void CheckGrounded()
        {
            isGrounded = Physics.CheckSphere(groundCheckTransform.position, groundCheckDistance, groundLayerMask);

            if (!isGrounded)
            {
                Debug.Log("m_characterController.y: " + m_characterController.transform.localPosition.y);
            }
            else
            {
                //m_playerPositionY = m_characterController.transform.localPosition.y
                Debug.Log("isGrounded: " + m_characterController.transform.localPosition.y);
            }

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
   

