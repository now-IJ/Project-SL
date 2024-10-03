using UnityEngine;

namespace RS
{
    [CreateAssetMenu(menuName = "Character Actions/Weapon Actions/Heavy Attack Action")]
    public class HeavyAttackWeaponItemAction : WeaponItemAction
    {
        [SerializeField] private string heavy_Attack_01 = "Main_Heavy_Attack_01";
        [SerializeField] private string heavy_Attack_02 = "Main_Heavy_Attack_02";
        [SerializeField] private string heavy_Attack_03 = "Main_Heavy_Attack_03";
        
        public override void AttemptToPerformAction(PlayerManager playerPerformingAction, WeaponItem weaponPerformingAction)
        {
            base.AttemptToPerformAction(playerPerformingAction, weaponPerformingAction);
            
            if(!playerPerformingAction.IsOwner)
                return;
            
            if(playerPerformingAction.playerNetworkManager.currentStamina.Value <= 0)
                return;
            
            if(!playerPerformingAction.isGrounded)
                return;
            
            PerformingHeavyAttack(playerPerformingAction, weaponPerformingAction);
        }

        private void PerformingHeavyAttack(PlayerManager playerPerformingAction, WeaponItem weaponPerformingAction)
        {
            // If player is attacking and can combo perform the combo
            if (playerPerformingAction.playerCombatManager.canComboWithMainHandWeapon && playerPerformingAction.isPerformingAction)
            {
                playerPerformingAction.playerCombatManager.canComboWithMainHandWeapon = false;
                
                // Perform an attack based on previous attack
                if (playerPerformingAction.playerCombatManager.lastAttackAnimationPerformed == heavy_Attack_01)
                {
                    playerPerformingAction.playerAnimationManager.PlayTargetAttackActionAnimation(AttackType.HeavyAttack02, heavy_Attack_02, true);
                }
                else if(playerPerformingAction.playerCombatManager.lastAttackAnimationPerformed == heavy_Attack_02)
                {
                    playerPerformingAction.playerAnimationManager.PlayTargetAttackActionAnimation(AttackType.HeavyAttack03, heavy_Attack_03, true);
                }
                else
                {
                    playerPerformingAction.playerAnimationManager.PlayTargetAttackActionAnimation(AttackType.HeavyAttack01, heavy_Attack_01, true);
                }
            }
            // just perform regular attack
            else if(!playerPerformingAction.isPerformingAction)
            {
                playerPerformingAction.playerAnimationManager.PlayTargetAttackActionAnimation(AttackType.HeavyAttack01, heavy_Attack_01, true);
            }
        }
    }
}