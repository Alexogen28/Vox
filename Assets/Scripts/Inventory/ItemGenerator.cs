using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ItemGenerator : MonoBehaviour
{
    [Header("Managers")]
    [SerializeField] private GameManager gameManager;

    [Header("Generation params")]
    [SerializeField] private int numberOfRarities = 4;

    private Dictionary<Rarity, List<BaseItemSO>> itemDictionary;


    public void BuildDictionary(List<BaseItemSO> itemsList)
    {
        itemDictionary = new();

        foreach (Rarity rarity in System.Enum.GetValues(typeof(Rarity)))
        {
            itemDictionary[rarity] = new List<BaseItemSO>();
        }

        foreach (BaseItemSO item in itemsList)
        {
            itemDictionary[item.rarity].Add(item);
        }
    }

    public Rarity DetermineRarity()
    {
        float rareChance = 65.0f;
        float legendaryChance = 88.0f;
        float obscureChance = 95.0f;

        float rarityRoll = Random.Range(0, 100.0f);

        if (rarityRoll > obscureChance)
            return Rarity.Obscure;

        if (rarityRoll > legendaryChance)
            return Rarity.Legendary;

        if (rarityRoll > rareChance)
            return Rarity.Rare;

        return Rarity.Common;
    }


    /*
     * Item types are
     * 0 - projectile bullet
     * 1 - magical bullet 
     * 2 - weapon
     * 3 - consumable
     */
    public void DetermineItemToGenerate(int itemType)
    {
        Rarity itemRarity = DetermineRarity();


    }

    public void GenerateItemAndAddToInventory(BaseItemSO itemToGenerate)
    {
        if(itemToGenerate is ConsumableSO)
        {
            GenerateConsumableAndAddToInventory(itemToGenerate as ConsumableSO);
            return;
        }

        if(itemToGenerate is WeaponSO)
        {
            GenerateWeaponAndAddToInventory(itemToGenerate as WeaponSO);
            return;
        }

        if(itemToGenerate is MagicalBulletSO)
        {
            GenerateMagicalAndAddToInventory(itemToGenerate as MagicalBulletSO);
            return;
        }

        if(itemToGenerate is FireableBulletSO)
        {
            GenerateProjectileAndAddToInventory(itemToGenerate as FireableBulletSO);
            return;
        }
    }

    private void GenerateProjectileAndAddToInventory(FireableBulletSO stats)
    {
        ProjectileBulletItem item = new ProjectileBulletItem();

        item.Init(stats);

        gameManager.inventoryManager.TryAddItem(item);
    }

    private void GenerateMagicalAndAddToInventory(MagicalBulletSO stats)
    {
        MagicalBulletItem item = new MagicalBulletItem();
        item.Init(stats);
        gameManager.inventoryManager.TryAddItem(item);
    }

    private void GenerateWeaponAndAddToInventory(WeaponSO stats)
    {
        WeaponItem item = new WeaponItem();
        item.Init(stats);
        gameManager.inventoryManager.TryAddItem(item);
    }

    private void GenerateConsumableAndAddToInventory(ConsumableSO stats)
    {
        
    }
}
