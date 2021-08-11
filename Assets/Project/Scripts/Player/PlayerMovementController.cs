using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Pun.Demo.PunBasics;

namespace Game.SweetsWar
{
    [RequireComponent(typeof(CharacterController), typeof(AudioSource))]
    public class PlayerMovementController : MonoBehaviourPunCallbacks, IPunObservable
    {
        public static GameObject localPlayerInstance;
        public Camera playerCamera;
        public AudioSource playerAudioSource;

        public float lookSensitivity = 200f;
        public float gravity = -20f;

        [Header("Health")]
        public float health = 100f;

        [Header("Ground Check")]
        public Transform groundCheckTransform;
        public LayerMask groundLayerMask;
        public float groundDistance = 0.4f;


        [Header("Movement")]
        public float speedNormal = 4f;
        public float speedMax = 8f;
        public float speedCrouching = 2.5f;
        public float speedSprinting = 6f;
        public float jumpHeight = 1f;

        [Header("Stance")]
        public float heightStanding = 1.5f;
        public float heightCrouching = 1f;

        public bool isDead { get; private set; }
        public bool isGrounded { get; private set; }
        public bool isCrouching { get; private set; }
        public bool isFiring;

        // private
        private CharacterController m_characterController;
        private Vector3 m_velocity;
        private float m_cameraHeightRatio = 0.9f;
        private float m_rotationX = 0f;
        private float m_heightPlayer;
        private float m_speedPlayer;
        private Animator m_animator;
     

        public void Awake()
        {
            /*
            if (this.beams == null)
            {
                Debug.LogError("<Color=Red><b>Missing</b></Color> Beams Reference.", this);
            }
            else
            {
                this.beams.SetActive(false);
            }
            */

            // #Important
            // used in GameManager.cs: we keep track of the localPlayer instance to prevent instanciation when levels are synchronized
            if (photonView.IsMine)
            {
                localPlayerInstance = gameObject;
                playerCamera.gameObject.SetActive(true);
            }

            // #Critical
            // we flag as don't destroy on load so that instance survives level synchronization, thus giving a seamless experience when levels load.
            DontDestroyOnLoad(gameObject);
        }
        void Start()
        {
            //Cursor.lockState = CursorLockMode.Locked;
            m_animator = GetComponent<Animator>();
            m_characterController = GetComponent<CharacterController>();
            m_speedPlayer = speedNormal;
            m_heightPlayer = heightStanding;

            /*
            CameraWork _cameraWork = gameObject.GetComponent<CameraWork>();

            if (_cameraWork != null)
            {
                if (photonView.IsMine)
                {
                    _cameraWork.OnStartFollowing();
                }
            }
            else
            {
                Debug.LogError("<Color=Red><b>Missing</b></Color> CameraWork Component on player Prefab.", this);
            }
            */

        }

        void Update()
        {
            if (photonView.IsMine == false && PhotonNetwork.IsConnected == true)
            {
                return;
            }

            checkGrounded();

            /* sprint */
            m_speedPlayer = isGrounded && Input.GetButton(GameConstants.BUTTON_SPRINT) ? speedSprinting : speedNormal;

            /* crouch */
            if (Input.GetButtonDown(GameConstants.BUTTON_CROUCH) && isGrounded)
            {
                isCrouching = !isCrouching;
                m_heightPlayer = isCrouching ? heightCrouching : heightStanding;
                m_speedPlayer = isCrouching ? speedCrouching : speedNormal;
                m_animator.SetBool(GameConstants.ANIMATION_CHROUCH, isCrouching);

                // change player's height
                //transform.localScale = new Vector3(1, m_heightPlayer, 1);
                transform.localPosition = new Vector3(transform.localPosition.x, m_heightPlayer, transform.localPosition.z);

                /*if (m_characterController.height != m_heightPlayer)
                {
                    // resize player and adjust camera position
                    m_characterController.height = m_heightPlayer;
                    m_characterController.center = isCrouching ? new Vector3(0, 0.5f, 0) : new Vector3(0, 0, 0);
                    //groundCheckTransform.localPosition = 
                    //Vector3.up * m_heightPlayer * 0.5f;
                    Debug.Log("m_characterController.center: " + m_characterController.center);
                    playerCamera.transform.localPosition = Vector3.Lerp(playerCamera.transform.localPosition, Vector3.up * m_heightPlayer * m_cameraHeightRatio, 10 * Time.deltaTime);
                    //m_Actor.aimPoint.transform.localPosition = m_characterController.center;
                }*/
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
            float x = Input.GetAxis(GameConstants.HORIZONTAL);
            float y = Input.GetAxis(GameConstants.VERTICAL);
            Vector3 move = transform.right * x + transform.forward * y;
            m_characterController.Move(move * m_speedPlayer * Time.deltaTime);
            m_animator.SetBool(GameConstants.ANIMATION_MOVE, true);

        }

        void checkGrounded()
        {
            isGrounded = Physics.CheckSphere(groundCheckTransform.position, groundDistance, groundLayerMask);
            if (isGrounded && m_velocity.y < 0)
            {
                m_velocity.y = -2f;
            }
            m_velocity.y += gravity * Time.deltaTime;
            m_characterController.Move(m_velocity * Time.deltaTime);
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
   

