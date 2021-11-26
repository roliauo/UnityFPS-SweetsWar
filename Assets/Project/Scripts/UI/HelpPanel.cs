using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Game.SweetsWar
{
    public class HelpPanel : MonoBehaviour
    {
        public Button Button_Next;
        public Image[] PagesImage;

        private byte m_pageNumber = 1;
        private void Start()
        {
            Button_Next.onClick.AddListener(() =>
            {
                if (m_pageNumber >= PagesImage.Length)
                {
                    PagesImage[m_pageNumber - 1].gameObject.SetActive(false);
                    m_pageNumber = 1;
                    PagesImage[0].gameObject.SetActive(true);
                    gameObject.SetActive(false);
                    //SetActivePanel(MainPanel.name);
                }
                else
                {
                    if (m_pageNumber > 0) PagesImage[m_pageNumber - 1].gameObject.SetActive(false);
                    PagesImage[m_pageNumber].gameObject.SetActive(true);
                    m_pageNumber++;
                }

            });
        }

    }
}
