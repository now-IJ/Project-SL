using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.TextCore.Text;

namespace RS
{
    [CreateAssetMenu(menuName = "AI/States/Combat Stance")]
    public class CombatStanceState : AIState
    {
        [Header("Attacks")]
        public List<AICharacterAttackAction> aiCharacterAttacks;        // List of all attacks that the character possesses
        protected List<AICharacterAttackAction> potentialAttacks;       // List of potential attacks that the character can use in one certain situation
        private AICharacterAttackAction chosenAttack;
        private AICharacterAttackAction previousAttack;
        protected bool hasAttack = false;
        
        [Header("Combo")] 
        [SerializeField] protected bool canPerformCombo = false;        // Can perform combo attack after initial attack
        [SerializeField] protected int chanceToPerformCombo = 25;       // Chance of combo (in percent)
        protected bool hasRolledForComboChance = false;                 // Roll only once per state, resets after leaving the state
        
        [Header("Engagement Distance")]
        public float maximumEngagementDistance = 5; // The distance needed to swap back to pursue state
        
        [Header("Pivot")]
        [SerializeField] protected bool enabledPivot;

        public override AIState Tick(AICharacterManager aiCharacter)
        {
            if (aiCharacter.isPerformingAction)
                return this;

            if (!aiCharacter.navMeshAgent.enabled)
                aiCharacter.navMeshAgent.enabled = true;

            if (enabledPivot)
            {
                if (!aiCharacter.aiCharacterNetworkManager.isMoving.Value)
                {
                    if (aiCharacter.aiCharacterCombatManager.viewableAngle < -30 ||
                        aiCharacter.aiCharacterCombatManager.viewableAngle > 30)
                    {
                        aiCharacter.aiCharacterCombatManager.PivotTowardsTarget(aiCharacter);
                    }
                }
            }

            aiCharacter.aiCharacterCombatManager.RotateTowardsAgent(aiCharacter);
            
            if (aiCharacter.aiCharacterCombatManager.currentTarget == null)
                return SwitchState(aiCharacter, aiCharacter.idle);

            if (!hasAttack)
            {
                GetNewAttack(aiCharacter);
            }
            else
            {
                aiCharacter.attack.currentAttack = chosenAttack;
                return SwitchState(aiCharacter, aiCharacter.attack);
            }

            if (aiCharacter.aiCharacterCombatManager.distanceToTarget > maximumEngagementDistance)
                return SwitchState(aiCharacter, aiCharacter.pursueTarget);
            
            NavMeshPath path = new NavMeshPath();
            aiCharacter.navMeshAgent.CalculatePath(aiCharacter.aiCharacterCombatManager.currentTarget.transform.position, path);
            aiCharacter.navMeshAgent.SetPath(path);
            
            return this;
        }

        protected virtual void GetNewAttack(AICharacterManager aiCharacter)
        {
            // Sort through possible attacks
            potentialAttacks = new List<AICharacterAttackAction>();

            foreach (AICharacterAttackAction potentialAttack in aiCharacterAttacks)
            {
                if(potentialAttack.minimumAttackRange > aiCharacter.aiCharacterCombatManager.distanceToTarget)
                    continue;
                
                if(potentialAttack.maximumAttackRange < aiCharacter.aiCharacterCombatManager.distanceToTarget)
                    continue;
                
                if(potentialAttack.minimumAttackAngle > aiCharacter.aiCharacterCombatManager.viewableAngle)
                    continue;
                
                if(potentialAttack.maximumAttackAngle < aiCharacter.aiCharacterCombatManager.viewableAngle)
                    continue;
                
                potentialAttacks.Add(potentialAttack);
            }
            
            if(potentialAttacks.Count <= 0)
                return;

            var totalWeight = 0;

            foreach (var attack in potentialAttacks)
            {
                totalWeight += attack.attackWeight;
            }

            var randomWeightValue = Random.Range(1, totalWeight + 1);
            var processedWeight = 0;

            foreach (var attack in potentialAttacks)
            {
                processedWeight += attack.attackWeight;

                if (randomWeightValue <= processedWeight)
                {
                    chosenAttack = attack;
                    previousAttack = attack;
                    hasAttack = true;
                    return;
                }
            }
        }

        protected virtual bool RollForOutcomeChance(int outcomeChance)
        {
            bool outcomeWillBePerformed = false;

            int randomPercentage = Random.Range(0, 100);

            if (randomPercentage < outcomeChance)
                outcomeWillBePerformed = true;

            return outcomeWillBePerformed;
        }

        protected override void ResetStateFlags(AICharacterManager aiCharacter)
        {
            base.ResetStateFlags(aiCharacter);

            hasRolledForComboChance = false;
            hasAttack = false;
        }
    }
}