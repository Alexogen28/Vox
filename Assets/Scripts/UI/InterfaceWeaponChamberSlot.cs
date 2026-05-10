using System;
using UnityEngine;
using UnityEngine.UI;

public class InterfaceWeaponChamberSlot : MonoBehaviour
{
    //chamberIds start from 64
    [SerializeField] private int chamberId;
    [SerializeField] private Image slotIcon;
    
    private InventoryManager inventoryManager;

    public static Action<int> OnChamberClicked;

    public int GetChamberId() => chamberId;

    private void Awake()
    {
        inventoryManager = FindFirstObjectByType<InventoryManager>();
    }
    private void OnEnable()
    {
        InventoryManager.OnUpdateWeaponChamber += UpdateItemSprite;
        InventoryManager.OnWeaponEquip += RefreshChamberSprite;
        RefreshChamberSprite();
    }

    private void OnDisable()
    {
        InventoryManager.OnUpdateWeaponChamber -= UpdateItemSprite;
        InventoryManager.OnWeaponEquip -= RefreshChamberSprite;
    }

    private void RefreshChamberSprite()
    {
        if(chamberId - 64 >= inventoryManager.GetEquipedWeapon().GetMaxChambers())
        {
            gameObject.SetActive(false);
            return;
        }

        gameObject.SetActive(true);
        var currentlyEquipedWeapon = inventoryManager.GetEquipedWeapon();
        Debug.Log("Current weapon is: " + currentlyEquipedWeapon.name);

        var chamber = currentlyEquipedWeapon.GetWeaponChambers()[chamberId - 64];
        Debug.Log("Current chamber contains: " + chamber.GetBulletInChamber().itemName);

        var bullet = chamber.GetBulletInChamber();
        slotIcon.sprite = bullet.itemSprite;
    }

    private void RefreshChamberSprite(InventoryManager inv)
    {
        if (chamberId - 64 >= inventoryManager.GetEquipedWeapon().GetMaxChambers())
        {
            gameObject.SetActive(false);
            return;
        }

        gameObject.SetActive(true);
        slotIcon.sprite = inventoryManager.GetEquipedWeapon().GetWeaponChambers()[chamberId - 64].GetBulletInChamber().itemSprite;
    }

    public void ChamberSlotClicked()
    {
        Debug.Log("Chamber slot clicked: " + chamberId);
        OnChamberClicked?.Invoke(chamberId);
    }

    private void UpdateItemSprite(int chamberId)
    {
        if(this.chamberId == chamberId)
            RefreshChamberSprite();
    }
}
