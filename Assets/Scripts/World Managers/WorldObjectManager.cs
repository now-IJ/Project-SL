using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

namespace RS
{
    public class WorldObjectManager : MonoBehaviour
    {
        public static WorldObjectManager instance;

        [Header("Objects")] 
        [SerializeField] private List<NetworkObjectSpawner> networkObjectSpawners;
        [SerializeField] private List<GameObject> spawnedObjects = new List<GameObject>();

        [Header("Fog Walls")] 
        public List<FogWallInteractable> fogWalls;
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

        public void SpawnObject(NetworkObjectSpawner networkObjectSpawner)
        {
            if (NetworkManager.Singleton.IsServer)
            {
                networkObjectSpawners.Add(networkObjectSpawner);
                networkObjectSpawner.AttemptToSpawnObject();
            }
        }
        
        public void AddFogWallToList(FogWallInteractable fogWall)
        {
            if (!fogWalls.Contains(fogWall))
            {
                fogWalls.Add(fogWall);
            }
        }

        public void RemoveFogWallFromList(FogWallInteractable fogWall)
        {
            if (fogWalls.Contains(fogWall))
            {
                fogWalls.Remove(fogWall);
            }
        }
    }
}