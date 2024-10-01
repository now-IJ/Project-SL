using System;
using UnityEngine;

namespace RS
{
    public class PlayerEffectsManager : CharacterEffectsManager
    {
        [Header("DEBUG")] 
        [SerializeField] private InstantCharacterEffect effectToTest;
        [SerializeField] private bool processEffect = false;

        private void Update()
        {
            if (processEffect)
            {
                processEffect = false;
                InstantCharacterEffect effect = Instantiate(effectToTest);
                ProcessInstantEffects(effect);
            }
        }
    }
}