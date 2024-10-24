using System;
using UnityEngine;
using UnityEngine.ProBuilder.MeshOperations;

namespace RS
{
    public class CharacterFootStepSFXEmitter : MonoBehaviour
    {
        private CharacterManager character;

        private AudioSource audioSource;
        private GameObject steppedOnObject;

        private bool hasTouchedGround = false;
        private bool hasPlayedFootStepSFX = false;

        [SerializeField] private float distanceToFloor;

        private void Awake()
        {
            audioSource = GetComponent<AudioSource>();
            character = GetComponentInParent<CharacterManager>();
        }

        private void FixedUpdate()
        {
            CheckForFootSteps();
        }

        private void CheckForFootSteps()
        {
            if(character == null)
                return;
            
            if(!character.characterNetworkManager.isMoving.Value)
                return;

            RaycastHit hit;
            
            if(Physics.Raycast(transform.position, character.transform.TransformDirection(Vector3.down), out hit, 0.05f,WorldUtilityManager.instance.GetEnvironmentLayers()))
            {
                hasTouchedGround = true;

                if (!hasPlayedFootStepSFX)
                    steppedOnObject = hit.transform.gameObject;
            }
            else
            {
                hasTouchedGround = false;
                hasPlayedFootStepSFX = false;
                steppedOnObject = null;
            }

            if (hasTouchedGround && !hasPlayedFootStepSFX)
            {
                hasPlayedFootStepSFX = true;
                PlayFootStepSFX();
            }
            
        }

        private void PlayFootStepSFX()
        {
            character.characterSoundFXManager.PlayFootStep();
        }
    }
}
