
using System.Collections;
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
        [SerializeField] private float setCameraHeightSpeed = 1.0f;
        [SerializeField] private float unlockedCameraHeight = 1.65f;
        [SerializeField] private float lockedCameraHeight = 2.0f;
        private Coroutine cameraLockOnHeightCoroutine;
        private List<CharacterManager> availableTargets = new List<CharacterManager>();
        public CharacterManager nearestLockOnTarget;
        public CharacterManager leftLockOnTarget;
        public CharacterManager rightLockOnTarget;
        
        
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

                    // If player is already locked on when searching for targets, find nearest left/right target
                    if (player.playerNetworkManager.isLockedOn.Value)
                    {
                        Vector3 relativeEnemyPosition = player.transform.InverseTransformPoint(availableTargets[j].transform.position);
                        var distanceFromLeftTarget = relativeEnemyPosition.x;
                        var distanceFromRightTarget = relativeEnemyPosition.x;

                        if(availableTargets[j] != player.playerCombatManager.currentTarget)
                            continue;
                        
                        // Check left side for targets
                        if (relativeEnemyPosition.x <= 0.00 && distanceFromLeftTarget > shortDistanceOfLeftTarget)
                        {
                            shortDistanceOfLeftTarget = distanceFromLeftTarget;
                            leftLockOnTarget = availableTargets[j];
                        }
                        // Check right side for targets
                        else if (relativeEnemyPosition.x >= 0.00 && distanceFromRightTarget < shortDistanceOfRightTarget)
                        {
                            shortDistanceOfRightTarget = distanceFromRightTarget;
                            rightLockOnTarget = availableTargets[j];
                        }
                    }
                }
                else
                {
                    ClearLockOnTargets();
                    player.playerNetworkManager.isLockedOn.Value = false;
                }
            }
        }

        public void SetLockCameraHeight()
        {
            if (cameraLockOnHeightCoroutine != null)
            {
                StopCoroutine(cameraLockOnHeightCoroutine);
            }

            cameraLockOnHeightCoroutine = StartCoroutine(SetCameraHeight());
        }
        
        public void ClearLockOnTargets()
        {
            nearestLockOnTarget = null;
            leftLockOnTarget = null;
            rightLockOnTarget = null;
            availableTargets.Clear();
        }

        public IEnumerator WaitThenFindNewTarget()
        {
            while (player.isPerformingAction)
            {
                yield return null;
            }
            
            ClearLockOnTargets();
            HandleLocatingLockedOnTargets();

            if (nearestLockOnTarget != null)
            {
                player.playerCombatManager.SetTarget(nearestLockOnTarget);
                player.playerNetworkManager.isLockedOn.Value = true;
            }

            yield return null;
        }

        private IEnumerator SetCameraHeight()
        {
            float duration = 1;
            float timer = 0;

            Vector3 velocity = Vector3.zero;
            Vector3 newLockedCameraHeight = new Vector3(cameraPivotTransform.transform.localPosition.x, lockedCameraHeight);
            Vector3 newUnlockedCameraHeight = new Vector3(cameraPivotTransform.transform.localPosition.x, unlockedCameraHeight);

            while (timer < duration)
            {
                timer += Time.deltaTime;

                if (player != null)
                {
                    if (player.playerCombatManager.currentTarget != null)
                    {
                        cameraPivotTransform.transform.localPosition = Vector3.SmoothDamp(
                            cameraPivotTransform.transform.localPosition, 
                            newLockedCameraHeight, 
                            ref velocity,
                            setCameraHeightSpeed);

                        cameraPivotTransform.transform.localRotation =
                            Quaternion.Slerp(cameraPivotTransform.transform.localRotation,
                                Quaternion.Euler(0, 0, 0), lockOnTargetFollowSpeed);
                    }
                    else
                    {
                        cameraPivotTransform.transform.localPosition = Vector3.SmoothDamp(
                                cameraPivotTransform.transform.localPosition, 
                                newUnlockedCameraHeight,
                                ref velocity, 
                                setCameraHeightSpeed);
                        
                        
                    }
                }

                yield return null;
            }
    
            // FAILSAFE
            if (player != null)
            {
                if (player.playerCombatManager.currentTarget != null)
                {
                    cameraPivotTransform.transform.localPosition = newLockedCameraHeight;

                    cameraPivotTransform.transform.localRotation = Quaternion.Euler(0, 0, 0);
                }
                else
                {
                    cameraPivotTransform.transform.localPosition = newUnlockedCameraHeight;
                }
            }

            yield return null;
        }
    }
}