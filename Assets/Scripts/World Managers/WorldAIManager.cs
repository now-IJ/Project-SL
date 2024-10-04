using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace RS
{
    public class WorldAIManager : MonoBehaviour
    {
        public static WorldAIManager instance;

        [Header("DEBUG")]
        [SerializeField] bool despawnCharacters = false;
        [SerializeField] bool respawnCharacters = false;
        
        [Header("Characters")]
        [SerializeField] GameObject[] aiCharacters;
        [SerializeField] private List<GameObject> spawnedCharacters = new List<GameObject>();
        
        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void Start()
        {
            if (NetworkManager.Singleton.IsServer)
            {
                StartCoroutine(WaitForSceneToLoadThenSpawn());
            }
        }

        private void Update()
        {
            if (despawnCharacters)
            {
                despawnCharacters = false;
                DespawnAllCharacters();
            }

            if (respawnCharacters)
            {
                respawnCharacters = false;
                SpawnAllCharacters();
            }
        }

        private IEnumerator WaitForSceneToLoadThenSpawn()
        {
            while (!SceneManager.GetActiveScene().isLoaded)
            {
                yield return null;
            }
            
            SpawnAllCharacters();
        }

        private void SpawnAllCharacters()
        {
            foreach (GameObject character in aiCharacters)
            {
                GameObject instantiatedCharacter = Instantiate(character);
                instantiatedCharacter.GetComponent<NetworkObject>().Spawn();
               spawnedCharacters.Add(instantiatedCharacter); 
            }
        }

        private void DespawnAllCharacters()
        {
            foreach (GameObject character in spawnedCharacters)
            {
                character.GetComponent<NetworkObject>().Despawn();
            }
        }

        private void DisableAllCharacters()
        {
            
        }
    }
}