using Photon.Pun;
using Photon.Pun.UtilityScripts;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Game.SweetsWar
{
    public class HUDManager : MonoBehaviour
    {
        public static HUDManager _instance;

        [Header("Treasure")]
        public GameObject TreasureAnnouncement;
        public Image TreasureTip;

        [Header("player")]
        public TMP_Text PlayerName;

        [Header("Menu")]
        public GameObject Menu;      
        public TMP_Text GameMode;
        public TMP_Text Room;
        public TMP_Text PlayerCount;
        public TMP_Text Version;
        public TMP_Text Region;
        public Button Button_Help;
        //public AudioSource audioSource_Music;
        public AudioSource audioSource_Sound;
        public AudioSource audioSource_Sound_Weapon;
        public Slider MusicVolumn;
        public Slider SoundVolumn;

        [Header("Help")]
        public GameObject HelpPanel;

        private bool m_helpFlag = false;
        private AudioSource m_audioSource_Music;

        private void Start()
        {
            if (!PhotonNetwork.IsConnected)
            {
                SceneManager.LoadScene(GameConstants.SCENE_TITLE);
                return;
            }

            _instance = this;

            PlayerName.text = PhotonNetwork.LocalPlayer.NickName;  //PlayerController._instance.photonView.Owner.NickName;
            PlayerName.color = GameConstants.GetColor((int)PhotonNetwork.LocalPlayer.CustomProperties[GameConstants.K_PROP_TEAM]);
            GameMode.text = (string)PhotonNetwork.CurrentRoom.CustomProperties[GameConstants.GAME_MODE];
            Room.text = PhotonNetwork.CurrentRoom.Name;
            PlayerCount.text = PhotonNetwork.CurrentRoom.PlayerCount + " / " + PhotonNetwork.CurrentRoom.MaxPlayers;
            Version.text = "ver. " + PhotonNetwork.GameVersion;
            Region.text = "region: " + PhotonNetwork.CloudRegion;

            if (PlayerController.localPlayerInstance != null)
            {
                Debug.Log("AudioSource Ready");
                audioSource_Sound = PlayerController.localPlayerInstance.GetComponent<AudioSource>();
            }
            m_audioSource_Music = GameObject.FindGameObjectWithTag("BGM").GetComponent<AudioSource>();
            MusicVolumn.value = m_audioSource_Music.volume;
            MusicVolumn.onValueChanged.AddListener((value) =>
            {
                //GameObject.FindGameObjectWithTag("");
                //FindObjectOfType<BackgroundMusic>().SetVolumn(value);
                //GameObject.FindGameObjectWithTag("BGM").GetComponent<BackgroundMusic>().SetVolumn(value);
                m_audioSource_Music.volume = value;
            });

            SoundVolumn.onValueChanged.AddListener((value) =>
            {
                if (audioSource_Sound) audioSource_Sound.volume = value;
                if (audioSource_Sound_Weapon) audioSource_Sound_Weapon.volume = value;
                //AudioListener.volume = value;
            });
            

            Button_Help.onClick.AddListener(() =>
            {
                m_helpFlag = true;
                HelpPanel.SetActive(true);
                Menu.SetActive(false);
            });
        }

        private void Update()
        {
            if (TreasureAnnouncement.activeInHierarchy && Input.GetKeyDown(KeyCode.Space))
            {
                TreasureAnnouncement.SetActive(false);
            }

            if (GameManager.Instance && !TreasureTip.gameObject.activeInHierarchy && GameManager.Instance.TreasureGoalID > -1)
            {
                SetTreasureGoal(GameManager.Instance.TreasureGoalID);
            }

            if (m_helpFlag && !Menu.activeInHierarchy && !HelpPanel.activeInHierarchy)
            {
                m_helpFlag = false;
                // back to Game Menu
                Menu.SetActive(true);
            }

            
            if (audioSource_Sound_Weapon == null &&
                PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue(GameConstants.K_PROP_WEAPON_VIEW_ID, out object id) && (int)id > -1)
            {
                if (PhotonView.Find((int)id)) audioSource_Sound_Weapon = PhotonView.Find((int)id).gameObject.GetComponent<AudioSource>();
                
                SoundVolumn.onValueChanged.AddListener((value) =>
                {
                    if (audioSource_Sound) audioSource_Sound.volume = value;
                    if (audioSource_Sound_Weapon) audioSource_Sound_Weapon.volume = value;
                });
            }
            
        }

        void changeVolume(AudioSource audioSource, float value)
        {
            audioSource.volume = value;
        }

        public void SetTreasureGoal(int id)
        {
            Debug.Log("SetTreasureGoal: " + id);
            foreach (Item item in CraftUIManager._instance.OutputItems)
            {
                if (item.ID == id)
                {
                    TreasureAnnouncement.GetComponentInChildren<Image>().sprite = item.Icon;
                    TreasureTip.sprite = item.TreasureTip; 
                    break;
                }

            }
            TreasureAnnouncement.SetActive(true);
            TreasureTip.gameObject.SetActive(true);
        }
    }
}
