using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    //Collision params
    private float radius;

    //Runtime stats
    private float damage;
    private float velocity;
    private float gravityStrength;
    private float lifetime;
    private int bounces;
    private int pierces;

    private bool canBounce;
    private bool canPierce;

    private float elapsedTime;
    private Vector3 moveDir;

    [SerializeField] private LayerMask hitMask = ~0;
    [SerializeField] private QueryTriggerInteraction triggerInteraction = QueryTriggerInteraction.Ignore;

    List<BulletEffect> bulletEffects;

    public void InitBullet(ProjectileBulletItem stats, Weapon playerWeapon, List<BulletEffect> effects, Vector3 moveDir)
    {
        damage = stats.damage;
        velocity = stats.velocity;
        lifetime = stats.lifetime;
        radius = stats.radius;
        canBounce = stats.canBounce;
        canPierce = stats.canPierce;

        bulletEffects = effects != null ? new List<BulletEffect>(effects) : new List<BulletEffect>();

        this.moveDir = moveDir.normalized;
        transform.position = playerWeapon.GetMuzzle().position;

        HandleOnFire();
    }

    void Update()
    {
        if (!IncrementLifetimeAndCheckExpire()) return;
        MoveBullet();
    }

    private bool IncrementLifetimeAndCheckExpire()
    {
        if (elapsedTime >= lifetime)
        {
            HandleOnExpire();
            return false;
        }

        elapsedTime += Time.deltaTime;
        return true;
    }

    private void MoveBullet()
    {
        Vector3 currentPos = transform.position;
        Vector3 moveDelta = moveDir * Time.deltaTime * velocity;

        if(Physics.SphereCast(currentPos, radius, moveDelta.normalized, out RaycastHit hit, moveDelta.magnitude, hitMask, triggerInteraction))
        {
            transform.position = hit.point;
            HandleOnHit(hit);
            return;
        }

        transform.position = currentPos + moveDelta;
    }

    private void HandleOnHit(RaycastHit hit)
    {
        if(hit.collider.TryGetComponent<Health>(out Health targetHealthComponent))
        {
            targetHealthComponent.TakeDamage(damage);
        }

        for(int i = 0; i < bulletEffects.Count; i++)
        {
            BulletEffect effect = bulletEffects[i];
            if (effect == null) continue;
            if (effect is IOnHitEffect onHit)
                onHit.OnHit(this, hit);
        }

        //small debug to check if the bullet actually goes poof
        Debug.Log("Bullet broke!"); 
        Destroy(gameObject);
    }

    private void HandleOnFire()
    {
        for (int i = 0; i < bulletEffects.Count; i++)
        {
            BulletEffect effect = bulletEffects[i];
            if (effect == null) continue;
            if(effect is IOnFireEffect onFire)
                onFire.OnFire(this);
        }
    }

    private void HandleOnExpire()
    {
        for (int i = 0; i < bulletEffects.Count; i++)
        {
            BulletEffect effect = bulletEffects[i];
            if (effect == null) continue;
            if( effect is IOnExpireEffect onExpire)
                onExpire.OnExpire(this);
        }

        Destroy(gameObject);
    }
}
