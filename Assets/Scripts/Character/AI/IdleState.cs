using UnityEngine;

namespace RS
{
    [CreateAssetMenu(menuName = "AI/States/Idle")]
    public class IdleState : AIState
    {
        public override AIState Tick(AICharacterManager aiCharacter)
        {
            if (aiCharacter.characterCombatManager.currentTarget != null)
            {
                // Return the pursue target
                return SwitchState(aiCharacter, aiCharacter.pursueTarget);
            }
            else
            {
                // Return this state to continuously search for a target
                aiCharacter.aiCharacterCombatManager.FindATargetViaLineOfSight(aiCharacter);
                return this;
            }

        }
    }
}