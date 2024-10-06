using UnityEngine;

namespace RS
{
    public class UndeadAttackDamageCollider : DamageCollider
    {
        private AICharacterManager undeadCharacter;

        protected override void Awake()
        {
            base.Awake();

            damageCollider = GetComponent<Collider>();
            undeadCharacter = GetComponentInParent<AICharacterManager>();
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
            damageEffect.angleHitFrom =Vector3.SignedAngle(undeadCharacter.transform.forward, damagedTarget.transform.forward, Vector3.up);
            damageEffect.contactPoint = contactPoint;

            if (damagedTarget.IsOwner)
            {
                damagedTarget.characterNetworkManager.NotifyServerOfCharacterDamageServerRPC(
                    damagedTarget.NetworkObjectId, undeadCharacter.NetworkObjectId, damageEffect.physicalDamage,
                    damageEffect.magicDamage, damageEffect.fireDamage, damageEffect.lightningDamage,
                    damageEffect.holyDamage, damageEffect.poiseDamage, damageEffect.angleHitFrom,
                    damageEffect.contactPoint.x, damageEffect.contactPoint.y, damageEffect.contactPoint.z);
            }
        }
    }
}