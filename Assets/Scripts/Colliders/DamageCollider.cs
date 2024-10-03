using System;
using System.Collections.Generic;
using UnityEngine;

namespace RS
{
    public class DamageCollider : MonoBehaviour
    {
        [Header("Collider")] 
        [SerializeField] protected Collider damageCollider;
        
        [Header("Damage")] 
        public float physicalDamage = 25;
        public float magicDamage = 0;
        public float fireDamage = 0;
        public float lightningDamage = 0;
        public float holyDamage = 0;

        [Header("Contact Point")] 
        protected Vector3 contactPoint;

        [Header("Characters Damaged")] 
        protected List<CharacterManager> charactersDamaged = new List<CharacterManager>();

        protected virtual void Awake()
        {
            
        }

        protected virtual void OnTriggerEnter(Collider other)
        {
            CharacterManager damagedTarget = other.GetComponentInParent<CharacterManager>();

            if (damagedTarget != null)
            {
                contactPoint = other.gameObject.GetComponent<Collider>().ClosestPointOnBounds(transform.position);
                
                DamageTarget(damagedTarget);
            }
        }

        protected virtual void DamageTarget(CharacterManager damagedTarget)
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
            
            damagedTarget.characterEffectsManager.ProcessInstantEffects(damageEffect);
        }

        public virtual void EnableDamageCollision()
        {
            damageCollider.enabled = true;
        }

        public virtual void DisableDamageCollision()
        {
            damageCollider.enabled = false;
            charactersDamaged.Clear();
        }
    }
}
