
using System.Collections.Generic;
using UnityEngine;

namespace RS
{
    public class PlayerCamera : MonoBehaviour
    {
        public static PlayerCamera instance;

        public PlayerManager player;
        public Camera cameraObject;
        [SerializeField] private Transform cameraPivotTransform;
        
        [Header("Camera Settings")] 
        [Tooltip("The bigger this number, the longer for the camera to reach its position during movement")]
        [SerializeField] private float cameraSmoothSpeed = 1;
        [SerializeField] private float leftAndRightRotationSpeed = 220;
        [SerializeField] private float upAndDownRotationSpeed = 220;
        [Tooltip("The lowest point the player is able to look down")]
        [SerializeField] private float minimumPivot = -45;
        [Tooltip("The highest point the player is able to look up")]
        [SerializeField] private float maximumPivot = 60;
        [SerializeField] private float cameraCollisionRadius = 0.2f;
        [SerializeField] private LayerMask collideWithLayers;
        
        [Header("Camera Values")]
        [SerializeField] private float leftAndRightLookAngle;
        [SerializeField] private float upAndDownLookAngle;
        private Vector3 cameraVelocity;
        private Vector3 cameraObjectPosition;
        private float cameraZPosition;
        private float targetCameraZPosition;

        [Header("Lock On")] 
        [SerializeField] private float lockOnRadius = 20;
        [SerializeField] private float minimumViewAbleAngle = -50;
        [SerializeField] private float maximumViewAbleAngle = 50;
        [SerializeField] private float maximumLockOnDistance = 20;
        [SerializeField] private float lockOnTargetFollowSpeed = 0.2f;
        private List<CharacterManager> availableTargets = new List<CharacterManager>();
        public CharacterManager nearestLockOnTarget;
        
        
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
            cameraZPosition = cameraObject.transform.localPosition.z;
        }

        public void HandleAllCameraActions()
        {
            if (player != null)
            {
                HandleFollowTarget();
                HandleRotations();
                HandleCollisions();
            }
        }

        private void HandleFollowTarget()
        {
            Vector3 targetCameraPosition = Vector3.SmoothDamp(transform.position, player.transform.position,
                ref cameraVelocity, cameraSmoothSpeed * Time.deltaTime);
            transform.position = targetCameraPosition;
        }

        private void HandleRotations()
        {
            if (player.playerNetworkManager.isLockedOn.Value)
            {
                // Left/Right Rotation
                Vector3 rotationDirection =
                    player.playerCombatManager.currentTarget.characterCombatManager.lockOnTargetTransform.position -
                    transform.position;
                rotationDirection.Normalize();
                rotationDirection.y = 0;

                Quaternion targetRotation = Quaternion.LookRotation(rotationDirection);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, lockOnTargetFollowSpeed);
                
                // Up/Down rotation
                rotationDirection =
                    player.playerCombatManager.currentTarget.characterCombatManager.lockOnTargetTransform.position -
                    cameraPivotTransform.position;
                rotationDirection.Normalize();

                targetRotation = Quaternion.LookRotation(rotationDirection);
                cameraPivotTransform.transform.rotation = Quaternion.Slerp(cameraPivotTransform.rotation,
                    targetRotation, lockOnTargetFollowSpeed);
                
                // Save rotation values to avoid snap
                leftAndRightLookAngle = transform.eulerAngles.y;
                upAndDownLookAngle = transform.eulerAngles.x;

            }
            else
            {
                leftAndRightLookAngle += (PlayerInputManager.instance.cameraHorizontal_Input * leftAndRightRotationSpeed) * Time.deltaTime;
                upAndDownLookAngle -= (PlayerInputManager.instance.cameraVertical_Input * upAndDownRotationSpeed) * Time.deltaTime;
                upAndDownLookAngle = Mathf.Clamp(upAndDownLookAngle, minimumPivot, maximumPivot);

                Vector3 cameraRotation = Vector3.zero;
                Quaternion targetRotation;
            
                // Left Right
                cameraRotation.y = leftAndRightLookAngle;
                targetRotation = Quaternion.Euler(cameraRotation);
                transform.rotation = targetRotation;

                // Up Down
                cameraRotation = Vector3.zero;
                cameraRotation.x = upAndDownLookAngle;
                targetRotation = Quaternion.Euler(cameraRotation);
                cameraPivotTransform.localRotation = targetRotation;
            }
        }

        private void HandleCollisions()
        {
            targetCameraZPosition = cameraZPosition;
            RaycastHit hit;
            Vector3 direction = cameraObject.transform.position - cameraPivotTransform.position;
            direction.Normalize();

            if (Physics.SphereCast(cameraPivotTransform.position, cameraCollisionRadius, direction, out hit,
                    Mathf.Abs(targetCameraZPosition), collideWithLayers))
            {
                float distanceFromHitObject = Vector3.Distance(cameraPivotTransform.position, hit.point);
                targetCameraZPosition = -(distanceFromHitObject - cameraCollisionRadius);
            }

            if (Mathf.Abs(targetCameraZPosition) < cameraCollisionRadius)
            {
                targetCameraZPosition = -cameraCollisionRadius;
            }

            cameraObjectPosition.z = Mathf.Lerp(cameraObject.transform.localPosition.z, targetCameraZPosition, 0.2f);
            cameraObject.transform.localPosition = cameraObjectPosition;

        }

        public void HandleLocatingLockedOnTargets()
        {
            float shortestDistance = Mathf.Infinity;            // Shortest Target to player
            float shortDistanceOfRightTarget = Mathf.Infinity;  // Shortest Target to the right of current Target
            float shortDistanceOfLeftTarget = -Mathf.Infinity;  // Shortest Target to the left of current Target
            
            
            Collider[] colliders = Physics.OverlapSphere(player.transform.position, lockOnRadius, WorldUtilityManager.instance.GetCharacterLayers());
            
            
            for (int i = 0; i < colliders.Length; i++)
            {
                
                CharacterManager lockOnTarget = colliders[i].GetComponent<CharacterManager>();

                Debug.Log(colliders[i].ToString());
                
                if (lockOnTarget != null)
                {
                    // Check if they are within our FOV
                    Vector3 lockOnTargetsDirection = lockOnTarget.transform.position - player.transform.position;
                    float distanceFromTarget = Vector3.Distance(player.transform.position, lockOnTarget.transform.position);
                    float viewableAngle = Vector3.Angle(lockOnTargetsDirection, cameraObject.transform.forward);
                    
                    // If target is dead check next target
                    if (lockOnTarget.characterNetworkManager.isDead.Value)
                        continue;
                    
                    // If target is player check next target
                    if(lockOnTarget.transform.root == player.transform.root)
                        continue; 

                    
                    // If the target is outside of FOV or blocked by environment continue to next target
                    if (viewableAngle >= minimumViewAbleAngle && viewableAngle <= maximumViewAbleAngle)
                    {
                        RaycastHit hit;
                        
                        if (Physics.Linecast(player.playerCombatManager.lockOnTargetTransform.position,
                                lockOnTarget.characterCombatManager.lockOnTargetTransform.position, out hit,WorldUtilityManager.instance.GetEnvironmentLayers()))
                        {
                            continue;
                        }
                        else
                        {
                            availableTargets.Add(lockOnTarget);
                        }
                    }
                }
            }
    
            // Sort through potential targets and check which to lock on
            for (int j = 0; j < availableTargets.Count; j++)
            {
                if (availableTargets[j] != null)
                {
                    float distanceFromTarget = Vector3.Distance(player.transform.position, availableTargets[j].transform.position);

                    if (distanceFromTarget < shortestDistance)
                    {
                        shortestDistance = distanceFromTarget;
                        nearestLockOnTarget = availableTargets[j];
                    }
                }
                else
                {
                    ClearLockOnTargets();
                    player.playerNetworkManager.isLockedOn.Value = false;
                }
            }
        }

        public void ClearLockOnTargets()
        {
            nearestLockOnTarget = null;
            availableTargets.Clear();
        }
    }
}