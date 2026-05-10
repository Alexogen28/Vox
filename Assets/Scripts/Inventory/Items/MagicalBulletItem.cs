using System.Collections.Generic;
using UnityEngine;

public class MagicalBulletItem : BulletItem
{
    [SerializeField] public List<BulletEffect> bulletEffects;

    public void Init(MagicalBulletSO stats)
    {
        //basic item stats
        this.itemName = stats.itemName;
        this.rarity = stats.rarity;
        this.itemSprite = stats.sprite;
        this.cost = stats.cost;

        bulletEffects = new(stats.magicalEffects);
    }
}
