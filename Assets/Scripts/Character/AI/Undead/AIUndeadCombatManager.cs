using UnityEngine;

namespace RS
{
    public class AIUndeadCombatManager : AICharacterCombatManager
    {
        [Header("Damage Colliders")]
        [SerializeField] private UndeadAttackDamageCollider attackDamageCollider;
        [SerializeField] private UndeadAttackDamageCollider footDamageCollider;

        [Header("Damage")] 
        [SerializeField] private int baseDamage = 25;
        [SerializeField] private float attack01DamageModifier = 1.0f;
        [SerializeField] private float attack02DamageModifier = 1.4f;

        public void SetAttack01Damage()
        {
            attackDamageCollider.physicalDamage = baseDamage * attack01DamageModifier;
        }
        public void SetAttack02Damage()
        {
            footDamageCollider.physicalDamage = baseDamage * attack02DamageModifier;
        }

        public void OpenHandDamageCollider()
        {
            aiCharacter.characterSoundFXManager.PlayAttackGrunt();
            attackDamageCollider.EnableDamageCollision();
        }

        public void CloseHandDamageCollider()
        {
            attackDamageCollider.DisableDamageCollision();
        }
        
        public void OpenFootDamageCollider()
        {
            aiCharacter.characterSoundFXManager.PlayAttackGrunt();
            footDamageCollider.EnableDamageCollision();
        }

        public void CloseFootDamageCollider()
        {
            footDamageCollider.DisableDamageCollision();
        }
    }
}