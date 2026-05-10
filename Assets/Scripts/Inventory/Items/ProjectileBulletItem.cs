using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class ProjectileBulletItem : BulletItem
{
    public GameObject prefab;
    public float damage;
    public float velocity;
    public float lifetime;
    public float radius;
    public bool canBounce;
    public bool canPierce;

    public List<BulletEffect> inherentMagicalEffects;

    public void Init(FireableBulletSO stats)
    {
        //basic item stats
        this.itemName = stats.itemName;
        this.rarity = stats.rarity;
        this.itemSprite = stats.sprite;
        this.cost = stats.cost;

        //customs stats
        this.prefab = stats.prefab;
        this.damage = stats.damageMod;
        this.velocity = stats.velocity;
        this.lifetime = stats.lifetime;
        this.radius = stats.radius;
        this.canBounce = stats.canBounce;
        this.canPierce = stats.canPierce;
        this.inherentMagicalEffects = new(stats.inherentMagicalEffects);
    }
}
