using System;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace RS
{
    public class TitleScreenManager : MonoBehaviour
    {
        public static TitleScreenManager instance;
        
        [Header("Menu Objects")]
        [SerializeField] private GameObject titleScreenMainMenu;
        [SerializeField] private GameObject titleScreenLoadMenu;
        
        [Header("Buttons")]
        [SerializeField] private Button loadMenuReturnButton;
        [SerializeField] private Button mainMenuNewGameButton;
        [SerializeField] private Button mainMenuLoadGameButton;
        [SerializeField] private Button noCharacterSlotPopUpButton;
        [SerializeField] private Button deleteCharacterSlotPopUpConfirmButton;

        [Header("Pop-Ups")]
        [SerializeField] GameObject noCharacterSlotsPopUp;
        [SerializeField] GameObject deleteCharacterSlotPopUp;

        [FormerlySerializedAs("currentCharacterSlot")] [Header("Character Slots")] 
        public CharacterSlot currentSelectedSlot = CharacterSlot.NO_SLOT;
        
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
        
        public void StartNetworkAsHost()
        {
            NetworkManager.Singleton.StartHost();
        }

        public void StartNewGame()
        {
            WorldSaveGameManager.instance.AttemptToCreateNewGame();
        }

        public void OpenLoadGameMenu()
        {
            titleScreenMainMenu.SetActive(false);
            titleScreenLoadMenu.SetActive(true);
            
            loadMenuReturnButton.Select();
        }

        public void CloseLoadGameMenu()
        {
            titleScreenLoadMenu.SetActive(false);
            titleScreenMainMenu.SetActive(true);
            
            mainMenuLoadGameButton.Select();
        }

        public void DisplayNoFreeCharacterSlot()
        {
            noCharacterSlotsPopUp.SetActive(true);
            noCharacterSlotPopUpButton.Select();
        }

        public void CloseNoFreeCharacterSlotPopUp()
        {
            noCharacterSlotsPopUp.SetActive(false);
            mainMenuNewGameButton.Select();
        }

        // Character Slots
        
        public void SelectCharacterSlot(CharacterSlot characterSlot)
        {
            currentSelectedSlot = characterSlot;
        }

        public void SelectNoSlot()
        {
            currentSelectedSlot = CharacterSlot.NO_SLOT;
        }

        public void AttemptToDeleteCharacterSlots()
        {
            if (currentSelectedSlot != CharacterSlot.NO_SLOT)
            {
                deleteCharacterSlotPopUp.SetActive(true);
                deleteCharacterSlotPopUpConfirmButton.Select();
            }
        }

        public void DeleteCharacterSlot()
        {
            deleteCharacterSlotPopUp.SetActive(false);
            WorldSaveGameManager.instance.DeleteGame(currentSelectedSlot);
            
            titleScreenLoadMenu.SetActive(false);
            titleScreenLoadMenu.SetActive(true);
            
            loadMenuReturnButton.Select();
            
        }
        
        public void CloseDeleteCharacterPopUp()
        {
            deleteCharacterSlotPopUp.SetActive(false);
            loadMenuReturnButton.Select();
        }
    }
}
