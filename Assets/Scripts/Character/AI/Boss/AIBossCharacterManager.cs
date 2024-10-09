using System.Collections;
using UnityEngine;
using Unity.Netcode;

namespace RS
{
    public class AIBossCharacterManager : AICharacterManager
    {
        public int bossID = 0;
        [SerializeField] private bool hasBeenDefeated;

        
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

                    if (hasBeenDefeated)
                    {
                        aiCharacterNetworkManager.isActive.Value = false;
                    }
                }
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
        
    }
}