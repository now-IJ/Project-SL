using System.Runtime.CompilerServices;
using UnityEngine;

namespace RS
{
    [System.Serializable]
    public class CharacterSaveData
    {

        [Header("Scene Index")] public int sceneIndex = 1;
        
        [Header("Character Name")]
        public string characterName = "Jiwona";

        [Header("Time Played")] 
        public float secondsPlayed;
        
        [Header("World Coordinates")] 
        public float xPosition;
        public float yPosition;
        public float zPosition;

        [Header("Resources")] 
        public int currentHealth;
        public float currentStamina;
        
        [Header("Stats")] 
        public int vitality;
        public int endurance;

        [Header("Bosses")] 
        public SerializableDictionary<int, bool> bossesAwakend;
        public SerializableDictionary<int, bool> bossesDefeated;

        public CharacterSaveData()
        {
            bossesAwakend = new SerializableDictionary<int, bool>();
            bossesDefeated = new SerializableDictionary<int, bool>();
        }
    }
}