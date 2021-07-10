using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Game.SweetsWar
{
    public class TitleMenuButton : MonoBehaviour
    {
        //public string sceneToLoad = "";
        public GameObject OptionsMenu;
        public GameObject MainMenu;
        public GameObject Title;

        private void Update()
        {

        }

        public void LoadTargetScene()
        {
            //SceneManager.LoadScene(sceneToLoad);
        }

        public void StartGame()
        {
            SceneManager.LoadScene(GameConstants.SCENE_GAME);
        }

        public void OnClickOptionsButton()
        {
            Title.SetActive(false);
            MainMenu.SetActive(false);
            OptionsMenu.SetActive(true);
        }

        public void QuitGame()
        {
            Debug.Log("quit");
            // PhotonNetwork.LeaveLobby();
            // PhotonNetwork.Disconnect();
            Application.Quit();
        }

    }
}

