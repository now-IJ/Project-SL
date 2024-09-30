using System;
using NUnit.Framework;
using TMPro;
using UnityEngine;

namespace RS
{
    public class UI_Charcter_Save_Slot : MonoBehaviour
    {
        private SaveFileDataWriter saveFileDataWriter;

        [Header("Game Slot")] 
        public CharacterSlot characterSlot;

        [Header("Character Info")] 
        public TextMeshProUGUI characterName;
        public TextMeshProUGUI timePlayed;

        private void OnEnable()
        {
            LoadSaveSlot();
        }

        private void LoadSaveSlot()
        {
            saveFileDataWriter = new SaveFileDataWriter();
            saveFileDataWriter.saveDataDirectoryPath = Application.persistentDataPath;

            switch (characterSlot)
            {
                case CharacterSlot.CharacterSlot_01:
                    saveFileDataWriter.saveFileName =
                        WorldSaveGameManager.instance.DecideCharacterFileNameBasedOnCharacterSlotBeingUsed(characterSlot);

                    if (saveFileDataWriter.CheckToSeeIfFileExists())
                    {
                        characterName.text = WorldSaveGameManager.instance.characterSlot01.characterName;
                    }
                    else
                    {
                        gameObject.SetActive(false);
                    }
                    break;
                
                case CharacterSlot.CharacterSlot_02:
                    
                    
                    saveFileDataWriter.saveFileName =
                        WorldSaveGameManager.instance.DecideCharacterFileNameBasedOnCharacterSlotBeingUsed(characterSlot);

                    if (saveFileDataWriter.CheckToSeeIfFileExists())
                    {
                        characterName.text = WorldSaveGameManager.instance.characterSlot02.characterName;
                    }
                    else
                    {
                        gameObject.SetActive(false);
                    }
                    break;
                
                case CharacterSlot.CharacterSlot_03:
                    
                    
                    saveFileDataWriter.saveFileName =
                        WorldSaveGameManager.instance.DecideCharacterFileNameBasedOnCharacterSlotBeingUsed(characterSlot);

                    if (saveFileDataWriter.CheckToSeeIfFileExists())
                    {
                        characterName.text = WorldSaveGameManager.instance.characterSlot03.characterName;
                    }
                    else
                    {
                        gameObject.SetActive(false);
                    }
                    break;
                
                case CharacterSlot.CharacterSlot_04:
                    
                    
                    saveFileDataWriter.saveFileName =
                        WorldSaveGameManager.instance.DecideCharacterFileNameBasedOnCharacterSlotBeingUsed(characterSlot);

                    if (saveFileDataWriter.CheckToSeeIfFileExists())
                    {
                        characterName.text = WorldSaveGameManager.instance.characterSlot04.characterName;
                    }
                    else
                    {
                        gameObject.SetActive(false);
                    }
                    break;
                
                case CharacterSlot.CharacterSlot_05:
                    
                    
                    saveFileDataWriter.saveFileName =
                        WorldSaveGameManager.instance.DecideCharacterFileNameBasedOnCharacterSlotBeingUsed(characterSlot);

                    if (saveFileDataWriter.CheckToSeeIfFileExists())
                    {
                        characterName.text = WorldSaveGameManager.instance.characterSlot05.characterName;
                    }
                    else
                    {
                        gameObject.SetActive(false);
                    }
                    break;
                
                case CharacterSlot.CharacterSlot_06:
                    
                    
                    saveFileDataWriter.saveFileName =
                        WorldSaveGameManager.instance.DecideCharacterFileNameBasedOnCharacterSlotBeingUsed(characterSlot);

                    if (saveFileDataWriter.CheckToSeeIfFileExists())
                    {
                        characterName.text = WorldSaveGameManager.instance.characterSlot06.characterName;
                    }
                    else
                    {
                        gameObject.SetActive(false);
                    }
                    break;
                
                case CharacterSlot.CharacterSlot_07:
                    
                    
                    saveFileDataWriter.saveFileName =
                        WorldSaveGameManager.instance.DecideCharacterFileNameBasedOnCharacterSlotBeingUsed(characterSlot);

                    if (saveFileDataWriter.CheckToSeeIfFileExists())
                    {
                        characterName.text = WorldSaveGameManager.instance.characterSlot07.characterName;
                    }
                    else
                    {
                        gameObject.SetActive(false);
                    }
                    break;
                
                case CharacterSlot.CharacterSlot_08:
                    
                    
                    saveFileDataWriter.saveFileName =
                        WorldSaveGameManager.instance.DecideCharacterFileNameBasedOnCharacterSlotBeingUsed(characterSlot);

                    if (saveFileDataWriter.CheckToSeeIfFileExists())
                    {
                        characterName.text = WorldSaveGameManager.instance.characterSlot08.characterName;
                    }
                    else
                    {
                        gameObject.SetActive(false);
                    }
                    break;
                
                case CharacterSlot.CharacterSlot_09:
                    
                    
                    saveFileDataWriter.saveFileName =
                        WorldSaveGameManager.instance.DecideCharacterFileNameBasedOnCharacterSlotBeingUsed(characterSlot);

                    if (saveFileDataWriter.CheckToSeeIfFileExists())
                    {
                        characterName.text = WorldSaveGameManager.instance.characterSlot09.characterName;
                    }
                    else
                    {
                        gameObject.SetActive(false);
                    }
                    break;
                
                case CharacterSlot.CharacterSlot_10:
                    
                    
                    saveFileDataWriter.saveFileName =
                        WorldSaveGameManager.instance.DecideCharacterFileNameBasedOnCharacterSlotBeingUsed(characterSlot);

                    if (saveFileDataWriter.CheckToSeeIfFileExists())
                    {
                        characterName.text = WorldSaveGameManager.instance.characterSlot10.characterName;
                    }
                    else
                    {
                        gameObject.SetActive(false);
                    }
                    break;
                
                default:
                    break;
            }
        }

        public void LoadGameFromCharacterSlot()
        {
            WorldSaveGameManager.instance.currentCharacterSlotBeingUsed = characterSlot;
            WorldSaveGameManager.instance.LoadGame();
        }
    }
}
