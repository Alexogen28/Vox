using UnityEngine;


/*
 * This is the empty pocket that you can place an
 * item in (namely, any InventoryItem) !!!
 * InventoryManager will have a list of these to manage
 * 
 * InventoryItem can be -- bullet, consumable, weapon
 * 
 * On construction it is empty,
 * and you get multiple methods to play with
 * 
 * You can get the item from the slot
 *         set the item in the slot
 *         remove the item from the slot
 *         
 *         
 */


[System.Serializable]
public class InventorySlot
{
    [SerializeField] private InventoryItem itemInSlot = null;

    public InventoryItem GetItemInSlot() => itemInSlot;
    public void SetItemInSlot(InventoryItem item) => itemInSlot = item;
    public void SetEmpty() => itemInSlot = null;
    public bool IsSlotOccupied => itemInSlot != null;
}
