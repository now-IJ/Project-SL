using System;
using Unity.Mathematics;
using UnityEngine;

namespace RS
{
    public class CharacterEffectsManager : MonoBehaviour
    {
        private CharacterManager character;

        [Header("VFX")] 
        [SerializeField] private GameObject bloodSplatterVFX;
        
        protected virtual void Awake()
        {
            character = GetComponent<CharacterManager>();
        }

        // PROCESS INSTANT EFFECTS
        public virtual void ProcessInstantEffects(InstantCharacterEffect effect)
        {
            effect.ProcessEffect(character);
        }

        public void PlayBloodSplatterVFX(Vector3 contactPoint)
        {
            // If this model has its own VFX play it
            if (bloodSplatterVFX != null)
            {
                GameObject bloodSplatter = Instantiate(bloodSplatterVFX, contactPoint, quaternion.identity);
            }
            // Else use the default one 
            else
            {
                GameObject bloodSplatter = Instantiate(WorldCharacterEffectsManager.instance.bloodSplatterVFX, contactPoint, quaternion.identity);
            }
        }
        // PROCESS TIMED EFFECTS
        
        // PROCESS STATIC EFFECTS
        
    }
}