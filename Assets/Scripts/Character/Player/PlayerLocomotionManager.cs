using UnityEngine;

namespace RS
{
    public class PlayerLocomotionManager : CharacterLocomotionManager
    {
        private PlayerManager player;
        
        [HideInInspector] public float verticalMovement;
        [HideInInspector] public float horizontalMovement;
        [HideInInspector] public float moveAmount;
        
        [Header("Movement Settings")]        
        [SerializeField] private float walkingSpeed = 2;
        [SerializeField] private float runningSpeed = 5;
        [SerializeField] private float sprintingSpeed = 7;
        [SerializeField] private float rotationSpeed = 15;
        private Vector3 movementDirection;
        private Vector3 targetRotationDirection;

        [Header("Dodge")] 
        private Vector3 rollDirection;
        
        protected override void Awake()
        {
            base.Awake();

            player = GetComponent<PlayerManager>();
        }

        protected override void Update()
        {
            base.Update();

            if (player.IsOwner)
            {
                player.characterNetworkManager.verticalMovement.Value = verticalMovement;
                player.characterNetworkManager.horizontalMovement.Value = horizontalMovement;
                player.characterNetworkManager.moveAmount.Value = moveAmount;
            }
            else
            {
                verticalMovement = player.characterNetworkManager.verticalMovement.Value;
                horizontalMovement = player.characterNetworkManager.horizontalMovement.Value;
                moveAmount = player.characterNetworkManager.moveAmount.Value;
                
                player.playerAnimationManager.UpdateAnimatorMovementParameters(0, moveAmount, player.playerNetworkManager.isSprinting.Value);
            }
        }

        public void HandleAllMovement()
        {
            HandleGroundedMovement();
            HandleRotation();
        }

        private void SetMovementValues()
        {
            verticalMovement = PlayerInputManager.instance.verticalInput;
            horizontalMovement = PlayerInputManager.instance.horizontalInput;
            moveAmount = PlayerInputManager.instance.moveAmount;
        }

        private void HandleGroundedMovement()
        {
            if(!player.canMove)
                return;
            
            SetMovementValues();
         
            movementDirection = PlayerCamera.instance.transform.forward * verticalMovement;
            movementDirection += PlayerCamera.instance.transform.right * horizontalMovement;
            movementDirection.Normalize();
            movementDirection.y = 0;

            if (player.playerNetworkManager.isSprinting.Value)
            {
                player.characterController.Move(movementDirection * (sprintingSpeed * Time.deltaTime));
            }
            else
            {
                if (PlayerInputManager.instance.moveAmount > 0.5f)
                {
                    // Running Speed
                    player.characterController.Move(movementDirection * (runningSpeed * Time.deltaTime));
                }
                else if (PlayerInputManager.instance.moveAmount <= 0.5f)
                {
                    // Walking Speed
                    player.characterController.Move(movementDirection * (walkingSpeed * Time.deltaTime));
                }
            }
        }

        private void HandleRotation()
        {
            if(!player.canRotate)
                return;
            
            targetRotationDirection = Vector3.zero;
            targetRotationDirection = PlayerCamera.instance.cameraObject.transform.forward * verticalMovement;
            targetRotationDirection += PlayerCamera.instance.cameraObject.transform.right * horizontalMovement;
            targetRotationDirection.Normalize();
            targetRotationDirection.y = 0;

            if (targetRotationDirection == Vector3.zero)
            {
                targetRotationDirection = transform.forward;
            }

            Quaternion newRotation = Quaternion.LookRotation(targetRotationDirection);
            Quaternion targetRotation = Quaternion.Slerp(transform.rotation, newRotation, rotationSpeed * Time.deltaTime);
            transform.rotation = targetRotation;
        }

        public void AttemptToPerformDodge()
        {
            if(player.isPerformingAction)
                return;
            
            // If player is moving while dodging perform a roll
            if (moveAmount > 0)
            {
                rollDirection = PlayerCamera.instance.cameraObject.transform.forward * verticalMovement;
                rollDirection += PlayerCamera.instance.cameraObject.transform.right * horizontalMovement;
                rollDirection.y = 0;
                rollDirection.Normalize();

                Quaternion playerRotation = Quaternion.LookRotation(rollDirection);
                player.transform.rotation = playerRotation;
                
                player.playerAnimationManager.PlayTargetActionAnimation("Roll_Forward_01", true, true);
            }
            // If not perform a backstep
            else
            {
                player.playerAnimationManager.PlayTargetActionAnimation("Back_Step_01", true, true);
            }
        }

        public void HandleSprinting()
        {
            // Player cannot sprint when performing an action
            if (player.isPerformingAction)
            {
                player.playerNetworkManager.isSprinting.Value = false;
            }
            
            // Player can only sprint if he is already walking and not tip toeing / stationary
            if (moveAmount >= 0.5f)
            {
                player.playerNetworkManager.isSprinting.Value = true;
            }
            else
            {
                player.playerNetworkManager.isSprinting.Value = false;
            }
            
        }
    }
}