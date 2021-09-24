using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Game.SweetsWar
{
    public class BackpackSlotPrefab : MonoBehaviour
    {
        public Button btn_Slot;

        private Item m_item;
        void Start()
        {
            btn_Slot.onClick.AddListener(() =>
            {
                Debug.Log("slot click: " + m_item.DisplayName);
            });
        }

        public void SetItem(Item item)
        {
            m_item = item;
            btn_Slot.image.sprite = item.Image;            
            btn_Slot.GetComponentInChildren<Text>().text = item.DisplayName;
            Debug.Log("DisplayName: " + item.DisplayName);
        }
    }

}
