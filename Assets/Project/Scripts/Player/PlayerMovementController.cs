using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;

namespace Game.SweetsWar
{
    [RequireComponent(typeof(CharacterController), typeof(AudioSource))]
    public class PlayerMovementController : MonoBehaviourPunCallbacks, IPunObservable
    {
        public static GameObject localPlayerInstance;
        public Camera playerCamera;
        public Text playerName;
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
        public float heightStanding = 1f;
        public float heightCrouching = 0.6f;

        public bool isDead { get; private set; }
        public bool isGrounded { get; private set; }
        public bool isCrouching { get; private set; }
        public bool isFiring;

        // private
        private CharacterController m_characterController;
        private Animator m_animator;
        private Vector3 m_velocity;
        private float m_cameraHeightRatio = 0.9f;
        private float m_rotationX = 0f;
        private float m_heightPlayer;
        private float m_speedPlayer;

        private GameObject sign;

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
            m_heightPlayer = heightStanding;

            
            playerName.text = photonView.Owner.NickName;
            Debug.LogFormat("name: {0}, key: {1}, photonView: {2}",
                PhotonNetwork.NickName, 
                PlayerPrefs.GetString(GameConstants.PLAYER_NAME_PREFAB_KEY), 
                photonView.Owner.NickName
              );

            // the player's name faces camera
            playerName.gameObject.transform.rotation = Camera.main.transform.rotation;
            //Debug.LogFormat("rotation: {0}, {1}", Camera.main.transform.rotation, playerCamera.transform.rotation);
            
            /*
            sign = new GameObject("player_label");
            sign.transform.rotation = Camera.main.transform.rotation; // Causes the text faces camera.
            TextMesh tm = sign.AddComponent<TextMesh>();
            tm.text = photonView.Owner.NickName;
            tm.color = new Color(0.8f, 0.8f, 0.8f);
            tm.fontStyle = FontStyle.Bold;
            tm.alignment = TextAlignment.Center;
            tm.anchor = TextAnchor.MiddleCenter;
            tm.characterSize = 0.065f;
            tm.fontSize = 60;
            */
        }

        void Update()
        {
            //playerName.gameObject.transform.position = localPlayerInstance.transform.position + Vector3.up * 3f;
            //Camera.main.transform.position + Vector3.forward * 3f;
            playerName.gameObject.transform.rotation = Camera.main.transform.rotation;
            //playerName.gameObject.transform.position = Camera.main.WorldToScreenPoint(localPlayerInstance.transform.position);
            //sign.transform.position = localPlayerInstance.transform.position + Vector3.up * 3f;

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
                m_heightPlayer = isCrouching ? heightCrouching : heightStanding;
                m_speedPlayer = isCrouching ? speedCrouching : speedNormal;             

                // change player's height
                //transform.localScale = new Vector3(1, m_heightPlayer, 1);
                //transform.localPosition = new Vector3(transform.localPosition.x, m_heightPlayer, transform.localPosition.z);
                m_characterController.height = m_heightPlayer;
                m_characterController.center = new Vector3(0, m_heightPlayer / 2, 0);
                playerCamera.transform.localPosition = Vector3.Lerp(playerCamera.transform.localPosition, Vector3.up * m_heightPlayer * m_cameraHeightRatio, 10 * Time.deltaTime);

                m_animator.SetBool(GameConstants.ANIMATION_CHROUCH, isCrouching);

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
   

