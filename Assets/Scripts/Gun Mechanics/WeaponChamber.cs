using UnityEngine;
using System;

[Serializable]
public class WeaponChamber
{
    [SerializeField] private BulletItem bulletInChamber = null;

    public BulletItem GetBulletInChamber() => bulletInChamber;
    //public BulletBase GetBaseBulletInChamber() => bulletInChamber as BulletItem;
    public void SetBullet(BulletItem bulletToSet) => bulletInChamber = bulletToSet;
    public void RemoveBulletFromChamber() => bulletInChamber = null;
    public bool IsChamberLoaded => bulletInChamber != null;
}
