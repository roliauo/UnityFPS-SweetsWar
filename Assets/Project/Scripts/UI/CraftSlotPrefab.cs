using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Game.SweetsWar
{
    public class CraftSlotPrefab : MonoBehaviour//, ISelectHandler
    {
        private Item m_Item;
        private int m_ItemViewID;
        void Start()
        {
            this.GetComponent<Button>().onClick.AddListener(() =>
            {
                //Debug.Log("slot click: " + m_item.DisplayName);
                CraftUIManager._instance.RemoveFromCraftSlots(m_Item, gameObject);
            });
        }

        public void SetItem(Item item)
        {
            //m_ItemViewID = viewID;
            m_Item = item;
            this.GetComponent<Image>().sprite = item.Icon;

        }

        /*
        public void OnSelect(BaseEventData eventData)
        {
            Debug.Log(m_item.DisplayName + " was selected.  eventData: " + eventData.used + ", "+ eventData.selectedObject.name);
        }
        */
    }

}
