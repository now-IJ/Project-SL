using UnityEngine;

namespace RS
{
    public class CharacterStatsManager : MonoBehaviour
    {
        private CharacterManager character;
        
        [Header("Stamina Regeneration")] 
        [SerializeField] private float staminaRegenerationDelay = 2;
        [SerializeField] private int staminaRegenerationAmount = 2;
        private float staminaRegenerationTimer = 0;
        private float staminaTickTimer = 0;

        protected virtual void Awake()
        {
            character = GetComponent<CharacterManager>();
        }
        
        protected virtual void Start()
        {
            
        }
        
        public int CalculateHealthBasedOnVitalityLevel(int vitality)
        {
            float health = 0;

            health = vitality * 15;

            return Mathf.RoundToInt(health);
        }
        
        public int CalculateStaminaBasedOnEnduranceLevel(int endurance)
        {
            float stamina = 0;

            stamina = endurance * 10;

            return Mathf.RoundToInt(stamina);
        }
        
        public virtual void RegenerateStamina()
        {
            if(!character.IsOwner)
                return;
            
            if(character.characterNetworkManager.isSprinting.Value)
                return;
            
            if(character.isPerformingAction)
                return;

            staminaRegenerationTimer += Time.deltaTime;

            if (staminaRegenerationTimer >= staminaRegenerationDelay)
            {
                if (character.characterNetworkManager.currentStamina.Value < character.characterNetworkManager.maxStamina.Value)
                {
                    staminaTickTimer += Time.deltaTime;

                    if (staminaTickTimer >= 0.1f)
                    {
                        staminaTickTimer = 0;
                        character.characterNetworkManager.currentStamina.Value += staminaRegenerationAmount;
                    }
                }
            }
        }

        public virtual void ResetStaminaRegenerationTimer(float previousStaminaAmount, float currentStaminaAmount)
        {
            if (currentStaminaAmount < previousStaminaAmount)
            {
                staminaRegenerationTimer = 0;
            }
        }
        
    }
}