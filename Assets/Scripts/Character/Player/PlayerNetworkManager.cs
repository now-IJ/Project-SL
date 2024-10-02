using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

namespace RS
{
    public class PlayerNetworkManager : CharacterNetworkManager
    {
        private PlayerManager player;
        
        public NetworkVariable<FixedString64Bytes> characterName =
            new NetworkVariable<FixedString64Bytes>("Character", 
                NetworkVariableReadPermission.Everyone,
                NetworkVariableWritePermission.Owner);

        [Header("Equipment")] 
        public NetworkVariable<int> currentRightHandedWeaponID =
            new NetworkVariable<int>(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
        
        public NetworkVariable<int> currentLeftHandedWeaponID =
            new NetworkVariable<int>(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
        
        public NetworkVariable<int> currentWeaponBeingUsed =
            new NetworkVariable<int>(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

        public NetworkVariable<bool> isUsingRightHand = 
            new NetworkVariable<bool>(false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

        public NetworkVariable<bool> isUsingLeftHand = 
            new NetworkVariable<bool>(false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

        protected override void Awake()
        {
            base.Awake();

            player = GetComponent<PlayerManager>();
        }

        public void SetCharacterActionHand(bool rightHandedAction)
        {
            if (rightHandedAction)
            {
                isUsingLeftHand.Value = false;
                isUsingRightHand.Value = true;
            }
            else
            {
                isUsingRightHand.Value = false;
                isUsingLeftHand.Value = true;
            }
        }

        public void SetNewMaxHealthValue(int oldVitality, int newVitality)
        {
            maxHealth.Value = player.playerStatsManager.CalculateHealthBasedOnVitalityLevel(newVitality);
            PlayerUIManager.instance.playerUIHudManager.SetMaxHealthValue(maxHealth.Value);
            currentHealth.Value = maxHealth.Value;
        }
        
        public void SetNewMaxStaminaValue(int oldEndurance, int newEndurance)
        {
            maxStamina.Value = player.playerStatsManager.CalculateStaminaBasedOnEnduranceLevel(newEndurance);
            PlayerUIManager.instance.playerUIHudManager.SetMaxStaminaValue(maxStamina.Value);
            currentStamina.Value = maxStamina.Value;
        }

        public void OnCurrentRightHandWeaponIDChanged(int oldID, int newID)
        {
            WeaponItem newWeapon = Instantiate(WorldItemDatabase.instance.GetWeaponByID(newID));

        player.playerInventoryManager.currentRightHandWeapon = newWeapon;
            player.playerEquipmentManager.LoadRightWeapon();
        }
        
        public void OnCurrenLeftHandWeaponIDChanged(int oldID, int newID)
        {
            WeaponItem newWeapon = Instantiate(WorldItemDatabase.instance.GetWeaponByID(newID));
            player.playerInventoryManager.currentLeftHandWeapon = newWeapon;
            player.playerEquipmentManager.LoadLeftWeapon();
        }
        
        public void OnCurrentWeaponBeingUsedIDChange(int oldID, int newID)
        {
            WeaponItem newWeapon = Instantiate(WorldItemDatabase.instance.GetWeaponByID(newID));
            player.playerCombatManager.currentWeaponBeingUsed = newWeapon;
        }
        
        // Item actions
        [ServerRpc]
        public void NotifyServerOfWeaponActionServerRPC(ulong clientID, int actionID, int weaponID)
        {
            if (IsServer)
            {
                NotifyServerOfWeaponActionClientRPC(clientID, actionID, weaponID);
            }
        }

        [ClientRpc]
        private void NotifyServerOfWeaponActionClientRPC(ulong clientID, int actionID, int weaponID)
        {
            if (clientID != NetworkManager.Singleton.LocalClientId)
            {
                PerformWeaponBasedAction(actionID, weaponID);
            }
        }

        private void PerformWeaponBasedAction(int actionID, int weaponID)
        {
            WeaponItemAction weaponAction = WorldActionManager.instance.GetWeaponItemActionByID(actionID);

            if (weaponAction != null)
            {
                weaponAction.AttemptToPerformAction(player, WorldItemDatabase.instance.GetWeaponByID(weaponID));
            }
            else
            {
                Debug.LogError("ACTION IS NULL");
            }
        }
    }
}