using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [Header("Managers")]
    [SerializeField] public PlayerController playerController;
    [SerializeField] public InventoryManager inventoryManager;
    [SerializeField] public InterfaceManager interfaceManager;
    [SerializeField] public ItemGenerator itemGenerator;
    [SerializeField] public WorldManager worldManager;

    [Header("Item List")]
    [SerializeField] private List<BaseItemSO> availableItemsList;

    [Header("Base stats")]
    [SerializeField] private int numberOfStartingItems;

    public List<BaseItemSO> GetAvailableItemList => availableItemsList;


    void Start()
    {
        worldManager.GenerateWorldOnStartup();

        playerController.GetAllComponents();
        playerController.actions.GetComponents();
        inventoryManager.InitialiseBasicBullet();
        inventoryManager.SetSlots();
        itemGenerator.BuildDictionary(availableItemsList);

        inventoryManager.SetStartingItems(availableItemsList);

        interfaceManager.InitializePlayerUI();
        interfaceManager.LoadBulletSprites();

        DeterminePlayerSpawnLocation();
    }

    private void DeterminePlayerSpawnLocation()
    {
        Vector3Int spawnChunk = worldManager.GetRandomChunkCoords();
        spawnChunk.y = 0;

        int xInChunk = (int) Random.Range(spawnChunk.x * worldManager.chunkSize * worldManager.voxelSize,
            spawnChunk.x * worldManager.chunkSize * worldManager.voxelSize + worldManager.chunkSize * worldManager.voxelSize);
        int zInChunk = (int) Random.Range(spawnChunk.z * worldManager.chunkSize * worldManager.voxelSize,
            spawnChunk.z * worldManager.chunkSize * worldManager.voxelSize + worldManager.chunkSize * worldManager.voxelSize);

        float yInChunk = worldManager.chunkSize * worldManager.voxelSize;
        Vector3 raycastPosition = new Vector3(xInChunk, yInChunk, zInChunk);

        LayerMask groundMask = LayerMask.GetMask("Ground");

        if (Physics.Raycast(raycastPosition, Vector3.down, out RaycastHit hit, Mathf.Infinity, groundMask))
        {
            yInChunk = hit.point.y + 2.5f;
        }

        playerController.TeleportPlayer(new Vector3(xInChunk, yInChunk, zInChunk));
    }

    // private void SetStartingWeapon()
    // {
    //     for (int i = 0; i < startingWeapons.Count; i++) 
    //     {
    //         inventoryManager.TryAddItem(startingWeapons[i]);
    //     }
    //     inventoryManager.HandleClickOnSlot(0);
    // }

    // private void SetStartingBullets()
    // {
    //     for (int i = 0; i < startingBullets.Count; i++)
    //     {
    //         inventoryManager.TryAddItem(startingBullets[i]);
    //     }
    // }
}
