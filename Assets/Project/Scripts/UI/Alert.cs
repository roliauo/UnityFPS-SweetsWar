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

        void Show()
        {
            gameObject.SetActive(true);
        }

        void Toggle()
        {
            gameObject.SetActive(!gameObject.activeInHierarchy);
        }

        void Show(string title, string txt, bool hasCancelButton)
        {

        }

        void Show(string str, Action onClickOK = null) //bool isConfirmType, 
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
                }
            });

            gameObject.SetActive(true);
        }
    }

}
