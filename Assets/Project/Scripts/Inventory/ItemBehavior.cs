using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Game.SweetsWar
{
    public class ItemBehavior : MonoBehaviour
    {
        public Inventory inventory;
        public Item item;

        private void OnTriggerEnter(Collider c)
        {
            if(c.gameObject.tag == GameConstants.TAG_PLAYER)
            {
                inventory.AddItems(item);
                                
                // Destroy the object in scene
                Destroy(this.gameObject);

                Debug.LogFormat("list size: {0}, {1}", inventory.ItemList.Capacity, inventory.ItemList.Count);
                Debug.LogFormat("item: ", item.name);

                foreach (var item in inventory.ItemList)
                {
                    Debug.LogFormat("backpack: {0}, {1}", item.DisplayName, item.Number);
                }
            }
       
        }

        /*
        public void OnPointerClick(PointerEventData pointerEventData)
        {
            Debug.Log(name + " Game Object Clicked!");

            if (inventory.ItemList.Contains())
        }
        */

    }
}

