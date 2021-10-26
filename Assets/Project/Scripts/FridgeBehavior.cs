using Photon.Pun;
using System.Collections;
using UnityEngine;

namespace Game.SweetsWar
{
    [RequireComponent(typeof(Animation))]
    public class FridgeBehavior : MonoBehaviourPunCallbacks, IPunObservable
    {
        public static FridgeBehavior _instance;
        public string ID;
        public float MaxDistance = 4f;
        public float HP = 100;
        public bool IsOpened = false;

        private Animation m_animation;
        private Animator m_animator;
        private const string k_Animation_FridgeOpen = "FridgeOpen";
        private const string k_Animation_FridgeClose = "FridgeClose";

        private void Awake()
        {
            if (_instance != null)
            {
                Destroy(this);
            }

            _instance = this;

        }

        void Start()
        {
            m_animator = GetComponent<Animator>();
            ID = PhotonNetwork.LocalPlayer.UserId;
            Debug.Log("ID: " + ID);
            // 如果不是自己的冰箱就用紅框表示
        }

        private void Update()
        {
            //transform.Rotate(new Vector3(0, 1, 0), Space.World);
        }
        /*
        // player touched
        private void OnTriggerEnter(Collider c) 
        {
            if (BackpackManerger._instance == null)
            {
                Debug.Log("Without BackpackManerger._instance!");
                return;
            }

            if(c.gameObject.tag == GameConstants.TAG_PLAYER)
            {
                BackpackManerger backpack = BackpackManerger._instance;
                if (backpack.Collect(item)) {
                    // Destroy the object in scene
                    Destroy(this.gameObject);
                }
            }     
        }
        */

        // on click item (collider)
        void OnMouseDown()
        {
            if (BackpackManerger._instance == null)
            {
                Debug.Log("Without BackpackManerger._instance!");
                return;
            }

            OpenFridge(true);

        }

        public void OpenFridge(bool state)
        {
            // Play the Animation and control the permission 
            float distance = Vector3.Distance(PlayerMovementController.localPlayerInstance.transform.position, transform.position);
            string me = PlayerMovementController.localPlayerInstance.GetComponent<PhotonView>().Owner.UserId;
            //Debug.Log("冰箱: " + ID + "我是: " + me + " " + ID == me);

            if (distance < MaxDistance && ID == me)
            {
                Debug.Log("冰箱打開state: " + state);
                IsOpened = state;

                m_animator.SetBool(k_Animation_FridgeOpen, IsOpened);

                if (IsOpened == false)
                {
                    m_animator.SetTrigger(k_Animation_FridgeClose);
                }

                // Open Craft System. 不監聽。由冰箱發送控制。
                Invoke("SetCraftPanel", IsOpened ? 2f : 0);
            }
        }

        public void playAnimation(bool isOpened) {
            m_animator.SetBool(k_Animation_FridgeOpen, isOpened);
            if (isOpened == false)
            {
                m_animator.SetTrigger(k_Animation_FridgeClose);
            }
        }

        private void SetCraftPanel()
        {
            GameManager.Instance.setCraftPanel(IsOpened);
        }

        #region IPunObservable implementation
        public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
        {
            ((IPunObservable)_instance).OnPhotonSerializeView(stream, info);
        }
        #endregion
    }
}

