using System;
using System.Globalization;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Serialization;

namespace RS
{
    public class CharacterNetworkManager : NetworkBehaviour
    {
        private CharacterManager character;

        [Header("Status")] 
        public NetworkVariable<bool> isDead = new NetworkVariable<bool>
        (false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
        
        [Header("Position")]
        public NetworkVariable<Vector3> networkPosition = new NetworkVariable<Vector3>
            (Vector3.zero, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
        
        public NetworkVariable<Quaternion> networkRotation = new NetworkVariable<Quaternion>
            (Quaternion.identity, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

        public Vector3 networkPositionVelocity;
        public float networkPositionSmoothTime = 0.1f;
        public float networkRotationSmoothTime = 0.1f;

        [Header("Animator")] 
        public NetworkVariable<bool> isMoving = new NetworkVariable<bool>
            (false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
        
        public NetworkVariable<float> horizontalMovement = new NetworkVariable<float>
            (0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

        public NetworkVariable<float> verticalMovement = new NetworkVariable<float>
            (0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
        
        public NetworkVariable<float> moveAmount = new NetworkVariable<float>
            (0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

        [Header("Target")] 
        public NetworkVariable<ulong> currentTargetNetworkObjectID =
            new NetworkVariable<ulong>(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
        
        [Header("Flags")] 
        public NetworkVariable<bool> isLockedOn = new NetworkVariable<bool>
            (false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
        
        public NetworkVariable<bool> isSprinting = new NetworkVariable<bool>
            (false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
        
        public NetworkVariable<bool> isJumping = new NetworkVariable<bool>
            (false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
        
        public NetworkVariable<bool> isChargingAttack = new NetworkVariable<bool>
            (false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

        
        [Header("Stats")] 
        public NetworkVariable<int> endurance = new NetworkVariable<int>
            (1, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
        
        public NetworkVariable<int> vitality = new NetworkVariable<int>
            (1, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
        
        [Header("Resources")]
        public NetworkVariable<float> currentStamina = new NetworkVariable<float>
            (0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
        
        public NetworkVariable<int> maxStamina = new NetworkVariable<int>
            (0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
        
        public NetworkVariable<int> currentHealth = new NetworkVariable<int>
            (0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
        
        public NetworkVariable<int> maxHealth = new NetworkVariable<int>
            (0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
        
        
        protected virtual void Awake()
        {
            character = GetComponent<CharacterManager>();
        }

        public void CheckHP(int oldValue, int mewValue)
        {
            if (currentHealth.Value <= 0)
            {
                StartCoroutine(character.ProcessDeathEvent());
            }

            if (character.IsOwner)
            {
                if (currentHealth.Value > maxHealth.Value)
                {
                    currentHealth.Value = maxHealth.Value;
                }
            }
        }

        public void OnCurrentLockOnTargetIDChange(ulong oldID, ulong newID)
        {
            if (!IsOwner)
            {
                character.characterCombatManager.currentTarget = 
                    NetworkManager.Singleton.SpawnManager.SpawnedObjects[newID].gameObject.GetComponent<CharacterManager>();
            }
        }

        public void OnCurrentIsLockedOnChanged(bool old, bool isLockedOn)
        {
            if (!isLockedOn)
            {
                character.characterCombatManager.currentTarget = null;
            }
        }

        public void OnIsChargingAttackChanged(bool oldStatus, bool newStatus)
        {
            character.animator.SetBool("IsChargingAttack", isChargingAttack.Value);
        }

        public void OnIsMovingChanged(bool oldStatus, bool newStatus)
        {
            character.animator.SetBool("IsMoving", isMoving.Value);
        }
        
        // Animation
        [ServerRpc]
        public void NotifyServerOfActionAnimationServerRPC(ulong ClientID, string animationID, bool applyRootMotion)
        {
            if (IsServer)
            {
                PlayActionAnimationOnAllClientsClientRPC(ClientID, animationID, applyRootMotion);
            }
        }

        [ClientRpc]
        public void PlayActionAnimationOnAllClientsClientRPC(ulong ClientID, string animationID, bool applyRootMotion)
        {
            if (ClientID != NetworkManager.Singleton.LocalClientId)
            {PerformActionAnimationFromServer(animationID, applyRootMotion);
                
            }
        }

        private void PerformActionAnimationFromServer(string animationID, bool applyRootMotion)
        {
            character.applyRootMotion = applyRootMotion;
            character.animator.CrossFade(animationID, 0.2f);
        }
        
        
        // Attack Animation
        [ServerRpc]
        public void NotifyServerOfAttackActionAnimationServerRPC(ulong ClientID, string animationID, bool applyRootMotion)
        {
            if (IsServer)
            {
                PlayAttackActionAnimationOnAllClientsClientRPC(ClientID, animationID, applyRootMotion);
            }
        }

        [ClientRpc]
        public void PlayAttackActionAnimationOnAllClientsClientRPC(ulong ClientID, string animationID, bool applyRootMotion)
        {
            if (ClientID != NetworkManager.Singleton.LocalClientId)
            {
                PerformAttackActionAnimationFromServer(animationID, applyRootMotion);
            }
        }

        private void PerformAttackActionAnimationFromServer(string animationID, bool applyRootMotion)
        {
            character.applyRootMotion = applyRootMotion;
            character.animator.CrossFade(animationID, 0.2f);
        }
        
        
        // DAMAGE
        [ServerRpc(RequireOwnership = false)]
        public void NotifyServerOfCharacterDamageServerRPC(
            ulong damagedCharacterID,
            ulong characterCausingDamageID,
            float physicalDamage,
            float magicDamage,
            float fireDamage,
            float lightningDamage,
            float holyDamage,
            float poiseDamage,
            float angleHitFrom,
            float contactPointX,
            float contactPointY,
            float contactPointZ)
        {
            if (IsServer)
            {
                NotifyServerOfCharacterDamageClientRPC(damagedCharacterID, characterCausingDamageID, physicalDamage,
                    magicDamage, fireDamage, lightningDamage, holyDamage, poiseDamage, angleHitFrom, contactPointX, contactPointY,
                    contactPointZ);
            }
        }
        
        [ClientRpc]
        public void NotifyServerOfCharacterDamageClientRPC(
            ulong damagedCharacterID,
            ulong characterCausingDamageID,
            float physicalDamage,
            float magicDamage,
            float fireDamage,
            float lightningDamage,
            float holyDamage,
            float poiseDamage,
            float angleHitFrom,
            float contactPointX,
            float contactPointY,
            float contactPointZ)
        {
            PerformCharacterDamageFromServer(damagedCharacterID, characterCausingDamageID, physicalDamage, magicDamage,
                fireDamage, lightningDamage, holyDamage, poiseDamage, angleHitFrom, contactPointX, contactPointY, contactPointZ);
        }
        
        public void PerformCharacterDamageFromServer(
            ulong damagedCharacterID,
            ulong characterCausingDamageID,
            float physicalDamage,
            float magicDamage,
            float fireDamage,
            float lightningDamage,
            float holyDamage,
            float poiseDamage,
            float angleHitFrom,
            float contactPointX,
            float contactPointY,
            float contactPointZ)
        {
            CharacterManager damagedCharacter = 
                NetworkManager.Singleton.SpawnManager.SpawnedObjects[damagedCharacterID].gameObject.GetComponent<CharacterManager>();
            
            CharacterManager characterCausingDamage =
                NetworkManager.Singleton.SpawnManager.SpawnedObjects[characterCausingDamageID].gameObject.GetComponent<CharacterManager>();

            TakeDamageEffect damageEffect = Instantiate(WorldCharacterEffectsManager.instance.takeDamageEffect);


            damageEffect.physicalDamage = physicalDamage;
            damageEffect.magicDamage = magicDamage;
            damageEffect.fireDamage = fireDamage;
            damageEffect.lightningDamage = lightningDamage;
            damageEffect.holyDamage = holyDamage;
            damageEffect.poiseDamage = poiseDamage;
            damageEffect.angleHitFrom = angleHitFrom;
            damageEffect.contactPoint = new Vector3(contactPointX, contactPointY, contactPointZ);
            damageEffect.characterCausingDamage = characterCausingDamage;
            
            damagedCharacter.characterEffectsManager.ProcessInstantEffects(damageEffect);
        }
    }
}