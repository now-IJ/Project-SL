using System;
using UnityEngine;

namespace RS
{
    public class CharacterEffectsManager : MonoBehaviour
    {
        private CharacterManager character;

        protected virtual void Awake()
        {
            character = GetComponent<CharacterManager>();
        }

        // PROCESS INSTANT EFFECTS
        public virtual void ProcessInstantEffects(InstantCharacterEffect effect)
        {
            effect.ProcessEffect(character);
        }
        
        
        // PROCESS TIMED EFFECTS
        
        // PROCESS STATIC EFFECTS
        
    }
}