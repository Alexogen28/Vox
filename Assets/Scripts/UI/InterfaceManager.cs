using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InterfaceManager : MonoBehaviour
{
    [Header("Managers and Controllers")]
    [SerializeField] private InventoryManager inventoryManager;

    [Header("UIs")]
    [SerializeField] private GameObject PlayerUI;
    [SerializeField] private GameObject InventoryUI;
    [SerializeField] private GameObject MenuUI;

    [Header("Player UI Sprites")]
    [SerializeField] private List<Image> bulletSprites = new();
    [SerializeField] private List<Image> activitySprites = new();
    [SerializeField] private List<Image> bulletSlotSprites = new();

    /*
     *    0 - No menu
     *    1 - Inventory
     *    2 - Options Screen
     */


    private int currentlyOpenMenu = 0;
    private int currentlyActiveChamber = 0;

    private void OnEnable()
    { 
        Weapon.OnWeaponFire += UpdatePlayerUI;
        Weapon.OnWeaponReload += UpdatePlayerUI;

        PlayerInputs.OnInventoryOpen += SwapInventoryMenu;

        /*
         * Note - this one is for the gameplayUI
         * Not for the inventory UI
         * 
         * And actually, this does nothing -- the Gameplay UI is disabled when I equip a weapon :v
         */
        InventoryManager.OnWeaponEquip += LoadBulletSprites;
        Debug.Log("InterfaceManager subscribed to all");
    }

    private void OnDisable()
    {
        Weapon.OnWeaponFire -= UpdatePlayerUI;
        Weapon.OnWeaponReload -= UpdatePlayerUI;

        PlayerInputs.OnInventoryOpen -= SwapInventoryMenu;

        InventoryManager.OnWeaponEquip -= LoadBulletSprites;
    }

    public void InitializePlayerUI()
    {
        for(int i = 0; i < bulletSprites.Count; i++)
            bulletSprites[i].enabled = false;

        for(int i = 0;i < activitySprites.Count; i++)
            activitySprites[i].enabled = false;

        activitySprites[0].enabled = true;
        currentlyOpenMenu = 0;
        PlayerUI.SetActive(true);
        InventoryUI.SetActive(false);
    }

    void RefreshGameplayUI()
    {
        var currentlyEquipedWeapon = inventoryManager.GetEquipedWeapon();
        List<WeaponChamber> weaponChambers = currentlyEquipedWeapon.GetWeaponChambers();

        for (int i = 0; i < bulletSprites.Count; i++)
        {
            if (i < currentlyEquipedWeapon.GetMaxChambers())
            {
                bulletSprites[i].enabled = true;
                bulletSprites[i].sprite = weaponChambers[i].GetBulletInChamber().itemSprite;
            }
            else bulletSprites[i].enabled = false;
        }

        ResetActivitySprites();
        
        currentlyActiveChamber = currentlyEquipedWeapon.GetCurrentChamber();
        activitySprites[currentlyActiveChamber].enabled = true;
    }

    void LoadBulletSprites(InventoryManager inventoryManager)
    {
        List<WeaponChamber> weaponChambers = inventoryManager.GetEquipedWeapon().GetWeaponChambers();

        for (int i = 0; i < bulletSprites.Count; i++)
        {
            if (i < inventoryManager.GetEquipedWeapon().GetMaxChambers())
            {
                bulletSprites[i].enabled = true;
                bulletSprites[i].sprite = weaponChambers[i].GetBulletInChamber().itemSprite;
            }
            else bulletSprites[i].enabled = false;
        }

        ResetActivitySprites();

        activitySprites[0].enabled = true;

        currentlyActiveChamber = 0;
    }

    public void LoadBulletSprites()
    {
        List<WeaponChamber> weaponChambers = inventoryManager.GetEquipedWeapon().GetWeaponChambers();

        for (int i = 0; i < bulletSprites.Count; i++)
        {
            if (i < inventoryManager.GetEquipedWeapon().GetMaxChambers())
            {
                bulletSprites[i].enabled = true;
                bulletSprites[i].sprite = weaponChambers[i].GetBulletInChamber().itemSprite;
            }
            else bulletSprites[i].enabled = false;
        }

        ResetActivitySprites();

        activitySprites[0].enabled = true;

        currentlyActiveChamber = 0;
    }

    private void ResetActivitySprites()
    {
        for (int i = 0; i < activitySprites.Count; i++)
            activitySprites[i].enabled = false;
    }

    void UpdatePlayerUI(Weapon weapon)
    {
        int newChamber = weapon.GetCurrentChamber();

        if(currentlyActiveChamber >=0)
            activitySprites[currentlyActiveChamber].enabled = false;

        if (newChamber < weapon.GetMaxChambers())
        {
            activitySprites[newChamber].enabled = true;
            currentlyActiveChamber = newChamber;
        }
        else
        {
            ResetActivitySprites();
            currentlyActiveChamber = -1;
        }
    }

    void SwapInventoryMenu(PlayerInputs playerInputs)
    {
        if(currentlyOpenMenu == 0)
        {
            currentlyOpenMenu = 1;
            PlayerUI.SetActive(false);
            InventoryUI.SetActive(true);
            Debug.Log("Changed to inventory");

            LoadBulletSlotsInInventoryUI();

            return;
        }

        if(currentlyOpenMenu == 1)
        {
            currentlyOpenMenu = 0;
            PlayerUI.SetActive(true);
            InventoryUI.SetActive(false);
            Debug.Log("Changed to normal UI");
            inventoryManager.ResetSelectedBullet();
            RefreshGameplayUI();
            return;
        }
    }

    private void LoadBulletSlotsInInventoryUI()
    {
        int currentEquipedWeaponMaxChambers = inventoryManager.GetEquipedWeapon().GetMaxChambers();
        List<WeaponChamber> weaponChambers = inventoryManager.GetEquipedWeapon().GetWeaponChambers();

        for (int i = 0; i < currentEquipedWeaponMaxChambers; i++)
        {
            bulletSlotSprites[i].enabled = true;
            bulletSlotSprites[i].sprite = weaponChambers[i].GetBulletInChamber().itemSprite;
        }
        for (int i = currentEquipedWeaponMaxChambers; i < 12; i++)
        {
            bulletSlotSprites[i].enabled = false;
        }
    }
}
