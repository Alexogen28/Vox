using System;
using UnityEngine;
using UnityEngine.UI;

public class InterfaceItemSlot : MonoBehaviour
{
    [SerializeField] private int slotId;
    [SerializeField] private Image slotIcon;
    [SerializeField] Sprite emptySprite;
    [SerializeField] Sprite blockedSprite;
    [SerializeField] Sprite selectionBorderSprite;
    private InventoryManager inventoryManager;

    public static Action<int> OnInventorySlotClicked;

    public int GetSlotId() => slotId;

    void Awake()
    {
        inventoryManager = FindFirstObjectByType<InventoryManager>();
    }

    private void OnEnable()
    {
        InventoryManager.OnBulletSlotClicked += ReturnedSlotClicked;
        Refresh();
    }

    private void OnDisable()
    {
        InventoryManager.OnBulletSlotClicked -= ReturnedSlotClicked;
    }

    void Refresh()
    {
        if (slotIcon == null) return;

        var itemInSlot = inventoryManager.TryGetItemInSlot(slotId);

        if (slotId >= inventoryManager.GetMaxSlots())
        {
            slotIcon.sprite = blockedSprite;
            return;
        }

        if (itemInSlot == null)
        {
            slotIcon.sprite = emptySprite;
            return;
        }

        slotIcon.sprite = itemInSlot.itemSprite;
        
    }

    public void InventorySlotClicked()
    {
        if(slotId < inventoryManager.GetMaxSlots())
        {
            OnInventorySlotClicked?.Invoke(slotId);
            if(inventoryManager.TryGetItemInSlot(slotId) is BulletItem)
            {

            }
        }
    }

    public void ReturnedSlotClicked(int slotClicked)
    {
        //if(slotId == slotClicked)
            
    }
}
