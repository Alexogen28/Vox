using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [Header("Managers")]
    [SerializeField] public PlayerController playerController;
    [SerializeField] public DecorationController decorationController;
    [SerializeField] public ObjectiveManager objectiveManager;
    [SerializeField] public InventoryManager inventoryManager;
    [SerializeField] public InterfaceManager interfaceManager;
    [SerializeField] public ItemGenerator itemGenerator;
    [SerializeField] public WorldManager worldManager;
    [SerializeField] public LevelManager levelManager;
    [SerializeField] public EnemySpawnManager enemySpawnManager;


    [Header("Item List")]
    [SerializeField] private List<BaseItemSO> availableItemsList;

    [Header("Base stats")]
    [SerializeField] private int numberOfStartingItems;

    public List<BaseItemSO> GetAvailableItemList => availableItemsList;


    void Start()
    {
        worldManager.GenerateWorldOnStartup();
        //Debug.Log("Successfully created world");

        playerController.GetAllComponents();
        playerController.actions.GetComponents();
        inventoryManager.InitialiseBasicBullet();
        inventoryManager.SetSlots();
        itemGenerator.BuildDictionary(availableItemsList);

        inventoryManager.SetStartingItems(availableItemsList);

        interfaceManager.InitializePlayerUI();
        interfaceManager.LoadBulletSprites();
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
