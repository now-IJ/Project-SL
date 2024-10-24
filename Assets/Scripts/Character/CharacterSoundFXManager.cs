using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace RS
{
    public class CharacterSoundFXManager : MonoBehaviour
    {
        private AudioSource audioSource;

        [Header("Damage Grunts")] 
        [SerializeField] protected AudioClip[] damageGrunts;
        
        [Header("Attack Grunts")] 
        [SerializeField] protected AudioClip[] attackGrunts;
        
        [Header("Foot Steps")] 
        [SerializeField] protected AudioClip[] footSteps;
        
        protected virtual void Awake()
        {
            audioSource = GetComponent<AudioSource>();
        }

        public void PlaySoundFX(AudioClip soundFX, float volume = 1f, bool randomizePitch = true, float pitchRandom = 0.1f)
        {
            audioSource.PlayOneShot(soundFX, volume);
            audioSource.pitch = 1;

            if (randomizePitch)
            {
                audioSource.pitch += Random.Range(-pitchRandom, pitchRandom);
            }
        }
        
        public void PlayRollSoundFX()
        {
            audioSource.PlayOneShot(WorldSoundFXManager.instance.rollSFX);
        }

        public virtual void PlayDamageGrunt()
        {
            if(damageGrunts.Length > 0)
                PlaySoundFX(WorldSoundFXManager.instance.ChooseRandomSFXFromArray(damageGrunts));
        }

        public virtual void PlayAttackGrunt()
        {
            if(attackGrunts.Length > 0)
                PlaySoundFX(WorldSoundFXManager.instance.ChooseRandomSFXFromArray(attackGrunts));
        }

        public virtual void PlayFootStep()
        {
            if(footSteps.Length > 0)
                PlaySoundFX(WorldSoundFXManager.instance.ChooseRandomSFXFromArray(footSteps));
        }
    }
}