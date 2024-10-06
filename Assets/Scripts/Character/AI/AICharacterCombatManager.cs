using System;
using UnityEngine;

namespace RS{
    public class AICharacterCombatManager : CharacterCombatManager
    {

        [Header("Target Information")] 
        public float distanceToTarget;
        public float viewableAngle;
        public Vector3 targetsDirection;
        
        [Header("Detection")] 
        [SerializeField] private float detectionRadius = 15;
        public float minimumFOV = -35;
        public float maximumFOV = 35;

        [Header("Recovery")] 
        public float actionRecoveryTimer = 0;

        [Header("Attack Rotation Speed")] 
        public float attackRotationSpeed = 25;
    
        public void FindATargetViaLineOfSight(AICharacterManager aiCharacter)
        {
            if (currentTarget != null)
                return;
            
            Collider[] colliders = Physics.OverlapSphere(aiCharacter.transform.position, detectionRadius, WorldUtilityManager.instance.GetCharacterLayers());

            for (int i = 0; i < colliders.Length; i++)
            {
                CharacterManager targetCharacter = colliders[i].transform.GetComponent<CharacterManager>();
                
                if(targetCharacter == null)
                    continue;
                
                if(targetCharacter == aiCharacter)
                    continue;
                
                if(targetCharacter.characterNetworkManager.isDead.Value)
                    continue;

                // Check if target is attackable
                if (WorldUtilityManager.instance.TargetIsDamageable(aiCharacter.characterCombatManager.characterGroup,
                        targetCharacter.characterCombatManager.characterGroup))
                {
                    // Is target infront of attacker?
                    Vector3 targetsDirection = targetCharacter.transform.position - aiCharacter.transform.position;

                    float angleOfPotentialTarget = Vector3.Angle(targetsDirection, aiCharacter.transform.forward);

                    if (angleOfPotentialTarget > minimumFOV && angleOfPotentialTarget < maximumFOV)
                    {
                        // Is in line of sight
                        if(Physics.Linecast(
                               aiCharacter.characterCombatManager.lockOnTargetTransform.position, 
                               targetCharacter.characterCombatManager.lockOnTargetTransform.position, 
                               WorldUtilityManager.instance.GetEnvironmentLayers()))
                        {
                            Debug.Log("BLOCKED LINE OF SIGHT");
                        }
                        else
                        {   
                            targetsDirection = targetCharacter.transform.position - transform.position;
                            viewableAngle = WorldUtilityManager.instance.GetAngleOfTarget(transform, targetsDirection);
                            aiCharacter.characterCombatManager.SetTarget(targetCharacter);
                            PivotTowardsTarget(aiCharacter);
                        }
                    }
                }
            }
        }

        public void PivotTowardsTarget(AICharacterManager aiCharacter)
        {
            if(aiCharacter.isPerformingAction)
                return;
            
            // Turn 90 right
            if (viewableAngle > 0 && viewableAngle <= 90)
            {
                aiCharacter.characterAnimationManager.PlayTargetActionAnimation("Turn_Right_90", true);
            }
            // Turn 180 right
            else if (viewableAngle > 90 && viewableAngle <= 180)
            {
                aiCharacter.characterAnimationManager.PlayTargetActionAnimation("Turn_Right_90", true);
            }
            // Turn 90 Left
            else if (viewableAngle < 0 && viewableAngle >= -90)
            {
                aiCharacter.characterAnimationManager.PlayTargetActionAnimation("Turn_Left_90", true);
            }
            // Turn 180 Left
            else if (viewableAngle < -90 && viewableAngle >= -180)
            {
                aiCharacter.characterAnimationManager.PlayTargetActionAnimation("Turn_Left_180", true);
            }
        }

        public void RotateTowardsAgent(AICharacterManager aiCharacter)
        {
            if (aiCharacter.aiCharacterNetworkManager.isMoving.Value)
            {
                aiCharacter.transform.rotation = aiCharacter.navMeshAgent.transform.rotation;
            }
        }

        public void RotateTowardsTarget(AICharacterManager aiCharacter)
        {
            if(currentTarget == null)
                return;
            
            if(!aiCharacter.canRotate)
                return;

            if(!aiCharacter.isPerformingAction)
                return;
            
            Vector3 targetDirection = currentTarget.transform.position - aiCharacter.transform.position;
            targetDirection.Normalize();
            targetDirection.y = 0;

            if (targetDirection == Vector3.zero)
                targetDirection = aiCharacter.transform.forward;

            Quaternion targetRotation = Quaternion.LookRotation(targetDirection);
            
            aiCharacter.transform.rotation = Quaternion.Slerp(aiCharacter.transform.rotation, targetRotation, attackRotationSpeed * Time.deltaTime);
        }
        
        public void HandleActionRecovery(AICharacterManager aiCharacter)
        {
            if (actionRecoveryTimer > 0)
            {
                if (!aiCharacter.isPerformingAction)
                {
                    actionRecoveryTimer -= Time.deltaTime;
                }
            }
        }
    }
}