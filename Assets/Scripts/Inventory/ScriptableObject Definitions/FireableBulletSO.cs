using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Vox Obscura/Fireable Bullet")]

public class FireableBulletSO : BaseItemSO
{
    [SerializeField] public GameObject prefab;
    [SerializeField] public float damageMod;
    [SerializeField] public float velocity;
    [SerializeField] public float lifetime;
    [SerializeField] public float radius;
    [SerializeField] public bool canBounce;
    [SerializeField] public bool canPierce;
    [SerializeField] public List<BulletEffect> inherentMagicalEffects;
}
