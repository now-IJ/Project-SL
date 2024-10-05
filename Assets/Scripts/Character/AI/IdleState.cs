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
                Debug.Log("HAS TARGET");
                return this;
            }
            else
            {
                // Return this state to continuously search for a target
                aiCharacter.aiCombatManager.FindATargetViaLineOfSight(aiCharacter);
                Debug.Log("Searching Target");
                return this;
            }

        }
    }
}