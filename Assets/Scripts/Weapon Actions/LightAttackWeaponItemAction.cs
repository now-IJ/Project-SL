using UnityEngine;
using UnityEngine.Serialization;

namespace RS
{
    [CreateAssetMenu(menuName = "Character Actions/Weapon Actions/Light Attack Action")]
    public class LightAttackWeaponItemAction : WeaponItemAction
    {
        [SerializeField] private string light_Attack_01 = "Main_Light_Attack_01";
        
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
            if (playerPerformingAction.playerNetworkManager.isUsingRightHand.Value)
            {
                playerPerformingAction.playerAnimationManager.PlayTargetAttackActionAnimation(light_Attack_01, true);
            }

            if (playerPerformingAction.playerNetworkManager.isUsingLeftHand.Value)
            {
                
            }
        }
    }
}