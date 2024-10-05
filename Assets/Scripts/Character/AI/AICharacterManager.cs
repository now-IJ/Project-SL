using UnityEngine;

namespace RS 
{
    public class AICharacterManager : CharacterManager
    {
        [HideInInspector] public AICombatManager aiCombatManager;
        
        [Header("Current State")] 
        [SerializeField] private AIState currentState;

        protected override void Awake()
        {
            base.Awake();

            aiCombatManager = GetComponent<AICombatManager>();
        }

        protected override void FixedUpdate()
        {
            base.FixedUpdate();
            
            ProcessStateMachine();
        }

        private void ProcessStateMachine()
        {
            AIState nextState = currentState?.Tick(this);

            if (nextState != null)
            {
                currentState = nextState;
            }
        }
    }
}
