using UnityEngine;
using UnityEngine.Serialization;

namespace RS
{
    [CreateAssetMenu(menuName = "Character Actions/Weapon Actions/Light Attack Action")]
    public class LightAttackWeaponItemAction : WeaponItemAction
    {
        [SerializeField] private string light_Attack_01 = "Main_Light_Attack_01";
        [SerializeField] private string light_Attack_02 = "Main_Light_Attack_02";
        
        public override void AttemptToPerformAction(PlayerManager playerPerformingAction, WeaponItem weaponPerformingAction)
        {
            base.AttemptToPerformAction(playerPerformingAction, weaponPerformingAction);
            
            if(!playerPerformingAction.IsOwner)
                return;
            
            if(playerPerformingAction.playerNetworkManager.currentStamina.Value <= 0)
                return;
            
            if(!playerPerformingAction.isGrounded)
                return;
            
            PerformingLightAttack(playerPerformingAction, weaponPerformingAction);
        }

        private void PerformingLightAttack(PlayerManager playerPerformingAction, WeaponItem weaponPerformingAction)
        {
            // If player is attacking and can combo perform the combo
            if (playerPerformingAction.playerCombatManager.canComboWithMainHandWeapon && playerPerformingAction.isPerformingAction)
            {
                playerPerformingAction.playerCombatManager.canComboWithMainHandWeapon = false;
                
                // Perform an attack based on previous attack
                if (playerPerformingAction.playerCombatManager.lastAttackAnimationPerformed == light_Attack_01)
                {
                    playerPerformingAction.playerAnimationManager.PlayTargetAttackActionAnimation(AttackType.LightAttack02, light_Attack_02, true);
                }
                else
                {
                    playerPerformingAction.playerAnimationManager.PlayTargetAttackActionAnimation(AttackType.LightAttack01, light_Attack_01, true);
                }
            }
            // just perform regular attack
            else if(!playerPerformingAction.isPerformingAction)
            {
                playerPerformingAction.playerAnimationManager.PlayTargetAttackActionAnimation(AttackType.LightAttack01, light_Attack_01, true);
            }
            
        }
    }
}