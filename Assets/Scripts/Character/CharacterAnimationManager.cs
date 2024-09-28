using System;
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
    }
}