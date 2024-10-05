using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

namespace RS
{
    public class PlayerInputManager : MonoBehaviour
    {
        public static PlayerInputManager instance;
        
        public PlayerManager player;
        
        private PlayerControls playerControls;
        
        
        [Header("Camera input")]
        [SerializeField] private Vector2 camera_Input;
        public float cameraVertical_Input;
        public float cameraHorizontal_Input;

        [Header("Lock-On input")] 
        [SerializeField] private bool lockOn_Input;
        [SerializeField] private bool lockOn_Left_Input;
        [SerializeField] private bool lockOn_Right_Input;
        private Coroutine lockOnCoroutine;
        
        [Header("Movement input")]
        private Vector2 movement_Input;
        public float vertical_Input;
        public float horizontal_Input;
        public float moveAmount;
        
        [Header("Player Actions")] 
        [SerializeField] private bool dodge_Input = false;
        [SerializeField] private bool sprint_Input = false;
        [SerializeField] private bool jump_Input = false;
        
        [Header("Inventory")]
        [SerializeField] private bool switch_Right_Weapon_Input; 
        [SerializeField] private bool switch_Left_Weapon_Input; 
            
        [Header("Bumper Input")]
        [SerializeField] private bool RB_Input = false;
        
        [Header("Trigger Inputs")]
        [SerializeField] private bool RT_Input = false;
        [SerializeField] private bool Hold_RT_Input = false;

        
        
        
        
        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
            }
            else
            {
                Destroy(gameObject);
            }
            
        }

        private void Start()
        {
            DontDestroyOnLoad(gameObject);
            
            SceneManager.activeSceneChanged += OnSceneChanged;
            
            instance.enabled = false;

            if (playerControls != null)
            {
                playerControls.Disable();
            }
        }

        private void OnSceneChanged(Scene oldScene, Scene newScene)
        {
            if (newScene.buildIndex == WorldSaveGameManager.instance.GetWorldSceneIndex())
            {
                instance.enabled = true;
                
                if (playerControls != null)
                {
                    playerControls.Enable();
                }
            }
            else
            {   
                instance.enabled = false;
                
                if (playerControls != null)
                {
                    playerControls.Disable();
                }
            }
        }
        
        private void OnEnable()
        {
            if (playerControls == null)
            {
                playerControls = new PlayerControls();

                // MOVEMENT AND CAMERA
                playerControls.PlayerMovement.Movement.performed += i => movement_Input = i.ReadValue<Vector2>();
                playerControls.PlayerCamera.Movement.performed += i => camera_Input = i.ReadValue<Vector2>();
                playerControls.PlayerCamera.Mouse.performed += i => camera_Input = i.ReadValue<Vector2>();
                
                // MOVEMENT ACTIONS
                playerControls.PlayerActions.Dodge.performed += i => dodge_Input = true;
                playerControls.PlayerActions.Sprint.performed += i => sprint_Input = true;
                playerControls.PlayerActions.Sprint.canceled += i => sprint_Input = false;
                playerControls.PlayerActions.Jump.performed += i => jump_Input = true;
                
                // Inventory
                playerControls.PlayerActions.SwitchRightWeapon.performed += i => switch_Right_Weapon_Input = true;
                playerControls.PlayerActions.SwitchLeftWeapon.performed += i => switch_Left_Weapon_Input= true;
                
                // Bumpers
                playerControls.PlayerActions.RB.performed += i => RB_Input = true;
                
                // TRIGGERS
                playerControls.PlayerActions.RT.performed += i => RT_Input = true;
                playerControls.PlayerActions.RTHold.performed += i => Hold_RT_Input = true;
                playerControls.PlayerActions.RTHold.canceled += i => Hold_RT_Input = false;
                
                // LOCK ON
                playerControls.PlayerActions.LockOn.performed += i => lockOn_Input = true;
                playerControls.PlayerActions.SeekLeftLockOnTarget.performed += i => lockOn_Left_Input = true;
                playerControls.PlayerActions.SeekRightLockOnTarget.performed += i => lockOn_Right_Input = true;
            }
            
            playerControls.Enable();
        }

        private void OnDestroy()
        {
            SceneManager.activeSceneChanged -= OnSceneChanged;
        }

        private void OnApplicationFocus(bool hasFocus)
        {
            if (enabled)
            {
                if (hasFocus)
                {
                    playerControls.Enable();
                }
                else
                {
                    playerControls.Disable();
                }
            }
        }

        private void Update()
        {
            HandleAllInputs();
        }
        
        private void HandleAllInputs()
        {
            HandleLockOnInput();
            HandleLockOnSwitchTargetInput();
            HandlePlayerMovementInput();
            HandleCameraMovementInput();
            HandleDodgeInput();
            HandleSprintInput();
            HandleJumpInput();
            HandleRBInput();
            HandleRTInput();
            HandleHoldRTInput();
            HandleSwitchRightWeapon();
            HandleSwitchLeftWeapon();
        }

        
        // Lock On
        
        private void HandleLockOnInput()
        {
            //Check for dead target
            if (player.playerNetworkManager.isLockedOn.Value)
            {
                if (player.playerCombatManager.currentTarget == null)
                    return;
                
                if (player.playerCombatManager.currentTarget.characterNetworkManager.isDead.Value)
                {
                    player.playerNetworkManager.isLockedOn.Value = false;
                }        
                
                // Attempt to find new target
                
                // Only one coroutine at a time
                if(lockOnCoroutine != null)
                    StopCoroutine(lockOnCoroutine);

                lockOnCoroutine = StartCoroutine(PlayerCamera.instance.WaitThenFindNewTarget());
            }
            
            if (lockOn_Input && player.playerNetworkManager.isLockedOn.Value)
            {
                lockOn_Input = false;
                PlayerCamera.instance.ClearLockOnTargets();
                player.playerNetworkManager.isLockedOn.Value = false;
                // Disable Lock on
                return;
            }    
            if (lockOn_Input && !player.playerNetworkManager.isLockedOn.Value)
            {
                lockOn_Input = false;

                // Attempt to Lock On
                PlayerCamera.instance.HandleLocatingLockedOnTargets();

                if (PlayerCamera.instance.nearestLockOnTarget != null)
                {
                    // Set the target as current target
                    player.playerCombatManager.SetTarget(PlayerCamera.instance.nearestLockOnTarget);
                    player.playerNetworkManager.isLockedOn.Value = true;
                }
            }    
        }

        private void HandleLockOnSwitchTargetInput()
        {
            if (lockOn_Left_Input)
            {
                lockOn_Left_Input = false;

                if (player.playerNetworkManager.isLockedOn.Value)
                {
                    PlayerCamera.instance.HandleLocatingLockedOnTargets();

                    if (PlayerCamera.instance.leftLockOnTarget != null)
                    {
                        player.playerCombatManager.SetTarget(PlayerCamera.instance.leftLockOnTarget);
                    }
                }
            }
            if (lockOn_Right_Input)
            {
                lockOn_Right_Input= false;

                if (player.playerNetworkManager.isLockedOn.Value)
                {
                    PlayerCamera.instance.HandleLocatingLockedOnTargets();

                    if (PlayerCamera.instance.rightLockOnTarget != null)
                    {
                        player.playerCombatManager.SetTarget(PlayerCamera.instance.rightLockOnTarget);
                    }
                }
            }
        }
        
        // MOVEMENT
        
        private void HandlePlayerMovementInput()
        {
            vertical_Input = movement_Input.y;
            horizontal_Input = movement_Input.x;

            moveAmount = Mathf.Clamp01(Mathf.Abs(vertical_Input) + Mathf.Abs(horizontal_Input));

            if (moveAmount <= 0.5 && moveAmount > 0)
            {
                moveAmount = 0.5f;
            }
            else if (moveAmount > 0.5f && moveAmount <= 1)
            {
                moveAmount = 1;
            }
            
            if(player == null) 
                return;

            if (moveAmount != 0)
            {
                player.playerNetworkManager.isMoving.Value = true;
            }
            else
            {
                player.playerNetworkManager.isMoving.Value = false;
            }

            if (!player.playerNetworkManager.isLockedOn.Value || player.playerNetworkManager.isSprinting.Value)
            {
                player.playerAnimationManager.UpdateAnimatorMovementParameters(0, moveAmount, player.playerNetworkManager.isSprinting.Value);
            }
            else
            {
                player.playerAnimationManager.UpdateAnimatorMovementParameters(horizontal_Input, vertical_Input, player.playerNetworkManager.isSprinting.Value);
            }
            
        }

        private void HandleCameraMovementInput()
        {
            cameraVertical_Input = camera_Input.y;
            cameraHorizontal_Input = camera_Input.x;
        }

        
        // ACTIONS
        
        private void HandleDodgeInput()
        {
            if (dodge_Input)
            {
                dodge_Input = false;
                
                player.playerLocomotionManager.AttemptToPerformDodge();
            }
        }

        private void HandleSprintInput()
        {
            if (sprint_Input)
            {
                player.playerLocomotionManager.HandleSprinting();
            }
            else
            {
                player.playerNetworkManager.isSprinting.Value = false;
            }
        }

        private void HandleJumpInput()
        {
            if (jump_Input)
            {
                jump_Input = false;
                
                player.playerLocomotionManager.AttemptToPerformJump();
            }
        }

        private void HandleRBInput()
        {
            if (RB_Input)
            {
                RB_Input = false;
                
                player.playerNetworkManager.SetCharacterActionHand(true);

                player.playerCombatManager.PerformWeaponBasedAction(
                    player.playerInventoryManager.currentRightHandWeapon.oh_rb_Action,
                    player.playerInventoryManager.currentRightHandWeapon);
            }
        }

        private void HandleRTInput()
        {
            if (RT_Input)
            {
                RT_Input = false;
                
                player.playerNetworkManager.SetCharacterActionHand(true);

                player.playerCombatManager.PerformWeaponBasedAction(
                    player.playerInventoryManager.currentRightHandWeapon.oh_rt_Action,
                    player.playerInventoryManager.currentRightHandWeapon);
            }
        }
        
        private void HandleHoldRTInput()
        {
            // Only check if already in charging action
            if (player.isPerformingAction)
            {
                if (player.playerNetworkManager.isUsingRightHand.Value)
                {
                    player.playerNetworkManager.isChargingAttack.Value = Hold_RT_Input;
                }
            }
        }
        
        private void HandleSwitchRightWeapon()
        {
            if (switch_Right_Weapon_Input)
            {
                switch_Right_Weapon_Input = false;
                player.playerEquipmentManager.SwitchRightWeapon();
            }
        }
        
        private void HandleSwitchLeftWeapon()
        {
            if (switch_Left_Weapon_Input)
            {
                switch_Left_Weapon_Input = false;
                player.playerEquipmentManager.SwitchLeftWeapon();
            }
        }
    }
}
