using System.Runtime.CompilerServices;
using UnityEngine;

namespace RS
{
    [System.Serializable]
    public class CharacterSaveData
    {
        [Header("Character Name")]
        public string characterName;

        [Header("Time Played")] 
        public float secondsPlayed;

        [Header("World Coordinates")] 
        public float xPosition;
        public float yPosition;
        public float zPosition;
    }
}