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
        
        [Header("Attack Modifiers")]
        public float light_Attack_01_Modifier = 1.0f;
        public float heavy_Attack_01_Modifier = 1.4f;
        public float charge_Attack_01_Modifier = 2.0f;
        
        [Header("Stamina Costs Modifiers")] 
        public int baseStaminaCost = 20;
        public float lightAttackStaminaCostMultiplier = 1;
        
        [Header("Actions")]
        public WeaponItemAction oh_rb_Action;
        public WeaponItemAction oh_rt_Action;
    }
}