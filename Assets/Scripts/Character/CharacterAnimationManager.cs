using System;
using Unity.Netcode;
using UnityEngine;

namespace RS
{
    public class CharacterAnimationManager : MonoBehaviour
    {
        private CharacterManager character;

        private float vertical;
        private float horizontal;
        
        protected virtual void Awake()
        {
            character = GetComponent<CharacterManager>();
        }

        public void UpdateAnimatorMovementParameters(float horizontalValue, float verticalValue)
        {
            character.animator.SetFloat("Horizontal", horizontalValue, 0.1f, Time.deltaTime);
            character.animator.SetFloat("Vertical", verticalValue, 0.1f, Time.deltaTime);
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