using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace RS
{
    public class PlayerManager : CharacterManager
    {
        [Header("Debug Menu")] 
        [SerializeField] private bool respawnCharacter = false;
        [SerializeField] private bool switchRightWeapon = false;
        [SerializeField] private bool switchLeftWeapon = false;
        
        [HideInInspector] public PlayerAnimationManager playerAnimationManager;
        [HideInInspector] public PlayerLocomotionManager playerLocomotionManager;
        [HideInInspector] public PlayerNetworkManager playerNetworkManager;
        [HideInInspector] public PlayerStatsManager playerStatsManager;
        [HideInInspector] public PlayerInventoryManager playerInventoryManager;
        [HideInInspector] public PlayerEquipmentManager playerEquipmentManager;
        
        protected override void Awake()
        {
            base.Awake();

            playerLocomotionManager = GetComponent<PlayerLocomotionManager>();
            playerAnimationManager = GetComponent<PlayerAnimationManager>();
            playerNetworkManager = GetComponent<PlayerNetworkManager>();
            playerStatsManager = GetComponent<PlayerStatsManager>();
            playerInventoryManager = GetComponent<PlayerInventoryManager>();
            playerEquipmentManager = GetComponent<PlayerEquipmentManager>();
        }

        protected override void Update()
        {
            base.Update();
            
            if(!IsOwner) 
                return;
            
            playerLocomotionManager.HandleAllMovement();
            
            playerStatsManager.RegenerateStamina();
            
            DebugMenu();
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

            playerNetworkManager.currentHealth.OnValueChanged += playerNetworkManager.CheckHP;

            
            // Equipment
            playerNetworkManager.currentRightHandedWeaponID.OnValueChanged +=
                playerNetworkManager.OnCurrentRightHandWeaponIDChanged;

            playerNetworkManager.currentLeftHandedWeaponID.OnValueChanged +=
                playerNetworkManager.OnCurrenLeftHandWeaponIDChanged;
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

        private void DebugMenu()
        {
            if (respawnCharacter)
            {
                respawnCharacter = false;
                ReviveCharacter();
            }

            if (switchRightWeapon)
            {
                switchRightWeapon = false;
                playerEquipmentManager.SwitchRightWeapon();
            }
            if (switchLeftWeapon)
            {
                switchLeftWeapon = false;
                playerEquipmentManager.SwitchLeftWeapon();
            }
        }
    }
}