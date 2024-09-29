using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

namespace RS
{
    public class TitleScreenManager : MonoBehaviour
    {
        [Header("Menu Objects")]
        [SerializeField] private GameObject titleScreenMainMenu;
        [SerializeField] private GameObject titleScreenLoadMenu;
        
        [Header("Buttons")]
        [SerializeField] Button loadMenuReturnButton;
        [SerializeField] Button mainMenuLoadGameButton;
        
        
        public void StartNetworkAsHost()
        {
            NetworkManager.Singleton.StartHost();
        }

        public void StartNewGame()
        {
            WorldSaveGameManager.instance.CreateNewGame();
            StartCoroutine(WorldSaveGameManager.instance.LoadWorldScene());
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
    }
}
