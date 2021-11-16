using UnityEngine;

namespace Game.SweetsWar
{
    [CreateAssetMenu(fileName = "New Weapon", menuName = "Weapon")]
    public class Weapon: Item
    {
        public float Damage;
        public float AttackRange;
        public float FireLoadingTime;
        public AudioClip fireSFX;
        public string k_AnimationName = null;
        //public new byte MaxNumber = 1;


    }
}

