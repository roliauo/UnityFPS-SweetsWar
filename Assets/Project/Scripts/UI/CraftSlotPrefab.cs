using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Game.SweetsWar
{
    public class CraftSlotPrefab : MonoBehaviour//, ISelectHandler
    {
        private Item m_item;
        void Start()
        {
            this.GetComponent<Button>().onClick.AddListener(() =>
            {
                Debug.Log("slot click: " + m_item.DisplayName);
                CraftUIManager._instance.RemoveFromCraftSlots(m_item, this.gameObject);
            });
        }

        public void SetItem(Item item)
        {
            m_item = item;
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
