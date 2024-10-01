using UnityEngine;
using UnityEngine.Serialization;

namespace RS
{
    [CreateAssetMenu(menuName = "Character Effects/Instant Effects/Take Damage")]
    public class TakeDamageEffect : InstantCharacterEffect
    {
        [Header("Character Causing Damage")]
        public CharacterManager characterCausingDamage;

        [Header("Damage")] 
        public float physicalDamage = 0;
        public float magicDamage = 0;
        public float fireDamage = 0;
        public float lightningDamage = 0;
        public float holyDamage = 0;
    
        [Header("Final Damage")]
        private int finalDamageDealt = 0;
        
        [Header("Poise")] 
        public float poiseDamage = 0;
        public bool poiseIsBroken = false;

        [Header("Animation")] 
        public bool playDamageAnimation = true;
        public bool manuallySelectedDamageAnimation = false;
        public string damageAnimation;

        [Header("Sound FX")] 
        public bool willPlayDamageSFX = true;
        public AudioClip elementalDamageSoundFX;

        [Header("Direction Damage Taken From")]
        public float angleHitFrom;
        public Vector3 contactPoint;
        
        public override void ProcessEffect(CharacterManager character)
        {
            base.ProcessEffect(character);
            
            // Don't apply damage to dead character
            if(character.characterNetworkManager.isDead.Value)
                return;
            
            // Check for invulnerability
            
            CalculateDamage(character);
            // check which direction damage came from
            // play damage animation
            // check for build ups (poison, bleed,etc)
            // play damage sfx
            // play damage vfx
            
            // AI behaviour
        }

        private void CalculateDamage(CharacterManager character)
        {
            if(!character.IsOwner)
                return;
            
            if (characterCausingDamage != null)
            {
                // check for damage modifiers
            }
            
            // check character for flat defenses 

            // check character for armor
            
            // add all damage types together and apply

            finalDamageDealt = Mathf.RoundToInt(physicalDamage + magicDamage + fireDamage + lightningDamage + holyDamage);

            if (finalDamageDealt <= 1)
            {
                finalDamageDealt = 1;
            }

            character.characterNetworkManager.currentHealth.Value -= finalDamageDealt;
            
            // calculate poise damage
        }
    }
}