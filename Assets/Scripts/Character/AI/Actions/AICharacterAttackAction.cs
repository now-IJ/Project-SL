using UnityEngine;

namespace RS
{
    public class AICharacterAttackAction : ScriptableObject
    {
        [Header("Attack")] 
        [SerializeField] private string attackAnimation;
        
        [Header("Combo Action")] 
        public AICharacterAttackAction comboAction;

        [Header("Action Values")] 
        [SerializeField] private AttackType attackType;
        public int attackWeight = 50;
        public float actionRecoveryTime = 1.5f;
        public float minimumAttackAngle = -35;
        public float maximumAttackAngle = 35;
        public float minimumAttackRange = 0;
        public float maximumAttackRange = 2;
        
        
        public void AttemptToPerformAction(AICharacterManager aiCharacter)
        {
            aiCharacter.characterAnimationManager.PlayTargetAttackActionAnimation(attackType, attackAnimation, true);    
        }
    }
}