using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using Unity.VisualScripting;

namespace RS
{
    public class AIBossCharacterManager : AICharacterManager
    {
        public int bossID = 0;
        [SerializeField] private bool hasBeenDefeated;
        [SerializeField] private bool hasBeenAwakened;
        [SerializeField] private List<FogWallInteractable> fogWalls;

        [Header("DEBUG MENU")] [SerializeField]
        private bool wakeBossUp = false;
        
        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();

            if (IsServer)
            {
                // if save data doesnt contains information about this boss, add it
                if (!WorldSaveGameManager.instance.currentCharacterData.bossesAwakend.ContainsKey(bossID))
                {
                    WorldSaveGameManager.instance.currentCharacterData.bossesAwakend.Add(bossID, false);
                    WorldSaveGameManager.instance.currentCharacterData.bossesDefeated.Add(bossID, false);
                }
                // Otherwise load the data
                else
                {
                    hasBeenDefeated = WorldSaveGameManager.instance.currentCharacterData.bossesDefeated[bossID];
                    hasBeenAwakened = WorldSaveGameManager.instance.currentCharacterData.bossesAwakend[bossID];
                    
                }
                
                // Locate Fog wall
                StartCoroutine(GetFogWallsFromWorldObjectManager());
                
                if (hasBeenAwakened)
                {
                    for(int i =0; i < fogWalls.Count; i++)
                    {
                        fogWalls[i].isActive.Value = true;
                    }
                }
                
                if (hasBeenDefeated)
                {
                    for(int i =0; i < fogWalls.Count; i++)
                    {
                        fogWalls[i].isActive.Value = false;
                    }
                    
                    aiCharacterNetworkManager.isActive.Value = false;
                }
            }
        }

        protected override void Update()
        {
            base.Update();

            if (wakeBossUp)
            {
                wakeBossUp = false;
                WakeBoss();
            }
        }

        public IEnumerator GetFogWallsFromWorldObjectManager()
        {
            while (WorldObjectManager.instance.fogWalls.Count == 0)
            {
                yield return new WaitForEndOfFrame();
            }
            
            fogWalls = new List<FogWallInteractable>();
                
            foreach (var fogWall in WorldObjectManager.instance.fogWalls)
            {
                if(fogWall.fogWallID == bossID)
                    fogWalls.Add(fogWall);
            }

        }
        
        public override IEnumerator ProcessDeathEvent(bool manuallySelectDeathAnimation = false)
        {
            if (IsOwner)
            {
                characterNetworkManager.currentHealth.Value = 0;
                characterNetworkManager.isDead.Value = true;

                // Reset Flags


                if (!manuallySelectDeathAnimation)
                {
                    characterAnimationManager.PlayTargetActionAnimation("Dead_01", true);
                }

                hasBeenDefeated = true;
                // if save data doesnt contains information about this boss, add it
                if (!WorldSaveGameManager.instance.currentCharacterData.bossesAwakend.ContainsKey(bossID))
                {
                    WorldSaveGameManager.instance.currentCharacterData.bossesAwakend.Add(bossID, true);
                    WorldSaveGameManager.instance.currentCharacterData.bossesDefeated.Add(bossID, true);
                }
                // Otherwise load the data
                else
                {
                    WorldSaveGameManager.instance.currentCharacterData.bossesAwakend.Remove(bossID);
                    WorldSaveGameManager.instance.currentCharacterData.bossesDefeated.Remove(bossID);

                    WorldSaveGameManager.instance.currentCharacterData.bossesAwakend.Add(bossID, true);
                    WorldSaveGameManager.instance.currentCharacterData.bossesDefeated.Add(bossID, true);
                }

                WorldSaveGameManager.instance.SaveGame();
            }
            // Play Death SFX

            yield return new WaitForSeconds(5);
            
            // Award player with runes
        }

        public void WakeBoss()
        {
            hasBeenAwakened = true;
            
            // if save data doesnt contains information about this boss, add it
            if (!WorldSaveGameManager.instance.currentCharacterData.bossesAwakend.ContainsKey(bossID))
            {
                WorldSaveGameManager.instance.currentCharacterData.bossesAwakend.Add(bossID, true);
            }
            // Otherwise load the data
            else
            {
                WorldSaveGameManager.instance.currentCharacterData.bossesAwakend.Remove(bossID);
                WorldSaveGameManager.instance.currentCharacterData.bossesAwakend.Add(bossID, true);
            }

            for (int i = 0; i < fogWalls.Count; i++)
            {
                fogWalls[i].isActive.Value = true;
            }
        }
    }
}