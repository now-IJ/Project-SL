using System;
using UnityEngine;

namespace RS
{
    public class CharacterCombatManager : MonoBehaviour
    {
        [Header("Attack Target")]
        public CharacterManager currentTarget;
        
        [Header("Attack Type")]
        public AttackType currentAttackType;

        [Header("Lock On Transform")] 
        public Transform lockOnTargetTransform;
        
        protected virtual void Awake()
        {
            
        }
    }
}
