using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using Unity.Netcode;

namespace RS
{
    public class PlayerManager : CharacterManager
    {
        [HideInInspector] public PlayerAnimationManager playerAnimationManager;
        [HideInInspector] public PlayerLocomotionManager playerLocomotionManager;
        [HideInInspector] public PlayerNetworkManager playerNetworkManager;
        [HideInInspector] public PlayerStatsManager playerStatsManager;
        [HideInInspector] public PlayerInventoryManager playerInventoryManager;
        [HideInInspector] public PlayerEquipmentManager playerEquipmentManager;
        [HideInInspector] public PlayerCombatManager playerCombatManager;
        
        protected override void Awake()
        {
            base.Awake();

            playerLocomotionManager = GetComponent<PlayerLocomotionManager>();
            playerAnimationManager = GetComponent<PlayerAnimationManager>();
            playerNetworkManager = GetComponent<PlayerNetworkManager>();
            playerStatsManager = GetComponent<PlayerStatsManager>();
            playerInventoryManager = GetComponent<PlayerInventoryManager>();
            playerEquipmentManager = GetComponent<PlayerEquipmentManager>();
            playerCombatManager = GetComponent<PlayerCombatManager>();
        }

        protected override void Update()
        {
            base.Update();
            
            if(!IsOwner) 
                return;
            
            playerLocomotionManager.HandleAllMovement();
            
            playerStatsManager.RegenerateStamina();
        }

        protected override void LateUpdate()
        {
            if(!IsOwner)
                return;
            
            base.LateUpdate();
            
            PlayerCamera.instance.HandleAllCameraActions();
        }

        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();

            NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnectedCallback;
            
            if (IsOwner)
            {
                PlayerCamera.instance.player = this;
                PlayerInputManager.instance.player = this;
                WorldSaveGameManager.instance.player = this;
                
                // Update the total amount of health or stamina when the stat linked to either changes
                playerNetworkManager.vitality.OnValueChanged += playerNetworkManager.SetNewMaxHealthValue;
                playerNetworkManager.endurance.OnValueChanged += playerNetworkManager.SetNewMaxStaminaValue;
                
                // Updates UI stat bars when a stat changes
                playerNetworkManager.currentHealth.OnValueChanged +=
                    PlayerUIManager.instance.playerUIHudManager.SetNewHealthValue;

                playerNetworkManager.currentStamina.OnValueChanged +=
                    PlayerUIManager.instance.playerUIHudManager.SetNewStaminaValue;
                
                playerNetworkManager.currentStamina.OnValueChanged +=
                    playerStatsManager.ResetStaminaRegenerationTimer;
            }

            // Stats
            playerNetworkManager.currentHealth.OnValueChanged += playerNetworkManager.CheckHP;

            // Lock On
            playerNetworkManager.isLockedOn.OnValueChanged += playerNetworkManager.OnCurrentIsLockedOnChanged;
            playerNetworkManager.currentTargetNetworkObjectID.OnValueChanged +=
                playerNetworkManager.OnCurrentLockOnTargetIDChange;
            
            // Equipment
            playerNetworkManager.currentRightHandedWeaponID.OnValueChanged +=
                playerNetworkManager.OnCurrentRightHandWeaponIDChanged;

            playerNetworkManager.currentLeftHandedWeaponID.OnValueChanged +=
                playerNetworkManager.OnCurrenLeftHandWeaponIDChanged;

            playerNetworkManager.currentWeaponBeingUsed.OnValueChanged +=
                playerNetworkManager.OnCurrentWeaponBeingUsedIDChange;
            
            // Flags
            playerNetworkManager.isChargingAttack.OnValueChanged +=
                playerNetworkManager.OnIsChargingAttackChanged;

            // Connecting as client
            if (IsOwner && !IsServer)
            {
                LoadGameDataToCurrentCharacterData(ref WorldSaveGameManager.instance.currentCharacterData);
            }
        }

        public override void OnNetworkDespawn()
        {
            base.OnNetworkDespawn();
            
            NetworkManager.Singleton.OnClientConnectedCallback -= OnClientConnectedCallback;
            
            if (IsOwner)
            {
                // Update the total amount of health or stamina when the stat linked to either changes
                playerNetworkManager.vitality.OnValueChanged += playerNetworkManager.SetNewMaxHealthValue;
                playerNetworkManager.endurance.OnValueChanged += playerNetworkManager.SetNewMaxStaminaValue;
                
                // Updates UI stat bars when a stat changes
                playerNetworkManager.currentHealth.OnValueChanged -=
                    PlayerUIManager.instance.playerUIHudManager.SetNewHealthValue;

                playerNetworkManager.currentStamina.OnValueChanged -=
                    PlayerUIManager.instance.playerUIHudManager.SetNewStaminaValue;
                
                playerNetworkManager.currentStamina.OnValueChanged -=
                    playerStatsManager.ResetStaminaRegenerationTimer;
            }

            // Stats
            playerNetworkManager.currentHealth.OnValueChanged -= playerNetworkManager.CheckHP;

            // Lock On
            playerNetworkManager.isLockedOn.OnValueChanged += playerNetworkManager.OnCurrentIsLockedOnChanged;
            playerNetworkManager.currentTargetNetworkObjectID.OnValueChanged -=
                playerNetworkManager.OnCurrentLockOnTargetIDChange;
            
            // Equipment
            playerNetworkManager.currentRightHandedWeaponID.OnValueChanged -=
                playerNetworkManager.OnCurrentRightHandWeaponIDChanged;

            playerNetworkManager.currentLeftHandedWeaponID.OnValueChanged -=
                playerNetworkManager.OnCurrenLeftHandWeaponIDChanged;

            playerNetworkManager.currentWeaponBeingUsed.OnValueChanged -=
                playerNetworkManager.OnCurrentWeaponBeingUsedIDChange;
            
            // Flags
            playerNetworkManager.isChargingAttack.OnValueChanged -=
                playerNetworkManager.OnIsChargingAttackChanged;
        }

        private void OnClientConnectedCallback(ulong clientID)
        {
            WorldGameSessionManager.instance.AddPlayerToActivePlayerList(this);
            
            if (!IsServer && IsOwner)
            {
                foreach (PlayerManager player in WorldGameSessionManager.instance.players)
                {
                    if (player != this)
                    {
                        player.LoadOtherPlayerCharacterWhenJoiningSever();
                    }
                }   
            }
        }

        public override IEnumerator ProcessDeathEvent(bool manuallySelectDeathAnimation = false)
        {
            if (IsOwner)
            {
                PlayerUIManager.instance.playerUIPopUpManager.SendYouDiedPopUp();
            }
            
            return base.ProcessDeathEvent(manuallySelectDeathAnimation);
        }

        public override void ReviveCharacter()
        {
            base.ReviveCharacter();

            playerNetworkManager.isDead.Value = false;

            if (IsOwner)
            {
                playerNetworkManager.currentHealth.Value = playerNetworkManager.maxHealth.Value;
                playerNetworkManager.currentStamina.Value = playerNetworkManager.maxStamina.Value;
                
                // Play rebirth effect
                playerAnimationManager.PlayTargetActionAnimation("Empty", false);
            }
        }

        public void SaveGameDataToCurrentCharacterData(ref CharacterSaveData currentCharacterData)
        {
            currentCharacterData.sceneIndex = SceneManager.GetActiveScene().buildIndex;
            currentCharacterData.characterName = playerNetworkManager.characterName.Value.ToString();
            currentCharacterData.xPosition = transform.position.x;
            currentCharacterData.yPosition = transform.position.y;
            currentCharacterData.zPosition = transform.position.z;

            currentCharacterData.currentHealth = playerNetworkManager.currentHealth.Value;
            currentCharacterData.currentStamina = playerNetworkManager.currentStamina.Value;
            
            currentCharacterData.vitality = playerNetworkManager.vitality.Value;
            currentCharacterData.endurance = playerNetworkManager.endurance.Value;
        }

        public void LoadGameDataToCurrentCharacterData(ref CharacterSaveData currentCharacterData)
        {
            playerNetworkManager.characterName.Value = currentCharacterData.characterName;
            Vector3 characterPosition = new Vector3(currentCharacterData.xPosition, currentCharacterData.yPosition, currentCharacterData.zPosition);
            transform.position = characterPosition;

            playerNetworkManager.vitality.Value = currentCharacterData.vitality;
            playerNetworkManager.endurance.Value = currentCharacterData.endurance;
            
            playerNetworkManager.maxHealth.Value = playerStatsManager.CalculateHealthBasedOnVitalityLevel(currentCharacterData.vitality);
            playerNetworkManager.maxStamina.Value = playerStatsManager.CalculateStaminaBasedOnEnduranceLevel(currentCharacterData.endurance);
            playerNetworkManager.currentHealth.Value = currentCharacterData.currentHealth;
            playerNetworkManager.currentStamina.Value = currentCharacterData.currentStamina;
            PlayerUIManager.instance.playerUIHudManager.SetMaxStaminaValue(playerNetworkManager.maxStamina.Value);
        }

        public void LoadOtherPlayerCharacterWhenJoiningSever()
        {
            // Sync Weapons
            playerNetworkManager.OnCurrentWeaponBeingUsedIDChange(0, playerNetworkManager.currentRightHandedWeaponID.Value);
            playerNetworkManager.OnCurrentWeaponBeingUsedIDChange(0, playerNetworkManager.currentLeftHandedWeaponID.Value);
            
            // Sync Armor
            
            // Lock On
            playerNetworkManager.OnCurrentLockOnTargetIDChange(0, playerNetworkManager.currentTargetNetworkObjectID.Value);
        }
    }
}