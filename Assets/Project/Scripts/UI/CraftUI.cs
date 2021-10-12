using UnityEngine;
using UnityEngine.UI;

namespace Game.SweetsWar
{
    public class CraftUI : MonoBehaviour
    {
        public GameObject Craft;
        //public Button Button_Slot;
        public Button Button_Mix;
        public Button Button_Close;
        public Button Button_Info;

        void Start()
        {
            Button_Close.onClick.AddListener(() =>
            {
                //Craft.SetActive(false);
                //GameManager.Instance.setCraftPanel(false);
                FridgeBehavior._instance.OpenFridge(false);
            });
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}

