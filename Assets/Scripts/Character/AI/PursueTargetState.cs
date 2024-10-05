using UnityEngine;
using UnityEngine.AI;

namespace RS
{
    [CreateAssetMenu(menuName = "AI/States/Pursue Target")]
    public class PursueTargetState : AIState
    {
        public override AIState Tick(AICharacterManager aiCharacter)
        {

            // Check if performing action
            if (aiCharacter.isPerformingAction)
                return this;

            // Check if target is null
            if (aiCharacter.aiCharacterCombatManager.currentTarget == null)
                return SwitchState(aiCharacter, aiCharacter.idle);

            // Check if NavMesh is active
            if (!aiCharacter.navMeshAgent.enabled)
                aiCharacter.navMeshAgent.enabled = true;
            
            aiCharacter.aiCharacterLocomotionManager.RotateTowardsAgent(aiCharacter);

            // Check if is in combat range

            // Check if target is not reachable, go home

            // Pursue target
            NavMeshPath path = new NavMeshPath();
            aiCharacter.navMeshAgent.CalculatePath(aiCharacter.aiCharacterCombatManager.currentTarget.transform.position, path);
            aiCharacter.navMeshAgent.SetPath(path);
            
            return this;
        }
    }
}