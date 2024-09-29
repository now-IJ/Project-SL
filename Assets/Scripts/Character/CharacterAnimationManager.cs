using System;
using Unity.Netcode;
using UnityEngine;

namespace RS
{
    public class CharacterAnimationManager : MonoBehaviour
    {
        private CharacterManager character;

        private int vertical;
        private int horizontal;
        
        protected virtual void Awake()
        {
            character = GetComponent<CharacterManager>();

            vertical = Animator.StringToHash("Vertical");
            horizontal = Animator.StringToHash("Horizontal");
        }

        public void UpdateAnimatorMovementParameters(float horizontalValue, float verticalValue, bool isSprinting)
        {
            float horizontalAmount = horizontalValue;
            float verticalAmount = verticalValue;
            if (isSprinting)
            {
                verticalAmount = 2;
            }
            character.animator.SetFloat(horizontal, horizontalAmount, 0.1f, Time.deltaTime);
            character.animator.SetFloat(vertical, verticalAmount, 0.1f, Time.deltaTime);
            
        }

        public virtual void PlayTargetActionAnimation(string targetAnimation, bool isPerformingAction,
            bool applyRootMotion = true, bool canRotate = false, bool canMove = false)
        {
            character.animator.CrossFade(targetAnimation, 0.2f);
            character.isPerformingAction = isPerformingAction;
            character.applyRootMotion = applyRootMotion;
            character.canRotate = canRotate;
            character.canMove = canMove;

            character.characterNetworkManager.NotifyServerOfActionAnimationServerRPC
            (NetworkManager.Singleton.LocalClientId, targetAnimation, applyRootMotion);
        }
    }
}