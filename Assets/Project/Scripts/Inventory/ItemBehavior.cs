using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Game.SweetsWar
{
    public class ItemBehavior : MonoBehaviour
    {
        //public Inventory inventory;
        public Item item;

        private void OnTriggerEnter(Collider c)
        {
            if (BackpackManerger._instance == null)
            {
                Debug.Log("Without BackpackManerger._instance!");
                return;
            }

            if(c.gameObject.tag == GameConstants.TAG_PLAYER)
            {
                //inventory.AddItems(item);
                BackpackManerger backpack = BackpackManerger._instance;
                backpack.AddOneItem(item);

                // Destroy the object in scene
                Destroy(this.gameObject);
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

