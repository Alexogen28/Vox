using UnityEngine;


/*
 * The base class for any item in the game
 * This is inherited by a bunch of other item types and is thus scalable
 */


public abstract class InventoryItem
{
    public string itemName;
    public Sprite itemSprite;
    public Rarity rarity;
    public float cost;
}
