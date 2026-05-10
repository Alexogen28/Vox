using UnityEngine;

[CreateAssetMenu(menuName = "Vox Obscura/Bullet Effects/Dummy")]

public class DummyEffect : BulletEffect, IOnFireEffect, IOnHitEffect
{
    public void OnFire(Bullet bullet)
    {
        Debug.Log("Applied dummy effect OnFire!");
    }

    public void OnHit(Bullet bullet, RaycastHit hit)
    {
        Debug.Log("Applied dummy effect for OnHit!" + hit.ToString());
    }
}
