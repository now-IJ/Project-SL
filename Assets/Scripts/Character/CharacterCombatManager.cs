using System;
using System.Globalization;
using Unity.Netcode;
using UnityEngine;

namespace RS
{
    public class CharacterCombatManager : NetworkBehaviour
    {
        protected CharacterManager character;

        [Header("Character Group")] 
        public CharacterGroup characterGroup;
        
        [Header("Last Attack Animation Performed")]
        public string lastAttackAnimationPerformed;
        
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

        public void EnableIsInvulnerable()
        {
            if (IsOwner)
            {
                character.characterNetworkManager.isInvulnerable.Value = true;
            }
        }

        public void DisableIsInvulnerable()
        {
            if (IsOwner)
            {
                character.characterNetworkManager.isInvulnerable.Value = false;
            }
        }
    }
}
