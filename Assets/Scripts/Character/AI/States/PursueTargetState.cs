using UnityEngine;
using UnityEngine.AI;

namespace RS
{
    [CreateAssetMenu(menuName = "AI/States/Pursue Target")]
    public class PursueTargetState : AIState
    {
        [SerializeField] protected bool enablePivot = true;
        
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

            // Check if target is out of view and then pivot
            if (enablePivot)
            {
                if (aiCharacter.aiCharacterCombatManager.viewableAngle <
                    aiCharacter.aiCharacterCombatManager.minimumFOV ||
                    aiCharacter.aiCharacterCombatManager.viewableAngle >
                    aiCharacter.aiCharacterCombatManager.maximumFOV)
                {
                    aiCharacter.aiCharacterCombatManager.PivotTowardsTarget(aiCharacter);
                }
            }

            aiCharacter.aiCharacterLocomotionManager.RotateTowardsAgent(aiCharacter);

            // Check if is in combat range
            if (aiCharacter.aiCharacterCombatManager.distanceToTarget <= aiCharacter.navMeshAgent.stoppingDistance)
                return SwitchState(aiCharacter, aiCharacter.attack);
            
            // Check if target is not reachable, go home

            aiCharacter.characterAnimationManager.UpdateAnimatorMovementParameters(0,1,false);
            
            // Pursue target
            NavMeshPath path = new NavMeshPath();
            aiCharacter.navMeshAgent.CalculatePath(aiCharacter.aiCharacterCombatManager.currentTarget.transform.position, path);
            aiCharacter.navMeshAgent.SetPath(path);
            
            
            return this;
        }
    }
}