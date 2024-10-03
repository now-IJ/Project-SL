
using UnityEngine;
using UnityEngine.Serialization;

namespace RS
{
    public class MeleeWeaponDamageCollider : DamageCollider
    {
        [Header("Attacking Character")] 
        public CharacterManager characterCausingDamage;

        [Header("Weapon Attack Modifiers")] 
        public float light_Attack_01_Modifier;
        public float heavy_Attack_01_Modifier;
        public float charge_Attack_01_Modifier;

        protected override void Awake()
        {
            base.Awake();
            if (damageCollider == null)
            {
                damageCollider = GetComponent<Collider>();
            }
            damageCollider.enabled = false;
        }

        protected override void OnTriggerEnter(Collider other)
        {
            CharacterManager damagedTarget = other.GetComponentInParent<CharacterManager>();

            if (damagedTarget != null)
            {
                if(damagedTarget == characterCausingDamage)
                    return;
            
                contactPoint = other.gameObject.GetComponent<Collider>().ClosestPointOnBounds(transform.position);
                
                DamageTarget(damagedTarget);
            }
        }

        protected override void DamageTarget(CharacterManager damagedTarget)
        {
            if(charactersDamaged.Contains(damagedTarget))
                return;
            
            charactersDamaged.Add(damagedTarget);

            TakeDamageEffect damageEffect = Instantiate(WorldCharacterEffectsManager.instance.takeDamageEffect);
            damageEffect.physicalDamage = physicalDamage;
            damageEffect.magicDamage = magicDamage;
            damageEffect.fireDamage = fireDamage;
            damageEffect.lightningDamage = lightningDamage;
            damageEffect.holyDamage = holyDamage;
            damageEffect.angleHitFrom =Vector3.SignedAngle(characterCausingDamage.transform.forward, damagedTarget.transform.forward, Vector3.up);

            switch (characterCausingDamage.characterCombatManager.currentAttackType)
            {
                case AttackType.LightAttack01:
                    ApplyAttackDamageModifiers(light_Attack_01_Modifier, damageEffect);
                    break;
                case AttackType.LightAttack02:
                    break;
                case AttackType.HeavyAttack01:
                    ApplyAttackDamageModifiers(heavy_Attack_01_Modifier, damageEffect);
                    break;
                case AttackType.ChargedAttack01:
                    ApplyAttackDamageModifiers(charge_Attack_01_Modifier, damageEffect);
                    break;
                default:
                    break;
            }
            
            //damagedTarget.characterEffectsManager.ProcessInstantEffects(damageEffect);

            if (characterCausingDamage.IsOwner)
            {
                damagedTarget.characterNetworkManager.NotifyServerOfCharacterDamageServerRPC(
                    damagedTarget.NetworkObjectId, characterCausingDamage.NetworkObjectId, damageEffect.physicalDamage,
                    damageEffect.magicDamage, damageEffect.fireDamage, damageEffect.lightningDamage,
                    damageEffect.holyDamage, damageEffect.poiseDamage, damageEffect.angleHitFrom,
                    damageEffect.contactPoint.x, damageEffect.contactPoint.y, damageEffect.contactPoint.z);
            }
        }

        private void ApplyAttackDamageModifiers(float modifier, TakeDamageEffect damage)
        {
            damage.physicalDamage *= modifier;
            damage.magicDamage *= modifier;
            damage.fireDamage *= modifier;
            damage.lightningDamage *= modifier;
            damage.holyDamage *= modifier;
            damage.poiseDamage *= modifier;
            
            // If attack is fully charged heavy, multiply by multiplier again
        }
    }
}