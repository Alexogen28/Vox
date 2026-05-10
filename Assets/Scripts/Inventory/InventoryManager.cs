using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    [Header("Controllers and Managers")]
    [SerializeField] private PlayerController playerController;
    [SerializeField] private ItemGenerator itemGenerator;

    [Header("UI Transform")]
    [SerializeField] private Transform positionForWeaponInUI;

    [Header("Player Stats")]
    [SerializeField] private int maxSlots = 6;

    [Header("Basics")]
    [SerializeField] private FireableBulletSO basicBulletSO;
    [SerializeReference] private List<InventorySlot> inventorySlots = new();

    private ProjectileBulletItem basicBullet;

    /*
     *  Note for future me --
     *  Bullet slots (or chambers, if you will!) are handled inside the Weapon.cs
     *  as it's the controller of the object that logically holds the bullets
     *  
     *  Instead, make an API inside InventoryManager to call the methods in
     *  Weapon.cs for accessing chambers 
     */

    // TODO
    // CAP INVENTORY SLOTS AT MAX VALUE (24 PROBABLY?)


    private Weapon currentEquipedWeapon = null;
    private int currentWeaponSlot = -1;

    private int currentlySelectedInventorySlot = -1;
    [SerializeField] private InventoryItem currentlySelectedItem;

    /*
     * EVENTS GO HERE
     */

    public static event Action<InventoryManager> OnWeaponEquip;
    public static event Action<int> OnBulletSlotClicked;
    public static event Action<int> OnBulletSlotDeselected;
    public static event Action<int> OnUpdateWeaponChamber;

    public int GetMaxSlots() => maxSlots;
    public Weapon GetEquipedWeapon() => currentEquipedWeapon;


    private void OnEnable()
    {
        InterfaceItemSlot.OnInventorySlotClicked += HandleClickOnSlot;
        InterfaceWeaponChamberSlot.OnChamberClicked += HandleClickOnSlot;
        currentlySelectedInventorySlot = -1;
    }

    private void OnDisable()
    {
        InterfaceItemSlot.OnInventorySlotClicked -= HandleClickOnSlot;
        InterfaceWeaponChamberSlot.OnChamberClicked -= HandleClickOnSlot;
        
        /*
         *  I dont know if this should go in OnDisable or in OnEnable
         *  so it goes into both!
         */
        currentlySelectedInventorySlot = -1;

    }

    public void InitialiseBasicBullet()
    {
        basicBullet = new ProjectileBulletItem();
        basicBullet.Init(basicBulletSO);
    }

    /*
     *  Handling of actual item slots by initializing them
     *  SetSlots will create empty InventorySlots (item in slot is null) and empty BulletSlots (a.k.a. chambers)
     *  while AddSlots will increase maximum nr of slots for the inventory and create the new slots
     *  
     *  Handle this in Game Manager to make sure stuff happens in the right order!
     *  
     *  Also can be called when you equip a weapon to reiterate over the available bullet slots.
     */

    public void SetSlots()
    {
        while (inventorySlots.Count < maxSlots) inventorySlots.Add(new InventorySlot());
    }

    public void AddSlots(int numberOfSlotsToAdd)
    {
        maxSlots += numberOfSlotsToAdd;
        SetSlots();
    }
    
    /*
     * Try to add an item to a slot, by sending it the actual item to be added
     * it checks to see if inventory is filled
     * and if not, just sets an item there by calling InventorySlot item setting method
     */

    public bool TryAddItem(InventoryItem itemToAdd)
    {
        for (int i = 0; i < inventorySlots.Count; i++)
        {
            if (inventorySlots[i].IsSlotOccupied == true) continue;

            inventorySlots[i].SetItemInSlot(itemToAdd);
            return true;
        }
        return false;
    }

    /*
     * Set the item in the respective slot to empty
     * Remember, arrays start at 0!
     * 
     * Also a getter for an item in a slot, nothing fancy
     */

    public bool TryRemoveItem(int slotToRemoveFrom)
    {
        if (slotToRemoveFrom < 0 || slotToRemoveFrom >= inventorySlots.Count) return false;
        inventorySlots[slotToRemoveFrom].SetEmpty();
        return true;
    }

    public InventoryItem TryGetItemInSlot(int slotToGetItemFrom)
    {
        if (slotToGetItemFrom < 0 || slotToGetItemFrom >= inventorySlots.Count) return null;
        return inventorySlots[slotToGetItemFrom].GetItemInSlot();
    }

    public void HandleClickOnSlot(int clickedSlot)
    {
        if (clickedSlot < 0) return;

        if (clickedSlot > 63)
        {
            HandleClickOnChamber(clickedSlot);
            return;
        }
        var item = inventorySlots[clickedSlot].GetItemInSlot();
        if (item is WeaponItem) HandleEquipWeapon(clickedSlot);
        if (item is BulletItem) HandleClickOnBullet(clickedSlot);
    }

    private void HandleClickOnChamber(int clickedSlot)
    {
        if (currentlySelectedInventorySlot < 0 || currentlySelectedInventorySlot > 63)
            return;

        BulletItem bullet = inventorySlots[currentlySelectedInventorySlot].GetItemInSlot() as BulletItem;
        if (bullet == null || currentEquipedWeapon == null)
            return;

        int chamberIndex = clickedSlot - 64;

        if (currentEquipedWeapon.TryLoadBullet(bullet, chamberIndex))
        {
            OnUpdateWeaponChamber?.Invoke(clickedSlot);
            Debug.Log("Loaded bullet at chamber: " + chamberIndex);
        }
    }

    /*
     * Handle equiping of weapon from passed slot
     * 
     * Checks and unequips the previous weapon if necessary
     */

    public void HandleEquipWeapon(int slotToEquipFrom)
    {
        if (currentWeaponSlot == slotToEquipFrom) return;

        UnequipPreviousWeapon();

        currentWeaponSlot = slotToEquipFrom;
        WeaponItem weaponToEquip = inventorySlots[slotToEquipFrom].GetItemInSlot() as WeaponItem;

        Transform rotationOfWeapon = playerController.gameObject.transform.GetChild(0);

        GameObject weaponToInstantiate = Instantiate(weaponToEquip.prefab, positionForWeaponInUI.position, rotationOfWeapon.rotation, positionForWeaponInUI);

        if (!weaponToInstantiate.TryGetComponent<Weapon>(out var weaponComponent))
        {
            Destroy(weaponToInstantiate);
            return;
        }

        currentEquipedWeapon = weaponComponent;
        weaponComponent.Init(weaponToEquip);

        playerController.actions.gunEnd = weaponComponent.GetMuzzle();


        for (int i = 0; i < weaponComponent.GetMaxChambers(); i++)
        {
            weaponComponent.TryLoadBullet(basicBullet, i);
        }

        OnWeaponEquip?.Invoke(this);
        SetSlots();
    }

    private void UnequipPreviousWeapon()
    {
        if (currentEquipedWeapon == null) return;

        Destroy(currentEquipedWeapon.gameObject);
        currentEquipedWeapon = null;
        currentWeaponSlot = -1;
    }

    /*
     * Method to get the currently equipped weapon
     * to handle some logic elsewhere
     */


    /*
     *  Method to handle what happens when you click on a bullet
     *  It should store the value inside the currently selected bullet 
     *  variable declared up top 
     *  and then do nothing more with it because the other interaction is part 
     *  of another bunch of code
     */

    public void HandleClickOnBullet(int clickedSlot)
    {
        if (currentlySelectedInventorySlot == clickedSlot)
        {
            currentlySelectedInventorySlot = -1;
            currentlySelectedItem = null;
            OnBulletSlotDeselected?.Invoke(clickedSlot);
            return;
        }

        currentlySelectedInventorySlot = clickedSlot;
        currentlySelectedItem = inventorySlots[clickedSlot].GetItemInSlot();
        OnBulletSlotClicked?.Invoke(clickedSlot);
    }

    public void ResetSelectedBullet()
    {
        currentlySelectedItem = null;
        currentlySelectedInventorySlot = -1;
    }

    /*
        TODO
        Generate a certain number of random items at start
    */

    public void SetStartingItems(List<BaseItemSO> itemsList, int numberOfItems)
    {
        foreach(BaseItemSO item in itemsList)
        {
            itemGenerator.GenerateItemAndAddToInventory(item);
        }
    }

    /*
        Set a fixed set of starter items
        A common weapon, a bullet and a magical bullet

        Add a chance to spawn in with a higher rarity item
        as a different overload
    */

    public void SetStartingItems(List<BaseItemSO> itemList)
    {
        List<WeaponSO> weapons = new();
        List<FireableBulletSO> projectiles = new();
        List<MagicalBulletSO> magics = new();

        System.Random rngesus = new System.Random();

        foreach(var item in itemList)
        {
            if(item.rarity != Rarity.Common)
                continue;

            if(item is WeaponSO weapon)
            {
                weapons.Add(weapon);
                continue;
            }

            if(item is FireableBulletSO boolet)
            {
                projectiles.Add(boolet);
                continue;
            }

            if(item is MagicalBulletSO magic)
            {
                magics.Add(magic);
                continue;
            }            
        }

        // int rand = rngesus.Next(weapons.Count);
        // WeaponSO pickedWeapon = weapons[rand];

        // rand = rngesus.Next(projectiles.Count);
        // FireableBulletSO pickedBullet = projectiles[rand];

        // rand = rngesus.Next(magics.Count);
        // MagicalBulletSO pickedMagic = magics[rand];

        WeaponSO pickedWeapon = weapons[0];
        FireableBulletSO pickedBullet = projectiles[0];
        MagicalBulletSO pickedMagic = magics[0];

        itemGenerator.GenerateItemAndAddToInventory(pickedWeapon);
        itemGenerator.GenerateItemAndAddToInventory(pickedBullet);
        itemGenerator.GenerateItemAndAddToInventory(pickedMagic);

        HandleEquipWeapon(0);
    }
}
