using UnityEngine;
using UnityEngine.Serialization;

namespace RS
{
    public class MeleeWeaponDamageCollider : DamageCollider
    {
        [Header("Attacking Character")] 
        public CharacterManager characterCausingDamage;
    }
}