using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Game.SweetsWar
{
    public class BackpackSlotPrefab : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        public Button btn_Slot;
        public TMP_Text ItemName;
        public TMP_Text ItemNumber;

        //private List<int> m_ItemViewIDList;
        private Item m_Item;
        void Start()
        {
            //m_ItemViewIDList = new List<int>();
            btn_Slot.onClick.AddListener(() =>
            {
                //if (m_ItemViewIDList.Count>0) Debug.Log("slot click(remove): " + m_Item.DisplayName + " m_ItemViewID: "+ m_ItemViewIDList[m_ItemViewIDList.Count-1]);

                BackpackManerger._instance.OnClickSlot(m_Item);
                //m_ItemViewIDList.RemoveAt(m_ItemViewIDList.Count - 1);
                /*
                // move into the craft box
                BackpackManerger._instance.Subtract(m_item);
                CraftUIManager._instance.AddToCraftSlots(m_item);
                */
            });
        }

        public void SetItem(Item item)
        {
            m_Item = item;
            btn_Slot.GetComponent<Image>().sprite = item.Icon;
            ItemName.text = item.DisplayName;
            ItemNumber.text = item.Number.ToString();
        }

        //滑鼠移入
        public void OnPointerEnter(PointerEventData eventData)
        {

            ItemName.gameObject.SetActive(true);

        }

        //滑鼠移出
        public void OnPointerExit(PointerEventData eventData)
        {
            ItemName.gameObject.SetActive(false);

        }

    }

}
