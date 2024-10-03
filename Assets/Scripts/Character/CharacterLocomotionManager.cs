using System;
using UnityEngine;

namespace RS
{
    public class CharacterLocomotionManager : MonoBehaviour
    {
        private CharacterManager character;

        [Header("Ground Check & Jumping")] 
        [SerializeField] protected float gravityForce = -5.55f;
        [SerializeField] private float groundCheckRadius = 1;
        [SerializeField] private LayerMask groundLayer;
        [SerializeField] private protected Vector3 yVelocity; // The force at which the player is pulled up or down Jumping/Falling
        [SerializeField] protected float groundedYVelocity = -20; // The force at which the player is sticking to the ground when he is grounded
        [SerializeField] protected float fallStartVelocity = -5; //The force at which the player begins to fall when they become ungrounded 
        protected bool fallingVelocityHasBeenSet = false;
        protected float inAirTime = 0;
        
        [Header("Flags")]
        public bool isRolling = false;
        
        protected virtual void Awake()
        {
            character = GetComponent<CharacterManager>();
        }

        protected virtual void Update()
        {
            HandleGroundCheck();

            if (character.isGrounded)
            {
                // not jumping
                if (yVelocity.y < 0)
                {
                    inAirTime = 0;
                    fallingVelocityHasBeenSet = false;
                    yVelocity.y = groundedYVelocity;
                }
            }
            else
            {
                if (!character.characterNetworkManager.isJumping.Value && !fallingVelocityHasBeenSet)
                {
                    fallingVelocityHasBeenSet = true;
                    yVelocity.y = fallStartVelocity;
                }

                inAirTime = inAirTime + Time.deltaTime;

                yVelocity.y += gravityForce * Time.deltaTime;
                character.animator.SetFloat("InAirTime", inAirTime);
               
            }
            
            character.characterController.Move(yVelocity * Time.deltaTime);
        }

        protected void HandleGroundCheck()
        {
            character.isGrounded = Physics.CheckSphere(character.transform.position, groundCheckRadius, groundLayer);
        }

        protected void OnDrawGizmosSelected()
        {
           //Gizmos.DrawSphere(character.transform.position, 20);
        }
    }
}
