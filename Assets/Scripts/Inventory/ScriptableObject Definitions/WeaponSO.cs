using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Vox Obscura/Weapon")]
public class WeaponSO : BaseItemSO
{
    [SerializeField] public GameObject prefab;
    [SerializeField] public int maxChambers;
    [SerializeField] public float reloadTime;
    [SerializeField] public List<BulletEffect> inherentWeaponEffects;
}
