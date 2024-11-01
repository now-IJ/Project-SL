using UnityEngine;

namespace RS
{
    public class AIBossCombatManager : AICharacterCombatManager
    {
        private AIBossCharacterManager aiBossCharacter;
        
        [Header("Damage Colliders")]
        [SerializeField] private BossDamageCollider attackDamageCollider;

        [Header("Damage")] 
        [SerializeField] private int baseDamage = 25;
        [SerializeField] private float attack01DamageModifier = 1.0f;
        [SerializeField] private float attack02DamageModifier = 1.4f;

        protected override void Awake()
        {
            base.Awake();

            aiBossCharacter = GetComponent<AIBossCharacterManager>();
        }

        public void SetAttack01Damage()
        {
            attackDamageCollider.physicalDamage = baseDamage * attack01DamageModifier;
        }
        public void SetAttack02Damage()
        {
            attackDamageCollider.physicalDamage = baseDamage * attack02DamageModifier;
        }
        public void OpenAttackDamageCollider()
        {
            aiCharacter.characterSoundFXManager.PlayAttackGrunt();
            attackDamageCollider.EnableDamageCollision();
            aiBossCharacter.characterSoundFXManager.PlayAttackGrunt();
        }

        public void CloseAttackDamageCollider()
        {
            attackDamageCollider.DisableDamageCollision();
        }
    }
}