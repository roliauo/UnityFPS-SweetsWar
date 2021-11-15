using Photon.Pun;
using System.Collections;
using UnityEngine;

namespace Game.SweetsWar
{
    [RequireComponent(typeof(Animator))]
    public class FridgeBehavior : MonoBehaviourPunCallbacks//, IPunInstantiateMagicCallback  //, IPunObservable
    {
        public int ID;
        public float MaxDistance = 4f;
        public float HP = 100;
        public bool IsOpened = false;

        //private Animation m_animation;
        private Animator m_animator;
        private const string k_Animation_FridgeOpen = "FridgeOpen";
        private const string k_Animation_FridgeClose = "FridgeClose";

        private void Awake()
        {
            DontDestroyOnLoad(gameObject);
        }

        void Start()
        {
            m_animator = GetComponent<Animator>();
            ID = photonView.ViewID;
            CraftUIManager._instance.AllPlayerCraftingInventories.Add(ID, new Inventory(9));
            //inventory = new Inventory(9);
            //ID = PlayerController.localPlayerInstance.GetComponent<PhotonView>().Owner.UserId; //PhotonNetwork.LocalPlayer.UserId; //PhotonNetwork.PlayerList[PhotonNetwork.CurrentRoom.PlayerCount-1].UserId; //PlayerController.localPlayerInstance.GetComponent<PhotonView>().Owner.UserId

            // 如果不是自己的冰箱就用紅框表示
        }

        #region PUN callback
        /*
        public void OnPhotonInstantiate(PhotonMessageInfo info)
        {
            object[] instantiationData = info.photonView.InstantiationData;
            //ID = (string)instantiationData[0];
            //inventory = (Inventory)instantiationData[1];
            Debug.Log("OnPhotonInstantiate: " + ID);
            //inventory = new Inventory(9);
            //CraftUIManager._instance.inventory = FridgeBehavior._instance.inventory; // Binding gameObject's data. if needed just show it.
        }
        */
        #endregion

        void OnMouseDown()
        {
            if (BackpackManerger._instance == null)
            {
                Debug.Log("Without BackpackManerger._instance!");
                return;
            }

            OpenFridge(true);

            // w2
            //OpenFridge_New(true);
            //photonView.RPC("RPC_OpenFridge", RpcTarget.MasterClient, photonView.ViewID);

        }

        public void OpenFridge(bool state)
        {
            // Play the Animation and control the permission 
            //float distance = Vector3.Distance(PlayerController.localPlayerInstance.transform.position, _instance.transform.position);
            float distance = Vector3.Distance(PlayerController.localPlayerInstance.transform.position, gameObject.transform.position);
            string me = PlayerController.localPlayerInstance.GetComponent<PhotonView>().Owner.UserId; //PhotonNetwork.LocalPlayer.UserId; 
            Debug.Log("冰箱: " + ID + " 我是: " + me + " distance: " + distance);

            if (distance < MaxDistance) //&& ID == me
            {

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
            Debug.Log("SetCraftPanel-ID: " + ID);
            GameManager.Instance.SetCraftPanel(IsOpened, ID);
        }

    }
}

