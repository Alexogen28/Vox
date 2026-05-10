using System.Collections.Generic;
using UnityEngine;

/*
 * A weapon item which inherits InventoryItem
 * 
 * This one lets you specificy different kinds of weapons,
 * with more slots, faster bullets, bigger base damage etc.
 * 
 * Weapon.cs is the actual runtime object! THIS IS ONLY FOR INVENTORY MANAGEMENT
 */

public class WeaponItem : InventoryItem
{
    public GameObject prefab;
    public int maxChambers;
    public float reloadTime;
    public List<BulletEffect> inherentWeaponEffects;

    public void Init(WeaponSO stats)
    {
        //stats inherent to InventoryItem
        this.itemName = stats.itemName;
        this.rarity = stats.rarity;
        this.itemSprite = stats.sprite;
        this.cost = stats.cost;

        //stats specific to WeaponItem
        this.prefab = stats.prefab;
        this.maxChambers = stats.maxChambers;
        this.reloadTime = stats.reloadTime;
        inherentWeaponEffects = new(stats.inherentWeaponEffects);
    }
}
