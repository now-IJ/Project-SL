using UnityEngine;

namespace RS
{
    public class PlayerCombatManager : CharacterCombatManager
    {
        private PlayerManager player;
        
        public WeaponItem currentWeaponBeingUsed;

        protected override void Awake()
        {
            base.Awake();

            player = GetComponent<PlayerManager>();
        }

        public void PerformWeaponBasedAction(WeaponItemAction weaponAction, WeaponItem weaponPerformingAction)
        {
            weaponAction.AttemptToPerformAction(player, weaponPerformingAction);
        }
    }
}