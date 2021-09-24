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

        // player touched
        private void OnTriggerEnter(Collider c) 
        {
            if (BackpackManerger._instance == null)
            {
                Debug.Log("Without BackpackManerger._instance!");
                return;
            }

            if(c.gameObject.tag == GameConstants.TAG_PLAYER)
            {
                BackpackManerger backpack = BackpackManerger._instance;
                backpack.Collect(item);

                // Destroy the object in scene
                Destroy(this.gameObject);
            }
       
        }

        // on click item
        private void OnMouseDown()
        {
            if (BackpackManerger._instance == null)
            {
                Debug.Log("Without BackpackManerger._instance!");
                return;
            }

            BackpackManerger backpack = BackpackManerger._instance;
            backpack.Collect(item);

            // Destroy the object in scene
            Destroy(this.gameObject);
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

