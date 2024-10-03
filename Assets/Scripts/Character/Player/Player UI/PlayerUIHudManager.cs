using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace RS
{
    public class PlayerUIHudManager : MonoBehaviour
    {
        [Header("Stat Bars")]
        [SerializeField] private UI_StatBar healthBar;
        [SerializeField] private UI_StatBar staminaBar;

        [Header("Quick Slots")] 
        [SerializeField] private Image rightWeaponQuickSlotIcon;
        [SerializeField] private Image leftWeaponQuickSlotIcon;
        
        private void Start()
        {
            SceneManager.activeSceneChanged += OnSceneChanged;
            
            this.gameObject.SetActive(false);
            
            rightWeaponQuickSlotIcon.enabled = false;
            rightWeaponQuickSlotIcon.sprite = null;
            
            leftWeaponQuickSlotIcon.enabled = false;
            leftWeaponQuickSlotIcon.sprite = null;
            
        }
        
        private void OnSceneChanged(Scene oldScene, Scene newScene)
        {
            if (newScene.buildIndex == WorldSaveGameManager.instance.GetWorldSceneIndex())
            {
                this.gameObject.SetActive(true);
            }
            else
            {
                this.gameObject.SetActive(false);
            }
        }

        public void RefreshHUD()
        {
            healthBar.gameObject.SetActive(false);
            healthBar.gameObject.SetActive(true);
            staminaBar.gameObject.SetActive(false);
            staminaBar.gameObject.SetActive(true);
        }
        
        public void SetNewHealthValue(int oldValue, int newValue)
        {
            healthBar.SetStat(newValue);
        }

        public void SetMaxHealthValue(int maxHealth)
        {
            healthBar.SetMaxStat(maxHealth);
        }
        
        public void SetNewStaminaValue(float oldValue, float newValue)
        {
            staminaBar.SetStat(Mathf.RoundToInt(newValue));
        }

        public void SetMaxStaminaValue(int maxStamina)
        {
            staminaBar.SetMaxStat(maxStamina);
        }

        public void SetRightWeaponQuickSlotIcon(int weaponID)
        {
            WeaponItem weapon = WorldItemDatabase.instance.GetWeaponByID(weaponID);
            
            // Check if that weapon is in the database
            if (weapon == null)
            {
                Debug.Log("Item not found");
                rightWeaponQuickSlotIcon.enabled = false;
                rightWeaponQuickSlotIcon.sprite = null;
                return;
            }

            // Check if weapon has icon
            if (weapon.itemIcon == null)
            {
                Debug.Log("Item has no icon");
                rightWeaponQuickSlotIcon.enabled = false;
                rightWeaponQuickSlotIcon.sprite = null;
                return;
            }

            rightWeaponQuickSlotIcon.sprite = weapon.itemIcon;
            rightWeaponQuickSlotIcon.enabled = true;

        }
        
        public void SetLeftWeaponQuickSlotIcon(int weaponID)
        {
            WeaponItem weapon = WorldItemDatabase.instance.GetWeaponByID(weaponID);
            
            // Check if that weapon is in the database
            if (weapon == null)
            {
                Debug.Log("Item not found");
                leftWeaponQuickSlotIcon.enabled = false;
                leftWeaponQuickSlotIcon.sprite = null;
                return;
            }

            // Check if weapon has icon
            if (weapon.itemIcon == null)
            {
                Debug.Log("Item has no icon");
                leftWeaponQuickSlotIcon.enabled = false;
                leftWeaponQuickSlotIcon.sprite = null;
                return;
            }

            leftWeaponQuickSlotIcon.sprite = weapon.itemIcon;
            leftWeaponQuickSlotIcon.enabled = true;

        }
    }
}