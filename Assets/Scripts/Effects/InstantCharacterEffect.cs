using UnityEngine;

namespace RS
{
    public class InstantCharacterEffect : ScriptableObject
    {
        [Header("Effect ID")] 
        public int instantEffectID;

        public virtual void ProcessEffect(CharacterManager character)
        {
            
        }
    }
}