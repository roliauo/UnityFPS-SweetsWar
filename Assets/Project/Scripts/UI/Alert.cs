using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game.SweetsWar
{
    public class Alert : MonoBehaviour
    {
        public TMP_Text Title;
        public TMP_Text TextArea;
        public Button Btn_OK;
        public Button Btn_Cancel;

        private void Start()
        {
            Btn_Cancel.onClick.AddListener(() => gameObject.SetActive(false));
        }

        public void Show()
        {
            gameObject.SetActive(true);
        }

        public void Toggle()
        {
            gameObject.SetActive(!gameObject.activeInHierarchy);
        }

        public void Show(string title, string txt, bool hasCancelButton)
        {

        }

        public void Show(string str, Action onClickOK = null) //bool isConfirmType, 
        {
            bool isConfirmType = (onClickOK != null);
            Title.text = isConfirmType ? "確認" : "警告";
            TextArea.text = str;
            Btn_Cancel.gameObject.SetActive(isConfirmType);
            Btn_OK.onClick.AddListener(() => { 
                if (isConfirmType)
                {
                    onClickOK();
                }
                else
                {
                    gameObject.SetActive(false);
                    GameManager.Instance.SetCursorMode(false);
                }
            });

            gameObject.SetActive(true);
            GameManager.Instance.SetCursorMode(true);
        }
    }

}
