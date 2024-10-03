using System;
using System.Globalization;
using Unity.Netcode;
using UnityEngine;

namespace RS
{
    public class CharacterCombatManager : NetworkBehaviour
    {
        private CharacterManager character;
        
        [Header("Attack Target")]
        public CharacterManager currentTarget;
        
        [Header("Attack Type")]
        public AttackType currentAttackType;

        [Header("Lock On Transform")] 
        public Transform lockOnTargetTransform;
        
        protected virtual void Awake()
        {
            character = GetComponent<CharacterManager>();
        }

        public virtual void SetTarget(CharacterManager newTarget)
        {
            if (character.IsOwner)
            {
                if (newTarget != null)
                {
                    currentTarget = newTarget;
                    
                    character.characterNetworkManager.currentTargetNetworkObjectID.Value =
                        newTarget.GetComponent<NetworkObject>().NetworkObjectId;
                    
                }
                else
                {
                    currentTarget = null;
                }
            }
        }
    }
}
