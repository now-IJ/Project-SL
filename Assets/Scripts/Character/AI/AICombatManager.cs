using System;
using UnityEngine;

namespace RS{
    public class AICombatManager : CharacterCombatManager
    {

        [Header("Detection")] 
        [SerializeField] private float detectionRadius = 15;
        [SerializeField] private float minimumViewableAngle = -35;
        [SerializeField] private float maximumViewableAngle = 35;

        
    
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

                    float viewableAngle = Vector3.Angle(targetsDirection, aiCharacter.transform.forward);

                    if (viewableAngle > minimumViewableAngle && viewableAngle < maximumViewableAngle)
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
                            aiCharacter.characterCombatManager.SetTarget(targetCharacter);
                        }
                    }
                }
            }
        }
    }
}