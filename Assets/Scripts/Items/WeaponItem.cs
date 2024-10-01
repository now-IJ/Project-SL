using UnityEngine;

namespace RS
{
    public class WeaponItem : Item
    {
        // Animator Controller override

        [Header("Weapon Model")] 
        public GameObject weaponModel;

        [Header("Weapon Requirements")] 
        public int strengthREQ = 0;
        public int dexREQ = 0;
        public int intREQ = 0;
        public int faithREQ = 0;
        
        [Header("Weapon Base Damage")]
        public int physicalDamage = 0;
        public int magicDamage = 0;
        public int fireDamage = 0;
        public int lightningDamage = 0;
        public int holyDamage = 0;

        [Header("Weapon Base Poise Damage")] 
        public float poiseDamage = 10;
        
        // Weapon modifiers

        [Header("Stamina Costs")] 
        public int baseStaminaCost = 20;
        
        [Header("Actions")]
        public WeaponItemAction oh_rb_Action;
    }
}