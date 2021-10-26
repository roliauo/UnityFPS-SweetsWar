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

        private Item m_item;
        void Start()
        {
            btn_Slot.onClick.AddListener(() =>
            {
                Debug.Log("slot click(remove): " + m_item.DisplayName);

                BackpackManerger._instance.OnClickSlot(m_item);

                /*
                // move into the craft box
                BackpackManerger._instance.Subtract(m_item);
                CraftUIManager._instance.AddToCraftSlots(m_item);
                */
            });
        }

        public void SetItem(Item item)
        {
            m_item = item;
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
