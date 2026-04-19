using UnityEngine;

public enum Rarity
{
    Common,
    Rare,
    Legendary,
    Obscure
}

public abstract class BaseItemSO : ScriptableObject
{
    [SerializeField] public string itemName;
    [SerializeField] public Rarity rarity; 
    [SerializeField] public Sprite sprite;
    [SerializeField] public float cost;
}
