using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    [Header("Gun stats")]
    [SerializeField] private int maxChambers;
    [SerializeField] private float reloadTime;
    [SerializeField] private List<BulletEffect> inherentWeaponEffects;

    [Header("Transforms")]
    [SerializeField] private Transform muzzle;
    [SerializeField] private Camera gameCamera;

    [SerializeField] private List<WeaponChamber> weaponChambers;
    private List<BulletEffect> bulletEffectsToInstantiate = new List<BulletEffect>(128);

    [SerializeField] private int currentChamber = 0;

    /*
     *  EVENTS
     */
    public static event Action<Weapon> OnWeaponFire;
    public static event Action<Weapon> OnWeaponReload;
    public int GetMaxChambers() => maxChambers;
    public int GetCurrentChamber() => currentChamber;
    public List<WeaponChamber> GetWeaponChambers() => weaponChambers;
    public Transform GetMuzzle() => muzzle;


    /*
     * Initialize weapon upon being equipped
     * 
     * This will be called by Inventory Manager, probably
     */
    public void Init(WeaponItem weaponStats)
    {
        this.maxChambers = weaponStats.maxChambers;
        this.reloadTime =  weaponStats.reloadTime;
        foreach(var toAdd in weaponStats.inherentWeaponEffects)
            this.inherentWeaponEffects.Add(toAdd);

        currentChamber = 0;

        gameCamera = Camera.main;

        InitializeChambers();
    }

    /*
     * Method for initialising all the chambers
     * of the current weapon to make sure
     * the List of chambers has an accurate size
     */

    private void InitializeChambers()
    {
        if (weaponChambers == null) weaponChambers = new List<WeaponChamber>();

        weaponChambers.Clear();
        weaponChambers.Capacity = maxChambers;

        //make sure to add WeaponChamber() not null!
        //I did mess it up initially and added null 
        //and it bombed the code
        for (int i = 0; i < maxChambers; i++)
        {
            weaponChambers.Add(new WeaponChamber());
        }
    }

    /*
     * Methods for loading and unloading the bullets
     * in the given chamber of the weapon
     */

    public bool TryLoadBullet(BulletItem bulletToLoad, int positionToLoad)
    {
        if (positionToLoad < 0 || positionToLoad >= maxChambers) return false;
        if (bulletToLoad == null) return false;

        weaponChambers[positionToLoad].SetBullet(bulletToLoad);
        return true;
    }

    public bool TryUnloadBullet(int positionInChamber)
    {
        if (positionInChamber < 0 || positionInChamber >= maxChambers) return false;
        if (weaponChambers[positionInChamber].IsChamberLoaded == false) return false;

        weaponChambers[positionInChamber].RemoveBulletFromChamber();
        return true;
    }

    /*
     * Unload all chambers of the weapon
     * so you can deselect all bullets 
     * when the weapon is unequiped
     */
    public void UnloadAll()
    {
        for (int i = 0; i < maxChambers; i++)
            TryUnloadBullet(i);
    }

    /*
     * Method for checking in the weapon needs to be reloaded
     * 
     * Will either return out if the currentChamber is past the last one
     * or will otherwise attempt to fire
     */

    public void TryFireWeapon()
    {
        if (weaponChambers == null || currentChamber >= maxChambers || currentChamber < 0) return;

        FireAndCycleChambers();
    }

    /*
     * Reloading method, sets the chamber back to 0
     * it's very rudimentary
     */

    public void ReloadWeapon()
    {
        StartCoroutine(ReloadCoroutine(2f));
        return;
    }

    IEnumerator ReloadCoroutine(float timeToWait)
    {
        yield return new WaitForSeconds(timeToWait);
        currentChamber = 0;
        OnWeaponReload?.Invoke(this);
    }

    /*
     * Cycling through chambers logic
     * Always start on a regular bullet
     * and cycle through all the next magical bullets
     * until you reach another regular bullet
     * 
     * This should always start on a regular bullet!
     * And if it doesnt -- just skip past it
     * 
     * But also, why would you ever not load a regular bullet first?
     * Better to guard against game testers and weird journalists
     * Remember that one Cuphead review?
     */

    private void FireAndCycleChambers()
    {
        bulletEffectsToInstantiate.Clear();

        if (inherentWeaponEffects != null)
        {
            for (int j = 0; j < inherentWeaponEffects.Count; j++)
            {
                bulletEffectsToInstantiate.Add(inherentWeaponEffects[j]);
            }
        }

        var bulletFromChamber = weaponChambers[currentChamber].GetBulletInChamber();
        currentChamber++;

        if(bulletFromChamber is MagicalBulletItem)
        {
            return;
        }


        ProjectileBulletItem bulletToFire = bulletFromChamber as ProjectileBulletItem;


        //check if I got a null bullet, and if I did, return out of method
        //this still moves to then next chamber because of previous call
        if (bulletToFire == null) return;

        /*
         * Cycle through chambers until you reach
         * a normal bullet or run out of chambers
         */

        int i;

        for(i = currentChamber; i < maxChambers; i++)
        {
            //get the bullet in the current chamber
            //note this chamber is *after* the one with the bullet that will be fired
            //this automatically places it inside correct variable type
            var currentBullet = weaponChambers[i].GetBulletInChamber();

            //if its a regular bullet, break out of for loop
            //but do not increment the chamber!
            if (currentBullet is ProjectileBulletItem) break;

            //check if the magical bullet is null
            //and if is, skip the current chamber
            if (currentBullet == null) continue;

            if (currentBullet is not MagicalBulletItem magicBullet) continue;

            if(magicBullet.bulletEffects == null) continue;

            for (int j = 0; j < magicBullet.bulletEffects.Count; j++)
            {
                //check if for some reason this spot in the List of effects is null
                if (magicBullet.bulletEffects[j] == null) continue;

                //load this effect of the magical bullet into the list of effects to instantiate
                bulletEffectsToInstantiate.Add(magicBullet.bulletEffects[j]);
            }
        }

        //pass the chamber that the cycling got to
        //back into currentChamber
        currentChamber = i;

        //deprecated while loop approach
        //while(currentChamber < maxChambers)
        //{
        //    BulletBase effectBullet = weaponChambers[currentChamber].GetBulletInChamber();
        //    if(effectBullet is FireableBulletSO) break;
        //    if(effectBullet == null)
        //    {
        //        currentChamber++;
        //    }

        //    MagicalBulletSO magicBullet = effectBullet as MagicalBulletSO;

        //    for(int i = 0; i < magicBullet.magicalEffects.Count; i++)
        //    {
        //        bulletEffectsToInstantiate.Add(magicBullet.magicalEffects[i]);
        //    }

        //    currentChamber++;
        //}


        //create the GameObject for the projectile
        if (bulletToFire.prefab == null) return;

        //get the direction the bullet should travel in
        Vector3 bulletDirection = GetAimPoint();
        Vector3 aimDir = (bulletDirection - muzzle.position).normalized;

        //instantiate game object
        GameObject bulletToInstantiate = Instantiate(bulletToFire.prefab, muzzle.transform.position, Quaternion.LookRotation(aimDir));

        //grab the bulletProjectile here
        if (!bulletToInstantiate.TryGetComponent<Bullet>(out var bulletProjectile))
        {
            Destroy(bulletToInstantiate);
            return;
        }

        //and intiailise it
        bulletProjectile.InitBullet(bulletToFire, this, bulletEffectsToInstantiate, aimDir);

        OnWeaponFire?.Invoke(this);
    }

    private Vector3 GetAimPoint()
    {
        Ray ray = gameCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        if(Physics.Raycast(ray, out RaycastHit hit, 1000))
        {
            return hit.point;
        }

        return ray.origin + ray.direction * 1000;
    }
}
