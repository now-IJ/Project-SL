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
        [SerializeField] private Vector2 cameraInput;
        public float cameraVerticalInput;
        public float cameraHorizontalInput;
        
        [Header("Movement input")]
        [SerializeField] private Vector2 movementInput;
        public float verticalInput;
        public float horizontalInput;
        public float moveAmount;

        [Header("Player Actions")] 
        [SerializeField] private bool dodgeInput = false;
        [SerializeField] private bool sprintInput = false;

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
        }

        private void OnSceneChanged(Scene oldScene, Scene newScene)
        {
            if (newScene.buildIndex == WorldSaveGameManager.instance.GetWorldSceneIndex())
            {
                instance.enabled = true;
            }
            else
            {
                instance.enabled = false;
            }
        }
        
        private void OnEnable()
        {
            if (playerControls == null)
            {
                playerControls = new PlayerControls();

                playerControls.PlayerMovement.Movement.performed += i => movementInput = i.ReadValue<Vector2>();
                playerControls.PlayerCamera.Movement.performed += i => cameraInput = i.ReadValue<Vector2>();
                playerControls.PlayerCamera.Mouse.performed += i => cameraInput = i.ReadValue<Vector2>();
                playerControls.PlayerActions.Dodge.performed += i => dodgeInput = true;
                playerControls.PlayerActions.Sprint.performed += i => sprintInput = true;
                playerControls.PlayerActions.Sprint.canceled += i => sprintInput = false;
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
            HandlePlayerMovementInput();
            HandleCameraMovementInput();
            HandleDodgeInput();
            HandleSprintInput();
        }

        
        // MOVEMENT
        
        private void HandlePlayerMovementInput()
        {
            verticalInput = movementInput.y;
            horizontalInput = movementInput.x;

            moveAmount = Mathf.Clamp01(Mathf.Abs(verticalInput) + Mathf.Abs(horizontalInput));

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
            
            player.playerAnimationManager.UpdateAnimatorMovementParameters(0, moveAmount, player.playerNetworkManager.isSprinting.Value);
        }

        private void HandleCameraMovementInput()
        {
            cameraVerticalInput = cameraInput.y;
            cameraHorizontalInput = cameraInput.x;
        }

        
        // ACTIONS
        
        private void HandleDodgeInput()
        {
            if (dodgeInput)
            {
                dodgeInput = false;
                
                player.playerLocomotionManager.AttemptToPerformDodge();
            }
        }

        private void HandleSprintInput()
        {
            if (sprintInput)
            {
                player.playerLocomotionManager.HandleSprinting();
            }
            else
            {
                player.playerNetworkManager.isSprinting.Value = false;
            }
        }
    }
}
