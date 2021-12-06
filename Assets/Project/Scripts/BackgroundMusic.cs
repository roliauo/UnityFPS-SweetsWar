using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Game.SweetsWar
{
    public class BackgroundMusic : MonoBehaviour
    {
        public static BackgroundMusic _instance;
        public AudioClip TitleMusic;
        public AudioClip GameStageMusic;

        private AudioSource m_audio;
        void Start()
        {

            GameObject[] objs = GameObject.FindGameObjectsWithTag(tag);

            if (objs.Length > 1)
            {
                Destroy(this.gameObject);
            }

            if (_instance != null)
            {
                Destroy(this);
            }
            _instance = this;

            m_audio = gameObject.GetComponent<AudioSource>();
            m_audio.clip = TitleMusic;
            DontDestroyOnLoad(gameObject);
        }

        public void ChangeMusic(AudioClip music)
        {
            m_audio.Stop();
            m_audio.clip = music;
            m_audio.Play();
        }

        public void ChangeMusic(int musicID)
        {
            m_audio.Stop();
            m_audio.clip = (musicID == 0) ? TitleMusic : GameStageMusic;
            m_audio.Play();
        }

        public void SetVolumn(float value)
        {
            Debug.Log("SetVolumn: " + value);
            m_audio.volume = value;
        }
    }
}
