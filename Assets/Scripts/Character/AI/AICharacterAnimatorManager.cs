using System;
using UnityEngine;

namespace RS
{
    public class AICharacterAnimatorManager : CharacterAnimationManager
    {
        private AICharacterManager aiCharacter;

        protected override void Awake()
        {
            base.Awake();

            aiCharacter = GetComponent<AICharacterManager>();
        }

        private void OnAnimatorMove()
        {
            // Host
            if (aiCharacter.IsOwner)
            {
                if (!aiCharacter.isGrounded)
                    return;

                Vector3 velocity = aiCharacter.animator.deltaPosition;

                aiCharacter.characterController.Move(velocity);
                aiCharacter.transform.rotation *= aiCharacter.animator.deltaRotation;
            }
            // Client
            else
            {
                if (!aiCharacter.isGrounded)
                    return;

                Vector3 velocity = aiCharacter.animator.deltaPosition;

                aiCharacter.characterController.Move(velocity);
                
                aiCharacter.transform.position = Vector3.SmoothDamp(
                    transform.position,
                    aiCharacter.characterNetworkManager.networkPosition.Value,
                    ref aiCharacter.aiCharacterNetworkManager.networkPositionVelocity,
                    aiCharacter.aiCharacterNetworkManager.networkPositionSmoothTime);
                
                aiCharacter.transform.rotation *= aiCharacter.animator.deltaRotation;
            }
        }
    }
}