using UnityEngine;

public class ItemPickup : MonoBehaviour
{
    InventoryManager inventoryManager;

    private InventoryItem item;

    void Start()
    {
        inventoryManager = FindFirstObjectByType<InventoryManager>();
    }

    /*
     * TODO
     * Gentle oscilation/movement of the item prefab
     * 
     */
    void Update()
    {
        
    }

    public void AddItemToPlayer()
    {
        inventoryManager.TryAddItem(item);
    }
}
