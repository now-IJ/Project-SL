using System;
using Unity.Netcode;
using UnityEngine;

namespace RS
{
    public class PlayerUIManager : MonoBehaviour
    {
        public static PlayerUIManager instance;
        
        [Header("Network Join")] 
        [SerializeField] private bool startGameAsClient;

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
            DontDestroyOnLoad(gameObject);
        }

        private void Update()
        {
            if (startGameAsClient)
            {
                startGameAsClient = false;
                NetworkManager.Singleton.Shutdown(); // Shut down the network in order to be able to rejoin as Client
                NetworkManager.Singleton.StartClient(); 
            }
        }
    }
}
