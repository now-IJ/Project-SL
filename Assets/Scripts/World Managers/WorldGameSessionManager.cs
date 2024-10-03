using System;
using System.Collections.Generic;
using UnityEngine;

namespace RS
{
    public class WorldGameSessionManager : MonoBehaviour
    {
        public static WorldGameSessionManager instance;
        
        [Header("Active Players In Session")] 
        public List<PlayerManager> players = new List<PlayerManager>();

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

        public void AddPlayerToActivePlayerList(PlayerManager player)
        {
            // Add the player only to the list if he is not already in it
            if (!players.Contains(player))
            {
                players.Add(player);
            }
            
            // check the list for null slots and remove them
            for (int i = players.Count - 1; i > -1; i--)
            {
                if (players[i] == null)
                {
                    players.RemoveAt(i);
                }
            }
        }
        
        public void RemovePlayerToActivePlayerList(PlayerManager player)
        {
            if (players.Contains(player))
            {
                players.Remove(player);
            }

            for (int i = players.Count - 1; i > -1; i--)
            {
                if (players[i] == null)
                {
                    players.RemoveAt(i);
                }
            }
        }
    }
}