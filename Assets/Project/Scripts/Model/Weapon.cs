using UnityEngine;

namespace Game.SweetsWar
{
    [CreateAssetMenu(fileName = "New Weapon", menuName = "Weapon")]
    public class Weapon: Item
    {
        public float Damage;
        public float AttackRange;
        public float FireLoadingTime;
        public AudioClip AttackSFX;
        //public string k_AnimationName = null;
        //public bool hasFireAnimation;

    }
}

